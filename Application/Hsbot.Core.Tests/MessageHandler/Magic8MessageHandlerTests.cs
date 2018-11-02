using System;
using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class Magic8MessageHandlerTests : MessageHandlerTestBase<Magic8MessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[] { "Will I be famous?", "Will I go to Peru?" };
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] { "Yo yo yo HSBot!" };

        public async Task ShouldReturnMagic8Answer()
        {
            var rng1 = new RandomNumberGeneratorFake { NextIntValue = 0 };
            var messageHandler1 = GetHandlerInstance(rng1);
            var response1 = await messageHandler1.HandleAsync();
            response1.SentMessages.Count.ShouldBe(1);
            response1.SentMessages.First().Text.ShouldBe("It is certain");

            var rng2 = new RandomNumberGeneratorFake { NextIntValue = 10 };
            var messageHandler2 = GetHandlerInstance(rng2);
            var response2 = await messageHandler2.HandleAsync();
            response2.SentMessages.Count.ShouldBe(1);
            response2.SentMessages.First().Text.ShouldBe("Reply hazy try again");
        }
    }
}