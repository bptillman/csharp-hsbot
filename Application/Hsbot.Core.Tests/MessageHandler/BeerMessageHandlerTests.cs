using System;
using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class BeerMessageHandlerTests : MessageHandlerTestBase<BeerMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[] { "Does anyone want coffee?", "What is your favorite beer?", "Beer me!" };
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] { "I think I'm going to make tea" };

        public async Task ShouldReturnBeerAnswers()
        {
            var rng1 = new RandomNumberGeneratorFake { NextIntValue = 0 };
            var messageHandler1 = GetHandlerInstance(rng1);
            var response1 = await messageHandler1.HandleAsync();
            response1.SentMessages.Count.ShouldBe(1);
            response1.SentMessages.First().Text.ShouldBe("In wine there is wisdom, in beer there is freedom, in water there is bacteria");

            var rng2 = new RandomNumberGeneratorFake { NextIntValue = 6 };
            var messageHandler2 = GetHandlerInstance(rng2);
            var response2 = await messageHandler2.HandleAsync();
            response2.SentMessages.Count.ShouldBe(1);
            response2.SentMessages.First().Text.ShouldBe("24 hours in a day, 24 beers in a case. Coincidence?");
        }
    }
}