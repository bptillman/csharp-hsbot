namespace Hsbot.Slack.Core
{
    public interface IHsbotConfig
    {
        string SlackApiKey { get; set; }
    }

    public class HsbotConfig : IHsbotConfig
    {
      public string SlackApiKey { get; set; }
    }
}
