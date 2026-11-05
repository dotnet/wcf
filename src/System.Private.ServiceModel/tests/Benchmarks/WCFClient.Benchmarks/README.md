# Current-source WCF client microbenchmarks

This BenchmarkDotNet suite references the repository's Primitives project and targets `$(DotNetVersion)`. It measures SOAP 1.1 UTF-8 text encoding/decoding from the checkout, rather than a published WCF NuGet package. The historical package-based `WCFCorePerf` Crank client remains unchanged.

`MemoryDiagnoser` reports managed allocated bytes per operation and Gen0/Gen1/Gen2 collections per 1,000 operations when observed. The full JSON exporter retains raw measurements, statistics, environment information, and allocation/GC metrics. Default CSV, GitHub Markdown, and HTML reports are also generated.

## Run

Use the repository's required SDK and follow the root build instructions; do not change `global.json` or target frameworks. From the repository root, after selecting that SDK:

```powershell
$project = 'src\System.Private.ServiceModel\tests\Benchmarks\WCFClient.Benchmarks\WCFClient.Benchmarks.csproj'
dotnet build $project --configuration Release
dotnet run --project $project --configuration Release --no-build -- --validate
dotnet run --project $project --configuration Release --no-build -- --filter '*' --job Short --artifacts artifacts\benchmarks\text-encoding
```

`--validate` checks both workloads across all parameter combinations without timing them. Every benchmark process also verifies its round trip in global setup. Validation failures throw; failed benchmark execution or an empty selection returns a nonzero exit code.

Use `--job Dry` instead of `--job Short` for an execution/exporter smoke check only. For a more stable baseline, omit `--job Short` and use BenchmarkDotNet's default run policy. Run on an otherwise idle machine, do not benchmark other applications concurrently, and preserve the complete artifacts directory. No external service, network listener, firewall, elevation, or certificate configuration is needed.

Each global setup prints `WCF_SOURCE_ASSEMBLY` with the loaded Primitives assembly path, informational version, SHA-256, runtime, and BenchmarkDotNet version. This runs in the measured child process, not only the launcher. BenchmarkDotNet may rebuild project references for its generated executable, so compare the actual loaded assembly provenance rather than assuming it is byte-identical to the launcher's copy.

Record the exact Git revision and dirty status with each baseline:

```powershell
git rev-parse HEAD
git status --short
dotnet --info
```

Keep the benchmark definition, SDK, runtime patch, architecture, build configuration, GC mode, pool mode, and payload parameters matched when comparing source revisions. Do not compare the old package-based Crank results directly with these microbenchmarks.

## What is measured

| Parameter/workload | Meaning |
|---|---|
| `BodyCharacters` | 1,024, 65,536, or 1,048,576 ASCII characters in the string body; not total encoded SOAP bytes |
| `Pooling=false` | Real `BufferManager` with pooling disabled; rent allocations are part of the operation |
| `Pooling=true` | Real `BufferManager` with a reusable 8 MiB pool budget; normal BenchmarkDotNet warmup precedes measurement |
| `Encode` | Create a Message, encode into a rented contiguous array, return the array, and close the Message; return encoded byte count to the benchmark consumer |
| `Decode` | Rent and copy a pre-encoded SOAP message, decode it, fully deserialize its string body, and close the Message, which returns the decoder-owned array |

Encoder construction, body/seed creation, provenance hashing, correctness checks, and pool cleanup are outside timed operations. Message creation/disposal and decode input rent/copy are intentionally included. Decode returns the actual string so that parsing/deserialization cannot be optimized away.

The pool-enabled and pool-disabled rows are distinct workloads, not a universal cold/warm comparison. Encoder-internal caches may warm in either mode. Decode necessarily allocates the returned string even with pooling.

## Interpretation and limits

This adds a reproducible current-source allocation/GC baseline, not a runtime optimization or a transport load test. Short/Dry jobs have limited statistical confidence; a dry result must not be used as steady-state performance evidence. A blank GC column means no collections were observed in that measurement, not that no managed memory was allocated.

Allocated bytes/op are managed allocation traffic, not retained heap, working set, peak memory, or a leak measurement. Results do not establish HTTP/NetTcp throughput, end-to-end allocation/op, cancellation behavior, concurrency scaling, or server performance. This suite is an initial encoding baseline; additional workloads can be added independently.

No automatic CI performance threshold, timing assertion, or machine-specific baseline is imposed. There is no runtime behavior change requiring a production regression test; use `--validate` and a Dry job to check benchmark correctness and discovery.
