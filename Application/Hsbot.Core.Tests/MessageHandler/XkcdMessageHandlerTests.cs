using Hsbot.Core.ApiClients;
using System.Threading.Tasks;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Hsbot.Core.MessageHandlers;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class XkcdMessageHandlerTests : MessageHandlerTestBase<XkcdMessageHandler>
    {
        private const string SomeAltText = "some alt text";
        private const string ResponseString = "xkcd.com";
        private const string NoResponseString = "Sorry";

        protected override string[] MessageTextsThatShouldBeHandled => new[]
        {
            "xkcd",
            "XKCD",
            "Xkcd latest",
            "xkcd 15",
            "xkcd Random",
            "xkcd but I don't know what I'm doing"
        };

        protected override string[] MessageTextsThatShouldNotBeHandled => new[]
        {
            "xkc"
        };

        public async Task ShouldGetLatestComicFromWebsite()
        {
            var messageHandler = GetHandlerInstance();

            var response = await messageHandler.TestHandleAsync("xkcd");

            response.SentMessages.Count.ShouldBe(3);
            response.SentMessages[0].IndicateTyping.ShouldBe(true);
            response.SentMessages[1].Text.ShouldContain(ResponseString);
            response.SentMessages[2].Text.ShouldBe(SomeAltText);
        }

        public async Task ShouldGetLatestComicFromWebsiteWithKeyword()
        {
            var messageHandler = GetHandlerInstance();

            var response = await messageHandler.TestHandleAsync("xkcd latest");

            response.SentMessages.Count.ShouldBe(3);
            response.SentMessages[0].IndicateTyping.ShouldBe(true);
            response.SentMessages[1].Text.ShouldContain(ResponseString);
            response.SentMessages[1].Text.ShouldNotContain("latest");
            response.SentMessages[2].Text.ShouldBe(SomeAltText);
        }

        public async Task ShouldGetRandomComicFromWebsite()
        {
            var messageHandler = GetHandlerInstance();

            var response = await messageHandler.TestHandleAsync("xkcd random");

            response.SentMessages.Count.ShouldBe(3);
            response.SentMessages[0].IndicateTyping.ShouldBe(true);
            response.SentMessages[1].Text.ShouldContain(ResponseString);
            response.SentMessages[1].Text.ShouldNotContain("random");
            response.SentMessages[2].Text.ShouldBe(SomeAltText);
        }

        public async Task ShouldGetSpecificComicFromWebsite()
        {
            var messageHandler = GetHandlerInstance();

            var response = await messageHandler.TestHandleAsync("xkcd 15");

            response.SentMessages.Count.ShouldBe(3);
            response.SentMessages[0].IndicateTyping.ShouldBe(true);
            response.SentMessages[1].Text.ShouldContain(ResponseString);
            response.SentMessages[1].Text.ShouldContain("15");
            response.SentMessages[2].Text.ShouldBe(SomeAltText);
        }

        public async Task ShouldGetNoResponseString()
        {
            var messageHandler = GetHandlerInstance();

            var response = await messageHandler.TestHandleAsync("xkcd this is wrong");

            response.SentMessages.Count.ShouldBe(2);
            response.SentMessages[0].IndicateTyping.ShouldBe(true);
            response.SentMessages[1].Text.ShouldContain($"{NoResponseString}");
        }

        protected override XkcdMessageHandler GetHandlerInstance()
        {
            var rng = new RandomNumberGeneratorFake { RandomIntValues = new[] { 24 }};
            var xkcdApiClient = new TestXkcdApiClient
            {
                InfoToReturn = new XkcdInfo
                {
                    Num = "123",
                    Img = "some image url",
                    Alt = SomeAltText,
                    Title = "some title from xkcd.com",
                }
            };

            var handler = new XkcdMessageHandler(rng, xkcdApiClient);

            return handler;
        }
    }
}
