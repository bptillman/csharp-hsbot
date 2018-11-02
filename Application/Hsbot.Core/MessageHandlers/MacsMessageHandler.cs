using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class MacsMessageHandler : MessageHandlerBase
    {
        public string[] Pics =
        {
            "http://i.imgur.com/fEbhFJs.png",
            "http://i.imgur.com/NSj5Yed.jpg",
            "http://i.imgur.com/ET7lt18.png",
            "http://i.imgur.com/mFh3qzV.jpg",
            "http://i.imgur.com/KEgZLy1.jpg",
            "http://i.imgur.com/P004fny.jpg",
            "http://i.imgur.com/l2B10sl/jpg",
            "http://i.imgur.com/jmZyaFX.jpg",
            "http://i.imgur.com/IZqHEPM.jpg"
        };

        public MacsMessageHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public override string[] TargetedChannels => FunChannels;
        public override bool DirectMentionOnly => false;

        public override double GetHandlerOdds(InboundMessage message)
        {
            return .50;
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield break;
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.Contains("it just works");
        }

        public override Task HandleAsync(IBotMessageContext context)
        {
            var index = RandomNumberGenerator.Generate(0, Pics.Length - 1);

            return ReplyToChannel(context, Pics[index]);
        }
    }
}
