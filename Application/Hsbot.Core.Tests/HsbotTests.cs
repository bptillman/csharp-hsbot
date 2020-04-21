using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Hsbot.Core.BotServices;
using Hsbot.Core.Messaging;
using Hsbot.Core.Messaging.Formatting;
using Hsbot.Core.Tests.Infrastructure;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using static Hsbot.Core.Tests.ServiceMocks;
using Moq;
using Shouldly;

namespace Hsbot.Core.Tests
{
    public class HsbotTests
    {
        public async Task ShouldCallHandlersWhenMessageReceived()
        {
            var logMock = MockLog();

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

            var chatConnectorMock = MockChatConnector();
            chatConnectorMock.Setup(x => x.MessageReceived).Returns(Observable.Return(Task.FromResult(inboundMessage)));

            var messageHandlerMock = new Mock<IInboundMessageHandler>();
            messageHandlerMock.Setup(x => x.HandleAsync(It.IsAny<InboundMessageContext>())).Returns(Task.CompletedTask);
            messageHandlerMock.Setup(x => x.Handles(It.IsAny<InboundMessage>()))
                .Returns(new HandlesResult {HandlesMessage = true});

            var hsbot = new Hsbot(logMock.Object, new []{ messageHandlerMock.Object }, Enumerable.Empty<IBotService>(), chatConnectorMock.Object);
            await hsbot.Connect();

            messageHandlerMock.Verify(x => x.Handles(inboundMessage), Times.Once);
            messageHandlerMock.Verify(x => x.HandleAsync(It.IsAny<InboundMessageContext>()), Times.Once);
        }

        public async Task ShouldNotCallHandlersWhenHelpMessageReceived()
        {
            var logMock = MockLog();

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

            var sentMessages = new List<OutboundResponse>();

            var chatConnectorMock = MockChatConnector();
            chatConnectorMock.Setup(x => x.MessageReceived).Returns(Observable.Return(Task.FromResult(inboundMessage)));
            chatConnectorMock.Setup(x => x.SendMessage(It.IsAny<OutboundResponse>()))
                .Returns(Task.CompletedTask)
                .Callback<OutboundResponse>(msg => sentMessages.Add(msg));

            var messageHandlerMock = new Mock<IInboundMessageHandler>();
            messageHandlerMock.Setup(x => x.HandleAsync(It.IsAny<InboundMessageContext>())).Returns(Task.CompletedTask);
            messageHandlerMock.Setup(x => x.Handles(It.IsAny<InboundMessage>()))
                .Returns(new HandlesResult {HandlesMessage = true});

            var hsbot = new Hsbot(logMock.Object, new []{ messageHandlerMock.Object }, Enumerable.Empty<IBotService>(), chatConnectorMock.Object);
            await hsbot.Connect();

            messageHandlerMock.Verify(x => x.Handles(inboundMessage), Times.Never);
            messageHandlerMock.Verify(x => x.HandleAsync(It.IsAny<InboundMessageContext>()), Times.Never);
            sentMessages.Count.ShouldBe(1);
            sentMessages.First().Text.ShouldStartWith("Available commands:");
        }

        public async Task ShouldNotCallHandlerWhenItDoesNotHandleMessage()
        {
            var logMock = MockLog();

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

            var chatConnectorMock = MockChatConnector();
            chatConnectorMock.Setup(x => x.MessageReceived).Returns(Observable.Return(Task.FromResult(inboundMessage)));

            var outboundResponse = new OutboundResponse();
            var messageHandlerMock = new Mock<IInboundMessageHandler>();
            messageHandlerMock.Setup(x => x.HandleAsync(It.IsAny<InboundMessageContext>())).Returns(Task.CompletedTask);
            messageHandlerMock.Setup(x => x.Handles(It.IsAny<InboundMessage>()))
                .Returns(new HandlesResult {HandlesMessage = false});

            var hsbot = new Hsbot(logMock.Object, new []{ messageHandlerMock.Object }, Enumerable.Empty<IBotService>(), chatConnectorMock.Object);
            await hsbot.Connect();

            messageHandlerMock.Verify(x => x.Handles(inboundMessage), Times.Once);
            messageHandlerMock.Verify(x => x.HandleAsync(It.IsAny<InboundMessageContext>()), Times.Never);
            chatConnectorMock.Verify(x => x.SendMessage(outboundResponse), Times.Never);
        }

        public async Task ShouldGetChatUserViaChatConnector()
        {
            var logMock = MockLog();
            var chatConnectorMock = MockChatConnector();

            var hsbot = new Hsbot(logMock.Object, Enumerable.Empty<IInboundMessageHandler>(), Enumerable.Empty<IBotService>(), chatConnectorMock.Object);
            await hsbot.Connect();

            await hsbot.GetChatUserById(string.Empty);

            chatConnectorMock.Verify(x => x.GetChatUserById(string.Empty), Times.Once);
        }

        public async Task ShouldSendMessageResponseViaChatConnector()
        {
            var logMock = MockLog();
            var chatConnectorMock = MockChatConnector();

            var hsbot = new Hsbot(logMock.Object, Enumerable.Empty<IInboundMessageHandler>(), Enumerable.Empty<IBotService>(), chatConnectorMock.Object);
            await hsbot.Connect();
            
            var outboundResponse = new OutboundResponse();
            await hsbot.SendMessage(outboundResponse);

            chatConnectorMock.Verify(x => x.SendMessage(outboundResponse), Times.Once);
        }
    }
}
