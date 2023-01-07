using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Services.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> result)
        {
            if (IsNullOrEmpty(enumerable))
                return;
            
            foreach (var current in enumerable)
            {
                result?.Invoke(current);
            }
        }
        
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> result)
        {
            if (IsNullOrEmpty(enumerable))
                return;
            var index = 0;
            foreach (var current in enumerable)
            {
                result?.Invoke(current, index++);
            }
        }
        
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        public static IEnumerable<int> Range(this int count, int startfrom = 0)
        {
            return Enumerable.Range(startfrom, count);
        }

        public static void Enumerate(this int count, Action<int> result)
        {
            foreach (var index in count.Range())
            {
                result?.Invoke(index);
            }
        }
        
        public static IEnumerable<T> Select<T>(this int count, Func<int, T> result)
        {
            return count.Range().Select(result.Invoke);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable.IsNullOrEmpty())
                return Array.Empty<T>();
            
            var random = new Random();
            return enumerable.OrderBy(_ => random.Next());
        }
    }
}