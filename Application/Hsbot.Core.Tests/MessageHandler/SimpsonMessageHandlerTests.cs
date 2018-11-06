namespace Hsbot.Core.Tests.MessageHandler
{
    using System.Threading.Tasks;
    using Infrastructure;
    using MessageHandlers;
    using Shouldly;

    public class SimpsonMessageHandlerTests : MessageHandlerTestBase<SimpsonMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[]
        {
            "simpson me wohoo",
            "Simpson Me sweet",
            "simpson me two words",
            "Simpson Me three words phrase"
        };

        protected override string[] MessageTextsThatShouldNotBeHandled => new[]
        {
            "simpsons me wohoo",
            "Simpsons Me sweet",
            "simpson quote",
            "simpsonMe phrase",
            "show simpson me beer",
            "do simpson me anything"
        };

        public async Task ShouldQueryWebsiteForImage()
        {
            var emptyResponse = "(doh) no images fit that";
            var imageResponse = "http://frinkiac.com/img/";
            var errorResponse = "Error: Service is not wo";

            var messageHandler = GetHandlerInstance();

            foreach (var command in MessageTextsThatShouldBeHandled)
            {
                var response = await messageHandler.HandleAsync(command);

                response.SentMessages.Count.ShouldBe(1);
                var message = response.SentMessages[0].Text.Substring(0, imageResponse.Length);
                message.ShouldBeOneOf(new [] { emptyResponse, imageResponse, errorResponse });
            }
        }
    }
}
