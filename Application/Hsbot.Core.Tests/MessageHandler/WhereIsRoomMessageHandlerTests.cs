using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class WhereIsRoomMessageHandlerTests : MessageHandlerTestBase<WhereIsRoomMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new [] {"where is promethium", "where is"};
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] {"where my cat", "were is"};

        public async Task ShouldReturnImageForPromethiumLocation()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.HandleAsync("where is promethium");
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages.First().Attachments.First().ImageUrl.ShouldBe("https://i.imgur.com/1S6ieRs.png");
        }

        public async Task ShouldReturnBarkForMissingRoom()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.HandleAsync("where is");
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages.First().Text.ShouldBe("Gimme a room name to look for!");
        }

        public async Task ShouldReturnBarkForRoomNotFound()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.HandleAsync("where is tiger");
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages.First().Text.ShouldNotBeNullOrEmpty();
            response.SentMessages.First().Attachments.ShouldBeEmpty();
        }

        public async Task ShouldReturnJuraForCofeeRoom()
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.HandleAsync("where is coffee");
            response.SentMessages.Count.ShouldBe(1);
            response.SentMessages.First().Text.ShouldBe(":jura:");
        }
    }
}
