using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SecureTokenGeneratR;

/// <summary>
/// Provides cryptographically sourced random numbers.
/// </summary>
internal static class CryptographicNumberGenerator
{
    /// <summary>
    /// Selects a random integer from a range, excluding specified values.
    /// </summary>
    /// <param name="start">The first value in the range.</param>
    /// <param name="end">The number of values in the range.</param>
    /// <param name="indicesToExclude">Values to omit from selection, or <see langword="null"/>.</param>
    /// <returns>A randomly selected value not present in <paramref name="indicesToExclude"/>.</returns>
    internal static int GetRandomIntFromRange(int start, int end, HashSet<int> indicesToExclude = null)
    {
        indicesToExclude ??= [];
        var range = Enumerable.Range(start, end).Except(indicesToExclude).ToList();

        var randomIndex = GetRandomUInt() % range.Count;
        return range.ElementAt(randomIndex);
    }

    /// <summary>
    /// Gets a random integer derived from a cryptographic random source.
    /// </summary>
    /// <returns>A cryptographically sourced random integer.</returns>
    internal static int GetRandomUInt()
    {
        using var rng = RandomNumberGenerator.Create();
        var buffer = new byte[sizeof(int)];
        rng.GetBytes(buffer);
        var randomNumber = Math.Abs(BitConverter.ToInt32(buffer, 0));
        return randomNumber;
    }

    /// <summary>
    /// Selects a uniformly distributed unsigned integer from an inclusive range.
    /// </summary>
    /// <param name="startInclusive">The smallest value in the range.</param>
    /// <param name="endInclusive">The largest value in the range.</param>
    /// <returns>A random value in the inclusive range.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="endInclusive"/> is less than <paramref name="startInclusive"/>.</exception>
    internal static uint GetRandomUIntFromRange(uint startInclusive, uint endInclusive)
    {
        if (startInclusive > endInclusive)
            throw new ArgumentOutOfRangeException(nameof(endInclusive), "The inclusive end must be at least the start.");

        var range = (ulong) endInclusive - startInclusive + 1;
        var acceptedRange = (1UL << 32) / range * range;
        uint randomNumber;

        do
        {
            using var rng = RandomNumberGenerator.Create();
            var buffer = new byte[sizeof(uint)];
            rng.GetBytes(buffer);
            randomNumber = BitConverter.ToUInt32(buffer, 0);
        } while ((ulong) randomNumber >= acceptedRange);

        return startInclusive + (uint) (randomNumber % range);
    }
}