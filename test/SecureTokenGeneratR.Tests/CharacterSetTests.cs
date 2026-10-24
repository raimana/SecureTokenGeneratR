using System;
using SecureTokenGeneratR.CharacterSets;
using Shouldly;
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

            var characterSet = new CharacterSet(startUnicodeRange, endUnicodeRange);

            for (var i = 0; i < 200000; i++)
            {
                var randomChar = characterSet.GetRandomChar();

                char.IsLower(randomChar).ShouldBeTrue();
            }
        }

        [Fact]
        public void ShouldReturnACharacterFromProvidedCharArray()
        {
            var charArray = @"!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~".ToCharArray();

            var characterSet = new CharacterSet(charArray);

            for (var i = 0; i < 200000; i++)
            {
                var randomChar = characterSet.GetRandomChar();

                charArray.ShouldContain(randomChar);
            }
        }

        [Fact]
        public void ShouldFailWhenStartRangeIsEqualToEndRange()
        {
            const int startUnicodeRange = 97;
            const int endUnicodeRange = 97;

            Action unicodeCharacterSetInvocation = () => new CharacterSet(startUnicodeRange, endUnicodeRange);

            Should.Throw<ArgumentOutOfRangeException>(unicodeCharacterSetInvocation);
        }

        [Fact]
        public void ShouldFailWhenStartRangeIsGreaterThanEndRange()
        {
            const int startUnicodeRange = 100;
            const int endUnicodeRange = 97;

            Action unicodeCharacterSetInvocation = () => new CharacterSet(startUnicodeRange, endUnicodeRange);

            Should.Throw<ArgumentOutOfRangeException>(unicodeCharacterSetInvocation);
        }
    }
}