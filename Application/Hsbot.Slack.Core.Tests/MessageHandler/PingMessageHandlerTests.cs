using System.Linq;
using System.Threading.Tasks;
using Hsbot.Slack.Core.MessageHandlers;
using Shouldly;

namespace Hsbot.Slack.Core.Tests.MessageHandler
{
    public class PingMessageHandlerTests : MessageHandlerTestBase<PingMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new [] {"ping"};
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] {"bling"};

        public async Task ShouldSayPong()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.HandleAsync();
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages.First().Text.ShouldBe("Pong!");
        }
    }
}
