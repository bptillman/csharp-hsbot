using System.Collections.Generic;

namespace Hsbot.Slack.Core.Messaging
{
    public interface IInboundMessageHandler
    {
        IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors();
        bool Handles(InboundMessage message);
        IEnumerable<OutboundResponse> Handle(InboundMessage message);
    }
}
