using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class MessageHandlerFake : MessageHandlerBase
    {
        public string[] TargetedChannelsValue { get; set; } = AllChannels;
        public bool DirectMentionOnlyValue { get; set; } = true;
        public double HandlerOddsValue { get; set; } = 1.1;
        public bool CanHandleReturnValue { get; set; } = true;

        public override string[] TargetedChannels => TargetedChannelsValue;
        public override bool DirectMentionOnly => DirectMentionOnlyValue;

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

        public override Task HandleAsync(IBotMessageContext context)
        {
            return Task.CompletedTask;
        }
    }
}
