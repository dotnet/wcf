// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.Text.Json;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace WCFClient.Benchmarks
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "--validate")
            {
                foreach (int bodyCharacters in new[] { 1024, 65536, 1048576 })
                {
                    foreach (bool pooling in new[] { false, true })
                    {
                        var benchmark = new TextEncodingBenchmarks { BodyCharacters = bodyCharacters, Pooling = pooling };
                        try
                        {
                            benchmark.Setup();
                            benchmark.Validate();
                            Console.WriteLine($"PASS BodyCharacters={bodyCharacters}, Pooling={pooling}");
                        }
                        finally
                        {
                            benchmark.Cleanup();
                        }
                    }
                }

                return 0;
            }

            Summary[] summaries = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args).ToArray();
            return summaries.Length > 0 && summaries.All(summary =>
                !summary.HasCriticalValidationErrors && summary.Reports.Any() && summary.Reports.All(report => report.Success)) ? 0 : 1;
        }

        internal static void WriteProvenance()
        {
            Assembly assembly = typeof(MessageEncoder).Assembly;
            Console.WriteLine("WCF_SOURCE_ASSEMBLY " + JsonSerializer.Serialize(new
            {
                assembly.Location,
                informationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion,
                sha256 = Convert.ToHexString(SHA256.HashData(System.IO.File.ReadAllBytes(assembly.Location))),
                runtime = RuntimeInformation.FrameworkDescription,
                benchmarkDotNetVersion = typeof(BenchmarkSwitcher).Assembly.GetName().Version?.ToString()
            }));
        }
    }
}
