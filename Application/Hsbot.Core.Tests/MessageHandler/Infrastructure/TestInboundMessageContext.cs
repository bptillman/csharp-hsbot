using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class TestInboundMessageContext : IInboundMessageContext
    {
        public List<OutboundResponse> SentMessages { get; } = new List<OutboundResponse>();

        public Dictionary<string, TestChatUser> ChatUsers { get; set; } = new Dictionary<string, TestChatUser>();

        public InboundMessage Message { get; set; }
        public IBotMessagingServices Bot { get; }

        public TestInboundMessageContext(InboundMessage message)
        {
            Message = message;
            Bot = new TestBotMessagingServices
            {
                SendMessageFunc = r =>
                {
                    SentMessages.Add(r);
                    return Task.CompletedTask;
                },
                GetChatUserByIdFunc = id => Task.FromResult((IUser)ChatUsers[id])
            };
        }
    }

    public class TestBotMessagingServices : IBotMessagingServices
    {
        public Func<OutboundResponse, Task> SendMessageFunc { get; set; }
        public Func<string, Task<IUser>> GetChatUserByIdFunc { get; set; }

        public Task<IUser> GetChatUserById(string userId)
        {
            return GetChatUserByIdFunc(userId);
        }

        public Task SendMessage(OutboundResponse response)
        {
            return SendMessageFunc(response);
        }
    }
}
