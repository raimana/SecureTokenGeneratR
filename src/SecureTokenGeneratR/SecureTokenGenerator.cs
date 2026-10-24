using System;
using System.Collections.Generic;
using System.Linq;
using SecureTokenGeneratR.Extensions;
using SecureTokenGeneratR.CharacterSets;
using static SecureTokenGeneratR.CryptographicNumberGenerator;

namespace SecureTokenGeneratR;

/// <summary>
/// Generates tokens using a configured token length and set of allowed characters.
/// </summary>
public interface ISecureTokenGenerator
{
    /// <summary>
    /// Generates a token using the configured options.
    /// </summary>
    /// <returns>The generated token.</returns>
    string Generate();
}

/// <summary>
/// An immutable configuration for generating cryptographically random tokens.
/// </summary>
public class SecureTokenGenerator : ISecureTokenGenerator
{
    private uint _allowedCharsetsSize;
    private uint _minimumTokenLength;
    private uint _maximumTokenLength;
    private uint _maxRepeatingCharCount;
    private IReadOnlyList<ICharacterSet> _allowedCharsets;

    private static List<ICharacterSet> DefaultAllowedCharSets
    {
        get
        {
            var lowerLatinCharset = new CharacterSet(97, 122);
            var upperLatinCharset = new CharacterSet(65, 90);
            var numbersCharset = new CharacterSet(48, 57);
            var specialCharset = new CharacterSet(@"!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~");
            return [lowerLatinCharset, upperLatinCharset, numbersCharset, specialCharset];
        }
    }

    /// <summary>
    /// Gets the estimated entropy, in bits, of a token generated with this configuration.
    /// </summary>
    public double GetTokenEntropy => _minimumTokenLength == _maximumTokenLength
        ? Entropy.CalculateTokenEntropy(_allowedCharsetsSize, _maximumTokenLength)
        : Entropy.CalculateTokenEntropy(_allowedCharsetsSize, _minimumTokenLength, _maximumTokenLength);

    /// <summary>
    /// Calculates the per-character entropy, in bits, of the specified token.
    /// </summary>
    /// <param name="token">The token to analyze.</param>
    /// <returns>The calculated per-character entropy.</returns>
    public double GetCharacterEntropy(string token) => Entropy.CalculatePerCharacterEntropy(token);

    /// <summary>
    /// Initializes a generator with the specified options.
    /// </summary>
    /// <param name="tokenLength">The fixed token length.</param>
    /// <param name="maxRepeatingCharCount">The maximum occurrences per character; zero disables the limit.</param>
    /// <param name="allowedCharSet">The allowed character sets, or <see langword="null"/> to use the defaults.</param>
    /// <remarks>Use the <see cref="SecureToken"/> fluent methods for new code.</remarks>
    /// <exception cref="ArgumentException">The allowed character sets cannot satisfy the repetition limit.</exception>
    [Obsolete("Use SecureToken.Default or the SecureToken fluent methods instead.")]
    public SecureTokenGenerator(uint tokenLength = 16, uint maxRepeatingCharCount = 0, IReadOnlyList<ICharacterSet> allowedCharSet = default)
        : this(tokenLength, maxRepeatingCharCount, allowedCharSet, true)
    {
    }

    private SecureTokenGenerator(uint tokenLength, uint maxRepeatingCharCount, IReadOnlyList<ICharacterSet> allowedCharSet, bool _)
        : this(tokenLength, tokenLength, maxRepeatingCharCount, allowedCharSet, true)
    {
    }

    private SecureTokenGenerator(uint minimumTokenLength, uint maximumTokenLength, uint maxRepeatingCharCount, IReadOnlyList<ICharacterSet> allowedCharSet, bool _)
    {
        SetOptions(minimumTokenLength, maximumTokenLength, maxRepeatingCharCount, allowedCharSet ?? DefaultAllowedCharSets);
    }

    internal static SecureTokenGenerator Create(uint tokenLength = 16, uint maxRepeatingCharCount = 0, IReadOnlyList<ICharacterSet> allowedCharSet = default) =>
        new(tokenLength, maxRepeatingCharCount, allowedCharSet, true);

    internal static SecureTokenGenerator CreateWithLengthRange(uint minimumTokenLength, uint maximumTokenLength, uint maxRepeatingCharCount = 0, IReadOnlyList<ICharacterSet> allowedCharSet = default)
    {
        if (minimumTokenLength == 0)
            throw new ArgumentOutOfRangeException(nameof(minimumTokenLength), "Token length must be greater than zero.");
        if (maximumTokenLength < minimumTokenLength || maximumTokenLength > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(maximumTokenLength), "Maximum token length must be at least the minimum and no greater than Int32.MaxValue.");

        return new SecureTokenGenerator(minimumTokenLength, maximumTokenLength, maxRepeatingCharCount, allowedCharSet ?? DefaultAllowedCharSets, true);
    }

    /// <summary>
    /// Returns a generator configured with a fixed token length.
    /// </summary>
    /// <param name="tokenLength">The token length.</param>
    /// <returns>A new generator with the specified token length.</returns>
    public SecureTokenGenerator WithLength(uint tokenLength) =>
        Create(tokenLength, _maxRepeatingCharCount, _allowedCharsets);

    /// <summary>
    /// Returns a generator that randomly selects a token length from an inclusive range for each token.
    /// </summary>
    /// <param name="minimumTokenLength">The smallest token length, inclusive.</param>
    /// <param name="maximumTokenLength">The largest token length, inclusive.</param>
    /// <returns>A new generator with the specified token length range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The range is invalid or exceeds the maximum supported array length.</exception>
    public SecureTokenGenerator WithLength(uint minimumTokenLength, uint maximumTokenLength) =>
        CreateWithLengthRange(minimumTokenLength, maximumTokenLength, _maxRepeatingCharCount, _allowedCharsets);

    /// <summary>
    /// Returns a generator with a limit on how many times a character may appear in a token.
    /// </summary>
    /// <param name="maxRepeatingCharCount">The maximum occurrences per character; zero disables the limit.</param>
    /// <returns>A new generator with the specified repetition limit.</returns>
    /// <exception cref="ArgumentException">The configured character sets cannot satisfy the repetition limit.</exception>
    public SecureTokenGenerator WithMaxRepeatingCharCount(uint maxRepeatingCharCount) =>
        CreateWithCurrentLengthRange(maxRepeatingCharCount, _allowedCharsets);

    /// <summary>
    /// Returns a generator with the specified character sequence added to its allowed sets.
    /// </summary>
    /// <param name="characters">The characters to add as an allowed set.</param>
    /// <returns>A new generator with the additional sequence.</returns>
    public SecureTokenGenerator AddCharset(IEnumerable<char> characters) =>
        AddCharset(new CharacterSet(characters));

    /// <summary>
    /// Returns a generator with the specified inclusive Unicode range added to its allowed sets.
    /// </summary>
    /// <param name="startRange">The first character in the range.</param>
    /// <param name="endRange">The last character in the range.</param>
    /// <returns>A new generator with the additional range.</returns>
    public SecureTokenGenerator AddCharset(int startRange, int endRange) =>
        AddCharset(new CharacterSet(startRange, endRange));

    /// <summary>
    /// Returns a generator whose only allowed character set is the specified sequence.
    /// </summary>
    /// <param name="characters">The characters to use.</param>
    /// <returns>A new generator configured to use the specified sequence.</returns>
    public SecureTokenGenerator ReplaceCharsets(IEnumerable<char> characters) =>
        CreateWithCurrentLengthRange(_maxRepeatingCharCount, [new CharacterSet(characters)]);

    /// <summary>
    /// Returns a generator whose only allowed character set is the specified inclusive Unicode range.
    /// </summary>
    /// <param name="startRange">The first character in the range.</param>
    /// <param name="endRange">The last character in the range.</param>
    /// <returns>A new generator configured to use the specified range.</returns>
    public SecureTokenGenerator ReplaceCharsets(int startRange, int endRange) =>
        CreateWithCurrentLengthRange(_maxRepeatingCharCount, [new CharacterSet(startRange, endRange)]);

    /// <summary>
    /// Returns a generator configured with only the supplied character sets.
    /// </summary>
    /// <param name="allowedCharsets">The character sets to use.</param>
    /// <returns>A new generator configured with the supplied character sets.</returns>
    public SecureTokenGenerator ReplaceCharsets(IReadOnlyList<ICharacterSet> allowedCharsets) =>
        CreateWithCurrentLengthRange(_maxRepeatingCharCount, allowedCharsets);

    private SecureTokenGenerator AddCharset(ICharacterSet charset)
    {
        ICharacterSet[] allowedCharsets = [.. _allowedCharsets, charset];
        return CreateWithCurrentLengthRange(_maxRepeatingCharCount, allowedCharsets);
    }

    private SecureTokenGenerator CreateWithCurrentLengthRange(uint maxRepeatingCharCount, IReadOnlyList<ICharacterSet> allowedCharsets) =>
        _minimumTokenLength == _maximumTokenLength
            ? Create(_maximumTokenLength, maxRepeatingCharCount, allowedCharsets)
            : CreateWithLengthRange(_minimumTokenLength, _maximumTokenLength, maxRepeatingCharCount, allowedCharsets);

    private void SetOptions(uint minimumTokenLength, uint maximumTokenLength, uint maxRepeatingCharCount, IReadOnlyList<ICharacterSet> allowedCharSet = default)
    {
        _allowedCharsets = (allowedCharSet ?? DefaultAllowedCharSets).ToArray();

        _allowedCharsetsSize = (uint) _allowedCharsets.Aggregate(0, (total, next) => total + next.Count);
        if (maxRepeatingCharCount > 0 && maximumTokenLength > _allowedCharsetsSize * maxRepeatingCharCount)
            throw new ArgumentException("Not enough unique characters to satisfy the constraint of maximum repeating character");

        _minimumTokenLength = minimumTokenLength;
        _maximumTokenLength = maximumTokenLength;
        _maxRepeatingCharCount = maxRepeatingCharCount;
    }


    /// <summary>
    /// Generates a token using this generator's configuration.
    /// </summary>
    /// <returns>A token containing at least one character from each configured set.</returns>
    public string Generate()
    {
        var tokenLength = _minimumTokenLength == _maximumTokenLength
            ? _maximumTokenLength
            : GetRandomUIntFromRange(_minimumTokenLength, _maximumTokenLength);
        var tokenBuffer = new char[tokenLength];

        tokenBuffer = FillWithOneCharFromAllowedSets(tokenBuffer);
        tokenBuffer = FillRemainingPositions(tokenBuffer);
        tokenBuffer = ReplaceRepeatingChars(tokenBuffer, _maxRepeatingCharCount);

        return new string(tokenBuffer);
    }


    private char[] FillWithOneCharFromAllowedSets(char[] tokenBuffer)
    {
        foreach (var charSet in _allowedCharsets)
        {
            var indicesToExclude = tokenBuffer.FindAllIndices(c => c != '\0').ToHashSetCompat();
            var charPosition = GetRandomIntFromRange(0, tokenBuffer.Length, indicesToExclude);
            tokenBuffer[charPosition] = charSet.GetRandomChar();
        }

        return tokenBuffer;
    }


    private char[] FillRemainingPositions(char[] tokenBuffer)
    {
        var indicesToExclude = tokenBuffer.FindAllIndices(c => c != '\0').ToHashSetCompat();
        do
        {
            var charPosition = GetRandomIntFromRange(0, tokenBuffer.Length, indicesToExclude);
            tokenBuffer[charPosition] = GetCharFromRandomCharacterSet();
            indicesToExclude = tokenBuffer.FindAllIndices(c => c != '\0').ToHashSetCompat();
        } while (indicesToExclude.Count < tokenBuffer.Length);

        return tokenBuffer;
    }


    private char[] ReplaceRepeatingChars(char[] tokenBuffer, uint maxRepeatingChars)
    {
        if (tokenBuffer.Contains('\0'))
            throw new ArgumentException($"{nameof(tokenBuffer)} contains one or more null characters");

        if (maxRepeatingChars == 0)
            return tokenBuffer;

        int duplicateIndex;
        do
        {
            duplicateIndex = tokenBuffer
                .Select((chr, idx) => new {Index = idx, Text = chr})
                .GroupBy(group => group.Text)
                .Where(grouping => grouping.Count() > maxRepeatingChars)
                .SelectMany(g => g, (_, x) => x.Index)
                .DefaultIfEmpty(-1)
                .FirstOrDefault();

            if (duplicateIndex >= 0)
            {
                tokenBuffer[duplicateIndex] = GetCharFromRandomCharacterSet();
            }
        } while (duplicateIndex != -1);


        return tokenBuffer;
    }


    private char GetCharFromRandomCharacterSet() => _allowedCharsets[GetRandomUInt() % _allowedCharsets.Count].GetRandomChar();
}