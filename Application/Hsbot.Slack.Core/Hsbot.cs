using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Hsbot.Slack.Core.Brain;
using Hsbot.Slack.Core.Connection;
using Hsbot.Slack.Core.Messaging;

namespace Hsbot.Slack.Core
{
    public class Hsbot : IDisposable
    {
        private readonly IHsbotLog _log;
        private readonly IEnumerable<IInboundMessageHandler> _messageHandlers;
        private readonly IBotBrainStorage<HsbotBrain> _brainStorage;

        private IDisposable _brainChangedSubscription;
        private IDisposable _onDisconnectSubscription;
        private IDisposable _onReconnectingSubscription;
        private IDisposable _onReconnectedSubscription;
        private IDisposable _onMessageReceivedSubscription;

        private readonly IHsbotChatConnector _connection;
        private bool _disconnecting = false;

        public HsbotBrain Brain { get; private set; }

        public Hsbot(IHsbotLog log,
            IEnumerable<IInboundMessageHandler> messageHandlers,
            IBotBrainStorage<HsbotBrain> brainStorage,
            IHsbotChatConnector connection)
        {
            _log = log;
            _messageHandlers = messageHandlers;
            _brainStorage = brainStorage;
            _connection = connection;
        }

        public async Task Connect()
        {
            await InitializeBrain();

            _log.Info("Connecting to messaging service");

            await _connection.Connect();
            _onDisconnectSubscription =_connection.Disconnected
                .Select(conn => Observable.FromAsync(ct => OnDisconnect()))
                .Concat()
                .Subscribe();

            _onReconnectingSubscription = _connection.Reconnecting
                .Select(conn => Observable.FromAsync(ct => OnReconnecting()))
                .Concat()
                .Subscribe();

            _onReconnectedSubscription = _connection.Reconnected
                .Select(conn => Observable.FromAsync(ct => OnReconnect()))
                .Concat()
                .Subscribe();

            _onMessageReceivedSubscription = _connection.MessageReceived
                .Select(msg => Observable.FromAsync(async f => await OnMessageReceived(await msg)))
                .Concat()
                .Subscribe();

            _log.Info("Connected successfully");
        }

        private Task OnDisconnect()
        {
            if (_disconnecting)
            {
                _log.Info("Disconnected");
                return Task.CompletedTask;
            }

            _log.Info("Disconnected from server, attempting to reconnect");
            return Reconnect();
        }

        public Task Disconnect()
        {
            _log.Info("Disconnecting");

            _disconnecting = true;
            return _connection.Disconnect();
        }

        private Task Reconnect()
        {
            _log.Info("Reconnecting");
            return Connect();
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

        private async Task OnMessageReceived(InboundMessage message)
        {
            var messageContext = new BotMessageContext(Brain, _log, message, SendMessage);

            var messageSnippet = $"{message.Username}: {message.TextWithoutBotName.Substring(0, Math.Min(message.TextWithoutBotName.Length, 25))}...";

            foreach (var inboundMessageHandler in _messageHandlers)
            {
                var handlerResult = inboundMessageHandler.Handles(message);

                _log.Debug($"Message [{messageSnippet}]: {inboundMessageHandler.GetType().Name} -> HandlesMessage={handlerResult.HandlesMessage}, BotIsMentioned={handlerResult.BotIsMentioned}, RandomRoll={handlerResult.RandomRoll}, MessageChannel={handlerResult.MessageChannel}");

                if (!handlerResult.HandlesMessage) continue;

                await inboundMessageHandler.HandleAsync(messageContext);
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

            try
            {
                Brain = await _brainStorage.Load();
                _brainChangedSubscription = Brain.BrainChanged
                    .Select(SaveBrain)
                    .Window(1) //ensure we only run 1 call to the save brain method at a given time
                    .Concat()
                    .Subscribe();

                _log.Info("Brain loaded from storage successfully");
            }

            catch (Exception e)
            {
                _log.Error("Error loading brain - falling back to an in-memory brain without persistence.");
                _log.Error("Brain load exception: {0}", e);

                Brain = new HsbotBrain();
            }
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

        public Task SendMessage(OutboundResponse response)
        {
            return _connection.SendMessage(response);
        }

        public void Dispose()
        {
            _brainChangedSubscription?.Dispose();
            _onDisconnectSubscription?.Dispose();
            _onReconnectedSubscription?.Dispose();
            _onReconnectingSubscription?.Dispose();
            _onMessageReceivedSubscription?.Dispose();
        }
    }
}
