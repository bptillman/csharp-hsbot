namespace Hsbot.Core.Tests.MessageHandler
{
    using System.Threading.Tasks;
    using Infrastructure;
    using MessageHandlers;
    using Shouldly;

    public class SimpsonMessageHandlerTests : MessageHandlerTestBase<SimpsonMessageHandler>
    {
        private const string ImageResponse = "https://frinkiac.com/img/";
        private const string GifResponse = "https://frinkiac.com/gif/";
        private const string ImageNotFound = "(doh) no images fit that quote";
        private const string GifNotFound = "(doh) no gifs fit that quote";

        protected override string[] MessageTextsThatShouldBeHandled => new[]
        {
            "simpson me wohoo",
            "Simpson Me sweet",
            "simpson me xqmrtvzpq123",
            "Simpson me steamed hams",
            "simpson me two words",
            "Simpson Me three words phrase",
            "simpson gif me wohoo",
            "Simpson Gif Me sweet",
            "simpson gif me xqmrtvzpq123",
            "Simpson Gif me steamed hams",
            "simpson gif me two words",
            "Simpson Gif Me three words phrase",
        };

        protected override string[] MessageTextsThatShouldNotBeHandled => new[]
        {
            "simpsons me wohoo",
            "Simpsons Me sweet",
            "simpson quote",
            "simpsonMe phrase",
            "show simpson me beer",
            "do simpson me anything",
            "simpsons gif me wohoo",
            "Simpsons Gif Me sweet",
            "simpson gif quote",
            "simpsonMe Gif phrase",
            "show simpson gif me beer",
            "do simpson Gif me anything"
        };

        public async Task ShouldGetImageFromWebsite()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.HandleAsync("simpson me steamed hams");
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages[0].Text.ShouldStartWith(ImageResponse);
        }

        public async Task ShouldNotGetImageFromWebsite()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.HandleAsync("simpson me xqmrtvzpq123");
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages[0].Text.ShouldBe(ImageNotFound);
        }

        public async Task ShouldGetGifFromWebsite()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.HandleAsync("simpson gif me steamed hams");
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages[0].Text.ShouldStartWith(GifResponse);
        }

        public async Task ShouldNotGetGifFromWebsite()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.HandleAsync("simpson gif me xqmrtvzpq123");
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages[0].Text.ShouldBe(GifNotFound);
        }
    }
}
