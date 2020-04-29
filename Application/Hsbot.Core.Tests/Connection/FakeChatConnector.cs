using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Hsbot.Core.Connection;
using Hsbot.Core.Messaging;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;

namespace Hsbot.Core.Tests.Connection
{
    public class FakeChatConnector : IHsbotChatConnector
    {
        public List<OutboundResponse> SentMessages { get; } = new List<OutboundResponse>();
        public Dictionary<string, TestFileUpload> FileUploads { get; } = new Dictionary<string, TestFileUpload>();
        public Dictionary<string, TestChatUser> ChatUsers { get; set; } = new Dictionary<string, TestChatUser>();

        private readonly Subject<IHsbotChatConnector> _disconnectedEvent = new Subject<IHsbotChatConnector>();
        private readonly Subject<IHsbotChatConnector> _reconnectingEvent = new Subject<IHsbotChatConnector>();
        private readonly Subject<IHsbotChatConnector> _reconnectedEvent = new Subject<IHsbotChatConnector>();
        private readonly Subject<Task<InboundMessage>> _messageReceivedEvent = new Subject<Task<InboundMessage>>();

        public IObservable<IHsbotChatConnector> Disconnected { get; }
        public IObservable<IHsbotChatConnector> Reconnecting { get; }
        public IObservable<IHsbotChatConnector> Reconnected { get; }
        public IObservable<Task<InboundMessage>> MessageReceived { get; }

        public FakeChatConnector()
        {
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

        public Task SendMessage(OutboundResponse response)
        {
            SentMessages.Add(response);
            return Task.CompletedTask;
        }

        public Task UploadFile(FileUploadResponse response)
        {
            using var ms = new MemoryStream();
            response.FileStream.CopyTo(ms);

            FileUploads.Add(response.FileName, new TestFileUpload
            {
                FileBytes = ms.ToArray(),
                FileName = response.FileName
            });

            return Task.CompletedTask;
        }

        public Task<IUser> GetChatUserById(string userId)
        {
            return ChatUsers.TryGetValue(userId, out var user) 
                ? Task.FromResult((IUser) user) 
                : Task.FromResult((IUser) null);
        }

        public Task<IUser[]> GetAllUsers()
        {
            return Task.FromResult(ChatUsers.Values.Select(u => (IUser) u).ToArray());
        }
    }
}
