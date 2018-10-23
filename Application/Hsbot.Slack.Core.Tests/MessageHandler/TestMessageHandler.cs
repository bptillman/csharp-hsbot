using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Hsbot.Slack.Core.MessageHandlers;
using Hsbot.Slack.Core.Messaging;
using Hsbot.Slack.Core.Random;

namespace Hsbot.Slack.Core.Tests.MessageHandler
{
    public class TestMessageHandler : MessageHandlerBase
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

        public TestMessageHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
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

        public override Task HandleAsync(BotMessageContext context)
        {
            return Task.CompletedTask;
        }
    }
}
