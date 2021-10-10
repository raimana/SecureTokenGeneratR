using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using SecureTokenGeneratR.CharacterSets;
using Xunit;

namespace SecureTokenGeneratR.Tests
{
    public class SecureTokenGeneratorTests
    {
        // Token Complexity Rules
        private static bool HasNumber(string secureToken) => Regex.IsMatch(secureToken, @"[0-9]+");
        private static bool HasUpperChar(string secureToken) => Regex.IsMatch(secureToken, @"[A-Z]+");
        private static bool HasLowerChar(string secureToken) => Regex.IsMatch(secureToken, @"[a-z]+");

        private static bool HasSpecialChar(string secureToken) =>
            Regex.IsMatch(secureToken, @"[!""#$%&'()*+,-./\\:;<=>?@[\]^_`{|}~]+");

        private static bool HasMinimum16Chars(string secureToken) => Regex.IsMatch(secureToken, @".{16,}");
        

        [Fact]
        [Trait("Category", "Statistics")]
        public void ShouldGenerateUniqueSecureTokens()
        {
            var secureTokenGenerator = new SecureTokenGenerator();
            var secureTokenList = new List<string>();

            for (var i = 0; i < 200000; i++)
            {
                secureTokenList.Add(secureTokenGenerator.Generate());
            }

            var areAllTokensUnique = secureTokenList
                .GroupBy(s => s)
                .All(group => group.Count() == 1);

            areAllTokensUnique.Should().BeTrue();
        }


        [Fact]
        [Trait("Category", "Statistics")]
        public void ShouldGenerateTokensHonouringUserProvidedOptions()
        {
            var lowerLatinCharset = new CharsetAsUnicodeRange(97, 122);
            var upperLatinCharset = new CharsetAsUnicodeRange(65, 90);
            var numbersCharset = new CharsetAsUnicodeRange(48, 57);
            var specialCharset = new CharsetAsSequence(@"!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~");
            var allowedCharSet = new List<ICharacterSet> { lowerLatinCharset, upperLatinCharset, numbersCharset, specialCharset};

            const int maxRepeatingCharCount = 2;
            const int tokenLength = 24;
            var secureTokenGenerator = new SecureTokenGenerator(tokenLength, maxRepeatingCharCount, allowedCharSet);
            
            for (var i = 0; i < 200000; i++)
            {
                var secureToken = secureTokenGenerator.Generate();
                HasNumber(secureToken).Should().BeTrue();
                HasUpperChar(secureToken).Should().BeTrue();
                HasLowerChar(secureToken).Should().BeTrue();
                HasSpecialChar(secureToken).Should().BeTrue();
                
                var groupsExceedingMaxRepeatingCharCount = secureToken
                    .Select((chr, idx) => new { Index = idx, Text = chr })
                    .GroupBy(group => group.Text)
                    .Where(grouping => grouping.Count() > maxRepeatingCharCount);
                groupsExceedingMaxRepeatingCharCount.Should().BeEmpty();
            }
        }

        
        [Fact]
        [Trait("Category", "Statistics")]
        public void ShouldGenerateTokensHonouringTheDefaultOptions()
        {
            var secureTokenGenerator = new SecureTokenGenerator();

            for (var i = 0; i < 200000; i++)
            {
                var secureToken = secureTokenGenerator.Generate();

                HasNumber(secureToken).Should().BeTrue();
                HasUpperChar(secureToken).Should().BeTrue();
                HasLowerChar(secureToken).Should().BeTrue();
                HasSpecialChar(secureToken).Should().BeTrue();
                HasMinimum16Chars(secureToken).Should().BeTrue();
            }
        }
        
        
        [Fact]
        public void ShouldReturnAnErrorWhenCharsetSizeIsInsufficientToSatisfyMaxRepeatingCharacterConstraint()
        {
            Action secureTokenGeneratorInvocation = () =>
            {
                var allowedCharSet = new List<ICharacterSet> { new CharsetAsSequence("abc") };
                new SecureTokenGenerator(6, 1, allowedCharSet);
            };

            secureTokenGeneratorInvocation.Should().Throw<ArgumentException>();
        }


        [Fact]
        public void ShouldNotFailMaxRepeatingConstraintValidationWhenMaxIsInfinite()
        {
            Action secureTokenGeneratorInvocation = () =>
            {
                var allowedCharSet = new List<ICharacterSet> { new CharsetAsSequence("abc") };
                new SecureTokenGenerator(6, 0, allowedCharSet);
            };

            secureTokenGeneratorInvocation.Should().NotThrow();
        }
    }
}