using System.Collections.Generic;
using System.Linq;
using Hsbot.Core.MessageHandlers.Celebrations;
using Hsbot.Core.Messaging;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler.Celebrations
{
    public class NominationCelebrationTests
    {
        [TestCaseSource(nameof(GetMessagesToTestMatching))]
        public void ShouldMatchProperly(string message, bool isMatch)
        {
            var celebration = new NominationCelebration(new TestJiraApiClient(), new RandomNumberGeneratorFake());

            var match = celebration.GetMatch(new InboundMessage {TextWithoutBotName = message});

            match.Success.ShouldBe(isMatch);
        }

        public void ShouldParseMultipleUsers()
        {
            var celebration = new NominationCelebration(new TestJiraApiClient(), new RandomNumberGeneratorFake());
            var match = celebration.GetMatch(new InboundMessage {TextWithoutBotName = "hva for <@bob> for dfe this is a long time coming" });

            var users = celebration.GetNomineeUserIds(match).ToArray();

            users.ShouldBe(new[] {"bob"});
        }

        public void ShouldReturnCorrectAwardResponseToValidateMapping()
        {
            var celebration = new NominationCelebration(new TestJiraApiClient(), new RandomNumberGeneratorFake());

            var match = celebration.GetMatch(new InboundMessage { TextWithoutBotName = "hva for <@bob> for dfe this is a long time coming" });

            var message = celebration.GetAwardRoomMessage(new[] {("bob", "123")}, "doug", match);
            message.ShouldBe("bob exhibits *_Drive for Excellence_* this is a long time coming nominated by: _doug_ [123]");
        }

        private static IEnumerable<object[]> GetMessagesToTestMatching() => new List<object[]>
        {
            new object[] {"hva for <@bob> for dfe this is a long time coming", true},
            new object[] {"hva for <@bob> for dfe because", true},
            new object[] {"hva to <@bob> for dfe this is a long time coming", true},
            new object[] {"hva <@bob> for dfe this is a long time coming", true},

            new object[] {"hva for bob for dfe this is a long time coming", false},
            new object[] {"hva for <@bob> for dfe", false},
            new object[] {"hva for <@bob> for something i don't know the names of the awards", false},
            new object[] {"hva for <@bob> dfe something to test", false},
        };
    }
}