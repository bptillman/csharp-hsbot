using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class ThesaurusMessageHandlerTests : MessageHandlerTestBase<ThesaurusMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[] {"lookup bad", "look up this word", "choices for word"};
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] {"look up", "lookup", "random"};

        public async Task ShouldLetUsKnowIfNothingIsReturned()
        {
            var handler = GetHandlerInstance();

            var result = await handler.TestHandleAsync("lookup anything");

            result.SentMessages.Count.ShouldBe(2);
            result.SentMessages[0].IndicateTyping.ShouldBeTrue();
            result.SentMessages[1].Text.ShouldBe("anything means nothing to me");
        }

        public async Task ShouldLetUsKnowTheInfoItFound()
        {
            var handler = GetHandlerInstance(new ThesaurusResponse
            {
                Definitions = new[] {"definition", "other definition"},
                Synonyms = new[] {"word", "other word"},
                Antonyms = new[] {"not word", "not other word"}
            });

            var result = await handler.TestHandleAsync("lookup anything");

            result.SentMessages.Count.ShouldBe(5);
            result.SentMessages[0].IndicateTyping.ShouldBeTrue();
            result.SentMessages[1].Text.ShouldBe("Information for *anything*:");
            result.SentMessages[2].Text.ShouldBe("Synonyms: word, other word");
            result.SentMessages[3].Text.ShouldBe("Antonyms: not word, not other word");
            result.SentMessages[4].Text.ShouldBe("Definitions:\n\t - definition\n\t - other definition");
        }

        protected override ThesaurusMessageHandler GetHandlerInstance()
        {
            return GetHandlerInstance(new ThesaurusResponse());
        }

        private ThesaurusMessageHandler GetHandlerInstance(ThesaurusResponse response)
        {
            var rng = new RandomNumberGeneratorFake { RandomIntValues = new[] { 0 } };
            var apiClient = new TestThesaurusApiClient
            {
                Response = response,
            };

            var handler = new ThesaurusMessageHandler(rng, apiClient);

            return handler;
        }
    }
}