using System;
using System.Collections.Generic;
using System.Linq;

namespace SecureTokenGeneratR
{
    public static class Entropy
    {
        // Based on Shannon's Entropy formula, expressed as:
        // Information(x) = LogBase10(probability(x)) / LogBase2 <=> Information(x) = -LogBase2(probability(x))
        // Example: given a string of 16 unique characters (4 bits of information) with one occurence of the lower case letter "a"
        // Probability of "a" is 1/16
        // Number of bits required to encode the lower case "a" is: H = -LogBase2(0.0625) = 4 
        // More details here: https://en.wikipedia.org/wiki/Entropy_%28information_theory%29
        public static double CalculatePerCharacterEntropy(string token)
        {
            var characterMap = new Dictionary<char, int>();
            foreach (var character in token)
            {
                if (!characterMap.ContainsKey(character))
                    characterMap.Add(character, 1);
                else
                    characterMap[character] += 1;
            }

            return characterMap.Select(keyValuePair => (double) keyValuePair.Value / token.Length)
                .Aggregate(0.0, (accumulatedEntropy, charFrequency) =>
                    accumulatedEntropy - charFrequency * (Math.Log(charFrequency) / Math.Log(2)));
        }

        public static double CalculateTokenEntropy(uint range, uint length)
        {
            var possibleCombinations = Math.Pow(range, length);
            return Math.Log(possibleCombinations, 2);
        }
    }
}