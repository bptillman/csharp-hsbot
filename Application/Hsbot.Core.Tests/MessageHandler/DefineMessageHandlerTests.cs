using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class DefineMessageHandlerTests : MessageHandlerTestBase<DefineMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[]
        {
            "define word",
            "define two words",
        };

        protected override string[] MessageTextsThatShouldNotBeHandled => new[]
        {
            "define",
            "dfine",
        };

        public async Task ShouldLetUsKnowIfNothingIsReturned()
        {
            var handler = GetHandlerInstance();

            var result = await handler.TestHandleAsync("define anything");

            result.SentMessages.Count.ShouldBe(2);
            result.SentMessages[0].IndicateTyping.ShouldBeTrue();
            result.SentMessages[1].Text.ShouldBe("anything means nothing to me");
        }

        public async Task ShouldLetUsKnowWhatDefinitionsItFound()
        {
            var handler = GetHandlerInstance(new DictionaryResponse
            {
                Definitions = new[] {"definition", "another definition"}
            });

            var result = await handler.TestHandleAsync("define anything");

            result.SentMessages.Count.ShouldBe(2);
            result.SentMessages[0].IndicateTyping.ShouldBeTrue();
            result.SentMessages[1].Text.ShouldBe("Here are definitions for *anything*:\n\t- definition\n\t- another definition");
        }

        public async Task ShouldLetUsKnowWhatRecommendationsItFound()
        {
            var handler = GetHandlerInstance(new DictionaryResponse
            {
                Recommendations = new[] {"word", "other word"}
            });

            var result = await handler.TestHandleAsync("define anything");

            result.SentMessages.Count.ShouldBe(2);
            result.SentMessages[0].IndicateTyping.ShouldBeTrue();
            result.SentMessages[1].Text.ShouldBe("Did you mean: word, other word");
        }

        protected override DefineMessageHandler GetHandlerInstance()
        {
            return GetHandlerInstance(new DictionaryResponse());
        }

        private DefineMessageHandler GetHandlerInstance(DictionaryResponse response)
        {
            var rng = new RandomNumberGeneratorFake { RandomIntValues = new[] { 0 } };
            var apiClient = new TestDictionaryApiClient()
            {
                Response = response,
            };

            var handler = new DefineMessageHandler(rng, apiClient);

            return handler;
        }
    }
}
