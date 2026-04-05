using BenchmarkDotNet.Attributes;
using DataAccess.Models;

namespace ScriptureMemory.Server.Tools;

[MemoryDiagnoser]
[ShortRunJob]
public class Benchmark
{
    [Params("John 3:16", "Ps.148.14-Ps.148.112", "1 John 2 1-5, 23")]
    public string Input { get; set; }

    [Benchmark(Baseline = true)]
    public Reference Parse() => ReferenceParser.Parse(Input);

    //[Benchmark]
    //public Reference ParseSpan() => ReferenceParser.ParseSpan(Input);
}
