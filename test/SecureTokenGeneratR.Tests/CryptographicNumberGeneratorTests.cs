using System.Collections.Generic;
using Shouldly;
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
                position.ShouldBeInRange(0, length - 1);
                position.ShouldNotBe(1);
                position.ShouldNotBe(2);
                position.ShouldNotBe(3);
                position.ShouldNotBe(5);
                position.ShouldNotBe(8);
                position.ShouldNotBe(13);
            }
        }
        
        [Fact]
        public void ShouldReturnPositiveNumber()
        {
            for (var i = 0; i < 200000; i++)
            {
                var randomUInt = GetRandomUInt();
                (randomUInt > 0u).ShouldBeTrue();
            }
        }
    }
}