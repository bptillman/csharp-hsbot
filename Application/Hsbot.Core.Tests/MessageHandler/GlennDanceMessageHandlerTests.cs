using System;
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

        public Func<GlennDanceMessageHandler, string[]> AnimatedFiles = x => x.AnimatedFiles;

        public async Task ShouldMakeGlennDanceAsManyTimesAsExpected()
        {
            var messageHandler = GetHandlerInstance();
            var sentRepliesPerMessageText = new[]
            {
                1,
                1,
                AnimatedFiles(messageHandler).Length,
                3,
                AnimatedFiles(messageHandler).Length,
                10,
                AnimatedFiles(messageHandler).Length,
                0,
                AnimatedFiles(messageHandler).Length
            };

            MessageTextsThatShouldBeHandled.Length.ShouldBe(sentRepliesPerMessageText.Length);

            for (var i = 0; i < MessageTextsThatShouldBeHandled.Length; i++)
            {
                var response = await messageHandler.HandleAsync(MessageTextsThatShouldBeHandled[i]);

                response.SentMessages.Count.ShouldBe(sentRepliesPerMessageText[i]);
                foreach (var sentMessage in response.SentMessages)
                {
                    sentMessage.Text.ShouldBeOneOf(AnimatedFiles(messageHandler));
                }
            }
        }
    }
}
