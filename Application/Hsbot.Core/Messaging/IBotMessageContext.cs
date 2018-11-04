namespace Hsbot.Core.Messaging
{
    public interface IBotMessageContext
    {
        InboundMessage Message { get; }
    }
}