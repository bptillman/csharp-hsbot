using System.Text.RegularExpressions;

namespace Hsbot.Core.MessageHandlers.Helpers
{
    internal static class RegexExtensions
    {
        public static int? CaptureToInt(this Capture capture)
        {
            return capture == null || string.IsNullOrEmpty(capture.Value)
                ? (int?)null
                : int.Parse(capture.Value);
        }
    }
}
