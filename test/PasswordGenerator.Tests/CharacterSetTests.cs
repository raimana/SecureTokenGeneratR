using System;
using FluentAssertions;
using PasswordGenerator.CharacterSets;
using Xunit;

namespace PasswordGenerator.Tests
{
    public class CharacterSetTests
    {
        [Fact]
        public void ShouldReturnACharacterFromProvidedUnicodeRange()
        {
            const int startUnicodeRange = 97; // lower case a
            const int endUnicodeRange = 122; // lower case z
            
            var characterSet = new UnicodeRangeCharacterSet(startUnicodeRange, endUnicodeRange);

            for (var i = 0; i < 200000; i++)
            {
                var randomChar = characterSet.GetRandomChar();

                char.IsLower(randomChar).Should().BeTrue();
            }
        }
        
        [Fact]
        public void ShouldReturnACharacterFromProvidedCharArray()
        {
            var charArray = @"!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~".ToCharArray();
            
            var characterSet = new CharsCollectionCharacterSet(charArray);

            for (var i = 0; i < 200000; i++)
            {
                var randomChar = characterSet.GetRandomChar();

                charArray.Should().Contain(randomChar);
            }
        }
        
        [Fact]
        public void ShouldFailWhenStartRangeIsEqualToEndRange()
        {
            const int startUnicodeRange = 97;
            const int endUnicodeRange = 97;

            Action unicodeCharacterSetInvocation = () =>  new UnicodeRangeCharacterSet(startUnicodeRange, endUnicodeRange);

            unicodeCharacterSetInvocation.Should().Throw<ArgumentOutOfRangeException>();
        }
        
        [Fact]
        public void ShouldFailWhenStartRangeIsGreaterThanEndRange()
        {
            const int startUnicodeRange = 100;
            const int endUnicodeRange = 97;

            Action unicodeCharacterSetInvocation = () =>  new UnicodeRangeCharacterSet(startUnicodeRange, endUnicodeRange);
            
            unicodeCharacterSetInvocation.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}