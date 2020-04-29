using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class NopeBombHandlerTests : MessageHandlerTestBase<NopeBombHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[] { "nope", "noope", "noooooooooooooooooooope", "NOPE" };
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] { "", "npe" };

        [TestCaseSource(nameof(GetMessageTestCases))]
        public async Task ShouldReturnOneImagePerO(string testMessage, int numberOfMessagesExpected)
        {
            var messageHandler = GetHandlerInstance();
            var response = await messageHandler.TestHandleAsync(testMessage);
            response.SentMessages.Count.ShouldBe(numberOfMessagesExpected);
            response.SentMessages.All(m => m.Text == "Image 0").ShouldBeTrue();
        }

        protected override NopeBombHandler GetHandlerInstance()
        {
            var rng = new RandomNumberGeneratorFake();
            var apiClient = new TestTumblrApiClient { Photos = new[] { new TumblrPhoto { Url = "Image 0" } } };
            var handler = new NopeBombHandler(rng, apiClient);

            return handler;
        }

        private static IEnumerable<object[]> GetMessageTestCases()
        {
            for (var i = 1; i <= 11; i++)
            {
                yield return new object[] {GetNope(i), Math.Min(i, 10)};
            }
        }

        private static string GetNope(int oCount)
        {
            return oCount <= 0 ? "npe" : $"n{new string('o', oCount)}pe";
        }
    }
}
