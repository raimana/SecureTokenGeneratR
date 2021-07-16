using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using PasswordGenerator.CharacterSets;

namespace PasswordGenerator.Benchmarks
{
    class Program
    {
        static void Main(string[] args) => BenchmarkRunner.Run<PasswordGeneratorBenchmark>();
    }

    [MemoryDiagnoser]
    public class PasswordGeneratorBenchmark
    {
        [Benchmark]
        public void GeneratePasswordUsingDefault() => _ = new PasswordGenerator().Generate();

        [Benchmark]
        public void GeneratePasswordUsingCustomOptions()
        {
            var allowedCharSet = new List<ICharacterSet> { new CharsCollectionCharacterSet(@"0123456789abcdefghijklmnopqrstuvwxyz!@#$%^&*()_+}{][:';")};
            _ = new PasswordGenerator(54, 1, allowedCharSet).Generate();
        }
    }
}