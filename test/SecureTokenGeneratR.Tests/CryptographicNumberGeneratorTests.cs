using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using static SecureTokenGeneratR.CryptographicNumberGenerator;

namespace SecureTokenGeneratR.Tests
{
    public class CryptographicNumberGeneratorTests
    {
        [Fact]
        public void ShouldReturnNumberWithinRangeExceptExcludedIndices()
        {
            const int length = 16;
            var indicesToExclude = new HashSet<int> {1, 2, 3, 5, 8, 13};

            for (var i = 0; i < 200000; i++)
            {
                var position = GetRandomIntFromRange(0, length, indicesToExclude);
                position.Should().BeInRange(0, length - 1);
                position.Should().NotBe(1)
                    .And.NotBe(2)
                    .And.NotBe(3)
                    .And.NotBe(5)
                    .And.NotBe(8)
                    .And.NotBe(13);
            }
        }
        
        [Fact]
        public void ShouldReturnPositiveNumber()
        {
            for (var i = 0; i < 200000; i++)
            {
                var randomUInt = GetRandomUInt();
                randomUInt.Should().BePositive();
            }
        }
    }
}