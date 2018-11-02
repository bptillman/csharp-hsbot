using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class MacsMessageHandlerTests : MessageHandlerTestBase<MacsMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[] {"it just works", "because it just works that way"};
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] {"it doesn't work"};

        public async Task ShouldReturnUrl()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.HandleAsync();
            response.SentMessages.Count.ShouldBe(1);
            messageHandler.CannedResponses.ShouldContain(response.SentMessages.First().Text);
        }
    }
}
