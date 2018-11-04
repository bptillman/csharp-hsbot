namespace Hsbot.Core.Messaging
{
    public class BotMessageContext : IBotMessageContext
    {
        public BotMessageContext(InboundMessage message)
        {
            Message = message;
        }

        public InboundMessage Message { get; }
    }
}