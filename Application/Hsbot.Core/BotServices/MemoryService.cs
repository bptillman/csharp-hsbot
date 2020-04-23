using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hsbot.Core.Brain;

namespace Hsbot.Core.BotServices
{
    public class Memory
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int RememberCount { get; set; }
    }

    public interface IMemoryService
    {
        int Count { get; }

        bool HasMemory(string key, out Memory value);
        
        bool GetMemory(string key, out Memory value);

        IEnumerable<Memory> GetAllMemories();

        PersistenceState Remember(Memory memory);
        PersistenceState Remember(string key, string value);

        bool Forget(string key, out Memory value);
    }

    public class MemoryService : IBotService, IMemoryService
    {
        private readonly IBotBrain _brain;
        private readonly ConcurrentDictionary<string, Memory> _memories;
        private BotServiceContext _botServiceContext;

        public const string MemoriesBrainStorageKey = "memories";

        public MemoryService(IBotBrain brain)
        {
            _brain = brain;
            _memories = new ConcurrentDictionary<string, Memory>();
        }

        public static readonly int StartupOrder = BotStartupOrder.After(HsbotBrainService.StartupOrder);

        public int GetStartupOrder()
        {
            return StartupOrder;
        }

        public Task Start(BotServiceContext context)
        {
            if (_botServiceContext != null) throw new InvalidOperationException("Service is already started");
            _botServiceContext = context;

            var existingMemories = _brain.GetItem<Dictionary<string, Memory>>(MemoriesBrainStorageKey);
            if (existingMemories != null)
            {
                foreach (var existingMemory in existingMemories)
                {
                    _memories.TryAdd(existingMemory.Key, existingMemory.Value);
                }
            }

            return Task.CompletedTask;
        }

        public Task Stop()
        {
            return Task.CompletedTask;
        }

        public int Count => _memories.Count;

        public IEnumerable<Memory> GetAllMemories()
        {
            return _memories.ToArray().Select(kvp => kvp.Value);
        }

        public PersistenceState Remember(Memory memory)
        {
            _memories.AddOrUpdate(memory.Key, memory, (k, v) => memory);
            return SaveMemoriesToBrain();
        }

        public PersistenceState Remember(string key, string value)
        {
            return Remember(new Memory
            {
                Key = key,
                Value = value,
                RememberCount = 0
            });
        }

        public bool HasMemory(string key, out Memory value)
        {
            return _memories.TryGetValue(key, out value);
        }

        public bool GetMemory(string key, out Memory value)
        {
            if (!_memories.TryGetValue(key, out value)) return false;

            _memories[key].RememberCount++;
            SaveMemoriesToBrain();

            return true;
        }

        public bool Forget(string key, out Memory value)
        {
            if (!_memories.TryRemove(key, out value)) return false;
            SaveMemoriesToBrain();

            return true;
        }

        private PersistenceState SaveMemoriesToBrain()
        {
            return _brain.SetItem(MemoriesBrainStorageKey, _memories);
        }
    }
}
