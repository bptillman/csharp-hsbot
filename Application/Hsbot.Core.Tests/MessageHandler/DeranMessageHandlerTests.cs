using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class DeranMessageHandlerTests : MessageHandlerTestBase<DeranMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new [] {"Deran", "deran"};
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] {"No d-ran here"};

        public async Task ShouldSayHiDeran()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.HandleAsync();
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages.First().Text.ShouldBe("http://i.imgur.com/reDPhBx.jpg");
        }
    }
}
