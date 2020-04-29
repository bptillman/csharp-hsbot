using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hsbot.Core.Brain;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.Brain;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class StorageMessageHandlerTests : MessageHandlerTestBase<StorageMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[]
        {
            "show storage",
            "show users"
        };

        protected override string[] MessageTextsThatShouldNotBeHandled => new[]
        {
            "show foo",
            "show storages",
            "show storage extra",
            "show user",
            "show users extra"
        };

        private class TestClass
        {
            public int IntValue { get; set; }
            public string StringValue { get; set; }
        }

        public async Task ShouldExportBrainAsJson()
        {
            var brain = new InMemoryBrain();
            var handler = GetHandlerInstance(brain);
            var message = handler.GetTestMessageThatWillBeHandled("show storage");
            var context = new TestInboundMessageContext(message);

            brain.SetItem("Test", new TestClass {IntValue = 23, StringValue = "foo bar"});

            await handler.HandleAsync(context);

            context.FileUploads.Count.ShouldBe(1);

            var fileName = context.FileUploads.Keys.First();
            fileName.ShouldStartWith("HsBotBrainDump");
            fileName.ShouldEndWith(".json");

            var fileContents = context.FileUploads[fileName].AsString(Encoding.UTF8);
            fileContents.ShouldBe("{\"Test\":{\"IntValue\":23,\"StringValue\":\"foo bar\"}}");
        }

        public async Task ShouldExportUsersAsCsv()
        {
            var handler = GetHandlerInstance();
            var message = handler.GetTestMessageThatWillBeHandled("show users");
            var context = new TestInboundMessageContext(message);
            context.ChatUsers.Add("Foo", new TestChatUser { Email = "foo@bar.com", FullName = "Foo Bar", Id = "Foo", IsEmployee = false});
            context.ChatUsers.Add("Bar", new TestChatUser { Email = "bar@foo.com", FullName = "Bar Foo", Id = "Bar", IsEmployee = true });

            await handler.HandleAsync(context);

            context.FileUploads.Count.ShouldBe(1);

            var fileName = context.FileUploads.Keys.First();
            fileName.ShouldStartWith("HsBotUserCache");
            fileName.ShouldEndWith(".csv");

            var fileContents = context.FileUploads[fileName].AsString(Encoding.UTF8);
            fileContents.ShouldBe(@"Id,Full Name,Email,Employee
Bar,Bar Foo,bar@foo.com,True
Foo,Foo Bar,foo@bar.com,False
");
        }

        protected override StorageMessageHandler GetHandlerInstance()
        {
            var brain = new FakeBrain();
            return GetHandlerInstance(brain);
        }

        private StorageMessageHandler GetHandlerInstance(IBotBrain brain)
        {
            var rng = new RandomNumberGeneratorFake();
            return new StorageMessageHandler(rng, brain);
        }
    }
}
