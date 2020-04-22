using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Newtonsoft.Json;

namespace Hsbot.Core.Brain
{
    public class HsbotBrain : IBotBrain
    {
        private readonly ConcurrentDictionary<string, string> _brainContents = new ConcurrentDictionary<string, string>();
        
        private readonly Subject<HsbotBrain> _brainChanged = new Subject<HsbotBrain>();
        public IObservable<HsbotBrain> BrainChanged => _brainChanged.AsObservable();

        public HsbotBrain()
        {
        }

        public HsbotBrain(IDictionary<string, string> brainContents)
        {
            SetContents(brainContents);
        }

        public ICollection<string> Keys => _brainContents.Keys;

        private void SetContents(IDictionary<string, string> brainContents)
        {
            foreach (var key in brainContents.Keys)
            {
                _brainContents[key] = brainContents[key];
            }
        }

        public T GetItem<T>(string key)
            where T: class
        {
            if (!_brainContents.ContainsKey(key))
                return null;

            var value = _brainContents[key];
            return JsonConvert.DeserializeObject<T>(value);
        }

        public PersistenceState SetItem<T>(string key, T value)
            where T: class
        {
            _brainContents[key] = JsonConvert.SerializeObject(value);
            _brainChanged.OnNext(this);

            return PersistenceState.InMemoryOnly;
        }
    }
}