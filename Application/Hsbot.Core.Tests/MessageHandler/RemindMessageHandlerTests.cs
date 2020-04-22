using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.BotServices;
using Hsbot.Core.Brain;
using Hsbot.Core.MessageHandlers;
using Hsbot.Core.Messaging.Formatting;
using Hsbot.Core.Tests.Infrastructure;
using Hsbot.Core.Tests.MessageHandler.Infrastructure;
using Shouldly;

namespace Hsbot.Core.Tests.MessageHandler
{
    public class RemindMessageHandlerTests : MessageHandlerTestBase<RemindMessageHandler>
    {
        protected override string[] MessageTextsThatShouldBeHandled => new[] {"remind me in 1 week to test", "remind me in 1 day to test", "remind me in 1 hour to test", "remind me in 1 minute to test"};
        protected override string[] MessageTextsThatShouldNotBeHandled => new[] {"remind me", "remind me to test", "remind me in 1 foo to test", "remind me in 1 hour"};

        public async Task ShouldWriteReminderToReminderService()
        {
            var reminderService = new FakeReminderService();
            var handler = GetHandlerInstance(reminderService);
            await handler.TestHandleAsync("remind me in 1 hour to test");

            reminderService.Reminders.Count.ShouldBe(1);
        }

        public async Task ShouldRespondWithConfirmation()
        {
            var botProvidedServices = new BotProvidedServicesFake();
            var reminderService = new FakeReminderService();
            var handler = GetHandlerInstance(reminderService, new TestSystemClock(), botProvidedServices);
            await handler.TestHandleAsync("remind me in 1 hour to test");

            botProvidedServices.SentMessages.Count.ShouldBe(1);
            botProvidedServices.SentMessages[0].Text.ShouldMatch("Ok, (.+), I'll remind you");
        }

        public async Task ShouldRespondWithWarningIfReminderNotPersisted()
        {
            var botProvidedServices = new BotProvidedServicesFake();
            var reminderService = new FakeReminderService {PersistenceState = PersistenceState.InMemoryOnly};
            var handler = GetHandlerInstance(reminderService, new TestSystemClock(), botProvidedServices);
            await handler.TestHandleAsync("remind me in 1 hour to test");

            botProvidedServices.SentMessages.Count.ShouldBe(2);
            botProvidedServices.SentMessages[0].Text.ShouldMatch("Ok, (.+), I'll remind you");
            botProvidedServices.SentMessages[1].Text.ShouldContain("Warning:");
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
            var systemClock = new TestSystemClock { UtcNow = now };
            var botProvidedServices = new BotProvidedServicesFake();

            var reminderService = new FakeReminderService();
            var handler = GetHandlerInstance(reminderService, systemClock, botProvidedServices);
            await handler.TestHandleAsync(messageText);

            reminderService.Reminders.Count.ShouldBe(1);
            reminderService.Reminders[0].ReminderDateInUtc.ShouldBe(expectedReminderTime);

            botProvidedServices.SentMessages.Count.ShouldBe(1);
        }

        protected override RemindMessageHandler GetHandlerInstance(BotProvidedServicesFake botProvidedServices = null)
        {
            return GetHandlerInstance(new FakeReminderService(), new TestSystemClock(), botProvidedServices);
        }

        private RemindMessageHandler GetHandlerInstance(FakeReminderService reminderService, TestSystemClock systemClock = null, BotProvidedServicesFake botProvidedServices = null)
        {
            //Since this RNG will always return 0, the check on the random roll in the handler will
            //always succeed, meaning the random roll will not cause the result of ShouldHandle
            //to be false
            var rng = new RandomNumberGeneratorFake { NextDoubleValue = 0.0 };
            systemClock ??= new TestSystemClock();
            var handler = new RemindMessageHandler(rng, systemClock, new InlineChatMessageTextFormatter(), reminderService)
            {
                BotProvidedServices = botProvidedServices ?? new BotProvidedServicesFake()
            };

            return handler;
        }

        private class FakeReminderService : IReminderService
        {
            public readonly List<Reminder> Reminders = new List<Reminder>();
            public PersistenceState PersistenceState { get; set; } = PersistenceState.Persisted;

            public PersistenceState AddReminder(Reminder reminder)
            {
                Reminders.Add(reminder);
                return PersistenceState;
            }

            public Task ProcessReminders()
            {
                return Task.CompletedTask;
            }
        }
    }
}
