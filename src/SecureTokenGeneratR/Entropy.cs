using System;
using System.Collections.Generic;
using System.Linq;

namespace SecureTokenGeneratR;

/// <summary>
/// Calculates entropy estimates for tokens.
/// </summary>
internal static class Entropy
{
    // Shannon's Entropy formula:
    // Information(x) = LogBase10(probability(x)) / LogBase2 <=> Information(x) = -LogBase2(probability(x))
    // Example: given a string of 16 unique characters (4 bits of information) with one occurence of the lower case letter "a"
    // Probability of "a" is 1/16
    // Number of bits required to encode the lower case "a" is: H = -LogBase2(0.0625) = 4 
    // See: https://en.wikipedia.org/wiki/Entropy_%28information_theory%29
    /// <summary>
    /// Calculates the Shannon entropy per character for the specified token.
    /// </summary>
    /// <param name="token">The token to analyze.</param>
    /// <returns>The per-character entropy in bits.</returns>
    internal static double CalculatePerCharacterEntropy(string token)
    {
        var characterMap = new Dictionary<char, int>();
        foreach (var character in token)
        {
            if (!characterMap.ContainsKey(character))
                characterMap.Add(character, 1);
            else
                characterMap[character] += 1;
        }

        return characterMap
            .Select(keyValuePair => (double) keyValuePair.Value / token.Length)
            .Aggregate(0.0, (accumulatedEntropy, charFrequency) => accumulatedEntropy - charFrequency * (Math.Log(charFrequency) / Math.Log(2)));
    }

    /// <summary>
    /// Estimates token entropy when each character is selected from a fixed-size range.
    /// </summary>
    /// <param name="range">The number of possible characters at each position.</param>
    /// <param name="length">The token length.</param>
    /// <returns>The estimated token entropy in bits.</returns>
    internal static double CalculateTokenEntropy(uint range, uint length)
    {
        var possibleCombinations = Math.Pow(range, length);
        return Math.Log(possibleCombinations, 2);
    }

    /// <summary>
    /// Estimates token entropy when the token length is selected uniformly from an inclusive range.
    /// </summary>
    /// <param name="range">The number of possible characters at each position.</param>
    /// <param name="minimumLength">The smallest token length, inclusive.</param>
    /// <param name="maximumLength">The largest token length, inclusive.</param>
    /// <returns>The estimated token entropy in bits.</returns>
    internal static double CalculateTokenEntropy(uint range, uint minimumLength, uint maximumLength)
    {
        var lengthCount = (double) maximumLength - minimumLength + 1;
        var averageLength = ((double) minimumLength + maximumLength) / 2;
        return Math.Log(lengthCount, 2) + averageLength * Math.Log(range, 2);
    }
}