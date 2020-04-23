using System;

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
    }
}
