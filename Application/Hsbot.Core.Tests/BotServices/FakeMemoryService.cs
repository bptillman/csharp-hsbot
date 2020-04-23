using System.Collections.Generic;
using System.Linq;
using Hsbot.Core.BotServices;
using Hsbot.Core.Brain;

namespace Hsbot.Core.Tests.BotServices
{
    public class FakeMemoryService : IMemoryService
    {
        public Dictionary<string, Memory> Memories = new Dictionary<string, Memory>();
        public PersistenceState PersistenceState = PersistenceState.Persisted;

        public int Count => Memories.Count;

        public bool HasMemory(string key, out Memory value)
        {
            return Memories.TryGetValue(key, out value);
        }

        public bool GetMemory(string key, out Memory value)
        {
            if (!Memories.TryGetValue(key, out value)) return false;
            value.RememberCount++;

            return true;
        }

        public IEnumerable<Memory> GetAllMemories()
        {
            return Memories.Values.OrderByDescending(m => m.RememberCount);
        }

        public PersistenceState Remember(Memory memory)
        {
            Memories[memory.Key] = memory;
            return PersistenceState;
        }

        public PersistenceState Remember(string key, string value)
        {
            return Remember(new Memory {Key = key, Value = value, RememberCount = 0});
        }

        public bool Forget(string key, out Memory value)
        {
            return Memories.Remove(key, out value);
        }
    }
}