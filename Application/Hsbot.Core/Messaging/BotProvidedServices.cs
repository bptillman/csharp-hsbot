using System;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.Connection;
using Hsbot.Core.Infrastructure;
using Hsbot.Core.Messaging.Formatting;

namespace Hsbot.Core.Messaging
{
    public class BotProvidedServices : IBotProvidedServices
    {
        public BotProvidedServices(IHsbotLog log, 
            Func<string, Task<IChatUser>> getUserByIdFunc,
            Func<OutboundResponse, Task> sendMessageFunc,
            IChatMessageTextFormatter chatMessageTextFormatter,
            ISystemClock systemClock)
        {
            Log = log;
            GetChatUserById = getUserByIdFunc;
            SendMessage = sendMessageFunc;
            MessageTextFormatter = chatMessageTextFormatter;
            SystemClock = systemClock;
        }

        public IHsbotLog Log { get; }
        public Func<string, Task<IChatUser>> GetChatUserById { get; }
        public Func<OutboundResponse, Task> SendMessage { get; }
        public IChatMessageTextFormatter MessageTextFormatter { get; }
        public ISystemClock SystemClock { get; }
    }
}
