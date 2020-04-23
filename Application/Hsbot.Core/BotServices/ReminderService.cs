using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Hsbot.Core.Brain;
using Hsbot.Core.Infrastructure;
using Hsbot.Core.Messaging;
using Hsbot.Core.Messaging.Formatting;

namespace Hsbot.Core.BotServices
{
    public class Reminder
    {
        public string UserId { get; set; }
        public string ChannelId { get; set; }
        public DateTime ReminderDateInUtc { get; set; }
        public string Message { get; set; }
    }

    public interface IReminderService
    {
        PersistenceState AddReminder(Reminder reminder);
        Task ProcessReminders();
    }

    public sealed class ReminderService : IReminderService, IBotService, IDisposable
    {
        private readonly ISystemClock _systemClock;
        private readonly IChatMessageTextFormatter _chatMessageTextFormatter;
        private readonly IBotBrain _brain;
        private IDisposable _reminderTimerHandle;
        private readonly object _remindersLock;
        private readonly List<Reminder> _reminders;
        private BotServiceContext _botServiceContext;

        public const string BrainStorageKey = "reminders";

        public int StartupOrder => 1; //needs to start after brain

        public ReminderService(ISystemClock systemClock, IChatMessageTextFormatter chatMessageTextFormatter, IBotBrain brain)
        {
            _systemClock = systemClock;
            _chatMessageTextFormatter = chatMessageTextFormatter;
            _brain = brain;
            _remindersLock = new object();
            _reminders = new List<Reminder>();
        }

        public async Task ProcessReminders()
        {
            var remindersToSend = new List<Reminder>();
            lock (_remindersLock)
            {
                //since the list is always sorted, we can be sure that we only need to look
                //at the front of the list to find expired items
                while (_reminders.Count > 0 && _reminders[0].ReminderDateInUtc <= _systemClock.UtcNow)
                {
                    remindersToSend.Add(_reminders[0]);
                    _reminders.RemoveAt(0);
                }

                if (remindersToSend.Count > 0)
                {
                    _brain.SetItem(BrainStorageKey, _reminders);
                }
            }

            foreach (var reminder in remindersToSend)
            {
                var messageText = $"{_chatMessageTextFormatter.FormatUserMention(reminder.UserId)} you asked me to remind you to {reminder.Message}";

                var outboundResponse = new OutboundResponse
                {
                    Channel = reminder.ChannelId,
                    MessageRecipientType = MessageRecipientType.Channel,
                    Text = messageText,
                    UserId = reminder.UserId
                };

                await _botServiceContext.Parent.SendMessage(outboundResponse);
            }
        }

        private async Task ReminderTimerElapsed()
        {
            await ProcessReminders();
        }

        public Task Start(BotServiceContext context)
        {
            if (_botServiceContext != null) throw new InvalidOperationException("Service is already started");

            lock (_remindersLock)
            {
                var persistedReminders = _brain.GetItem<List<Reminder>>(BrainStorageKey);
                if (persistedReminders != null)
                {
                    _reminders.AddRange(persistedReminders);
                }

                //always sort after adding a new entry so we can be sure that the
                //front of the list is next to expire
                _reminders.Sort((lhs, rhs) => DateTime.Compare(lhs.ReminderDateInUtc, rhs.ReminderDateInUtc));
            }

            _botServiceContext = context;
            _reminderTimerHandle = Observable.Interval(new TimeSpan(0, 0, 0, 1))
                .Select(t => Observable.FromAsync(ct => ReminderTimerElapsed()))
                .Concat()
                .Window(1)
                .Subscribe();

            return Task.CompletedTask;
        }

        public Task Stop()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _reminderTimerHandle?.Dispose();
        }

        public PersistenceState AddReminder(Reminder reminder)
        {
            lock (_remindersLock)
            {
                _reminders.Add(reminder);

                //always sort after adding a new entry so we can be sure that the
                //front of the list is next to expire
                _reminders.Sort((lhs, rhs) => DateTime.Compare(lhs.ReminderDateInUtc, rhs.ReminderDateInUtc));
                return _brain.SetItem(BrainStorageKey, _reminders);
            }
        }
    }
}