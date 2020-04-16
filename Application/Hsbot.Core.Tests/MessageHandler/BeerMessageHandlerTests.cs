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
            var rng = new RandomNumberGeneratorFake { NextIntValue = 0 };
            var messageHandler = GetHandlerInstance(rng);

            foreach (var cannedResponse in messageHandler.CannedResponses)
            {
                var response = await messageHandler.TestHandleAsync();
                response.SentMessages.Count.ShouldBe(1);
                response.SentMessages.First().Text.ShouldBe(cannedResponse);
                response.SentMessages.Clear();

                rng.NextIntValue += 1;
            }

            var coffeeResponse = await messageHandler.TestHandleAsync("Coffee");
            coffeeResponse.SentMessages.Count.ShouldBe(1);
            coffeeResponse.SentMessages.First().Text.ShouldBe("Coffee? How about a beer?");
        }
    }
}