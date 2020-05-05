using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Hsbot.Core;
using Hsbot.Core.Connection;
using Hsbot.Core.Messaging;
using Hsbot.Hosting.Web.LocalDebug.Hubs;
using Hsbot.Slack;
using Microsoft.AspNetCore.SignalR;

namespace Hsbot.Hosting.Web.LocalDebug
{
    public class DebugChatConnector : IHsbotChatConnector
    {
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly Subject<IHsbotChatConnector> _disconnectedEvent = new Subject<IHsbotChatConnector>();
        private readonly Subject<IHsbotChatConnector> _reconnectingEvent = new Subject<IHsbotChatConnector>();
        private readonly Subject<IHsbotChatConnector> _reconnectedEvent = new Subject<IHsbotChatConnector>();
        private readonly Subject<Task<InboundMessage>> _messageReceivedEvent = new Subject<Task<InboundMessage>>();

        public readonly Dictionary<string, SlackUser> ChatUsers = new Dictionary<string, SlackUser>();

        public IObservable<IHsbotChatConnector> Disconnected { get; }
        public IObservable<IHsbotChatConnector> Reconnecting { get; }
        public IObservable<IHsbotChatConnector> Reconnected { get; }
        public IObservable<Task<InboundMessage>> MessageReceived { get; }

        public readonly string BotName = "@hsbot";
        public readonly string[] BotAliases = { "hsbot", "@hsbot", "csharpbot", "@csharpbot" };

        public DebugChatConnector(IHubContext<ChatHub> chatHubContext)
        {
            _chatHubContext = chatHubContext;
            Disconnected = _disconnectedEvent;
            Reconnecting = _reconnectingEvent;
            Reconnected = _reconnectedEvent;
            MessageReceived = _messageReceivedEvent;
        }

        public Task Connect()
        {
            return Task.CompletedTask;
        }

        public Task Disconnect()
        {
            _disconnectedEvent.OnNext(this);
            return Task.CompletedTask;
        }

        public void ReceiveMessage(InboundMessage message)
        {
            _messageReceivedEvent.OnNext(Task.FromResult(message));
        }

        public async Task SendMessage(OutboundResponse response)
        {
            if (string.IsNullOrEmpty(response.UserId)) response.UserId = BotName;
            await _chatHubContext.Clients.All.SendAsync(ChatHub.ReceiveMethodName, response);
        }

        public async Task UploadFile(FileUploadResponse response)
        {
            if (string.IsNullOrEmpty(response.UserId)) response.UserId = BotName;

            await using var ms = new MemoryStream();
            response.FileStream.CopyTo(ms);

            var fileContentResponse = new FileContentResponse
            {
                Channel = response.Channel,
                UserId = response.UserId,
                MessageRecipientType = response.MessageRecipientType,
                FileBytes = ms.ToArray(),
                FileName = response.FileName,
            };

            await _chatHubContext.Clients.All.SendAsync(ChatHub.ReceiveUploadMethodName, fileContentResponse);
        }

        public Task<IUser> GetChatUserById(string userId)
        {
            return ChatUsers.TryGetValue(userId, out var user)
                ? Task.FromResult((IUser)user)
                : Task.FromResult((IUser)null);
        }

        public Task<IUser[]> GetAllUsers()
        {
            return Task.FromResult(ChatUsers.Values.Select(u => (IUser)u).ToArray());
        }
    }
}
