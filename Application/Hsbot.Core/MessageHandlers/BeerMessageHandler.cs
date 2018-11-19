using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class BeerMessageHandler : MessageHandlerBase
    {
        public BeerMessageHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public override string[] TargetedChannels => FunChannels;
        public override bool DirectMentionOnly => false;

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield return new MessageHandlerDescriptor { Command = "will <ask your question with a question mark>?", Description = "Ask the Magic 8 ball to predict the future!!" };
        }

        public static readonly string[] Quips =
        {
            "In wine there is wisdom, in beer there is freedom, in water there is bacteria",
            "Hello? Is it beer you're looking for?",
            "Keep calm, it's beer o'clock",
            ":dosequis: I don't always drink beer, just kidding, or course I do.",
            "Beauty is in the eye of the beer holder",
            "Always do sober what you said you'd do drunk. That will teach you to keep your mouth shut.",
            "24 hours in a day, 24 beers in a case. Coincidence?",
            "Alcohol: the cause of, and solution to, all of life's problems.",
            ":beer:",
            ":shiner:",
            ":lonestar:",
            ""
        };

        public override Task HandleAsync(IBotMessageContext context)
        {
            if (context.Message.Contains("coffee"))
            {
                return ReplyToChannel(context, "Coffee? How about a beer?");
            }
            var random = RandomNumberGenerator.Generate(0, Quips.Length);
            return ReplyToChannel(context, Quips[random]);
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.Contains("beer") || message.Contains("coffee");
        }
    }
}
