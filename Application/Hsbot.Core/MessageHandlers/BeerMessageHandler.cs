using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class BeerMessageHandler : MessageHandlerBase
    {
        public override string[] CannedResponses => new[]
        {
            "In wine there is wisdom, in beer there is freedom, in water there is bacteria",
            "Hello? Is it beer you're looking for?",
            "Keep calm, it's beer o'clock",
            ":dos_equis: I don't always drink beer, just kidding, or course I do.",
            "Beauty is in the eye of the beer holder",
            "Always do sober what you said you'd do drunk. That will teach you to keep your mouth shut.",
            "24 hours in a day, 24 beers in a case. Coincidence?",
            "Alcohol: the cause of, and solution to, all of life's problems.",
            ":beer:",
            ":beers:",
            ":shiner:",
            ":lone_star:"
        };

        public override string[] TargetedChannels => FunChannels;
        public override bool DirectMentionOnly => false;

        public BeerMessageHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield break;
        }

        public override Task HandleAsync(IInboundMessageContext messageContext)
        {
            var message = messageContext.Message;
            var response = message.Contains("coffee")
                ? message.CreateResponse("Coffee? How about a beer?")
                : message.CreateResponse(GetRandomCannedResponse());

            return messageContext.SendResponse(response);
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.Contains("beer") || message.Contains("coffee");
        }
    }
}
