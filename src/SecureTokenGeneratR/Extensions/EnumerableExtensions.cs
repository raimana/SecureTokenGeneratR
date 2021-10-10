using System;
using System.Collections.Generic;

namespace SecureTokenGeneratR.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<int> FindAllIndices<TSource>(this IEnumerable<TSource> items,
            Func<TSource, bool> predicate)
        {
            var index = 0;
            foreach (var item in items)
            {
                if (predicate(item))
                {
                    yield return index;
                }

                index++;
            }
        }

        
        public static HashSet<TSource> ToHashSetCompat<TSource>(this IEnumerable<TSource> source) => source.ToHashSetCompat(null);

        
        public static HashSet<TSource> ToHashSetCompat<TSource>(this IEnumerable<TSource> source,
            IEqualityComparer<TSource> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return new HashSet<TSource>(source, comparer);
        }
    }
}