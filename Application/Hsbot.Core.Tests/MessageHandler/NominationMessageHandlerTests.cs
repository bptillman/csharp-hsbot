using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.MessageHandlers.Celebrations;
using Hsbot.Core.Messaging;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class NominationMessageHandlerTests : MessageHandlerTestBase<NominationMessageHandler>
    {
        private readonly string _bragRoom = "#brags-and-awards";
        private readonly SubmissionResponse _successResponse = new SubmissionResponse {Key = "theKey"};
        private readonly SubmissionResponse _failureResponse = new SubmissionResponse {Failed = true};
        private readonly string _jiraErrorMessage = "I failed...";

        protected override string[] MessageTextsThatShouldBeHandled => new []
        {
            "hva for <@bob> for dfe this is a long time coming",
            "hva for <@bob> for dfe. this is a long time coming",
            "hva for <@bob> for dfe . this is a long time coming",
            "hva for <@bob> for dfe .this is a long time coming",
            "hva for <@bob> for dfe because",
            "hva to <@bob> for dfe this is a long time coming",
            "hva <@bob> for dfe this is a long time coming",
            "brag on <@bob> for this is a long time coming",
            "brag about <@bob> for because",
            "brag on <@bob> for this is a long time coming",
            "brag <@bob> for this is a long time coming",
            "brag <@bob>,<@doug> for this is a long time coming",
            "brag <@bob> & <@doug> for this is a long time coming",
            "brag <@bob> <@doug> for this is a long time coming",
        };
        protected override string[] MessageTextsThatShouldNotBeHandled => new[]
        {
            "hva for bob for dfe this is a long time coming",
            "hva for <@bob> for dfe",
            "hva for <@bob> for something i don't know the names of the awards",
            "hva for <@bob> dfe something to test",
            "brag on bob for this is a long time coming",
            "brag about <@bob>",
        };

        public async Task ShouldSayCannotGiveHvaForNonEmployee()
        {
            var messageHandler = GetHandlerInstance();
            var message = GetMessage(messageHandler, "hva to <@bot> for dfe this bot is great");
            var context = GetMessageContext(message);

            await messageHandler.HandleAsync(context);

            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages.First().Text.ShouldBe(":blush: Sorry, only employees can be nominated.");
        }

        public async Task ShouldNotAllowSelfNominations()
        {
            var messageHandler = GetHandlerInstance();
            var message = GetMessage(messageHandler, "hva to <@nobody> for dfe this person is great");
            var context = GetMessageContext(message);

            await messageHandler.HandleAsync(context);

            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages.First().Text.ShouldBe(":disapproval: nominating yourself is not allowed!");
        }

        public async Task ShouldMessageToBragRoomWhenNotInBragRoom()
        {
            var messageHandler = GetHandlerInstance();
            var message = GetMessage(messageHandler, "hva to <@bob> for dfe this person is great");
            message.Channel = "randomRoom";
            var context = GetMessageContext(message);

            await messageHandler.HandleAsync(context);

            context.SentMessages.Count.ShouldBe(2);

            var firstMessage = context.SentMessages[0]; 
            firstMessage.Channel.ShouldBe(message.Channel);
            firstMessage.Text.ShouldBe("Your nomination for bob [theKey] was successfully retrieved and processed!");

            var secondMessage = context.SentMessages[1];
            secondMessage.Channel.ShouldBe(_bragRoom);
        }

        public async Task ShouldNotMessageToBragRoomWhenInBragRoom()
        {
            var messageHandler = GetHandlerInstance();
            var message = GetMessage(messageHandler, "hva to <@bob> for dfe this person is great");
            message.ChannelName = _bragRoom;
            var context = GetMessageContext(message);

            await messageHandler.HandleAsync(context);

            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages.First().Text.ShouldBe("Your nomination for bob [theKey] was successfully retrieved and processed!");
        }

        public async Task ShouldReturnCannedResponse()
        {
            var messageHandler = GetHandlerInstance(_failureResponse);
            var message = GetMessage(messageHandler, "hva to <@bob> for dfe this person is great");
            var context = GetMessageContext(message);

            await messageHandler.HandleAsync(context);

            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages.First().Text.ShouldBe(":doh: My time circuits must be shorting out, I couldn't do that :sad_panda:, please don't let me get struck by lightning :build:");
        }

        protected override NominationMessageHandler GetHandlerInstance()
        {
            return GetHandlerInstance(_successResponse);
        }

        protected TestInboundMessageContext GetMessageContext(InboundMessage inboundMessage)
        {
            return new TestInboundMessageContext(inboundMessage)
            {
                ChatUsers =
                {
                    {"bob", new TestChatUser {Id = "bob", IsEmployee = true, Email = "bob@bob.com", FullName = "bob"}},
                    {"bot", new TestChatUser {IsEmployee = false}},
                    {"notInJira", new TestChatUser {IsEmployee = true}},
                    {"nobody", new TestChatUser {Id = "nobody", IsEmployee = true, FullName = "nobody"}},
                }
            };
        }

        private InboundMessage GetMessage(MessageHandlerBase handler, string messageText)
        {
            var inboundMessage = new InboundMessage
            {
                BotIsMentioned = true,
                BotId = "test",
                BotName = "test",
                Channel = handler.GetTestMessageChannel(),
                ChannelName = handler.GetTestMessageChannel(),
                FullText = messageText,
                MessageRecipientType = MessageRecipientType.Channel,
                RawText = messageText,
                TextWithoutBotName = messageText,
                UserChannel = "",
                UserEmail = "test@test.com",
                UserId = "nobody",
                Username = "nobody"
            };
            return inboundMessage;
        }

        private NominationMessageHandler GetHandlerInstance(SubmissionResponse hvaResponse)
        {
            //Since this RNG will always return 0, the check on the random roll in the handler will
            //always succeed, meaning the random roll will not cause the result of ShouldHandle
            //to be false
            var rng = new RandomNumberGeneratorFake { NextDoubleValue = 0.0 };

            var jiraApiClient = new TestJiraApiClient
            {
                ErrorMessage = _jiraErrorMessage,
                SubmissionResponse = hvaResponse,
            };

            ICelebration nominationCelebration = new BragCelebration(jiraApiClient);
            ICelebration bragCelebration = new NominationCelebration(jiraApiClient, rng);

            var handler = new NominationMessageHandler(new[] {bragCelebration, nominationCelebration}, rng);

            return handler;
        }
    }
}
