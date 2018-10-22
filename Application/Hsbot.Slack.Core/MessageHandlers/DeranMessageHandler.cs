using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Slack.Core.Messaging;
using Hsbot.Slack.Core.Random;

namespace Hsbot.Slack.Core.MessageHandlers
{
    public class DeranMessageHandler : MessageHandlerBase
    {
        public DeranMessageHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public override string[] TargetedChannels => FunChannels;
        public override bool DirectMentionOnly => false;

        public override double GetHandlerOdds(InboundMessage message)
        {
            return 0.1;
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield break;
        }

        public override Task HandleAsync(BotMessageContext context)
        {
            return context.ReplyToChannel("http://i.imgur.com/reDPhBx.jpg");
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.Contains("deran");
        }
    }
}
