using System;
using System.Net;
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

        public string FormatUserMention(string userId)
        {
            return $"<@{userId}>";
        }

        public string FormatChannelMention(string channelName)
        {
            return $"<#{channelName}>";
        }

        public string FormatHyperlink(string url, string displayText)
        {
            var encodedUrl = url;
            if (url.Contains("?"))
            {
                var queryIndex = url.IndexOf("?", StringComparison.Ordinal);
                var queryString = queryIndex + 1 < url.Length
                    ? url.Substring(queryIndex + 1, url.Length - queryIndex - 1)
                    : "";

                encodedUrl = url.Substring(0, queryIndex) + "?" + WebUtility.UrlEncode(queryString);
            }

            return string.IsNullOrWhiteSpace(displayText)
                ? encodedUrl
                : $"<{encodedUrl}|{displayText}>";
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

        private static long GetUnixEpochTime(DateTime date)
        {
            return new DateTimeOffset(date).ToUnixTimeSeconds();
        }
    }
}
