namespace Hsbot.Core
{
    public interface IHsbotConfig
    {
        string SlackApiKey { get; set; }
        string BrainStorageConnectionString { get; }
        string BrainStorageKey { get; }
        string BrainName { get; }
        string GoogleApiKey { get; set; }
    }

    public class HsbotConfig : IHsbotConfig
    {
      public string SlackApiKey { get; set; }
      public string BrainStorageConnectionString { get; set; }
      public string BrainStorageKey { get; set; }
      public string BrainName { get; set; }
      public string GoogleApiKey { get; set; }
    }
}
