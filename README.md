# HttpArena Benchmarks with GenHttp, Minimal Api and Controllers
## Benchmarking with Bombardier

### Setup
Install bombardier
`go install github.com/codesenberg/bombardier@latest`

Run the project
`dotnet run -c Release --project HttpArena\HttpArena.csproj`

Select which implementation you would like to test.

Run the load tests.
```
# Small Payload
bombardier -c 125 -d 30s -H "Accept: application/json" http://localhost:5000/api/small

# Medium Payload
bombardier -c 125 -d 30s -H "Accept: application/json" http://localhost:5000/api/medium

# Large Payload
bombardier -c 125 -d 30s -H "Accept: application/json" http://localhost:5000/api/large
```

### Results
| Payload Size | Framework | Reqs/sec Avg | Reqs/sec Stdev | Reqs/sec Max | Latency Avg (ms) | Latency Stdev (ms) | Latency Max (ms) | 2xx Requests | Throughput (MB/s) |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **Small** | GenHTTP (Kestrel) | 85,316.58 | 23,769.26 | 133,409.77 | 1.46 | 0.86 | 178.94 | 2,551,652 | 46.06 |
| **Small** | Minimal API | **106,762.31** | 24,852.93 | **142,083.93** | **1.17** | 0.73 | 255.69 | **3,187,555** | 33.53 |
| **Small** | Controller | 97,196.84 | 12,046.87 | 201,012.29 | 1.28 | 0.18 | **55.63** | 2,920,551 | 30.72 |
| **Medium** | GenHTTP (Kestrel) | **72,179.92** | 18,428.96 | 97,240.91 | **1.73** | 0.56 | 174.68 | **2,159,622** | **282.83** |
| **Medium** | Minimal API | 71,568.79 | 18,174.88 | **98,915.81** | 1.74 | 0.27 | **60.89** | 2,154,103 | 266.05 |
| **Medium** | Controller | 60,102.14 | 19,096.47 | 86,442.08 | 2.08 | 0.93 | 169.77 | 1,796,676 | 221.92 |
| **Large** | GenHTTP (Kestrel) | **3,237.88** | 2,420.89 | **27,402.62** | **44.12** | 12.99 | 426.87 | **85,040** | **1,095.68** |
| **Large** | Minimal API | 2,320.43 | 2,629.39 | 24,185.30 | 58.02 | 20.52 | 670.15 | 64,627 | 834.16 |
| **Large** | Controller | 2,339.13 | 2,465.69 | 19,913.71 | 57.20 | 10.81 | **314.85** | 65,552 | 845.68 |

[View Google Spreadsheet](https://docs.google.com/spreadsheets/d/16jWmyXpnuFT08jM68aFNuWIU7ewb5oxzVK00xudCrtc/edit?gid=0#gid=0)

### Takeaways

1. For small payloads Minimal API is the clear winner
2. For medium payloads the gap between minimal api and genhttp closes
3. For large payloads genhttp is the clear winner

## Benchmarking with BenchMarkDotNet
### Setup
Run the project
`dotnet run -c Release --project HttpBenchmark`

### Results
```
BenchmarkDotNet v0.15.8, Windows 11 (10.0.22000.2538/21H2/SunValley)
Intel Core i7-7700 CPU 3.60GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
```
| Method     | PayloadSize | Mean        | Error     | StdDev     | Median      | Q3          | Gen0    | Gen1    | Gen2    | Allocated  |
|----------- |------------ |------------:|----------:|-----------:|------------:|------------:|--------:|--------:|--------:|-----------:|
| **GenHttp**    | **large**       | **2,060.24 μs** | **66.439 μs** | **188.476 μs** | **2,032.99 μs** | **2,170.16 μs** | **54.6875** | **39.0625** | **39.0625** | **1175.06 KB** |
| MinimalApi | large       | 1,840.01 μs | 64.150 μs | 187.130 μs | 1,814.31 μs | 1,954.76 μs | 46.8750 | 31.2500 | 31.2500 | 1184.47 KB |
| Controller | large       | 1,973.29 μs | 72.764 μs | 207.600 μs | 1,929.09 μs | 2,082.51 μs | 54.6875 | 39.0625 | 39.0625 | 1172.58 KB |
| **GenHttp**    | **medium**      |   **102.23 μs** |  **1.969 μs** |   **4.023 μs** |   **102.02 μs** |   **104.85 μs** |  **0.4883** |       **-** |       **-** |   **14.18 KB** |
| MinimalApi | medium      |   116.97 μs |  2.318 μs |   6.026 μs |   115.01 μs |   120.51 μs |  0.4883 |       - |       - |   15.78 KB |
| Controller | medium      |   126.48 μs |  2.514 μs |   6.579 μs |   123.25 μs |   131.00 μs |  0.4883 |       - |       - |   17.39 KB |
| **GenHttp**    | **small**       |    **80.88 μs** |  **1.588 μs** |   **1.408 μs** |    **80.47 μs** |    **81.46 μs** |       **-** |       **-** |       **-** |    **2.99 KB** |
| MinimalApi | small       |    94.94 μs |  0.828 μs |   0.734 μs |    94.87 μs |    95.14 μs |       - |       - |       - |    4.57 KB |
| Controller | small       |   101.85 μs |  1.907 μs |   1.873 μs |   101.50 μs |   102.97 μs |  0.2441 |       - |       - |    6.19 KB |
