namespace Hsbot.Core
{
    public interface IHsbotConfig
    {
        string SlackApiKey { get; set; }
        string BrainStorageConnectionString { get; }
        string BrainStorageKey { get; }
        string BrainName { get; }
        string TumblrApiKey { get; }
        string JiraApiKey { get; set; }
        string DictionaryApiKey { get; set; }
        string ThesaurusApiKey { get; set; }
    }

    public class HsbotConfig : IHsbotConfig
    {
      public string SlackApiKey { get; set; }
      public string BrainStorageConnectionString { get; set; }
      public string BrainStorageKey { get; set; }
      public string BrainName { get; set; }
      public string TumblrApiKey { get; set; }
      public string JiraApiKey { get; set; }
      public string DictionaryApiKey { get; set; }
      public string ThesaurusApiKey { get; set; }
    }
}
