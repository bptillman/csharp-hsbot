using System.Reactive.Linq;
using System.Threading.Tasks;
using Hsbot.Core.Brain;
using Hsbot.Core.Connection;
using Hsbot.Core.Messaging;
using Moq;

namespace Hsbot.Core.Tests
{
    public static class ServiceMocks
    {
        public static Mock<IHsbotLog> MockLog()
        {
            var logMock = new Mock<IHsbotLog>();
            logMock.Setup(x => x.Info(It.IsAny<string>(), It.IsAny<object[]>()));
            logMock.Setup(x => x.Warn(It.IsAny<string>(), It.IsAny<object[]>()));
            logMock.Setup(x => x.Debug(It.IsAny<string>(), It.IsAny<object[]>()));

            return logMock;
        }

        public static Mock<IBotBrainStorage<HsbotBrain>> MockBrainStorage()
        {
            var brainStorageMock = new Mock<IBotBrainStorage<HsbotBrain>>();
            brainStorageMock.Setup(x => x.Load()).Returns(Task.FromResult(new HsbotBrain()));
            brainStorageMock.Setup(x => x.Save(It.IsAny<HsbotBrain>())).Returns(Task.CompletedTask);

            return brainStorageMock;
        }

        public static Mock<IHsbotChatConnector> MockChatConnector()
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
