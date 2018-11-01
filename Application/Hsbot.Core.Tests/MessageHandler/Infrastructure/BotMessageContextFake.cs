using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.Brain;
using Hsbot.Core.Messaging;
using Moq;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class BotMessageContextFake : IBotMessageContext
    {
        public BotMessageContextFake(InboundMessage message, HsbotBrain brain = null, Mock<IHsbotLog> logMock = null, Action<OutboundResponse> sendMessageAction = null)
        {
            Message = message;
            
            SendMessage = response =>
            {
                SentMessages.Add(response);
                sendMessageAction?.Invoke(response);

                return Task.CompletedTask;
            };

            LogMock = logMock ?? MockLog();

            Brain = brain ?? new HsbotBrain();
        }

        public Mock<IHsbotLog> LogMock { get; }
        public List<OutboundResponse> SentMessages { get; } = new List<OutboundResponse>();

        public IBotBrain Brain { get; }
        public IHsbotLog Log => LogMock.Object;
        public InboundMessage Message { get; }
        public Func<OutboundResponse, Task> SendMessage { get; }

        private Mock<IHsbotLog> MockLog()
        {
            var result = new Mock<IHsbotLog>();
            result.Setup(x => x.Info(It.IsAny<string>(), It.IsAny<object[]>()));
            result.Setup(x => x.Warn(It.IsAny<string>(), It.IsAny<object[]>()));
            result.Setup(x => x.Debug(It.IsAny<string>(), It.IsAny<object[]>()));

            return result;
        }
    }
}
