using System;
using Hsbot.Core.Messaging.Formatting;

namespace Hsbot.Slack
{
    public class SlackChatMessageTextFormatter : IChatMessageTextFormatter
    {
        //formatted as per https://api.slack.com/docs/message-formatting#message_formatting
        public string FormatDate(DateTime date, string format)
        {
            var formatString = format
                .Replace(DateFormat.DateNumeric, "{date_num}")
                .Replace(DateFormat.Date, "{date}")
                .Replace(DateFormat.DateLong, "{date_short}")
                .Replace(DateFormat.Time, "{time}")
                .Replace(DateFormat.TimeLong, "{time_secs}");

            var unixEpochTime = GetUnixEpochTime(date);
            var fallbackText = date.ToLongDateString();
            return $"<!date^{unixEpochTime}^{formatString}|{fallbackText}>";
        }

        private static long GetUnixEpochTime(DateTime date)
        {
            return new DateTimeOffset(date).ToUnixTimeSeconds();
        }
    }
}
