using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class CatBombHandlerTests : MessageHandlerTestBase<CatBombHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[] { "cat me", "cat bomb", "cat bomb NotANumber", "cat bomb 3" };
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] { "cat" };

        public async Task ShouldReturnOneImageForCatMe()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.TestHandleAsync("cat me");
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages.First().Text.ShouldBe("Image 0");
        }

        public async Task ShouldReturnFiveImagesForCatBomb()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.TestHandleAsync("cat bomb");
            response.SentMessages.Count.ShouldBe(5);
            response.SentMessages.All(m => m.Text == "Image 0").ShouldBeTrue();
        }

        public async Task ShouldReturnFiveImagesForCatBombWithInvalidNumber()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.TestHandleAsync("cat bomb NotANumber");
            response.SentMessages.Count.ShouldBe(5);
            response.SentMessages.All(m => m.Text == "Image 0").ShouldBeTrue();
        }

        public async Task ShouldReturnNImagesForCatBombN()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.TestHandleAsync("cat bomb 3");
            response.SentMessages.Count.ShouldBe(3);
            response.SentMessages.All(m => m.Text == "Image 0").ShouldBeTrue();

            response.SentMessages.Clear();

            response = await messageHandler.TestHandleAsync("cat bomb 10");
            response.SentMessages.Count.ShouldBe(10);
            response.SentMessages.All(m => m.Text == "Image 0").ShouldBeTrue();
        }

        protected override CatBombHandler GetHandlerInstance(BotProvidedServicesFake botProvidedServices = null)
        {
            var rng = new RandomNumberGeneratorFake();
            var apiClient = new TestTumblrApiClient { Photos = new[] { new TumblrPhoto { Url = "Image 0" } } };
            var handler = new CatBombHandler(rng, apiClient)
            {
                BotProvidedServices = botProvidedServices ?? new BotProvidedServicesFake()
            };

            return handler;
        }
    }
}
