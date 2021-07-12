using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class SubmitTimeSheetHandlerTests : MessageHandlerTestBase<SubmitTimeSheetHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new [] {"submit your time", "submit something time", "submit time", "SUBMIT YOUR TIME", "Submit your time" };
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] {"submit", "time", "something" };

        public async Task ShouldReturnResponse()
        {
            var rng = new RandomNumberGeneratorFake { NextIntValue = 0 };
            var messageHandler = GetHandlerInstance(rng);

            var response = await messageHandler.TestHandleAsync();
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages.First().Text.ShouldBe("https://media.giphy.com/media/QNv4mKyspa8j6/giphy.gif");
        }
    }
}
