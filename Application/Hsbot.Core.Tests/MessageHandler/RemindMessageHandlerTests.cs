using System;
using System.Collections.Generic;
using System.Text;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class RemindMessageHandlerTests : MessageHandlerTestBase<RemindMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[] {"remind me in 1 week to test", "remind me in 1 day to test", "remind me in 1 hour to test", "remind me in 1 minute to test"};
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] {"remind me", "remind me to test", "remind me in 1 foo to test", "remind me in 1 hour"};
    }
}
