using System.Collections.Generic;
using Hsbot.Core.ApiClients;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class PugBombMessageHandlerTests : MessageHandlerTestBase<PugBombMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[] {"pug me", "pug bomb", "pug bomb 4"};
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] {"pug"};

        protected override PugBombMessageHandler GetHandlerInstance()
        {
            var testPugClient = new TestPugClient
            {
                Pugs = new List<PugInfo>() { new PugInfo { Img = "PugPoleonDynamite"} }
            };

            var rng = new RandomNumberGeneratorFake { NextDoubleValue = 0.0 };

            var handler = new PugBombMessageHandler(rng, testPugClient);

            return handler;
        }
    }
}
