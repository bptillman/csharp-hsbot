using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class PugBombMessageHandlerTests : MessageHandlerTestBase<PugBombMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[] {"pug me", "pug bomb", "pug bomb 4"};
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] {"pug"};

    }
}
