using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.ApiClients;
using Hsbot.Core.Brain;
using Hsbot.Core.Connection;
using Hsbot.Core.Infrastructure;
using Hsbot.Core.Messaging;
using Hsbot.Core.Messaging.Formatting;
using Hsbot.Core.Tests.Infrastructure;
using Moq;

namespace Hsbot.Core.Tests.MessageHandler.Infrastructure
{
    public class BotProvidedServicesFake : IBotProvidedServices
    {
        public BotProvidedServicesFake()
        {
            GetChatUserById = userId => { return new Task<IChatUser>(() => UserToReturn); };

            SendMessage = response =>
            {
                SentMessages.Add(response);
                return Task.CompletedTask;
            };

            LogMock = MockLog();
            Brain = new HsbotBrain();
            MessageTextFormatter = new InlineChatMessageTextFormatter();
            SystemClock = new TestSystemClock();
            TumblrApiClient = new TestTumblrApiClient();
        }

        public Mock<IHsbotLog> LogMock { get; set; }
        public IChatUser UserToReturn { get; set; } = new TestChatUser();
        public List<OutboundResponse> SentMessages { get; } = new List<OutboundResponse>();

        public IBotBrain Brain { get; set; }
        public IHsbotLog Log => LogMock.Object;
        public Func<string, Task<IChatUser>> GetChatUserById { get; }
        public Func<OutboundResponse, Task> SendMessage { get; }
        public IChatMessageTextFormatter MessageTextFormatter { get; set; }
        public ISystemClock SystemClock { get; set; }
        public ITumblrApiClient TumblrApiClient { get; set; }

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
