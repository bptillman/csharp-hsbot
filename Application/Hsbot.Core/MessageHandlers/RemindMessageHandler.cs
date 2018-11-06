using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsbot.Core.Messaging;
using Hsbot.Core.Messaging.Formatting;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class RemindMessageHandler : MessageHandlerBase, IDisposable
    {
        private readonly Regex _remindRegex = new Regex(@"remind me in ((?:(?:\d+) (?:weeks?|days?|hours?|hrs?|minutes?|mins?|seconds?|secs?)[ ,]*(?:and)? +)+)to (.*)", RegexOptions.Compiled);

        private readonly object _remindersLock = new Object();
        private List<Reminder> _reminders;

        private IDisposable _reminderTimerHandle;

        public const string BrainStorageKey = "Reminders";

        public RemindMessageHandler(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield return new MessageHandlerDescriptor
            {
                Command = "remind me in <time> to <action>",
                Description = "Set a reminder in <time> to do an <action> <time> is in the format 1 day, 2 hours, 5 minutes etc. Time segments are optional, as are commas"
            };
        }

        protected override void OnBotProvidedServicesConfigured()
        {
            lock (_remindersLock)
            {
                _reminders = Brain.GetItem<List<Reminder>>(BrainStorageKey) ?? new List<Reminder>();
                SortRemindersByDate();
            }

            _reminderTimerHandle = Observable.Interval(new TimeSpan(0, 0, 0, 1))
                .Select(t => Observable.FromAsync(ct => ReminderTimerElapsed()))
                .Concat()
                .Window(1)
                .Subscribe();
        }

        private void SortRemindersByDate()
        {
            _reminders.Sort((lhs,rhs) => DateTime.Compare(lhs.ReminderDateInUtc, rhs.ReminderDateInUtc));
        }

        private async Task ReminderTimerElapsed()
        {
            var remindersToSend = new List<Reminder>();
            lock (_remindersLock)
            {
                //since the list is always sorted, we can be sure that we only need to look
                //at the front of the list to find expired items
                while (_reminders.Count > 0 && _reminders[0].ReminderDateInUtc <= SystemClock.UtcNow)
                {
                    remindersToSend.Add(_reminders[0]);
                    _reminders.RemoveAt(0);
                }

                if (remindersToSend.Count > 0)
                {
                    Brain.SetItem(BrainStorageKey, _reminders);
                }
            }

            foreach (var reminder in remindersToSend)
            {
                var messageText = $"{MessageTextFormatter.FormatUserMention(reminder.UserId)} you asked me to remind you to {reminder.Message}";

                var outboundResponse = new OutboundResponse
                {
                    Channel = reminder.ChannelId,
                    MessageRecipientType = MessageRecipientType.Channel,
                    Text = messageText,
                    UserId = reminder.UserId
                };

                await SendMessage(outboundResponse);
            }
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.IsMatch(_remindRegex);
        }

        public override Task HandleAsync(InboundMessage message)
        {
            var match = message.Match(_remindRegex);
            var time = match.Groups[1].Value;
            var action = match.Groups[2].Value;

            var secondsOffset = GetSecondsOffsetFromTimeString(time);
            var reminderDateInUtc = SystemClock.UtcNow.AddSeconds(secondsOffset);

            var reminder = new Reminder
            {
                Message = action,
                ReminderDateInUtc = reminderDateInUtc,
                UserId = message.UserId,
                ChannelId = message.Channel,
            };

            lock (_remindersLock)
            {
                _reminders.Add(reminder);

                //always sort after adding a new entry so we can be sure that the
                //front of the list is next to expire
                SortRemindersByDate(); 
                Brain.SetItem(BrainStorageKey, _reminders);
            }

            return SendMessage(message.CreateResponse($"Ok, {MessageTextFormatter.FormatUserMention(reminder.UserId)}, I'll remind you to {action} on {MessageTextFormatter.FormatDate(reminderDateInUtc, DateFormat.DateNumeric)} at {MessageTextFormatter.FormatDate(reminderDateInUtc, DateFormat.TimeLong)}"));
        }

        private long GetSecondsOffsetFromTimeString(string time)
        {
            time = Regex.Replace(time, @"^\s+|\s+$", "");
            var result = TimePeriod.All.Sum(tp => tp.MatchedPeriodInSeconds(time));

            return result;
        }

        public void Dispose()
        {
            _reminderTimerHandle?.Dispose();
        }

        internal class Reminder
        {
            public string UserId { get; set; }
            public string ChannelId { get; set; }
            public DateTime ReminderDateInUtc { get; set; }
            public string Message { get; set; }
        }

        internal class TimePeriod
        {
            public TimePeriod(string matchString, long unitLengthInSeconds)
            {
                MatchRegex = new Regex("^.*?([\\d\\.]+)\\s*(?:(?:" + matchString + ")).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                UnitLengthInSeconds = unitLengthInSeconds;
            }

            public Regex MatchRegex { get; }
            public long UnitLengthInSeconds { get; }

            public long MatchedPeriodInSeconds(string time)
            {
                var match = MatchRegex.Match(time);
                if (match.Success)
                {
                    return UnitLengthInSeconds * int.Parse(match.Groups[1].Value);
                }

                return 0;
            }

            public static readonly TimePeriod Weeks = new TimePeriod("weeks?", 604800);
            public static readonly TimePeriod Days = new TimePeriod("days?", 86400);
            public static readonly TimePeriod Hours = new TimePeriod("hours?|hrs?", 3600);
            public static readonly TimePeriod Minutes = new TimePeriod("minutes?|mins?", 60);
            public static readonly TimePeriod Seconds = new TimePeriod("seconds?|secs?", 1);

            public static readonly TimePeriod[] All = {Weeks, Days, Hours, Minutes, Seconds};
        }
    }
}
