using System;
using FluentAssertions;
using SecureTokenGeneratR.CharacterSets;
using Xunit;

namespace SecureTokenGeneratR.Tests
{
    public class CharacterSetTests
    {
        [Fact]
        public void ShouldReturnACharacterFromProvidedUnicodeRange()
        {
            const int startUnicodeRange = 97; // lower case a
            const int endUnicodeRange = 122; // lower case z
            
            var characterSet = new CharsetAsUnicodeRange(startUnicodeRange, endUnicodeRange);

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
            
            var characterSet = new CharsetAsSequence(charArray);

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

            Action unicodeCharacterSetInvocation = () =>  new CharsetAsUnicodeRange(startUnicodeRange, endUnicodeRange);

            unicodeCharacterSetInvocation.Should().Throw<ArgumentOutOfRangeException>();
        }
        
        [Fact]
        public void ShouldFailWhenStartRangeIsGreaterThanEndRange()
        {
            const int startUnicodeRange = 100;
            const int endUnicodeRange = 97;

            Action unicodeCharacterSetInvocation = () =>  new CharsetAsUnicodeRange(startUnicodeRange, endUnicodeRange);
            
            unicodeCharacterSetInvocation.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}