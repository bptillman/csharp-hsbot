using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class MessageHandlerFake : MessageHandlerBase
    {
        public string[] TargetedChannelsValue { get; set; } = AllChannels;
        public bool DirectMentionOnlyValue { get; set; } = true;
        public double HandlerOddsValue { get; set; } = 1.1;
        public bool CanHandleReturnValue { get; set; } = true;
        public string[] CannedResponsesValue { get; set; }

        public override string[] TargetedChannels => TargetedChannelsValue;
        public override bool DirectMentionOnly => DirectMentionOnlyValue;
        public override string[] CannedResponses => CannedResponsesValue;

        public override double GetHandlerOdds(InboundMessage message)
        {
            return HandlerOddsValue;
        }

        public MessageHandlerFake(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield return new MessageHandlerDescriptor();
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return CanHandleReturnValue;
        }

        public override Task HandleAsync(IInboundMessageContext context)
        {
            return Task.CompletedTask;
        }
    }
}
