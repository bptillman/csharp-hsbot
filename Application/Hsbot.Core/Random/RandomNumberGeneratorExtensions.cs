using System;
using System.Collections.Generic;
using System.Linq;

namespace Hsbot.Core.Random
{
    public static class RandomNumberGeneratorExtensions
    {
        public static T GetRandomValue<T>(this IRandomNumberGenerator rng, T[] values)
        {
            if (values == null || values.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(values)} argument cannot be empty");
            }

            return values[rng.Generate(0, values.Length)];
        }

        public static KeyValuePair<TKey, TValue> GetRandomValue<TKey, TValue>(this IRandomNumberGenerator rng, Dictionary<TKey, TValue> values)
        {
            if (values == null || values.Count == 0)
            {
                throw new InvalidOperationException($"{nameof(values)} argument cannot be empty");
            }

            var randomKey = rng.GetRandomValue(values.Keys.ToArray());
            return new KeyValuePair<TKey, TValue>(randomKey, values[randomKey]);
        }
    }
}
