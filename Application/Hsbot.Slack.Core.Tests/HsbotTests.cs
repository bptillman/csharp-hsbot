using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Hsbot.Slack.Core.Brain;
using Hsbot.Slack.Core.Connection;
using Hsbot.Slack.Core.Messaging;
using Moq;

namespace Hsbot.Slack.Core.Tests
{
    public class HsbotTests
    {
        public async Task ShouldLoadBrainOnConnect()
        {
            var logMock = MockLog();
            var brainStorageMock = MockBrainStorage();
            var chatConnectorMock = MockChatConnector();

            var hsbot = new Hsbot(logMock.Object, Enumerable.Empty<IInboundMessageHandler>(), brainStorageMock.Object, chatConnectorMock.Object);
            await hsbot.Connect();

            brainStorageMock.Verify(x => x.Load(), Times.Once);
        }

        public async Task ShouldSaveBrainWhenItChanges()
        {
            var logMock = MockLog();
            var brainStorageMock = MockBrainStorage();

            var chatConnectorMock = MockChatConnector();

            var hsbot = new Hsbot(logMock.Object, Enumerable.Empty<IInboundMessageHandler>(), brainStorageMock.Object, chatConnectorMock.Object);
            await hsbot.Connect();

            hsbot.Brain.SetItem("test", "value");

            brainStorageMock.Verify(x => x.Save(It.IsAny<HsbotBrain>()), Times.Once);
        }

        public async Task ShouldNotSaveBrainIfInitialLoadFailed()
        {
            var logMock = MockLog();
            var brainStorageMock = MockBrainStorage();
            brainStorageMock.Setup(x => x.Load()).Throws(new Exception());

            var chatConnectorMock = MockChatConnector();

            var hsbot = new Hsbot(logMock.Object, Enumerable.Empty<IInboundMessageHandler>(), brainStorageMock.Object, chatConnectorMock.Object);
            await hsbot.Connect();

            hsbot.Brain.SetItem("test", "value");

            brainStorageMock.Verify(x => x.Save(It.IsAny<HsbotBrain>()), Times.Never);
        }

        public async Task ShouldCallHandlersWhenMessageReceived()
        {
            var logMock = MockLog();
            var brainStorageMock = MockBrainStorage();

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
            messageHandlerMock.Setup(x => x.Handle(It.IsAny<InboundMessage>())).Returns(new[] {outboundResponse});
            messageHandlerMock.Setup(x => x.Handles(It.IsAny<InboundMessage>()))
                .Returns(new HandlesResult {HandlesMessage = true});

            var hsbot = new Hsbot(logMock.Object, new []{ messageHandlerMock.Object }, brainStorageMock.Object, chatConnectorMock.Object);
            await hsbot.Connect();

            messageHandlerMock.Verify(x => x.Handles(inboundMessage), Times.Once);
            messageHandlerMock.Verify(x => x.Handle(inboundMessage), Times.Once);
            chatConnectorMock.Verify(x => x.SendMessage(outboundResponse), Times.Once);
        }

        public async Task ShouldSendMessageResponseViaChatConnector()
        {
            var logMock = MockLog();
            var brainStorageMock = MockBrainStorage();
            var chatConnectorMock = MockChatConnector();

            var hsbot = new Hsbot(logMock.Object, Enumerable.Empty<IInboundMessageHandler>(), brainStorageMock.Object, chatConnectorMock.Object);
            await hsbot.Connect();
            
            var outboundResponse = new OutboundResponse();
            await hsbot.SendMessage(outboundResponse);

            chatConnectorMock.Verify(x => x.SendMessage(outboundResponse), Times.Once);
        }

        private static Mock<IHsbotLog> MockLog()
        {
            var logMock = new Mock<IHsbotLog>();
            logMock.Setup(x => x.Info(It.IsAny<string>(), It.IsAny<object[]>()));
            logMock.Setup(x => x.Warn(It.IsAny<string>(), It.IsAny<object[]>()));
            logMock.Setup(x => x.Debug(It.IsAny<string>(), It.IsAny<object[]>()));

            return logMock;
        }

        private static Mock<IBotBrainStorage<HsbotBrain>> MockBrainStorage()
        {
            var brainStorageMock = new Mock<IBotBrainStorage<HsbotBrain>>();
            brainStorageMock.Setup(x => x.Load()).Returns(Task.FromResult(new HsbotBrain()));
            brainStorageMock.Setup(x => x.Save(It.IsAny<HsbotBrain>())).Returns(Task.CompletedTask);

            return brainStorageMock;
        }

        private static Mock<IHsbotChatConnector> MockChatConnector()
        {
            var chatConnectorMock = new Mock<IHsbotChatConnector>();
            chatConnectorMock.Setup(x => x.SendMessage(It.IsAny<OutboundResponse>())).Returns(Task.CompletedTask);
            chatConnectorMock.Setup(x => x.Connect()).Returns(Task.CompletedTask);
            chatConnectorMock.Setup(x => x.Disconnect()).Returns(Task.CompletedTask);
            chatConnectorMock.Setup(x => x.Disconnected).Returns(Observable.Empty<IHsbotChatConnector>());
            chatConnectorMock.Setup(x => x.MessageReceived).Returns(Observable.Empty<Task<InboundMessage>>());
            chatConnectorMock.Setup(x => x.Reconnected).Returns(Observable.Empty<IHsbotChatConnector>());
            chatConnectorMock.Setup(x => x.Reconnecting).Returns(Observable.Empty<IHsbotChatConnector>());

            return chatConnectorMock;
        }
    }
}
