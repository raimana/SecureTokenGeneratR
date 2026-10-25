using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SecureTokenGeneratR.CharacterSets;

namespace SecureTokenGeneratR.Benchmarks
{
    class Program
    {
        static void Main(string[] args) => BenchmarkRunner.Run<SecureTokenGeneratorBenchmark>();
    }

    [MemoryDiagnoser]
    public class SecureTokenGeneratorBenchmark
    {
        [Benchmark]
        public void GenerateSecureTokenUsingDefault() => _ = SecureToken.Default.Generate();

        [Benchmark]
        public void GenerateSecureTokenUsingCustomOptions()
        {
            var allowedCharSet = new List<ICharacterSet> { new CharacterSet(@"0123456789abcdefghijklmnopqrstuvwxyz!@#$%^&*()_+}{][:';")};
            _ = SecureToken.WithLength(54)
                .WithMaxRepeatingCharCount(1)
                .ReplaceCharsets(allowedCharSet)
                .Generate();
        }
    }
}