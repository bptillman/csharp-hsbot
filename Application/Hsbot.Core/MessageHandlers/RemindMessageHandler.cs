using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hsbot.Core.BotServices;
using Hsbot.Core.Brain;
using Hsbot.Core.Infrastructure;
using Hsbot.Core.Messaging;
using Hsbot.Core.Messaging.Formatting;
using Hsbot.Core.Random;

namespace Hsbot.Core.MessageHandlers
{
    public class RemindMessageHandler : MessageHandlerBase
    {
        private readonly ISystemClock _systemClock;
        private readonly IChatMessageTextFormatter _messageTextFormatter;
        private readonly IReminderService _reminderService;
        private readonly Regex _remindRegex = new Regex(@"remind me in ((?:(?:\d+) (?:weeks?|days?|hours?|hrs?|minutes?|mins?|seconds?|secs?)[ ,]*(?:and)? +)+)to (.*)", RegexOptions.Compiled);

        public RemindMessageHandler(IRandomNumberGenerator randomNumberGenerator, ISystemClock systemClock, IChatMessageTextFormatter messageTextFormatter, IReminderService reminderService) : base(randomNumberGenerator)
        {
            _systemClock = systemClock;
            _messageTextFormatter = messageTextFormatter;
            _reminderService = reminderService;
        }

        public override IEnumerable<MessageHandlerDescriptor> GetCommandDescriptors()
        {
            yield return new MessageHandlerDescriptor
            {
                Command = "remind me in <time> to <action>",
                Description = "Set a reminder in <time> to do an <action> <time> is in the format 1 day, 2 hours, 5 minutes etc. Time segments are optional, as are commas"
            };
        }

        protected override bool CanHandle(InboundMessage message)
        {
            return message.IsMatch(_remindRegex);
        }

        public override async Task HandleAsync(IInboundMessageContext context)
        {
            var message = context.Message;
            var match = message.Match(_remindRegex);
            var time = match.Groups[1].Value;
            var action = match.Groups[2].Value;

            var secondsOffset = GetSecondsOffsetFromTimeString(time);
            var reminderDateInUtc = _systemClock.UtcNow.AddSeconds(secondsOffset);

            var reminder = new Reminder
            {
                Message = action,
                ReminderDateInUtc = reminderDateInUtc,
                UserId = message.UserId,
                ChannelId = message.Channel,
            };

            var reminderResult = _reminderService.AddReminder(reminder);

            var responseText = $"Ok, {_messageTextFormatter.FormatUserMention(reminder.UserId)}, I'll remind you to {action} on {_messageTextFormatter.FormatDate(reminderDateInUtc, DateFormat.DateNumeric)} at {_messageTextFormatter.FormatDate(reminderDateInUtc, DateFormat.TimeLong)}";
            await context.SendResponse(responseText);

            if (reminderResult == PersistenceState.InMemoryOnly)
            {
                responseText = $"{_messageTextFormatter.Bold("Warning:")} my brain is currently on the fritz, so I won't remember your reminder if I'm rebooted";
                await context.SendResponse(responseText);
            }
        }

        private long GetSecondsOffsetFromTimeString(string time)
        {
            time = Regex.Replace(time, @"^\s+|\s+$", "");
            var result = TimePeriod.All.Sum(tp => tp.MatchedPeriodInSeconds(time));

            return result;
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
