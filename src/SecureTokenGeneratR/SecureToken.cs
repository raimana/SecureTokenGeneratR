using System.Collections.Generic;
using SecureTokenGeneratR.CharacterSets;

namespace SecureTokenGeneratR;

/// <summary>
/// Provides entry points for creating secure token generators with fluent configuration.
/// </summary>
public static class SecureToken
{
    private static readonly SecureTokenGenerator DefaultGenerator = SecureTokenGenerator.Create();

    /// <summary>
    /// Gets a reusable generator configured with the default token length and character sets.
    /// </summary>
    public static SecureTokenGenerator Default => DefaultGenerator;

    /// <summary>
    /// Creates a generator that produces tokens with a fixed length.
    /// </summary>
    /// <param name="tokenLength">The token length.</param>
    /// <returns>A generator configured with the specified token length.</returns>
    public static SecureTokenGenerator WithLength(uint tokenLength) =>
        SecureTokenGenerator.Create(tokenLength: tokenLength);

    /// <summary>
    /// Creates a generator that selects a random token length from an inclusive range for each token.
    /// </summary>
    /// <param name="minimumTokenLength">The smallest token length, inclusive.</param>
    /// <param name="maximumTokenLength">The largest token length, inclusive.</param>
    /// <returns>A generator configured with the specified token length range.</returns>
    public static SecureTokenGenerator WithLength(uint minimumTokenLength, uint maximumTokenLength) =>
        SecureTokenGenerator.CreateWithLengthRange(minimumTokenLength, maximumTokenLength);

    /// <summary>
    /// Creates a generator with a limit on how many times a character may appear in a token.
    /// </summary>
    /// <param name="maxRepeatingCharCount">The maximum occurrences per character; zero disables the limit.</param>
    /// <returns>A generator configured with the specified repetition limit.</returns>
    public static SecureTokenGenerator WithMaxRepeatingCharCount(uint maxRepeatingCharCount) =>
        SecureTokenGenerator.Create(maxRepeatingCharCount: maxRepeatingCharCount);

    /// <summary>
    /// Adds a character sequence to the default allowed character sets.
    /// </summary>
    /// <param name="characters">The characters to add as an allowed set.</param>
    /// <returns>A generator with the default sets and the additional sequence.</returns>
    public static SecureTokenGenerator AddCharset(IEnumerable<char> characters) =>
        Default.AddCharset(characters);

    /// <summary>
    /// Adds an inclusive Unicode range to the default allowed character sets.
    /// </summary>
    /// <param name="startRange">The first character in the range.</param>
    /// <param name="endRange">The last character in the range.</param>
    /// <returns>A generator with the default sets and the additional range.</returns>
    public static SecureTokenGenerator AddCharset(int startRange, int endRange) =>
        Default.AddCharset(startRange, endRange);

    /// <summary>
    /// Creates a generator whose only allowed character set is the specified sequence.
    /// </summary>
    /// <param name="characters">The characters to use.</param>
    /// <returns>A generator configured to use the specified sequence.</returns>
    public static SecureTokenGenerator ReplaceCharsets(IEnumerable<char> characters) =>
        SecureTokenGenerator.Create(allowedCharSet: [new CharacterSet(characters)]);

    /// <summary>
    /// Creates a generator whose only allowed character set is the specified inclusive Unicode range.
    /// </summary>
    /// <param name="startRange">The first character in the range.</param>
    /// <param name="endRange">The last character in the range.</param>
    /// <returns>A generator configured to use the specified range.</returns>
    public static SecureTokenGenerator ReplaceCharsets(int startRange, int endRange) =>
        SecureTokenGenerator.Create(allowedCharSet: [new CharacterSet(startRange, endRange)]);

    /// <summary>
    /// Creates a generator that uses only the supplied character sets.
    /// </summary>
    /// <param name="allowedCharsets">The character sets to use.</param>
    /// <returns>A generator configured with the supplied character sets.</returns>
    public static SecureTokenGenerator ReplaceCharsets(IReadOnlyList<ICharacterSet> allowedCharsets) =>
        SecureTokenGenerator.Create(allowedCharSet: allowedCharsets);

}
