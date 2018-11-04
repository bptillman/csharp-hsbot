using Hsbot.Core.Messaging;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class BotMessageContextFake : IBotMessageContext
    {
        public BotMessageContextFake(InboundMessage message)
        {
            Message = message;
        }

        public InboundMessage Message { get; }
    }
}
