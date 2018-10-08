using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Hsbot.Slack.Core.Brain;
using Hsbot.Slack.Core.Messaging;
using SlackConnector;
using SlackConnector.Models;

namespace Hsbot.Slack.Core
{
    public class Hsbot : IDisposable
    {
        private readonly IHsbotConfig _hsbotConfig;
        private readonly IHsbotLog _log;
        private readonly IEnumerable<IInboundMessageHandler> _messageHandlers;
        private readonly IBotBrainStorage<HsbotBrain> _brainStorage;

        private IDisposable _brainChangedSubscription;

        private ISlackConnection _connection;
        private bool _disconnecting = false;

        public string Id { get; private set; } //internal id of the bot
        public string Name { get; private set; } //official handle of the bot
        public string[] AddressableNames { get; private set; } //names by which the bot may be addressed in the chat app

        public HsbotBrain Brain { get; private set; }
        
        public Hsbot(IHsbotConfig hsbotConfig,
            IHsbotLog log,
            IEnumerable<IInboundMessageHandler> messageHandlers,
            IBotBrainStorage<HsbotBrain> brainStorage)
        {
            _hsbotConfig = hsbotConfig;
            _log = log;
            _messageHandlers = messageHandlers;
            _brainStorage = brainStorage;
        }

        public async Task Connect()
        {
            await InitializeBrain();

            _log.Info("Connecting to messaging service");

            var connector = new SlackConnector.SlackConnector();
            _connection = await connector.Connect(_hsbotConfig.SlackApiKey);

            _connection.OnDisconnect += OnDisconnect;
            _connection.OnReconnecting += OnReconnecting;
            _connection.OnReconnect += OnReconnect;
            _connection.OnMessageReceived += OnMessageReceived;

            Id = _connection?.Self?.Id;
            Name = _connection?.Self?.Name;
            AddressableNames = GetAddressableNames(Name, Id);

            _log.Info("Connected successfully");
        }

        private void OnDisconnect()
        {
            if (_disconnecting)
            {
                _log.Info("Disconnected");
            }

            else
            {
                _log.Info("Disconnected from server, attempting to reconnect");
                Reconnect();
            }
        }

        public async Task Disconnect()
        {
            _log.Info("Disconnecting");

            _disconnecting = true;
            if (_connection?.IsConnected == true)
            {
                await _connection.Close();
            }
        }

        private void Reconnect()
        {
            _log.Info("Reconnecting");
            if (_connection != null)
            {
                _connection.OnDisconnect += OnDisconnect;
                _connection.OnReconnecting += OnReconnecting;
                _connection.OnReconnect += OnReconnect;
                _connection.OnMessageReceived += OnMessageReceived;
                _connection = null;
            }

            _disconnecting = false;

            Connect()
                .ContinueWith(task =>
                {
                    if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                    {
                        _log.Info("Connection restored.");
                    }

                    else
                    {
                        _log.Error("Error while reconnecting: {0}", task.Exception);
                    }
                });
        }

        private Task OnReconnecting()
        {
            _log.Info("Attempting to reconnect");
            return Task.CompletedTask;
        }

        private Task OnReconnect()
        {
            _log.Info("Reconnected successfully");
            return Task.CompletedTask;
        }

        private async Task OnMessageReceived(SlackMessage message)
        {
            var userChannel = await GetUserChannel(message);

            var inboundMessage = new InboundMessage
            {
                RawText = message.RawData,
                FullText = message.Text,
                TextWithoutBotName = message.GetTextWithoutBotName(AddressableNames),
                UserId = message.User.Id,
                Username = message.User.Name,
                UserEmail = message.User.Email,
                Channel = message.ChatHub.Id,
                ChannelName = message.ChatHub.Name,
                MessageRecipientType = message.ChatHub.Type.ToMessageRecipientType(),
                UserChannel = userChannel,
                BotName = Name,
                BotId = Id,
                BotIsMentioned = message.MentionsBot
            };

            var messageSnippet = $"{message.User.Name}: {message.Text.Substring(0, Math.Min(message.Text.Length, 25))}...";

            foreach (var inboundMessageHandler in _messageHandlers)
            {
                var handlerResult = inboundMessageHandler.Handles(inboundMessage);

                _log.Debug($"Message [{messageSnippet}]: {inboundMessageHandler.GetType().Name} -> HandlesMessage={handlerResult.HandlesMessage}, BotIsMentioned={handlerResult.BotIsMentioned}, RandomRoll={handlerResult.RandomRoll}, MessageChannel={handlerResult.MessageChannel}");

                if (!handlerResult.HandlesMessage) continue;

                var responses = inboundMessageHandler.Handle(inboundMessage);
                await SendMessage(responses);
            }
        }

        private async Task InitializeBrain()
        {
            _log.Info("Initializing brain");
            if (Brain != null)
            {
                _log.Info("Brain already initialized, skipping");
                return;
            }

            Brain = await _brainStorage.Load();
            _brainChangedSubscription = Brain.BrainChanged
                .Select(SaveBrain)
                .Window(1) //ensure we only run 1 call to the save brain method at a given time
                .Subscribe();

            _log.Info("Brain loaded from storage successfully");
        }

        private async Task SaveBrain(HsbotBrain brain)
        {
            _log.Debug("Received brain change event - saving to storage");
            try
            {
                await _brainStorage.Save(brain);
                _log.Debug("Received brain change event - brain saved successfully");
            }

            catch (Exception e)
            {
                _log.Error("Failed to save brain to storage: {0}", e);
            }
        }

        public async Task SendMessage(IEnumerable<OutboundResponse> responses)
        {
            foreach (var outboundResponse in responses)
            {
                await SendMessage(outboundResponse);
            }
        }

        public async Task SendMessage(OutboundResponse response)
        {
            var chatHub = await GetChatHub(response);

            if (chatHub != null)
            {
                if (response.IndicateTyping)
                {
                    await _connection.IndicateTyping(chatHub);
                }

                else
                {
                    var botMessage = new BotMessage
                    {
                        ChatHub = chatHub,
                        Attachments = response.Attachments.ToSlackAttachments(),
                        Text = response.Text
                    };

                    await _connection.Say(botMessage);
                }
            }
        }

        private async Task<SlackChatHub> GetChatHub(OutboundResponse response)
        {
            switch (response.MessageRecipientType)
            {
                case MessageRecipientType.Channel:
                    return new SlackChatHub { Id = response.Channel };
                case MessageRecipientType.DirectMessage when string.IsNullOrEmpty(response.Channel):
                    return await GetUserChatHub(response.UserId);
                case MessageRecipientType.DirectMessage:
                    return new SlackChatHub { Id = response.Channel };
                default:
                    return new SlackChatHub { Id = response.Channel };
            }
        }

        private async Task<SlackChatHub> GetUserChatHub(string userId, bool joinChannel = true)
        {
            SlackChatHub chatHub = null;

            if (_connection.UserCache.ContainsKey(userId))
            {
                var username = "@" + _connection.UserCache[userId].Name;
                chatHub = _connection.ConnectedDMs().FirstOrDefault(x => x.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
            }

            if (chatHub == null && joinChannel)
            {
                chatHub = await _connection.JoinDirectMessageChannel(userId);
            }

            return chatHub;
        }

        private async Task<string> GetUserChannel(SlackMessage message)
        {
            return (await GetUserChatHub(message.User.Id, joinChannel: false) ?? new SlackChatHub()).Id;
        }

        private static string[] GetAddressableNames(string botName, string botId)
        {
            return new []
            {
                $"{botName}:",
                botName,
                $"<@{botId}>:",
                $"<@{botId}>",
                $"@{botName}:",
                $"@{botName}",
            };
        }

        public void Dispose()
        {
            _brainChangedSubscription?.Dispose();
        }
    }
}
