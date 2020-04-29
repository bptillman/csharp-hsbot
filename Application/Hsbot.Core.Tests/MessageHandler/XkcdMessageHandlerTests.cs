namespace Hsbot.Core.Tests.MessageHandler
{
    using System.Threading.Tasks;
    using Infrastructure;
    using MessageHandlers;
    using Shouldly;

    public class XkcdMessageHandlerTests : MessageHandlerTestBase<XkcdMessageHandler>
    {
        private const string ResponseString = "xkcd.com";

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
            response.SentMessages.Count.ShouldBe(2);
            response.SentMessages[0].IndicateTyping.ShouldBe(true);
            response.SentMessages[1].Text.ShouldContain(ResponseString);
        }

        public async Task ShouldGetLatestComicFromWebsiteWithKeyword()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.TestHandleAsync("xkcd latest");
            response.SentMessages.Count.ShouldBe(2);
            response.SentMessages[0].IndicateTyping.ShouldBe(true);
            response.SentMessages[1].Text.ShouldContain(ResponseString);
        }

        public async Task ShouldGetRandomComicFromWebsite()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.TestHandleAsync("xkcd random");
            response.SentMessages.Count.ShouldBe(2);
            response.SentMessages[0].IndicateTyping.ShouldBe(true);
            response.SentMessages[1].Text.ShouldContain(ResponseString);
        }

        public async Task ShouldGetSpecificComicFromWebsite()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.TestHandleAsync("xkcd 15");
            response.SentMessages.Count.ShouldBe(2);
            response.SentMessages[0].IndicateTyping.ShouldBe(true);
            response.SentMessages[1].Text.ShouldContain($"{ResponseString}");
        }

        protected override XkcdMessageHandler GetHandlerInstance()
        {
            var rng = new RandomNumberGeneratorFake { RandomIntValues = new[] { 24 }};
            var handler = new XkcdMessageHandler(rng);

            return handler;
        }
    }
}
