using System;
using System.Threading.Tasks;
using Hsbot.Core.Brain;

namespace Hsbot.Core.Messaging
{
    public class BotMessageContext : IBotMessageContext
    {
        public BotMessageContext(IBotBrain brain, 
            IHsbotLog log, 
            InboundMessage message,
            Func<OutboundResponse, Task> sendMessageFunc)
        {
            Brain = brain;
            Log = log;
            Message = message;
            SendMessage = sendMessageFunc;
        }

        public IBotBrain Brain { get; }
        public IHsbotLog Log { get; }
        public InboundMessage Message { get; }
        public Func<OutboundResponse, Task> SendMessage { get; }
    }
}