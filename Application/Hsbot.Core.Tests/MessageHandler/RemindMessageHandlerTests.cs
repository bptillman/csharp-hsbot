using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.Brain;
using Hsbot.Core.Infrastructure;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Moq;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class RemindMessageHandlerTests : MessageHandlerTestBase<RemindMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[] {"remind me in 1 week to test", "remind me in 1 day to test", "remind me in 1 hour to test", "remind me in 1 minute to test"};
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] {"remind me", "remind me to test", "remind me in 1 foo to test", "remind me in 1 hour"};

        public async Task ShouldWriteReminderToBrain()
        {
            var brainMock = GetBrainMock();
            var botProvidedServices = new BotProvidedServicesFake
            {
                Brain = brainMock.Object
            };

            var handler = GetHandlerInstance(new RandomNumberGeneratorFake(), botProvidedServices);
            await handler.TestHandleAsync("remind me in 1 hour to test");

            brainMock.Verify(m => m.SetItem(RemindMessageHandler.BrainStorageKey, It.IsAny<List<RemindMessageHandler.Reminder>>()), Times.Once);
        }

        public async Task ShouldRespondWithConfirmation()
        {
            var brainMock = GetBrainMock();
            var botProvidedServices = new BotProvidedServicesFake
            {
                Brain = brainMock.Object
            };

            var handler = GetHandlerInstance(new RandomNumberGeneratorFake(), botProvidedServices);
            await handler.TestHandleAsync("remind me in 1 hour to test");

            botProvidedServices.SentMessages.Count.ShouldBe(1);
            botProvidedServices.SentMessages[0].Text.ShouldMatch("Ok, (.+), I'll remind you");
        }

        public Task ShouldSetReminderInOneSecond()
        {
            var now = DateTime.UtcNow;
            var expectedReminderTime = now.AddSeconds(1);
            return AssertReminderSet("remind me in 1 second to test", now, expectedReminderTime);
        }

        public Task ShouldSetReminderInOneMinute()
        {
            var now = DateTime.UtcNow;
            var expectedReminderTime = now.AddMinutes(1);
            return AssertReminderSet("remind me in 1 minute to test", now, expectedReminderTime);
        }

        public Task ShouldSetReminderInOneHour()
        {
            var now = DateTime.UtcNow;
            var expectedReminderTime = now.AddHours(1);
            return AssertReminderSet("remind me in 1 hour to test", now, expectedReminderTime);
        }

        public Task ShouldSetReminderInOneDay()
        {
            var now = DateTime.UtcNow;
            var expectedReminderTime = now.AddDays(1);
            return AssertReminderSet("remind me in 1 day to test", now, expectedReminderTime);
        }

        public Task ShouldSetReminderInOneWeek()
        {
            var now = DateTime.UtcNow;
            var expectedReminderTime = now.AddDays(7);
            return AssertReminderSet("remind me in 1 week to test", now, expectedReminderTime);
        }

        public Task ShouldSetReminderInFuture()
        {
            var now = DateTime.UtcNow;
            var expectedReminderTime = now
                .AddSeconds(1)
                .AddMinutes(2)
                .AddHours(3)
                .AddDays(4)
                .AddDays(5 * 7);

            return AssertReminderSet("remind me in 5 weeks 4 days 3 hours 2 minutes 1 second to test", now, expectedReminderTime);
        }

        private async Task AssertReminderSet(string messageText, DateTime now, DateTime expectedReminderTime)
        {
            var reminderList = new List<RemindMessageHandler.Reminder>();
            var brainMock = GetBrainMock(reminderList);

            var systemClockMock = new Mock<ISystemClock>();
            systemClockMock.Setup(m => m.UtcNow).Returns(now);

            var botProvidedServices = new BotProvidedServicesFake
            {
                Brain = brainMock.Object,
                SystemClock = systemClockMock.Object
            };

            var handler = GetHandlerInstance(new RandomNumberGeneratorFake(), botProvidedServices);
            await handler.TestHandleAsync(messageText);

            reminderList.Count.ShouldBe(1);
            reminderList[0].ReminderDateInUtc.ShouldBe(expectedReminderTime);

            botProvidedServices.SentMessages.Count.ShouldBe(1);
        }

        private Mock<IBotBrain> GetBrainMock(List<RemindMessageHandler.Reminder> reminderList = null)
        {
            reminderList = reminderList ?? new List<RemindMessageHandler.Reminder>();
            var mock = new Mock<IBotBrain>();
            mock.Setup(b => b.Keys).Returns(new List<string>());
            mock.Setup(b =>
                b.SetItem(RemindMessageHandler.BrainStorageKey, It.IsAny<List<RemindMessageHandler.Reminder>>()));
            mock.Setup(b => b.GetItem<List<RemindMessageHandler.Reminder>>(RemindMessageHandler.BrainStorageKey)).Returns(reminderList);

            return mock;
        }
    }
}
