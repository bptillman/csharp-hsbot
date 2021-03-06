using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class DeranMessageHandler : MessageHandlerBase
    {
        public DeranMessageHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public override string[] TargetedChannels => FunChannels;
        public override bool DirectMentionOnly => false;

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield break;
        }

        public override Task HandleAsync(IInboundMessageContext context)
        {
            return context.SendResponse("http://i.imgur.com/reDPhBx.jpg");
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.Contains("deran");
        }
    }
}
