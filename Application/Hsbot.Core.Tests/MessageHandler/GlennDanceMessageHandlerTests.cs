using System.Threading.Tasks;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class GlennDanceMessageHandlerTests : MessageHandlerTestBase<GlennDanceMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[]
        {
            "make glenn dance",
            "Make Glenn dance!!",
            "glenn dance bomb",
            "Glenn Dance Bomb 3",
            "Glenn Dance Bomb XYZ",
            "glenn dance bomb 10",
            "glenn dance bomb -4",
            "glenn dance bomb 0",
            "glenn dance bomb 15!! they should be only AnimatedFiles.Length",
        };

        protected override string[] MessageTextsThatShouldNotBeHandled => new[]
        {
            "make him dance",
            "make glenn dancing",
            "glenn",
            "glenn dance",
            "let's make glenn dance",
            "I want to see glenn dance bomb 5",
            "Do glenn dance bomb"
        };
        
        public void CannedResponsesShouldHaveItems()
        {
            var messageHandler = GetHandlerInstance();
            var cannedResponses = messageHandler.CannedResponses;

            cannedResponses.ShouldNotBeNull();
            cannedResponses.Length.ShouldBeGreaterThan(0);
        }

        public async Task ShouldMakeGlennDanceAsManyTimesAsExpected()
        {
            var messageHandler = GetHandlerInstance();
            var sentRepliesPerMessageText = new[]
            {
                1,
                1,
                messageHandler.CannedResponses.Length,
                3,
                messageHandler.CannedResponses.Length,
                10,
                messageHandler.CannedResponses.Length,
                0,
                messageHandler.CannedResponses.Length
            };

            MessageTextsThatShouldBeHandled.Length.ShouldBe(sentRepliesPerMessageText.Length);

            for (var i = 0; i < MessageTextsThatShouldBeHandled.Length; i++)
            {
                var response = await messageHandler.TestHandleAsync(MessageTextsThatShouldBeHandled[i]);

                response.SentMessages.Count.ShouldBe(sentRepliesPerMessageText[i]);
                foreach (var sentMessage in response.SentMessages)
                {
                    sentMessage.Text.ShouldBeOneOf(messageHandler.CannedResponses);
                }

                //clear out response messages on every iteration -- otherwise it would
                //accumulate messages from all iterations.
                response.SentMessages.Clear();
            }
        }
    }
}
