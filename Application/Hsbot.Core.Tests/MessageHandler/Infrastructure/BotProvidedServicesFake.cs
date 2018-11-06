using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.Brain;
using Hsbot.Core.Infrastructure;
using Hsbot.Core.Messaging;
using Hsbot.Core.Messaging.Formatting;
using Hsbot.Core.Tests.Infrastructure;
using Moq;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class BotProvidedServicesFake : IBotProvidedServices
    {
        public BotProvidedServicesFake(HsbotBrain brain = null, 
            Mock<IHsbotLog> logMock = null, 
            Action<OutboundResponse> sendMessageAction = null, 
            IChatMessageTextFormatter messageTextFormatter = null,
            ISystemClock systemClock = null)
        {
            SendMessage = response =>
            {
                SentMessages.Add(response);
                sendMessageAction?.Invoke(response);

                return Task.CompletedTask;
            };

            LogMock = logMock ?? MockLog();
            Brain = brain ?? new HsbotBrain();
            MessageTextFormatter = messageTextFormatter ?? new InlineChatMessageTextFormatter();
            SystemClock = systemClock ?? new TestSystemClock();
        }

        public Mock<IHsbotLog> LogMock { get; }
        public List<OutboundResponse> SentMessages { get; } = new List<OutboundResponse>();

        public IBotBrain Brain { get; }
        public IHsbotLog Log => LogMock.Object;
        public Func<OutboundResponse, Task> SendMessage { get; }
        public IChatMessageTextFormatter MessageTextFormatter { get; }
        public ISystemClock SystemClock { get; }

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
