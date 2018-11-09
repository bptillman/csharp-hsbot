using System.Threading.Tasks;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class DndMessageHandlerTests : MessageHandlerTestBase<DndMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[] { "roll d20", "roll 2d6", "roll 1d4+2" };
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] { "roll", "d20", "2d6", "1d4+2" };

        public async Task ShouldRollSingleDice()
        {
            var rng = new RandomNumberGeneratorFake {NextIntValue = 5};
            var handler = GetHandlerInstance(rng);

            var result = await handler.HandleAsync("roll d6");
            result.SentMessages.Count.ShouldBe(1);
            result.SentMessages[0].Text.ShouldBe("rolled a 5 (5)+0");
        }

        public async Task ShouldRollMultipleDice()
        {
            var rng = new RandomNumberGeneratorFake {RandomIntValues = new []{ 1, 2, 3, 4, 5, 6}};
            var handler = GetHandlerInstance(rng);

            var result = await handler.HandleAsync("roll 6d6");
            result.SentMessages.Count.ShouldBe(1);
            result.SentMessages[0].Text.ShouldBe("rolled a 21 (1, 2, 3, 4, 5, 6)+0");
        }

        public async Task ShouldRollMultipleDiceWithModifier()
        {
            var rng = new RandomNumberGeneratorFake {RandomIntValues = new []{ 1, 2, 3, 4, 5, 6}};
            var handler = GetHandlerInstance(rng);

            var result = await handler.HandleAsync("roll 6d6+4");
            result.SentMessages.Count.ShouldBe(1);
            result.SentMessages[0].Text.ShouldBe("rolled a 25 (1, 2, 3, 4, 5, 6)+4");
        }
    }
}
