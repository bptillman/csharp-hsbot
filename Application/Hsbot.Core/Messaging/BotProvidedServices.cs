using System;
using System.Threading.Tasks;
using Hsbot.Core.Brain;

namespace Hsbot.Core.Messaging
{
    public class BotProvidedServices : IBotProvidedServices
    {
        public BotProvidedServices(IBotBrain brain, 
            IHsbotLog log, 
            Func<OutboundResponse, Task> sendMessageFunc)
        {
            Brain = brain;
            Log = log;
            SendMessage = sendMessageFunc;
        }

        public IBotBrain Brain { get; }
        public IHsbotLog Log { get; }
        public Func<OutboundResponse, Task> SendMessage { get; }
    }
}
