```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8037/25H2/2025Update/HudsonValley2)
13th Gen Intel Core i7-13620H 2.40GHz, 1 CPU, 16 logical and 10 physical cores
.NET SDK 10.0.201
  [Host]   : .NET 8.0.25 (8.0.25, 8.0.2526.11203), X64 RyuJIT x86-64-v3 [AttachedDebugger]
  ShortRun : .NET 8.0.25 (8.0.25, 8.0.2526.11203), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method    | Input                | Mean       | Error     | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------- |--------------------- |-----------:|----------:|---------:|------:|--------:|-------:|----------:|------------:|
| **Parse**     | **1 John 2 1-5, 23**     |   **469.0 ns** |  **76.52 ns** |  **4.19 ns** |  **1.00** |    **0.01** | **0.0057** |    **1560 B** |        **1.00** |
| ParseSpan | 1 John 2 1-5, 23     |   319.0 ns |  60.39 ns |  3.31 ns |  0.68 |    0.01 | 0.0033 |     880 B |        0.56 |
|           |                      |            |           |          |       |         |        |           |             |
| **Parse**     | **John 3:16**            |   **228.6 ns** |  **82.98 ns** |  **4.55 ns** |  **1.00** |    **0.02** | **0.0041** |    **1032 B** |        **1.00** |
| ParseSpan | John 3:16            |   156.5 ns |  30.07 ns |  1.65 ns |  0.68 |    0.01 | 0.0017 |     456 B |        0.44 |
|           |                      |            |           |          |       |         |        |           |             |
| **Parse**     | **Ps.148.14-Ps.148.112** | **1,170.2 ns** | **806.79 ns** | **44.22 ns** |  **1.00** |    **0.05** | **0.0095** |    **2728 B** |        **1.00** |
| ParseSpan | Ps.148.14-Ps.148.112 |   950.8 ns | 140.83 ns |  7.72 ns |  0.81 |    0.03 | 0.0067 |    1800 B |        0.66 |
