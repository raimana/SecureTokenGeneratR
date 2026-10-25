using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static SecureTokenGeneratR.CryptographicNumberGenerator;

namespace SecureTokenGeneratR.CharacterSets;

/// <summary>
/// Represents a set of distinct characters supplied as a sequence or inclusive Unicode range.
/// </summary>
public class CharacterSet : ICharacterSet
{
    private readonly char[] _characters;
    private readonly int _startRange;

    /// <summary>
    /// Gets the number of distinct characters in the set.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Creates a character set from the distinct characters in a sequence.
    /// </summary>
    /// <param name="characters">The character sequence.</param>
    public CharacterSet(IEnumerable<char> characters)
    {
        _characters = characters.Distinct().ToArray();
        Count = _characters.Length;
    }

    /// <summary>
    /// Creates a character set from an inclusive Unicode range.
    /// </summary>
    /// <param name="startRange">The first character in the range.</param>
    /// <param name="endRange">The last character in the range.</param>
    /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="startRange"/> is greater than or equal to <paramref name="endRange"/>.</exception>
    public CharacterSet(int startRange, int endRange)
    {
        if (startRange >= endRange)
            throw new System.ArgumentOutOfRangeException(nameof(startRange), startRange, "Start is equal or greater than end of range");

        Count = endRange - startRange + 1;
        _startRange = startRange;
    }

    /// <summary>
    /// Selects a random character from the set.
    /// </summary>
    /// <returns>A randomly selected character.</returns>
    public char GetRandomChar()
    {
        var randomCharSelected = '\0';

        if (Count > 0)
        {
            var randomValue = GetRandomUInt();
            randomCharSelected = _characters == null
                ? (char) (_startRange + randomValue % Count)
                : _characters[randomValue % Count];
        }

        Debug.Assert(randomCharSelected != '\0', $"{nameof(GetRandomChar)} returned a null character");

        return randomCharSelected;
    }
}
