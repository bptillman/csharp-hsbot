﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.Connection;
using Hsbot.Core.Messaging;
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
        }

        public IChatUser UserToReturn { get; set; } = new TestChatUser();
        public List<OutboundResponse> SentMessages { get; } = new List<OutboundResponse>();

        public Func<string, Task<IChatUser>> GetChatUserById { get; }
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
