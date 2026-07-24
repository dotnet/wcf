// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Reflection;
using System.Runtime.Versioning;
using Infrastructure.Common;
using Xunit;

// Regression tests for the slice-5c bug:
//   .NET 8+ disables implicit DTC promotion by default. Setting
//   TransactionManager.ImplicitDistributedTransactions from inside the
//   xunit-console test host is too late: the DTC proxy reads the flag
//   on first use, which xunit's discovery layer triggers before our
//   test code runs. The MSMQ transactional scenarios therefore have to
//   be gated by an environment variable that the dev / CI sets only
//   when the runtime has the flag in place from the start.
//
// These tests pin the env-var contract so a future refactor of
// ConditionalTestDetectors doesn't silently re-enable the failing
// scenarios on every machine.
[SupportedOSPlatform("windows")]
public static class MsmqConditionsTest
{
    private const string EnvVar = "WCF_MSMQ_ENABLE_DTC_TESTS";

    private static readonly Type s_detectors =
        typeof(ConditionalWcfTest).Assembly
            .GetType("Infrastructure.Common.ConditionalTestDetectors", throwOnError: true);

    private static bool InvokeDetector(string name)
    {
        MethodInfo m = s_detectors.GetMethod(name,
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.NotNull(m);
        return (bool)m.Invoke(null, null);
    }

    [WcfFact]
    public static void ImplicitDtcEnabled_DefaultsToFalse()
    {
        string previous = Environment.GetEnvironmentVariable(EnvVar);
        Environment.SetEnvironmentVariable(EnvVar, null);
        try
        {
            Assert.False(InvokeDetector("IsImplicitDtcEnabled"));
        }
        finally
        {
            Environment.SetEnvironmentVariable(EnvVar, previous);
        }
    }

    [WcfTheory]
    [InlineData("true")]
    [InlineData("TRUE")]
    [InlineData("True")]
    public static void ImplicitDtcEnabled_HonorsEnvVar(string envValue)
    {
        string previous = Environment.GetEnvironmentVariable(EnvVar);
        Environment.SetEnvironmentVariable(EnvVar, envValue);
        try
        {
            // Detector is Windows-only by design (MSMQ is Windows-only).
            // On non-Windows hosts the env var is ignored and the
            // detector always returns false.
            bool expected = OperatingSystem.IsWindows();
            Assert.Equal(expected, InvokeDetector("IsImplicitDtcEnabled"));
        }
        finally
        {
            Environment.SetEnvironmentVariable(EnvVar, previous);
        }
    }

    [WcfTheory]
    [InlineData("false")]
    [InlineData("0")]
    [InlineData("")]
    [InlineData("anything-else")]
    public static void ImplicitDtcEnabled_OnlyAcceptsTrueLiteral(string envValue)
    {
        string previous = Environment.GetEnvironmentVariable(EnvVar);
        Environment.SetEnvironmentVariable(EnvVar, envValue);
        try
        {
            Assert.False(InvokeDetector("IsImplicitDtcEnabled"));
        }
        finally
        {
            Environment.SetEnvironmentVariable(EnvVar, previous);
        }
    }

    // Regression: mqrt.dll presence is the gate for MSMQ scenario
    // tests. Anything that swaps the check (e.g. a service-running
    // check that misses non-running queue managers, or a registry probe
    // that needs admin) would silently start failing on Helix workers
    // that can't satisfy the new check.
    [WcfFact]
    public static void MsmqInstalled_ChecksMqrtDll()
    {
        bool detectorSaysInstalled = InvokeDetector("IsMsmqInstalled");
        string mqrt = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "mqrt.dll");
        Assert.Equal(System.IO.File.Exists(mqrt), detectorSaysInstalled);
    }
}
