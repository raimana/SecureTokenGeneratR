using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using PasswordGenerator.CharacterSets;
using Xunit;

namespace PasswordGenerator.Tests
{
    public class PasswordGeneratorTests
    {
        // Password Complexity Rules
        private static bool HasNumber(string passwordString) => Regex.IsMatch(passwordString, @"[0-9]+");
        private static bool HasUpperChar(string passwordString) => Regex.IsMatch(passwordString, @"[A-Z]+");
        private static bool HasLowerChar(string passwordString) => Regex.IsMatch(passwordString, @"[a-z]+");

        private static bool HasSpecialChar(string passwordString) =>
            Regex.IsMatch(passwordString, @"[!""#$%&'()*+,-./\\:;<=>?@[\]^_`{|}~]+");

        private static bool HasMinimum16Chars(string passwordString) => Regex.IsMatch(passwordString, @".{16,}");
        

        [Fact]
        [Trait("Category", "Statistics")]
        public void ShouldGenerateUniquePasswords()
        {
            var passwordGenerator = new PasswordGenerator();
            var passwordList = new List<string>();

            for (var i = 0; i < 200000; i++)
            {
                passwordList.Add(passwordGenerator.Generate());
            }

            var areAllPasswordsUnique = passwordList
                .GroupBy(s => s)
                .All(group => group.Count() == 1);

            areAllPasswordsUnique.Should().BeTrue();
        }


        [Fact]
        [Trait("Category", "Statistics")]
        public void ShouldGeneratePasswordsHonouringUserProvidedOptions()
        {
            var lowerLatinCharset = new UnicodeRangeCharacterSet(97, 122);
            var upperLatinCharset = new UnicodeRangeCharacterSet(65, 90);
            var numbersCharset = new UnicodeRangeCharacterSet(48, 57);
            var specialCharset = new CharsCollectionCharacterSet(@"!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~");
            var allowedCharSet = new List<ICharacterSet> { lowerLatinCharset, upperLatinCharset, numbersCharset, specialCharset};

            const int maxRepeatingCharCount = 2;
            var passwordGenerator = new PasswordGenerator(24, maxRepeatingCharCount, allowedCharSet);
            
            for (var i = 0; i < 200000; i++)
            {
                var password = passwordGenerator.Generate();
                HasNumber(password).Should().BeTrue();
                HasUpperChar(password).Should().BeTrue();
                HasLowerChar(password).Should().BeTrue();
                HasSpecialChar(password).Should().BeTrue();
                
                var groupsExceedingMaxRepeatingCharCount = password
                    .Select((chr, idx) => new { Index = idx, Text = chr })
                    .GroupBy(group => group.Text)
                    .Where(grouping => grouping.Count() > maxRepeatingCharCount);
                groupsExceedingMaxRepeatingCharCount.Should().BeEmpty();
            }
        }

        
        [Fact]
        [Trait("Category", "Statistics")]
        public void ShouldGeneratePasswordsHonouringDefaultPasswordOptionsRequirements()
        {
            var passwordGenerator = new PasswordGenerator();

            for (var i = 0; i < 200000; i++)
            {
                var password = passwordGenerator.Generate();

                HasNumber(password).Should().BeTrue();
                HasUpperChar(password).Should().BeTrue();
                HasLowerChar(password).Should().BeTrue();
                HasSpecialChar(password).Should().BeTrue();
                HasMinimum16Chars(password).Should().BeTrue();
            }
        }
        
        
        [Fact]
        public void ShouldReturnAnErrorWhenCharsetSizeIsInsufficientToSatisfyMaxRepeatingCharacterConstraint()
        {
            Action passwordGeneratorInvocation = () =>
            {
                var allowedCharSet = new List<ICharacterSet> { new CharsCollectionCharacterSet("abc") };
                new PasswordGenerator(6, 1, allowedCharSet);
            };

            passwordGeneratorInvocation.Should().Throw<ArgumentException>();
        }


        [Fact]
        public void ShouldNotFailMaxRepeatingConstraintValidationWhenMaxIsInfinite()
        {
            Action passwordGeneratorInvocation = () =>
            {
                var allowedCharSet = new List<ICharacterSet> { new CharsCollectionCharacterSet("abc") };
                new PasswordGenerator(6, 0, allowedCharSet);
            };

            passwordGeneratorInvocation.Should().NotThrow();
        }
    }
}