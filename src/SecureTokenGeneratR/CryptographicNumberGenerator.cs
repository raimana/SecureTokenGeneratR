using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SecureTokenGeneratR;

public static class CryptographicNumberGenerator
{
    public static int GetRandomIntFromRange(int start, int end, HashSet<int> indicesToExclude = null)
    {
        indicesToExclude ??= [];
        var range = Enumerable.Range(start, end).Except(indicesToExclude).ToList();

        var randomIndex = GetRandomUInt() % range.Count;
        return range.ElementAt(randomIndex);
    }

    public static int GetRandomUInt()
    {
        using var rng = RandomNumberGenerator.Create();
        var buffer = new byte[sizeof(int)];
        rng.GetBytes(buffer);
        var randomNumber = Math.Abs(BitConverter.ToInt32(buffer, 0));
        return randomNumber;
    }
}