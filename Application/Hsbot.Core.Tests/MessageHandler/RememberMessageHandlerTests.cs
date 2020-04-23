using System.Threading.Tasks;
using Hsbot.Core.BotServices;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Messaging.Formatting;
using Hsbot.Core.Tests.BotServices;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class RememberMessageHandlerTests : MessageHandlerTestBase<RememberMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[]
        {
            "what is key",
            "what is key?",
            "remember key is value",
            "what do you remember",
            "what do you remember?",
            "forget key",
            "what are your favorite memories",
            "what are your favorite memories?",
            "random memory"
        };

        public async Task ShouldRememberExistingMemory()
        {
            var memoryService = new FakeMemoryService();
            memoryService.Remember("foo", "bar");

            var handler = GetHandlerInstance(memoryService);

            var context = await handler.TestHandleAsync("what is foo");
            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages[0].Text.ShouldBe("bar");
        }

        public async Task ShouldNotRememberMemoryThatDoesNotExist()
        {
            var memoryService = new FakeMemoryService();
            memoryService.Remember("foo", "bar");

            var handler = GetHandlerInstance(memoryService);

            var context = await handler.TestHandleAsync("what is baz");
            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages[0].Text.ShouldBe("I don't remember anything matching `baz`");
        }

        public async Task ShouldRememberMemory()
        {
            var memoryService = new FakeMemoryService();

            var handler = GetHandlerInstance(memoryService);

            var context = await handler.TestHandleAsync("remember foo is bar");
            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages[0].Text.ShouldBe("Ok, I'll remember foo is bar");

            memoryService.HasMemory("foo", out var memory).ShouldBe(true);
            memory.Value.ShouldBe("bar");
        }

        public async Task ShouldUpdateMemoryCountUponRemembering()
        {
            var memoryService = new FakeMemoryService();
            memoryService.Remember("foo", "bar");

            var handler = GetHandlerInstance(memoryService);

            var context = await handler.TestHandleAsync("what is foo");
            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages[0].Text.ShouldBe("bar");

            memoryService.HasMemory("foo", out var memory).ShouldBe(true);
            memory.Value.ShouldBe("bar");
            memory.RememberCount.ShouldBe(1);
        }

        public async Task ShouldListAllMemories()
        {
            var memoryService = new FakeMemoryService();
            memoryService.Remember("foo", "bar");
            memoryService.Remember("baz", "qux");
            memoryService.Remember("quux", "quuz");

            var handler = GetHandlerInstance(memoryService);

            var context = await handler.TestHandleAsync("what do you remember");
            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages[0].Text.ShouldBe("I remember:\r\nbaz\r\nfoo\r\nquux");
        }

        public async Task ShouldForgetMemory()
        {
            var memoryService = new FakeMemoryService();
            memoryService.Remember("foo", "bar");

            var handler = GetHandlerInstance(memoryService);

            var context = await handler.TestHandleAsync("forget foo");
            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages[0].Text.ShouldBe("Ok, I'll forget that foo is bar");

            memoryService.HasMemory("foo", out var memory).ShouldBe(false);
        }

        public async Task ShouldOrderFavoriteMemoriesByMemoryCount()
        {
            var memoryService = new FakeMemoryService();
            memoryService.Remember(new Memory { Key = "foo", Value = "value", RememberCount = 1 });
            memoryService.Remember(new Memory { Key = "bar", Value = "value", RememberCount = 4 });
            memoryService.Remember(new Memory { Key = "baz", Value = "value", RememberCount = 2 });
            memoryService.Remember(new Memory { Key = "qux", Value = "value", RememberCount = 3 });
            memoryService.Remember(new Memory { Key = "quux", Value = "value", RememberCount = 5 });
            memoryService.Remember(new Memory { Key = "quuz", Value = "value", RememberCount = 6 });

            var handler = GetHandlerInstance(memoryService);

            var context = await handler.TestHandleAsync("what are your favorite memories");
            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages[0].Text.ShouldBe(@"My favorite memories are:
quuz
quux
bar
qux
baz");

        }

        public async Task ShouldGetRandomMemory()
        {
            var memoryService = new FakeMemoryService();
            memoryService.Remember(new Memory { Key = "foo", Value = "foo value", RememberCount = 1 });
            memoryService.Remember(new Memory { Key = "bar", Value = "bar value", RememberCount = 2 });
            memoryService.Remember(new Memory { Key = "baz", Value = "baz value", RememberCount = 3 });

            var rng = new RandomNumberGeneratorFake {RandomIntValues = new[] {0, 1, 2}};

            var handler = GetHandlerInstance(memoryService, rng);

            var context = await handler.TestHandleAsync("random memory");
            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages[0].Text.ShouldBe("bar\r\nbar value");

            context = await handler.TestHandleAsync("random memory");
            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages[0].Text.ShouldBe("baz\r\nbaz value");

            context = await handler.TestHandleAsync("random memory");
            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages[0].Text.ShouldBe("foo\r\nfoo value");
        }

        protected override RememberMessageHandler GetHandlerInstance()
        {
            var rng = new RandomNumberGeneratorFake();
            var memoryService = new FakeMemoryService();
            var formatter = new InlineChatMessageTextFormatter();

            var handler = new RememberMessageHandler(rng, memoryService, formatter);

            return handler;
        }

        private RememberMessageHandler GetHandlerInstance(FakeMemoryService memoryService, RandomNumberGeneratorFake rng = null)
        {
            var formatter = new InlineChatMessageTextFormatter();

            var handler = new RememberMessageHandler(rng ?? new RandomNumberGeneratorFake(), memoryService, formatter);

            return handler;
        }
    }
}
