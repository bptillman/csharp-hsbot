using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class ChannelInfoMessageHandlerTests : MessageHandlerTestBase<ChannelInfoMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new [] {"admin channel info"};
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] {"channel info"};

        public async Task ShouldSayChannelInfo()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.TestHandleAsync();
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages.First().Text.ShouldBe("Channel: Fake channel\nChannel name: Fake channel");
        }
    }
}
