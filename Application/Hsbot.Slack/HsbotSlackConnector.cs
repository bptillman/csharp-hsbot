using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Hsbot.Core;
using Hsbot.Core.Connection;
using Hsbot.Core.Messaging;
using SlackConnector;
using SlackConnector.EventHandlers;
using SlackConnector.Models;
using SocketLite.Services;

namespace Hsbot.Slack
{
    public class HsbotSlackConnector : IHsbotChatConnector
    {
        private readonly IHsbotConfig _config;
        private readonly IHsbotLog _log;
        private ISlackConnection _connection;

        public string Id { get; private set; } //internal id of the bot
        public string Name { get; private set; } //official handle of the bot
        public string[] AddressableNames { get; private set; } //names by which the bot may be addressed in the chat app


        public IObservable<IHsbotChatConnector> Disconnected { get; private set; }
        public IObservable<IHsbotChatConnector> Reconnecting { get; private set; }
        public IObservable<IHsbotChatConnector> Reconnected { get; private set; }
        public IObservable<Task<InboundMessage>> MessageReceived { get; private set; }

        public HsbotSlackConnector(IHsbotConfig config, IHsbotLog log)
        {
            _config = config;
            _log = log;
        }

        public async Task Connect()
        {
            if (_connection != null)
            {
                _log.Warn($"Connection object in {nameof(HsbotSlackConnector)} is not null. I think I'm already connected, so I'm not attempting to connect again.");
                return;
            }

            _log.Info("Connecting to Slack");

            var connector = new SlackConnector.SlackConnector();
            _connection = await connector.Connect(_config.SlackApiKey);

            Id = _connection?.Self?.Id;
            Name = _connection?.Self?.Name;
            AddressableNames = GetAddressableNames(Name, Id);

            _log.Info($"Connected successfully with Id={Id}, Name={Name}");

            Disconnected = Observable.FromEvent<DisconnectEventHandler, Unit>
            (
                handler => () => handler(Unit.Default),
                h => _connection.OnDisconnect += h,
                h => _connection.OnDisconnect -= h
            )
            .Select(x => this);

            Reconnecting = Observable.FromEvent<ReconnectEventHandler, Unit>
            (
                handler => () =>
                {
                    handler(Unit.Default);
                    return Task.CompletedTask;
                },
                h => _connection.OnReconnecting += h,
                h => _connection.OnReconnecting -= h
            )
            .Select(x => this);

            Reconnected = Observable.FromEvent<ReconnectEventHandler, Unit>
            (
                handler => () =>
                {
                    handler(Unit.Default);
                    return Task.CompletedTask;
                },
                h => _connection.OnReconnect += h,
                h => _connection.OnReconnect -= h
            )
            .Select(x => this);

            MessageReceived = Observable.FromEvent<MessageReceivedEventHandler, SlackMessage>
            (
                handler => msg =>
                {
                    handler(msg);
                    return Task.CompletedTask;
                },
                h => _connection.OnMessageReceived += h,
                h => _connection.OnMessageReceived -= h
            ).Select(msg => TransformSlackMessage(msg));
        }

        public Task Disconnect()
        {
            _log.Info("Disconnecting from Slack");
            return _connection.Close();
        }

        public async Task SendMessage(OutboundResponse response)
        {
            var chatHub = await GetChatHub(response);

            if (chatHub != null)
            {
                if (response.IndicateTyping)
                {
                    _log.Debug($"Sending IndicateTyping message to channel {chatHub.Name}");
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

                    _log.Debug($"Sending message to channel {chatHub.Name} with {botMessage.Attachments.Count} attachments.  Response text='{response.Text}'");
                    await _connection.Say(botMessage);
                }
            }
        }

        public async Task<IChatUser> GetChatUserById(string userId)
        {
            if (!_connection.UserCache.TryGetValue(userId, out var user))
            {
                var users = await _connection.GetUsers();
                user = users.Single(x => x.Id == userId);
            }

            return new SlackUser
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.Name,
                FullName = $"{user.FirstName} {user.LastName}",
            };
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
            return new[]
            {
                $"{botName}:",
                botName,
                $"<@{botId}>:",
                $"<@{botId}>",
                $"@{botName}:",
                $"@{botName}",
            };
        }

        private async Task<InboundMessage> TransformSlackMessage(SlackMessage message)
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

            return inboundMessage;
        }
    }
}