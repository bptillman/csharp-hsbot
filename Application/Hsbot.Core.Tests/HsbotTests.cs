using System;
using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.BotServices;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Messaging;
using Hsbot.Core.Tests.Connection;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests
{
    public class HsbotTests
    {
        public async Task ShouldCallHandlersWhenMessageReceived()
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

            var messageHandler = new FakeMessageHandler();
            var chatConnector = new FakeChatConnector();
            var hsbot = new Hsbot(new FakeLogger<Hsbot>(), new []{ messageHandler }, Enumerable.Empty<IBotService>(), new RandomNumberGeneratorFake(), chatConnector);
            await hsbot.Connect();

            var message = new InboundMessage
            {
                BotIsMentioned = true,
                BotId = "test",
                BotName = "test",
                Channel = "test",
                ChannelName = "test",
                FullText = "test",
                MessageRecipientType = MessageRecipientType.Channel,
                RawText = "test",
                TextWithoutBotName = "test",
                UserChannel = "",
                UserEmail = "test@test.com",
                UserId = "nobody",
                Username = "nobody"
            };

            chatConnector.ReceiveMessage(message);

            messageHandler.HandledMessages.Count.ShouldBe(1);
        }

        public async Task ShouldNotCallHandlersWhenHelpMessageReceived()
        {
            var inboundMessage = new InboundMessage
            {
                BotIsMentioned = true,
                BotId = "test",
                BotName = "test",
                Channel = "fake channel",
                ChannelName = "fake channel",
                FullText = "help",
                MessageRecipientType = MessageRecipientType.Channel,
                RawText = "help",
                TextWithoutBotName = "help",
                UserChannel = "",
                UserEmail = "test@test.com",
                UserId = "nobody",
                Username = "nobody"
            };

            var chatConnector = new FakeChatConnector();
            var messageHandler = new FakeMessageHandler();

            var hsbot = new Hsbot(new FakeLogger<Hsbot>(), new []{ messageHandler }, Enumerable.Empty<IBotService>(), new RandomNumberGeneratorFake(), chatConnector);
            await hsbot.Connect();

            chatConnector.ReceiveMessage(inboundMessage);

            messageHandler.HandledMessages.Count.ShouldBe(0);
            chatConnector.SentMessages.Count.ShouldBe(1);
            chatConnector.SentMessages.First().Text.ShouldStartWith("Available commands:");
        }

        public async Task ShouldNotCallHandlerWhenItDoesNotHandleMessage()
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

            var chatConnector = new FakeChatConnector();
            var messageHandler = new FakeMessageHandler {HandlesMessage = false};

            var hsbot = new Hsbot(new FakeLogger<Hsbot>(), new []{ messageHandler }, Enumerable.Empty<IBotService>(), new RandomNumberGeneratorFake(), chatConnector);
            await hsbot.Connect();

            chatConnector.ReceiveMessage(inboundMessage);

            messageHandler.HandledMessages.Count.ShouldBe(0);
            chatConnector.SentMessages.Count.ShouldBe(0);
        }

        public async Task ShouldGetChatUserViaChatConnector()
        {
            var chatConnector = new FakeChatConnector();
            chatConnector.ChatUsers.Add("foo", new TestChatUser { Id = "foo", Email = "foo@bar.com", FullName = "Foo Bar", IsEmployee = true});

            var hsbot = new Hsbot(new FakeLogger<Hsbot>(), Enumerable.Empty<IInboundMessageHandler>(), Enumerable.Empty<IBotService>(), new RandomNumberGeneratorFake(), chatConnector);
            await hsbot.Connect();

            var user = await hsbot.GetChatUserById("foo");
            user.Id.ShouldBe("foo");
            user.Email.ShouldBe("foo@bar.com");
            user.FullName.ShouldBe("Foo Bar");
            user.IsEmployee.ShouldBeTrue();
        }

        public async Task ShouldSendMessageResponseViaChatConnector()
        {
            var chatConnector = new FakeChatConnector();

            var hsbot = new Hsbot(new FakeLogger<Hsbot>(), Enumerable.Empty<IInboundMessageHandler>(), Enumerable.Empty<IBotService>(), new RandomNumberGeneratorFake(), chatConnector);
            await hsbot.Connect();
            
            var outboundResponse = new OutboundResponse();
            await hsbot.SendMessage(outboundResponse);

            chatConnector.SentMessages.Count.ShouldBe(1);
        }

        public async Task ShouldReportThrownExceptionBackToUser()
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

            var chatConnector = new FakeChatConnector();
            var messageHandler = new FakeMessageHandler
            {
                HandlesMessage = true,
                HandlerAction = c => throw new Exception("exception message")
            };

            var barkIndex = 1;
            var randomNumberGeneratorFake = new RandomNumberGeneratorFake { NextIntValue = barkIndex };
            var hsbot = new Hsbot(new FakeLogger<Hsbot>(), new[] { messageHandler }, Enumerable.Empty<IBotService>(), randomNumberGeneratorFake, chatConnector);
            await hsbot.Connect();

            chatConnector.ReceiveMessage(inboundMessage);

            messageHandler.HandledMessages.Count.ShouldBe(1);
            chatConnector.SentMessages.Count.ShouldBe(1);
            chatConnector.SentMessages[0].Text.ShouldBe(hsbot.ErrorBarks[barkIndex]);
        }

        public async Task ShouldReportThrownMessageHandlerExceptionBackToUser()
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

            var chatConnector = new FakeChatConnector();
            var messageHandler = new FakeMessageHandler
            {
                HandlesMessage = true,
                HandlerAction = c => throw new MessageHandlerException("error response to channel", "diagnostic info")
            };

            var barkIndex = 1;
            var randomNumberGeneratorFake = new RandomNumberGeneratorFake {NextIntValue = barkIndex};
            var hsbot = new Hsbot(new FakeLogger<Hsbot>(), new[] { messageHandler }, Enumerable.Empty<IBotService>(), randomNumberGeneratorFake, chatConnector);
            await hsbot.Connect();

            chatConnector.ReceiveMessage(inboundMessage);

            messageHandler.HandledMessages.Count.ShouldBe(1);
            chatConnector.SentMessages.Count.ShouldBe(1);
            chatConnector.SentMessages[0].Text.ShouldBe("error response to channel");
        }

        public async Task ShouldUseBarkForMessageHandlerExceptionWithNoResponse()
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

            var chatConnector = new FakeChatConnector();
            var messageHandler = new FakeMessageHandler
            {
                HandlesMessage = true,
                HandlerAction = c => throw new MessageHandlerException("", "diagnostic info")
            };

            var barkIndex = 1;
            var randomNumberGeneratorFake = new RandomNumberGeneratorFake { NextIntValue = barkIndex };
            var hsbot = new Hsbot(new FakeLogger<Hsbot>(), new[] { messageHandler }, Enumerable.Empty<IBotService>(), randomNumberGeneratorFake, chatConnector);
            await hsbot.Connect();

            chatConnector.ReceiveMessage(inboundMessage);

            messageHandler.HandledMessages.Count.ShouldBe(1);
            chatConnector.SentMessages.Count.ShouldBe(1);
            chatConnector.SentMessages[0].Text.ShouldBe(hsbot.ErrorBarks[barkIndex]);
        }
    }
}