using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Hsbot.Core;
using Hsbot.Core.Connection;
using Hsbot.Core.Messaging;
using Hsbot.Slack;

namespace Hsbot.Hosting.Web.LocalDebug
{
    public class DebugChatConnector : IHsbotChatConnector
    {
        private readonly Subject<IHsbotChatConnector> _disconnectedEvent = new Subject<IHsbotChatConnector>();
        private readonly Subject<IHsbotChatConnector> _reconnectingEvent = new Subject<IHsbotChatConnector>();
        private readonly Subject<IHsbotChatConnector> _reconnectedEvent = new Subject<IHsbotChatConnector>();
        private readonly Subject<Task<InboundMessage>> _messageReceivedEvent = new Subject<Task<InboundMessage>>();
        private readonly Subject<OutboundResponse> _messageSentEvent = new Subject<OutboundResponse>();
        private readonly Subject<FileUploadResponse> _fileUploadedEvent = new Subject<FileUploadResponse>();

        public readonly Dictionary<string, SlackUser> ChatUsers = new Dictionary<string, SlackUser>();

        public IObservable<IHsbotChatConnector> Disconnected { get; }
        public IObservable<IHsbotChatConnector> Reconnecting { get; }
        public IObservable<IHsbotChatConnector> Reconnected { get; }
        public IObservable<Task<InboundMessage>> MessageReceived { get; }
        public IObservable<OutboundResponse> MessageSent { get; }
        public IObservable<FileUploadResponse> FileUploaded { get; }

        public DebugChatConnector()
        {
            Disconnected = _disconnectedEvent;
            Reconnecting = _reconnectingEvent;
            Reconnected = _reconnectedEvent;
            MessageReceived = _messageReceivedEvent;
            MessageSent = _messageSentEvent;
            FileUploaded = _fileUploadedEvent;
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

        public Task SendMessage(OutboundResponse response)
        {
            _messageSentEvent.OnNext(response);
            return Task.CompletedTask;
        }

        public Task UploadFile(FileUploadResponse response)
        {
            _fileUploadedEvent.OnNext(response);
            return Task.CompletedTask;
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
