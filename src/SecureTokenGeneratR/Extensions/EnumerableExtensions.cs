using System;
using System.Collections.Generic;

namespace SecureTokenGeneratR.Extensions;

/// <summary>
/// Provides extension methods for enumerable sequences.
/// </summary>
internal static class EnumerableExtensions
{
    /// <summary>
    /// Returns the indices of all items that satisfy a predicate.
    /// </summary>
    /// <typeparam name="TSource">The type of the sequence elements.</typeparam>
    /// <param name="items">The sequence to search.</param>
    /// <param name="predicate">The condition to test for each item.</param>
    /// <returns>The indices of items that satisfy <paramref name="predicate"/>.</returns>
    internal static IEnumerable<int> FindAllIndices<TSource>(this IEnumerable<TSource> items, Func<TSource, bool> predicate)
    {
        var index = 0;
        foreach (var item in items)
        {
            if (predicate(item)) 
                yield return index;
                
            index++;
        }
    }


    /// <summary>
    /// Creates a hash set containing the elements of a sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the sequence elements.</typeparam>
    /// <param name="source">The sequence to copy.</param>
    /// <returns>A hash set containing the sequence elements.</returns>
    internal static HashSet<TSource> ToHashSetCompat<TSource>(this IEnumerable<TSource> source) => source.ToHashSetCompat(null);


    private static HashSet<TSource> ToHashSetCompat<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return new HashSet<TSource>(source, comparer);
    }
}