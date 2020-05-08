using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class PugBombMessageHandlerTests : MessageHandlerTestBase<PugBombMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[] {"pug me", "pug bomb", "pug bomb 4"};
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] {"pug"};

        public async Task ShouldReturnOneImageForPugMe()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.TestHandleAsync("pug me");
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages.First().Text.ShouldBe("PugPoleonDynamite");
        }

        public async Task ShouldReturnFiveImagesForPugBomb()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.TestHandleAsync("pug bomb");
            response.SentMessages.Count.ShouldBe(5);
            response.SentMessages.All(m => m.Text == "PugPoleonDynamite").ShouldBeTrue();
        }

        public async Task ShouldReturnFiveImagesForPugBombWithInvalidNumber()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.TestHandleAsync("pug bomb NotANumber");
            response.SentMessages.Count.ShouldBe(5);
            response.SentMessages.All(m => m.Text == "PugPoleonDynamite").ShouldBeTrue();
        }

        public async Task ShouldReturnNImagesForPugBombN()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.TestHandleAsync("pug bomb 3");
            response.SentMessages.Count.ShouldBe(3);
            response.SentMessages.All(m => m.Text == "PugPoleonDynamite").ShouldBeTrue();

            response.SentMessages.Clear();

            response = await messageHandler.TestHandleAsync("pug bomb 10");
            response.SentMessages.Count.ShouldBe(10);
            response.SentMessages.All(m => m.Text == "PugPoleonDynamite").ShouldBeTrue();
        }

        protected override PugBombMessageHandler GetHandlerInstance()
        {
            var testPugClient = new TestPugClient
            {
                Pugs = Enumerable.Repeat(new PugInfo { Img = "PugPoleonDynamite" }, 20)
            };

            var rng = new RandomNumberGeneratorFake { NextDoubleValue = 0.0 };

            var handler = new PugBombMessageHandler(rng, testPugClient);

            return handler;
        }
    }
}
