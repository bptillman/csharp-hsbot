using System;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Messaging;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class MessageHandlerBaseTests
    {
        public void ShouldHandleWhenAllConditionsMet()
        {
            var rng = new RandomNumberGeneratorFake {NextDoubleValue = 0.0};
            var handler = GetTestMessageHandler(rng);
            var inboundMessage = GetInboundMessage();

            var handleResult = handler.Handles(inboundMessage);
            handleResult.HandlesMessage.ShouldBe(true);
        }

        public void ShouldHandleWhenAllConditionsMetAndBotIsMentioned()
        {
            var rng = new RandomNumberGeneratorFake {NextDoubleValue = 0.0};
            var handler = GetTestMessageHandler(rng);
            handler.DirectMentionOnlyValue = true;

            var inboundMessage = GetInboundMessage();
            inboundMessage.BotIsMentioned = true;

            var handleResult = handler.Handles(inboundMessage);
            handleResult.HandlesMessage.ShouldBe(true);
        }

        public void ShouldHandleWhenAllConditionsMetAndRandomRollIsLessThanHandlerOdds()
        {
            var rng = new RandomNumberGeneratorFake {NextDoubleValue = 0.1};
            var handler = GetTestMessageHandler(rng);
            handler.HandlerOddsValue = 0.11;

            var inboundMessage = GetInboundMessage();

            var handleResult = handler.Handles(inboundMessage);
            handleResult.HandlesMessage.ShouldBe(true);
        }

        public void ShouldHandleWhenAllConditionsMetAndMessageIsInTargetedChannel()
        {
            var rng = new RandomNumberGeneratorFake {NextDoubleValue = 0.0};
            var handler = GetTestMessageHandler(rng);
            handler.TargetedChannelsValue = new[] {"somechannel", "someotherchannel"};

            var inboundMessage = GetInboundMessage();
            inboundMessage.ChannelName = "someotherchannel";

            var handleResult = handler.Handles(inboundMessage);
            handleResult.HandlesMessage.ShouldBe(true);
        }

        public void ShouldNotHandleWhenCanHandleIsFalse()
        {
            var rng = new RandomNumberGeneratorFake {NextDoubleValue = 0.0};
            var handler = GetTestMessageHandler(rng);
            handler.CanHandleReturnValue = false;

            var inboundMessage = GetInboundMessage();

            var handleResult = handler.Handles(inboundMessage);
            handleResult.HandlesMessage.ShouldBe(false);
        }

        public void ShouldNotHandleWhenBotIsNotMentioned()
        {
            var rng = new RandomNumberGeneratorFake {NextDoubleValue = 0.0};
            var handler = GetTestMessageHandler(rng);
            handler.DirectMentionOnlyValue = true;

            var inboundMessage = GetInboundMessage();
            inboundMessage.BotIsMentioned = false;

            var handleResult = handler.Handles(inboundMessage);
            handleResult.HandlesMessage.ShouldBe(false);
        }

        public void ShouldNotHandleWhenRandomRollIsGreaterThanOrEqualToHandlerOdds()
        {
            var rng = new RandomNumberGeneratorFake {NextDoubleValue = 0.11};
            var handler = GetTestMessageHandler(rng);
            handler.HandlerOddsValue = 0.10;

            var inboundMessage = GetInboundMessage();

            var handleResult = handler.Handles(inboundMessage);
            handleResult.HandlesMessage.ShouldBe(false);
        }

        public void ShouldNotHandleWhenMessageIsInUntargetedChannel()
        {
            var rng = new RandomNumberGeneratorFake {NextDoubleValue = 0.0};
            var handler = GetTestMessageHandler(rng);
            handler.TargetedChannelsValue = new[] {"somechannel", "someotherchannel"};

            var inboundMessage = GetInboundMessage();
            inboundMessage.ChannelName = "untargetedchannel";

            var handleResult = handler.Handles(inboundMessage);
            handleResult.HandlesMessage.ShouldBe(false);
        }

        public void ShouldThrowExceptionIfCannedResponsesIsNotSet()
        {
            var rng = new RandomNumberGeneratorFake {NextDoubleValue = 0.0};
            var handler = GetTestMessageHandler(rng);

            Should.Throw<Exception>(() => { handler.GetRandomCannedResponse(); }).Message.ShouldBe("CannedResponses list cannot be empty");
        }

        public void ShouldThrowExceptionIfCannedResponsesIsNull()
        {
            var rng = new RandomNumberGeneratorFake {NextDoubleValue = 0.0};
            var handler = GetTestMessageHandler(rng);
            handler.CannedResponsesValue = null;

            Should.Throw<Exception>(() => { handler.GetRandomCannedResponse(); }).Message.ShouldBe("CannedResponses list cannot be empty");
        }

        public void ShouldReturnBarkForSetBarkList()
        {
            var rng = new RandomNumberGeneratorFake {NextDoubleValue = 0.0};
            var handler = GetTestMessageHandler(rng);
            handler.CannedResponsesValue = new[] {"Test Bark 1", "Test Bark 2"};
            handler.GetRandomCannedResponse().ShouldContain("Test Bark");
        }

        private static MessageHandlerFake GetTestMessageHandler(RandomNumberGeneratorFake rng)
        {
            var handler = new MessageHandlerFake(rng)
            {
                CanHandleReturnValue = true,
                DirectMentionOnlyValue = false,
                HandlerOddsValue = 1.0,
                TargetedChannelsValue = MessageHandlerBase.AllChannels,
                CannedResponsesValue = new string[0]
            };

            return handler;
        }

        private static InboundMessage GetInboundMessage()
        {
            var inboundMessage = new InboundMessage
            {
                BotIsMentioned = true,
                BotId = "test",
                BotName = "test",
                Channel = "fake channel",
                ChannelName = "fake channel",
                FullText = "test message",
                MessageRecipientType = MessageRecipientType.Channel,
                RawText = "test message",
                TextWithoutBotName = "test message",
                UserChannel = "",
                UserEmail = "test@test.com",
                UserId = "nobody",
                Username = "nobody"
            };

            return inboundMessage;
        }
    }
}
