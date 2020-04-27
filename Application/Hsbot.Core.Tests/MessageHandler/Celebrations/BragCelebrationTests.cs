using System.Collections.Generic;
using System.Linq;
using Hsbot.Core.MessageHandlers.Celebrations;
using Hsbot.Core.Messaging;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler.Celebrations
{
    public class BragCelebrationTests
    {
        [TestCaseSource(nameof(GetMessagesToTestMatching))]
        public void ShouldMatchProperly(string message, bool isMatch)
        {
            var celebration = new BragCelebration(new TestJiraApiClient());

            var match = celebration.GetMatch(new InboundMessage {TextWithoutBotName = message});

            match.Success.ShouldBe(isMatch);
        }

        [TestCaseSource(nameof(GetMessagesToTestUserParsing))]
        public void ShouldParseMultipleUsers(string message, string[] expectedUsers)
        {
            var celebration = new BragCelebration(new TestJiraApiClient());
            var match = celebration.GetMatch(new InboundMessage { TextWithoutBotName = message });

            var users = celebration.GetNomineeUserIds(match).ToArray();

            users.ShouldBe(expectedUsers);
        }

        public void ShouldReturnCorrectAwardResponseToValidateMapping()
        {
            var celebration = new BragCelebration(new TestJiraApiClient());

            var match = celebration.GetMatch(new InboundMessage { TextWithoutBotName = "brag for <@bob> for this is a long time coming" });

            var message = celebration.GetAwardRoomMessage(new[] {("bob", "123")}, "doug", match);
            message.ShouldBe("Kudos to *bob* for this is a long time coming bragged by: _doug_ [123]");
        }

        private static IEnumerable<object[]> GetMessagesToTestMatching() => new List<object[]>
        {
            new object[] {"brag for <@bob> for this is a long time coming", true},
            new object[] {"brag for <@bob> for because", true},
            new object[] {"brag to <@bob> for this is a long time coming", true},
            new object[] {"brag <@bob> for this is a long time coming", true},
            new object[] {"brag <@bob>,<@doug> for this is a long time coming", true},
            new object[] {"brag <@bob> & <@doug> for this is a long time coming", true},
            new object[] {"brag <@bob> <@doug> for this is a long time coming", true},
            new object[] {"brag <@bob> <@doug> this is a long time coming", true},

            new object[] {"brag for bob for this is a long time coming", false},
            new object[] {"brag for <@bob>", false},
        };

        private static IEnumerable<object[]> GetMessagesToTestUserParsing => new List<object[]>
        {
            new object[] {"brag <@bob> for this is a long time coming", new[] {"bob"}},
            new object[] {"brag <@bob>,<@doug> for this is a long time coming", new[] {"bob", "doug"}},
            new object[] {"brag <@bob> & <@doug> for this is a long time coming", new[] {"bob", "doug"}},
            new object[] {"brag <@bob> <@doug> for this is a long time coming", new[] {"bob", "doug"}},
        };
    }
}
