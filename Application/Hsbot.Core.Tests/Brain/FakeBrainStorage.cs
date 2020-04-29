using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hsbot.Core.Brain;

namespace Hsbot.Core.Tests.Brain
{
    public class FakeBrainStorage<T> : IBotBrainStorage<T>
        where T: IBotBrain, new()
    {
        public T Brain { get; set; } = new T();
        public bool ThrowExceptionOnLoad { get; set; }
        public List<T> SavedBrains { get; } = new List<T>();

        public Task<T> Load()
        {
            if (ThrowExceptionOnLoad) throw new Exception();
            return Task.FromResult(Brain);
        }

        public Task Save(T brain)
        {
            SavedBrains.Add(brain);
            return Task.CompletedTask;
        }
    }
}