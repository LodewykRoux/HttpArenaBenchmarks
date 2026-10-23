using BenchmarkDotNet.Running;
using HttpBenchmark;

var summary = BenchmarkRunner.Run<ApiBenchmarks>();
