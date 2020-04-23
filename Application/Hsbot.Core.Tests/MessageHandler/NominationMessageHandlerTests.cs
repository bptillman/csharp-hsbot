using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Messaging;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class NominationMessageHandlerTests : MessageHandlerTestBase<NominationMessageHandler>
    {
        private readonly string _bragRoom = "CE9K4LTFD";
        private readonly HvaResponse _hvaSuccessResponse = new HvaResponse {HvaKey = "theKey", Message = "Success"};
        private readonly HvaResponse _hvaFailureResponse = new HvaResponse {Failed = true};
        private readonly string _jiraErrorMessage = "I failed...";

        protected override string[] MessageTextsThatShouldBeHandled => new []
        {
            "hva for <@bob> for dfe this is a long time coming",
            "hva for <@bob> for dfe because",
            "hva to <@bob> for dfe this is a long time coming",
            "hva <@bob> for dfe this is a long time coming",
        };
        protected override string[] MessageTextsThatShouldNotBeHandled => new[]
        {
            "hva for bob for dfe this is a long time coming",
            "hva for <@bob> for dfe",
            "hva for <@bob> for something i don't know the names of the awards",
            "hva for <@bob> dfe something to test"
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
            var message = GetMessage(messageHandler, "hva to <@nobody> for dfe this bot is great");
            var context = GetMessageContext(message);

            await messageHandler.HandleAsync(context);

            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages.First().Text.ShouldBe(":disapproval: nominating yourself is not allowed!");
        }

        public async Task ShouldReturnMessageIfUserNotFoundInJira()
        {
            var messageHandler = GetHandlerInstance();
            var message = GetMessage(messageHandler, "hva to <@notInJira> for dfe this bot is great");
            var context = GetMessageContext(message);

            await messageHandler.HandleAsync(context);

            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages.First().Text.ShouldBe($":doh: {_jiraErrorMessage}");
        }

        public async Task ShouldMessageToBragRoomWhenNotInBragRoom()
        {
            var messageHandler = GetHandlerInstance();
            var message = GetMessage(messageHandler, "hva to <@bob> for dfe this bot is great");
            message.Channel = "randomRoom";
            var context = GetMessageContext(message);

            await messageHandler.HandleAsync(context);

            context.SentMessages
                .SingleOrDefault(x => x.Channel == message.Channel && x.Text == _hvaSuccessResponse.Message)
                .ShouldNotBeNull();
            context.SentMessages
                .SingleOrDefault(x => x.Channel == _bragRoom && x.Text == "bob exhibits *_dfe_*\nthis bot is great\nnominated by: _nobody_\ntheKey")
                .ShouldNotBeNull();
        }

        public async Task ShouldNotMessageToBragRoomWhenInBragRoom()
        {
            var messageHandler = GetHandlerInstance();
            var message = GetMessage(messageHandler, "hva to <@bob> for dfe this bot is great");
            message.Channel = _bragRoom;
            var context = GetMessageContext(message);

            await messageHandler.HandleAsync(context);

            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages.First().Text.ShouldBe(_hvaSuccessResponse.Message);
        }

        public async Task ShouldReturnCannedResponse()
        {
            var messageHandler = GetHandlerInstance(_hvaFailureResponse);
            var message = GetMessage(messageHandler, "hva to <@bob> for dfe this bot is great");
            var context = GetMessageContext(message);

            await messageHandler.HandleAsync(context);

            context.SentMessages.Count.ShouldBe(1);
            context.SentMessages.First().Text.ShouldBe("My time circuits must be shorting out, I couldn't do that :sad_panda:, please don't let me get struck by lightning :build:");
        }

        protected override NominationMessageHandler GetHandlerInstance()
        {
            return GetHandlerInstance(_hvaSuccessResponse);
        }

        protected TestInboundMessageContext GetMessageContext(InboundMessage inboundMessage)
        {
            return new TestInboundMessageContext(inboundMessage)
            {
                ChatUsers =
                {
                    {"bob", new TestChatUser {Id = "bob", IsEmployee = true, Email = "bob@bob.com", FullName="bob"}},
                    {"bot", new TestChatUser {IsEmployee = false}},
                    {"nobody", new TestChatUser {Id = "nobody", IsEmployee = true, FullName="nobody"}},
                    {"notInJira", new TestChatUser {Id = "notInJira", IsEmployee = true}},
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

        private NominationMessageHandler GetHandlerInstance(HvaResponse hvaResponse)
        {
            //Since this RNG will always return 0, the check on the random roll in the handler will
            //always succeed, meaning the random roll will not cause the result of ShouldHandle
            //to be false
            var rng = new RandomNumberGeneratorFake { NextDoubleValue = 0.0 };

            var jiraApiClient = new TestJiraApiClient
            {
                ErrorMessage = _jiraErrorMessage,
                HvaResponse = hvaResponse,
                Users = new[]
                {
                    new TestJiraUser {DisplayName = "bobby", Email = "bob@bob.com"},
                    new TestJiraUser {DisplayName = "nobody", Email = "nobody@nobody.com"},
                }
            };

            var handler = new NominationMessageHandler(jiraApiClient, rng);

            return handler;
        }
    }
}
