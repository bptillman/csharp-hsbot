using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class ChannelInfoMessageHandler : MessageHandlerBase
    {
        public ChannelInfoMessageHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            return new List<MessageHandlerDescriptor>();
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.StartsWith("admin channel info");
        }

        public override async Task HandleAsync(IInboundMessageContext context)
        {
            var message = context.Message;
            await context.SendResponse($"Channel: {message.Channel}\nChannel name: {message.ChannelName}");
        }
    }
}
