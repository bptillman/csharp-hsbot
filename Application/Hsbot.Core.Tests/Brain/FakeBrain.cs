using System;
using System.Collections.Generic;
using Hsbot.Core.Brain;
using Newtonsoft.Json;

namespace Hsbot.Core.Tests.Brain
{
    public class FakeBrain : IBotBrain
    {
        public Dictionary<string, string> BrainContents = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public PersistenceState PersistenceState { get; set; } = PersistenceState.Persisted;

        public ICollection<string> Keys => BrainContents.Keys;
        
        public T GetItem<T>(string key) where T : class
        {
            if (!BrainContents.ContainsKey(key))
                return null;

            var value = BrainContents[key];
            return JsonConvert.DeserializeObject<T>(value);
        }

        public PersistenceState SetItem<T>(string key, T value) where T : class
        {
            BrainContents[key] = JsonConvert.SerializeObject(value);
            return PersistenceState;
        }

        public string BrainDump()
        {
            return JsonConvert.SerializeObject(BrainContents);
        }
    }
}
