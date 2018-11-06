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
            "Simpson Me sweet with meme caption one",
            "simpson me xqmrtvzpq123 with meme caption two",
            "Simpson me steamed hams with meme captionize it",
            "Simpson Gif me steamed hams with meme as funny as it is",
            "simpson gif me two words with meme another meme",
            "Simpson Gif Me three words phrase with meme final meme"
        };

        protected override string[] MessageTextsThatShouldNotBeHandled => new[]
        {
            "simpsons me wohoo",
            "simpson me",
            "simpson gif me",
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
            "do simpson Gif me anything",
            "simpson quote with meme",
            "simpsonMe phrase with memes",
            "show simpson me beer with meme no meme",
            "do simpson me anything with meme never a meme",
            "simpsons gif me wohoo with meme it wishes to be a meme",
            "Simpsons Gif Me sweet with meme never a meme",
        };

        public async Task ShouldGetImageFromWebsite()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.HandleAsync("simpson me steamed hams");
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages[0].Text.ShouldStartWith(ImageResponse);
            response.SentMessages[0].Text.ShouldNotContain("?b64lines=");
        }

        public async Task ShouldNotGetImageFromWebsite()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.HandleAsync("simpson me xqmrtvzpq123");
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages[0].Text.ShouldBe(ImageNotFound);
        }

        public async Task ShouldGetImageWithMemeFromWebsite()
        {
            var meme = "look, a caption!";
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.HandleAsync("simpson me steamed hams with meme " + meme);
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages[0].Text.ShouldStartWith(ImageResponse);
            response.SentMessages[0].Text.ShouldEndWith("?b64lines=" + SimpsonMessageHandler.Base64Encode(meme));
        }

        public async Task ShouldGetGifFromWebsite()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.HandleAsync("simpson gif me steamed hams");
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages[0].Text.ShouldStartWith(GifResponse);
            response.SentMessages[0].Text.ShouldNotContain("?b64lines=");
        }

        public async Task ShouldNotGetGifFromWebsite()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.HandleAsync("simpson gif me xqmrtvzpq123");
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages[0].Text.ShouldBe(GifNotFound);
        }

        public async Task ShouldGetGifWithMemeFromWebsite()
        {
            var meme = "burgers everywhere";
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.HandleAsync("simpson gif me steamed hams with meme " + meme);
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages[0].Text.ShouldStartWith(GifResponse);
            response.SentMessages[0].Text.ShouldEndWith("?b64lines=" + SimpsonMessageHandler.Base64Encode(meme));
        }
    }
}
