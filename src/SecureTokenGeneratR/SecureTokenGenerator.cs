using System;
using System.Collections.Generic;
using System.Linq;
using SecureTokenGeneratR.Extensions;
using SecureTokenGeneratR.CharacterSets;
using static SecureTokenGeneratR.CryptographicNumberGenerator;

namespace SecureTokenGeneratR
{
    public class SecureTokenGenerator : ISecureTokenGenerator
    {
        private uint _tokenLength;
        private uint _maxRepeatingCharCount;
        private IReadOnlyList<ICharacterSet> _allowedCharsets;

        private static List<ICharacterSet> DefaultAllowedCharSets
        {
            get
            {
                var lowerLatinCharset = new CharsetAsUnicodeRange(97, 122);
                var upperLatinCharset = new CharsetAsUnicodeRange(65, 90);
                var numbersCharset = new CharsetAsUnicodeRange(48, 57);
                var specialCharset = new CharsetAsSequence(@"!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~");
                return new List<ICharacterSet> { lowerLatinCharset, upperLatinCharset, numbersCharset, specialCharset };
            }
        }

        public SecureTokenGenerator(uint tokenLength = 16, uint maxRepeatingCharCount = 0,
            IReadOnlyList<ICharacterSet> allowedCharSet = default) =>
            SetOptions(tokenLength, maxRepeatingCharCount, allowedCharSet ?? DefaultAllowedCharSets);

        
        public void SetOptions(uint tokenLength, uint maxRepeatingCharCount,
            IReadOnlyList<ICharacterSet> allowedCharSet = default)
        {
            _allowedCharsets = allowedCharSet ?? DefaultAllowedCharSets;
            
            var allowedCharsetsSize = _allowedCharsets.Aggregate(0, (total, next) => total + next.Count);
            if (maxRepeatingCharCount > 0 && tokenLength >= allowedCharsetsSize * maxRepeatingCharCount) 
                throw new ArgumentException($"Not enough unique characters to satisfy the maximum repeating character constraint");

            _tokenLength = tokenLength;
            _maxRepeatingCharCount = maxRepeatingCharCount;
        }

        
        public string Generate()
        {
            var tokenBuffer = new char[_tokenLength];

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
                    .Select((chr, idx) => new { Index = idx, Text = chr })
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


        private char GetCharFromRandomCharacterSet() =>
            _allowedCharsets[GetRandomUInt() % _allowedCharsets.Count].GetRandomChar();
    }
}