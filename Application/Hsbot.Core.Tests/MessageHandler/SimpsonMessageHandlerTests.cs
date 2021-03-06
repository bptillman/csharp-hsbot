using System.Collections.Generic;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Hsbot.Core.MessageHandlers;
using Shouldly;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;

namespace Hsbot.Core.Tests.MessageHandler
{

    public class SimpsonMessageHandlerTests : MessageHandlerTestBase<SimpsonMessageHandler>
    {
        private const string ImageResponse = "https://frinkiac.com/img/";
        private const string ImageWithMemeResponse = "https://frinkiac.com/meme/";
        private const string GifResponse = "https://frinkiac.com/gif/";
        private const string ImageNotFound = ":doh: no images fit that quote";
        private const string GifNotFound = ":doh: no gifs fit that quote";

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
            var response = await messageHandler.TestHandleAsync("simpson me steamed hams");
            response.SentMessages.Count.ShouldBe(2);
            response.SentMessages[0].IndicateTyping.ShouldBe(true);
            response.SentMessages[1].Text.ShouldStartWith(ImageResponse);
            response.SentMessages[1].Text.ShouldNotContain("?b64lines=");
        }

        public async Task ShouldNotGetImageFromWebsite()
        {
            var messageHandler = GetHandlerInstance(new List<FrinkiacImage>());
            var response = await messageHandler.TestHandleAsync("simpson me xqmrtvzpq123");
            response.SentMessages.Count.ShouldBe(2);
            response.SentMessages[0].IndicateTyping.ShouldBe(true);
            response.SentMessages[1].Text.ShouldBe(ImageNotFound);
        }

        public async Task ShouldGetImageWithMemeFromWebsite()
        {
            var meme = "look, a caption!";
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.TestHandleAsync("simpson me steamed hams with meme " + meme);
            response.SentMessages.Count.ShouldBe(2);
            response.SentMessages[1].Text.ShouldStartWith(ImageWithMemeResponse);
            response.SentMessages[1].Text.ShouldEndWith("?b64lines=" + SimpsonMessageHandler.Base64Encode(meme));
        }

        public async Task ShouldGetGifFromWebsite()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.TestHandleAsync("simpson gif me steamed hams");
            response.SentMessages.Count.ShouldBe(2);
            response.SentMessages[0].IndicateTyping.ShouldBe(true);
            response.SentMessages[1].Text.ShouldStartWith(GifResponse);
            response.SentMessages[1].Text.ShouldNotContain("?b64lines=");
        }

        public async Task ShouldNotGetGifFromWebsite()
        {
            var messageHandler = GetHandlerInstance(new List<FrinkiacImage>());
            var response = await messageHandler.TestHandleAsync("simpson gif me xqmrtvzpq123");
            response.SentMessages.Count.ShouldBe(2);
            response.SentMessages[1].Text.ShouldBe(GifNotFound);
        }

        public async Task ShouldGetGifWithMemeFromWebsite()
        {
            var meme = "burgers everywhere";
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.TestHandleAsync("simpson gif me steamed hams with meme " + meme);
            response.SentMessages.Count.ShouldBe(2);
            response.SentMessages[0].IndicateTyping.ShouldBe(true);
            response.SentMessages[1].Text.ShouldStartWith(GifResponse);
            response.SentMessages[1].Text.ShouldEndWith("?b64lines=" + SimpsonMessageHandler.Base64Encode(meme));
        }

        protected override SimpsonMessageHandler GetHandlerInstance()
        {
            return GetHandlerInstance(new[]
            {
                new FrinkiacImage
                {
                    Id = 123,
                    Episode = "An Episode",
                    TimeStamp = 123123,
                }
            });
        }

        private SimpsonMessageHandler GetHandlerInstance(IEnumerable<FrinkiacImage> images)
        {
            //Since this RNG will always return 0, the check on the random roll in the handler will
            //always succeed, meaning the random roll will not cause the result of ShouldHandle
            //to be false
            var rng = new RandomNumberGeneratorFake { NextDoubleValue = 0.0 };

            var testSimpsonsApiClient = new TestSimpsonApiClient
            {
                Images = images,
            };

            var handler = new SimpsonMessageHandler(rng, testSimpsonsApiClient);

            return handler;
        }
    }
}
