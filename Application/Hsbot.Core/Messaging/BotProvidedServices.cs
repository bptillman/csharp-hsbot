using System;
using System.Threading.Tasks;
using Hsbot.Core.Connection;

namespace Hsbot.Core.Messaging
{
    public class BotProvidedServices : IBotProvidedServices
    {
        public BotProvidedServices(Func<string, Task<IChatUser>> getUserByIdFunc, Func<OutboundResponse, Task> sendMessageFunc)
        {
            GetChatUserById = getUserByIdFunc;
            SendMessage = sendMessageFunc;
        }

        public Func<string, Task<IChatUser>> GetChatUserById { get; }
        public Func<OutboundResponse, Task> SendMessage { get; }
    }
}
