using System.Collections.Generic;

namespace Hsbot.Core.Brain
{
    public interface IBotBrain
    {
        ICollection<string> Keys { get; }
        T GetItem<T>(string key) where T: class;
        PersistenceState SetItem<T>(string key, T value) where T: class;
    }
}
