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
        public void GenerateSecureTokenUsingDefault() => _ = new SecureTokenGenerator().Generate();

        [Benchmark]
        public void GenerateSecureTokenUsingCustomOptions()
        {
            var allowedCharSet = new List<ICharacterSet> { new CharsetAsSequence(@"0123456789abcdefghijklmnopqrstuvwxyz!@#$%^&*()_+}{][:';")};
            _ = new SecureTokenGenerator(54, 1, allowedCharSet).Generate();
        }
    }
}