using System;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.Brain;
using Hsbot.Core.Connection;
using Hsbot.Core.Infrastructure;
using Hsbot.Core.Messaging.Formatting;

namespace Hsbot.Core.Messaging
{
    public class BotProvidedServices : IBotProvidedServices
    {
        public BotProvidedServices(IBotBrain brain, 
            IHsbotLog log, 
            Func<string, Task<IChatUser>> getUserByIdFunc,
            Func<OutboundResponse, Task> sendMessageFunc,
            IChatMessageTextFormatter chatMessageTextFormatter,
            ISystemClock systemClock,
            ITumblrApiClient tumblrApiClient)
        {
            Brain = brain;
            Log = log;
            GetChatUserById = getUserByIdFunc;
            SendMessage = sendMessageFunc;
            MessageTextFormatter = chatMessageTextFormatter;
            SystemClock = systemClock;
            TumblrApiClient = tumblrApiClient;
        }

        public IBotBrain Brain { get; }
        public IHsbotLog Log { get; }
        public Func<string, Task<IChatUser>> GetChatUserById { get; }
        public Func<OutboundResponse, Task> SendMessage { get; }
        public IChatMessageTextFormatter MessageTextFormatter { get; }
        public ISystemClock SystemClock { get; }
        public ITumblrApiClient TumblrApiClient { get; }
    }
}
