using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.Connection;
using Hsbot.Core.Messaging;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class TestInboundMessageContext : IInboundMessageContext
    {
        public List<OutboundResponse> SentMessages { get; } = new List<OutboundResponse>();

        public Dictionary<string, TestChatUser> ChatUsers { get; set; } = new Dictionary<string, TestChatUser>();

        public InboundMessage Message { get; set; }
        public Func<OutboundResponse, Task> SendMessage { get; }
        public Func<string, Task<IUser>> GetChatUserById { get; }

        public TestInboundMessageContext(InboundMessage message)
        {
            Message = message;
            SendMessage = r =>
            {
                SentMessages.Add(r);
                return Task.CompletedTask;
            };
            GetChatUserById = id => Task.FromResult((IUser) ChatUsers[id]);
        }
    }
}
