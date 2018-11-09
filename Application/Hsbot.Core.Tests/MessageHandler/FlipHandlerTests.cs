using System.Threading.Tasks;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class FlipHandlerTests : MessageHandlerTestBase<FlipMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[] { "flip message", "megaflip message" };
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] {"random flip in middle of text", "random megaflip in middle of text"};

        public async Task ShouldFlipText()
        {
            var handler = GetHandlerInstance();

            var result = await handler.TestHandleAsync("flip test");
            result.SentMessages.Count.ShouldBe(1);
            result.SentMessages[0].Text.ShouldBe("(╯°□°）╯︵ ʇǝsʇ");
        }

        public async Task ShouldMegaFlipText()
        {
            var expectedResult = @"(╯°□°）╯︵
┳┳┳┳┳┳　　|
┓┏┓┏┓┃　ʇǝsʇ
┛┗┛┗┛┃
┓┏┓┏┓┃
┛┗┛┗┛┃
┓┏┓┏┓┃
┛┗┛┗┛┃
┓┏┓┏┓┃
┻┻┻┻┻┻";
            var handler = GetHandlerInstance();

            var result = await handler.TestHandleAsync("megaflip test");
            result.SentMessages.Count.ShouldBe(1);
            result.SentMessages[0].Text.ShouldBe(expectedResult);
        }
    }
}
