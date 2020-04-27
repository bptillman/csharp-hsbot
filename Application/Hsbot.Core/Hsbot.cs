using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Hsbot.Core.BotServices;
using Hsbot.Core.Connection;
using Hsbot.Core.Messaging;

namespace Hsbot.Core
{
    public sealed class Hsbot : IDisposable
    {
        private readonly IHsbotLog _log;
        private readonly IEnumerable<IInboundMessageHandler> _messageHandlers;
        private readonly IEnumerable<IBotService> _botServices;
        private readonly List<MessageHandlerDescriptor> _messageHandlerDescriptors;
        
        private IDisposable _onDisconnectSubscription;
        private IDisposable _onReconnectingSubscription;
        private IDisposable _onReconnectedSubscription;
        private IDisposable _onMessageReceivedSubscription;

        private readonly IHsbotChatConnector _connection;
        private bool _disconnecting = false;

        public Hsbot(IHsbotLog log,
            IEnumerable<IInboundMessageHandler> messageHandlers,
            IEnumerable<IBotService> botServices,
            IHsbotChatConnector connection)
        {
            _log = log;
            _messageHandlers = messageHandlers;
            _botServices = botServices;
            _connection = connection;
            _messageHandlerDescriptors = _messageHandlers
                .SelectMany(mh => mh.GetCommandDescriptors())
                .OrderBy(d => d.Command)
                .ToList();
        }

        public async Task Connect()
        {
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

            await StartServices();
        }

        private Task OnDisconnect()
        {
            if (_disconnecting)
            {
                _log.Info("Disconnected");
            }

            else
            {
                _log.Info("Disconnected from server, attempting to reconnect automatically");
            }
            
            return Task.CompletedTask;
        }

        public async Task Disconnect()
        {
            _log.Info("Disconnecting");

            _disconnecting = true;
            await _connection.Disconnect();

            await ShutdownServices();
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
            if (message.StartsWith("help"))
            {
                var response = GetHelpResponse(message);
                await SendMessage(response);
                return;
            }

            var messageSnippet = $"{message.Username}: {message.TextWithoutBotName.Substring(0, Math.Min(message.TextWithoutBotName.Length, 25))}...";
            var messageContext = new InboundMessageContext(message, SendMessage, GetChatUserById);

            foreach (var inboundMessageHandler in _messageHandlers)
            {
                var handlerResult = inboundMessageHandler.Handles(message);

                _log.Debug($"Message [{messageSnippet}]: {inboundMessageHandler.GetType().Name} -> HandlesMessage={handlerResult.HandlesMessage}, BotIsMentioned={handlerResult.BotIsMentioned}, RandomRoll={handlerResult.RandomRoll}, MessageChannel={handlerResult.MessageChannel}");

                if (!handlerResult.HandlesMessage) continue;

                try
                {
                    await inboundMessageHandler.HandleAsync(messageContext);
                }
                catch (Exception e)
                {
                    _log.Error($"Error handling message: {e}");
                }
            }
        }

        private OutboundResponse GetHelpResponse(InboundMessage message)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Available commands:");
            sb.AppendLine();

            foreach (var messageHandlerDescriptor in _messageHandlerDescriptors)
            {
                sb.AppendLine($"{messageHandlerDescriptor.Command} - {messageHandlerDescriptor.Description}");
            }

            return message.CreateResponse(sb.ToString());
        }

        private async Task StartServices()
        {
            var servicesToStart = _botServices.OrderBy(s => s.GetStartupOrder());
            var botServiceContext = new BotServiceContext {Parent = this};

            foreach (var botService in servicesToStart)
            {
                _log.Info($"Starting {botService.GetType().Name}");
                await botService.Start(botServiceContext);
            }
        }

        private async Task ShutdownServices()
        {
            //shutdown in reverse order of startup
            var servicesToStop = _botServices.OrderByDescending(s => s.GetStartupOrder());

            foreach (var botService in servicesToStop)
            {
                _log.Info($"Stopping {botService.GetType().Name}");
                await botService.Stop();
            }
        }

        public async Task SendMessage(IEnumerable<OutboundResponse> responses)
        {
            foreach (var outboundResponse in responses)
            {
                await SendMessage(outboundResponse);
            }
        }

        public async Task<IUser> GetChatUserById(string userId)
        {
            return await _connection.GetChatUserById(userId);
        }

        public Task SendMessage(OutboundResponse response)
        {
            return _connection.SendMessage(response);
        }

        public void Dispose()
        {
            _onDisconnectSubscription?.Dispose();
            _onReconnectedSubscription?.Dispose();
            _onReconnectingSubscription?.Dispose();
            _onMessageReceivedSubscription?.Dispose();
        }
    }
}
