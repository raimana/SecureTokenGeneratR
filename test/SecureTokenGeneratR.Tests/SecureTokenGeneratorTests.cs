using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SecureTokenGeneratR.CharacterSets;
using Shouldly;
using Xunit;

namespace SecureTokenGeneratR.Tests
{
    public class SecureTokenGeneratorTests
    {
        // Token Complexity Rules
        private static bool HasNumber(string secureToken) => Regex.IsMatch(secureToken, @"[0-9]+");
        private static bool HasUpperChar(string secureToken) => Regex.IsMatch(secureToken, @"[A-Z]+");
        private static bool HasLowerChar(string secureToken) => Regex.IsMatch(secureToken, @"[a-z]+");

        private static bool HasSpecialChar(string secureToken) => Regex.IsMatch(secureToken, @"[!""#$%&'()*+,-./\\:;<=>?@[\]^_`{|}~]+");

        private static bool HasMinimum16Chars(string secureToken) => Regex.IsMatch(secureToken, @".{16,}");


        [Fact]
        [Trait("Category", "Statistics")]
        public void ShouldGenerateUniqueSecureTokens()
        {
            var secureTokenGenerator = SecureToken.Default;
            var secureTokenList = new List<string>();

            for (var i = 0; i < 200000; i++)
            {
                secureTokenList.Add(secureTokenGenerator.Generate());
            }

            var areAllTokensUnique = secureTokenList
                .GroupBy(s => s)
                .All(group => group.Count() == 1);

            areAllTokensUnique.ShouldBeTrue();
        }


        [Fact]
        [Trait("Category", "Statistics")]
        public void ShouldGenerateTokensHonouringUserProvidedOptions()
        {
            var lowerLatinCharset = new CharacterSet(97, 122);
            var upperLatinCharset = new CharacterSet(65, 90);
            var numbersCharset = new CharacterSet(48, 57);
            var specialCharset = new CharacterSet(@"!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~");
            var allowedCharSet = new List<ICharacterSet> { lowerLatinCharset, upperLatinCharset, numbersCharset, specialCharset };

            const int maxRepeatingCharCount = 2;
            const int tokenLength = 24;
            var secureTokenGenerator = SecureToken.WithLength(tokenLength)
                .WithMaxRepeatingCharCount(maxRepeatingCharCount)
                .ReplaceCharsets(allowedCharSet);

            for (var i = 0; i < 200000; i++)
            {
                var secureToken = secureTokenGenerator.Generate();
                HasNumber(secureToken).ShouldBeTrue();
                HasUpperChar(secureToken).ShouldBeTrue();
                HasLowerChar(secureToken).ShouldBeTrue();
                HasSpecialChar(secureToken).ShouldBeTrue();

                var groupsExceedingMaxRepeatingCharCount = secureToken
                    .Select((chr, idx) => new { Index = idx, Text = chr })
                    .GroupBy(group => group.Text)
                    .Where(grouping => grouping.Count() > maxRepeatingCharCount);
                groupsExceedingMaxRepeatingCharCount.ShouldBeEmpty();
            }
        }

        [Fact]
        public void ShouldGenerateTokensWithFluentConfiguration()
        {
            var generator = SecureToken.ReplaceCharsets("ab")
                .AddCharset('X', 'Y')
                .WithLength(8)
                .WithMaxRepeatingCharCount(2);

            for (var i = 0; i < 10; i++)
            {
                var token = generator.Generate();

                token.Length.ShouldBe(8);
                token.Any(c => "ab".Contains(c)).ShouldBeTrue();
                token.Any(c => "XY".Contains(c)).ShouldBeTrue();
                token.GroupBy(character => character).All(group => group.Count() <= 2).ShouldBeTrue();
            }
        }

        [Fact]
        public void ShouldGenerateTokensWithRandomLengthWithinConfiguredRange()
        {
            var generator = SecureToken.WithLength(8, 24)
                .ReplaceCharsets("0123456789");

            var tokens = Enumerable.Range(0, 100)
                .Select(_ => generator.Generate())
                .ToList();

            tokens.All(token => token.Length is >= 8 and <= 24).ShouldBeTrue();
            tokens.Select(token => token.Length).Distinct().Count().ShouldBeGreaterThan(1);
        }

        [Fact]
        public void ShouldRejectInvalidRandomLengthRanges()
        {
            Should.Throw<ArgumentOutOfRangeException>(() => SecureToken.WithLength(0, 24));
            Should.Throw<ArgumentOutOfRangeException>(() => SecureToken.WithLength(24, 8));
            Should.Throw<ArgumentOutOfRangeException>(() => SecureToken.WithLength(8, (uint) int.MaxValue + 1));
        }

        [Fact]
        public void ShouldReplaceConfiguredCharsetsWithSequenceOrRange()
        {
            var sequenceToken = SecureToken.AddCharset("ab")
                .AddCharset("XY")
                .ReplaceCharsets("0123456789")
                .WithLength(8)
                .Generate();
            var rangeToken = SecureToken.AddCharset("ab")
                .ReplaceCharsets('0', '9')
                .WithLength(8)
                .Generate();

            sequenceToken.All(char.IsDigit).ShouldBeTrue();
            rangeToken.All(char.IsDigit).ShouldBeTrue();
        }

        [Fact]
        public void ShouldCopyAllowedCharsetsWhenConfiguringFluently()
        {
            var allowedCharSet = new List<ICharacterSet>
            {
                new CharacterSet("ab"),
                new CharacterSet("XY")
            };
            var generator = SecureToken.WithLength(8)
                .ReplaceCharsets(allowedCharSet);

            allowedCharSet.Clear();
            allowedCharSet.Add(new CharacterSet("z"));

            var token = generator.Generate();

            token.Any(c => "ab".Contains(c)).ShouldBeTrue();
            token.Any(c => "XY".Contains(c)).ShouldBeTrue();
            token.ShouldNotContain("z");
        }

        [Fact]
        public void ShouldGenerateDefaultTokensFromSecureTokenEntryPoint()
        {
            var token = SecureToken.Default.Generate();

            token.Length.ShouldBe(16);
        }


        [Fact]
        [Trait("Category", "Statistics")]
        public void ShouldGenerateTokensHonouringTheDefaultOptions()
        {
            var secureTokenGenerator = SecureToken.Default;

            for (var i = 0; i < 200000; i++)
            {
                var secureToken = secureTokenGenerator.Generate();

                HasNumber(secureToken).ShouldBeTrue();
                HasUpperChar(secureToken).ShouldBeTrue();
                HasLowerChar(secureToken).ShouldBeTrue();
                HasSpecialChar(secureToken).ShouldBeTrue();
                HasMinimum16Chars(secureToken).ShouldBeTrue();
            }
        }

        [Fact]
        public void ShouldKeepLegacyConstructorAvailableAndObsolete()
        {
            var constructor = typeof(SecureTokenGenerator).GetConstructor(new[]
            {
                typeof(uint),
                typeof(uint),
                typeof(IReadOnlyList<ICharacterSet>)
            });

            constructor.ShouldNotBeNull();
            Attribute.IsDefined(constructor, typeof(ObsoleteAttribute)).ShouldBeTrue();

            var allowedCharSet = new List<ICharacterSet> { new CharacterSet("abc") };
            var generator = (SecureTokenGenerator)constructor.Invoke(new object[] { 8u, 0u, allowedCharSet });
            generator.Generate().Length.ShouldBe(8);
        }


        [Fact]
        public void ShouldReturnAnErrorWhenCharsetSizeIsInsufficientToSatisfyMaxRepeatingCharacterConstraint()
        {
            Action secureTokenGeneratorInvocation = () =>
            {
                var allowedCharSet = new List<ICharacterSet> { new CharacterSet("abc") };
                _ = SecureToken.WithLength(6)
                    .WithMaxRepeatingCharCount(1)
                    .ReplaceCharsets(allowedCharSet);
            };

            Should.Throw<ArgumentException>(secureTokenGeneratorInvocation);
        }


        [Fact]
        public void ShouldNotFailMaxRepeatingConstraintValidationWhenMaxIsInfinite()
        {
            Action secureTokenGeneratorInvocation = () =>
            {
                var allowedCharSet = new List<ICharacterSet> { new CharacterSet("abc") };
                _ = SecureToken.WithLength(6)
                    .WithMaxRepeatingCharCount(0)
                    .ReplaceCharsets(allowedCharSet);
            };

            Should.NotThrow(secureTokenGeneratorInvocation);
        }

        [Fact]
        public void ShouldReturnPerCharacterEntropy()
        {
            var charset = new CharacterSet("0123456789ABCDEF");
            var allowedCharSet = new List<ICharacterSet> { charset };

            var secureTokenGenerator = SecureToken.WithLength(16)
                .WithMaxRepeatingCharCount(1)
                .ReplaceCharsets(allowedCharSet);
            var token = secureTokenGenerator.Generate();
            
            var characterEntropy = secureTokenGenerator.GetCharacterEntropy(token);
            characterEntropy.ShouldBe(4d);
        }

        [Fact]
        public void ShouldReturnTokenEntropy()
        {
            var charset = new CharacterSet("0123456789ABCDEF");
            var allowedCharSet = new List<ICharacterSet> { charset };

            var secureTokenGenerator = SecureToken.WithLength(16)
                .WithMaxRepeatingCharCount(1)
                .ReplaceCharsets(allowedCharSet);

            var tokenEntropy = secureTokenGenerator.GetTokenEntropy;
            tokenEntropy.ShouldBe(64d);
        }
    }
}