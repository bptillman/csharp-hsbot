using System.Collections.Generic;

namespace Hsbot.Slack.Core.Brain
{
    public interface IBotBrain
    {
        ICollection<string> Keys { get; }
        T GetItem<T>(string key) where T: class;
        void SetItem<T>(string key, T value) where T: class;
    }
}
