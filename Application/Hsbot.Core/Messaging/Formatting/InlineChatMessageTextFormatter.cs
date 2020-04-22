using System;

namespace Hsbot.Core.Messaging.Formatting
{
    public class InlineChatMessageTextFormatter : IChatMessageTextFormatter
    {
        public string FormatDate(DateTime date, string format)
        {
            var formatString = format
                .Replace(DateFormat.DateNumeric, "YYYY-MM-DD")
                .Replace(DateFormat.Date, "MMMM-dd-yyyy")
                .Replace(DateFormat.DateLong, "dddd, MMMM dd yyyy")
                .Replace(DateFormat.Time, "hh:mm tt")
                .Replace(DateFormat.TimeLong, "hh:mm:ss tt");

            return date.ToString(formatString);
        }

        public string FormatUserMention(string userId)
        {
            return userId;
        }

        public string FormatChannelMention(string channelName)
        {
            return channelName;
        }

        public string FormatHyperlink(string url, string displayText)
        {
            return $"{displayText} ({url})";
        }

        public string Bold(string text)
        {
            return $"*{text}*";
        }

        public string Italic(string text)
        {
            return $"_{text}_";
        }

        public string Strikethough(string text)
        {
            return $"~{text}~";
        }
    }
}