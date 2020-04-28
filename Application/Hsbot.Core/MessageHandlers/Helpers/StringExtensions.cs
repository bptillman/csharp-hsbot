namespace Hsbot.Core.MessageHandlers.Helpers
{
    internal static class StringExtensions
    {
        public static string ToJiraAwardText(this string awardType)
        {
            switch (awardType.ToLower())
            {
                case "dfe":
                case "grit":
                    return "Drive for Excellence";
                case "pav":
                case "humility":
                    return "People are Valued";
                case "com":
                case "candor":
                    return "Honest Communication";
                case "plg":
                case "curiosity":
                    return "Passion for Learning and Growth";
                case "own":
                case "agency":
                    return "Own Your Experience";
                default:
                    return null;
            }
        }
    }
}
