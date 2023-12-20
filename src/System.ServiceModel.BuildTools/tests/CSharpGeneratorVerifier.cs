﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

public static class CSharpGeneratorVerifier<TSourceGenerator>
#if ROSLYN4_0_OR_GREATER
    where TSourceGenerator : IIncrementalGenerator, new()
#else
    where TSourceGenerator : ISourceGenerator, new()
#endif
{
    public class Test : CSharpSourceGeneratorTest<EmptySourceGeneratorProvider, DefaultVerifier>
    {
        public Test()
        {
            ReferenceAssemblies = System.ServiceModel.BuildTools.Tests.ReferenceAssembliesHelper.Default.Value;
            TestState.AdditionalReferences.Add(typeof(System.ServiceModel.ServiceContractAttribute).Assembly);
        }

        protected override CompilationOptions CreateCompilationOptions()
        {
            var compilationOptions = base.CreateCompilationOptions();
            return compilationOptions.WithSpecificDiagnosticOptions(
                 compilationOptions.SpecificDiagnosticOptions.SetItems(GetNullableWarningsFromCompiler()));
        }

        public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Default;

        private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
        {
            string[] args = { "/warnaserror:nullable" };
            var commandLineArguments = CSharpCommandLineParser.Default.Parse(args, baseDirectory: Environment.CurrentDirectory, sdkDirectory: Environment.CurrentDirectory);
            var nullableWarnings = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;

            return nullableWarnings;
        }

        protected override ParseOptions CreateParseOptions()
            => ((CSharpParseOptions)base.CreateParseOptions()).WithLanguageVersion(LanguageVersion);

#if ROSLYN4_0_OR_GREATER
        protected override IEnumerable<ISourceGenerator> GetSourceGenerators()
        {
            yield return new TSourceGenerator().AsSourceGenerator();
        }
#else
        protected override IEnumerable<ISourceGenerator> GetSourceGenerators()
        {
            yield return new TSourceGenerator();
        }
#endif

        protected override bool IsCompilerDiagnosticIncluded(Diagnostic diagnostic, CompilerDiagnostics compilerDiagnostics) => false;
    }
}

