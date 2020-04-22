using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.BotServices;
using Hsbot.Core.Brain;
using Hsbot.Core.Messaging;
using Hsbot.Core.Messaging.Formatting;
using Hsbot.Core.Tests.Brain;
using Hsbot.Core.Tests.Infrastructure;
using static Hsbot.Core.Tests.ServiceMocks;
using Moq;
using Shouldly;

namespace Hsbot.Core.Tests.BotServices
{
    public class ReminderServiceTests
    {
        public void ShouldSaveReminderToBrain()
        {
            var now = DateTime.UtcNow;
            var systemClock = new TestSystemClock {UtcNow = now};

            var brain = new InMemoryBrain();

            var reminderService = new ReminderService(systemClock, new InlineChatMessageTextFormatter(), brain);

            var reminder = new Reminder
            {
                ChannelId = "test",
                Message = "this is a reminder",
                ReminderDateInUtc = now.AddSeconds(-1),
                UserId = "test"
            };

            reminderService.AddReminder(reminder);

            var reminderList = brain.GetItem<List<Reminder>>(ReminderService.BrainStorageKey);
            reminderList.Count.ShouldBe(1);
            reminderList[0].Message.ShouldBe(reminder.Message);
        }

        public void AddReminderShouldReturnBrainPersistenceState()
        {
            var brain = new FakeBrain {PersistenceState = PersistenceState.Persisted};
            var reminderService = new ReminderService(new TestSystemClock(), new InlineChatMessageTextFormatter(), brain);

            var reminder = new Reminder
            {
                ChannelId = "test",
                Message = "this is a reminder",
                ReminderDateInUtc = DateTime.UtcNow.AddSeconds(-1),
                UserId = "test"
            };

            var result = reminderService.AddReminder(reminder);
            result.ShouldBe(PersistenceState.Persisted);

            brain.PersistenceState = PersistenceState.InMemoryOnly;
            result = reminderService.AddReminder(reminder);
            result.ShouldBe(PersistenceState.InMemoryOnly);
        }

        public async Task ShouldSendRemindersPastDueDate()
        {
            var now = DateTime.UtcNow;
            var systemClock = new TestSystemClock { UtcNow = now };
            var brain = new InMemoryBrain();

            var reminderService = new ReminderService(systemClock, new InlineChatMessageTextFormatter(), brain);

            var pastDueReminder = new Reminder
            {
                ChannelId = "test",
                Message = "this is a reminder",
                ReminderDateInUtc = now.AddSeconds(-1),
                UserId = "test"
            };

            var futureReminder = new Reminder
            {
                ChannelId = "test",
                Message = "this is a future reminder",
                ReminderDateInUtc = now.AddHours(1),
                UserId = "test"
            };

            reminderService.AddReminder(pastDueReminder);
            reminderService.AddReminder(futureReminder);

            var reminderList = brain.GetItem<List<Reminder>>(ReminderService.BrainStorageKey);
            reminderList.Count.ShouldBe(2);

            var chatConnector = MockChatConnector();
            var hsbot = new Hsbot(MockLog().Object, Array.Empty<IInboundMessageHandler>(), Enumerable.Empty<IBotService>(), chatConnector.Object);
            var context = new BotServiceContext {Parent = hsbot};

            await reminderService.Start(context);
            await reminderService.ProcessReminders();

            chatConnector.Verify(x => x.SendMessage(It.IsAny<OutboundResponse>()), Times.Once);

            reminderList = brain.GetItem<List<Reminder>>(ReminderService.BrainStorageKey);
            reminderList.Count.ShouldBe(1);
            reminderList[0].Message.ShouldBe(futureReminder.Message);
            reminderList[0].ReminderDateInUtc.ShouldBe(futureReminder.ReminderDateInUtc);
        }
    }
}
