using System;
using System.Collections.Generic;

namespace Hsbot.Core
{
    internal static class DictionaryExtensions
    {
        public static Dictionary<string, TValue> ToCaseInsensitiveDictionary<TValue>(this Dictionary<string, TValue> sourceDictionary)
        {
            return sourceDictionary == null || sourceDictionary.Count == 0
            ? new Dictionary<string, TValue>(StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, TValue>(sourceDictionary, StringComparer.OrdinalIgnoreCase);
        }
    }
}
