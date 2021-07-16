using System;
using System.Collections.Generic;
using System.Linq;
using PasswordGenerator.CharacterSets;
using PasswordGenerator.Extensions;
using static PasswordGenerator.CryptographicNumberGenerator;

namespace PasswordGenerator
{
    public class PasswordGenerator : IPasswordGenerator
    {
        private uint _passwordLength;
        private uint _maxRepeatingCharCount;
        private IReadOnlyList<ICharacterSet> _allowedCharsets;

        private static List<ICharacterSet> DefaultAllowedCharSets
        {
            get
            {
                var lowerLatinCharset = new UnicodeRangeCharacterSet(97, 122);
                var upperLatinCharset = new UnicodeRangeCharacterSet(65, 90);
                var numbersCharset = new UnicodeRangeCharacterSet(48, 57);
                var specialCharset = new CharsCollectionCharacterSet(@"!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~");
                return new List<ICharacterSet> { lowerLatinCharset, upperLatinCharset, numbersCharset, specialCharset };
            }
        }

        public PasswordGenerator(uint passwordLength = 16, uint maxRepeatingCharCount = 16,
            IReadOnlyList<ICharacterSet> allowedCharSet = default) =>
            SetOptions(passwordLength, maxRepeatingCharCount, allowedCharSet ?? DefaultAllowedCharSets);

        
        public void SetOptions(uint passwordLength, uint maxRepeatingCharCount,
            IReadOnlyList<ICharacterSet> allowedCharSet = default)
        {
            _allowedCharsets = allowedCharSet ?? DefaultAllowedCharSets;
            
            var allowedCharsetsSize = _allowedCharsets.Aggregate(0, (total, next) => total + next.Count);
            if (maxRepeatingCharCount > 0 && passwordLength >= allowedCharsetsSize * maxRepeatingCharCount) 
                throw new ArgumentException($"Not enough unique characters to satisfy the maximum repeating character constraint");

            _passwordLength = passwordLength;
            _maxRepeatingCharCount = maxRepeatingCharCount;
        }

        
        public string Generate()
        {
            var passwordBuffer = new char[_passwordLength];

            passwordBuffer = FillWithOneCharFromAllowedSets(passwordBuffer);
            passwordBuffer = FillRemainingPositions(passwordBuffer);
            passwordBuffer = ReplaceRepeatingChars(passwordBuffer, _maxRepeatingCharCount);

            return new string(passwordBuffer);
        }


        private char[] FillWithOneCharFromAllowedSets(char[] passwordBuffer)
        {
            foreach (var charSet in _allowedCharsets)
            {
                var indicesToExclude = passwordBuffer.FindAllIndices(c => c != '\0').ToHashSetCompat();
                var charPosition = GetRandomIntFromRange(0, passwordBuffer.Length, indicesToExclude);
                passwordBuffer[charPosition] = charSet.GetRandomChar();
            }

            return passwordBuffer;
        }


        private char[] FillRemainingPositions(char[] passwordBuffer)
        {
            var indicesToExclude = passwordBuffer.FindAllIndices(c => c != '\0').ToHashSetCompat();
            do
            {
                var charPosition = GetRandomIntFromRange(0, passwordBuffer.Length, indicesToExclude);
                passwordBuffer[charPosition] = GetCharFromRandomCharacterSet();
                indicesToExclude = passwordBuffer.FindAllIndices(c => c != '\0').ToHashSetCompat();
            } while (indicesToExclude.Count < passwordBuffer.Length);

            return passwordBuffer;
        }


        private char[] ReplaceRepeatingChars(char[] passwordBuffer, uint maxRepeatingChars)
        {
            if (passwordBuffer.Contains('\0'))
                throw new ArgumentException($"{nameof(passwordBuffer)} contains one or more null characters");

            if (maxRepeatingChars == 0)
                return passwordBuffer;

            int duplicateIndex;
            do
            {
                duplicateIndex = passwordBuffer
                    .Select((chr, idx) => new { Index = idx, Text = chr })
                    .GroupBy(group => group.Text)
                    .Where(grouping => grouping.Count() > maxRepeatingChars)
                    .SelectMany(g => g, (_, x) => x.Index)
                    .DefaultIfEmpty(-1)
                    .FirstOrDefault();

                if (duplicateIndex >= 0)
                {
                    passwordBuffer[duplicateIndex] = GetCharFromRandomCharacterSet();
                }
            } while (duplicateIndex != -1);


            return passwordBuffer;
        }


        private char GetCharFromRandomCharacterSet() =>
            _allowedCharsets[GetRandomUInt() % _allowedCharsets.Count].GetRandomChar();
    }
}