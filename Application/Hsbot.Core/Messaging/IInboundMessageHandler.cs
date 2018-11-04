using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hsbot.Core.Messaging
{
    public interface IInboundMessageHandler
    {
        IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors();
        HandlesResult Handles(InboundMessage message);
        Task HandleAsync(IBotMessageContext context);

        IBotProvidedServices BotProvidedServices { get; set; }
    }

    public class HandlesResult
    {
        public bool HandlesMessage { get; set; }
        
        public bool HandlerDirectionMentionOnly { get; set; }
        public bool BotIsMentioned { get; set; }

        public double HandlerOdds { get; set; }
        public double? RandomRoll { get; set; }

        public string[] HandlerTargetedChannels { get; set; }
        public string MessageChannel { get; set; }

        public bool HandlerCanHandleResult { get;set; }
    }
}
