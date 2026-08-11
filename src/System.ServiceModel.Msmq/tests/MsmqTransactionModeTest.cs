// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Reflection;
using System.Runtime.Versioning;
using System.Transactions;
using Infrastructure.Common;
using Xunit;

// Verifies the 3-way transaction-mode dispatch in MsmqMessagingInterop.
// Originally pinned to MSMQ.Messaging.MessageQueueTransactionType
// (slice 5c). Slice 9 replaced MSMQ.Messaging with a hand-rolled
// P/Invoke layer; the dispatch contract is unchanged but the enum
// is now System.ServiceModel.Channels.MsmqTransactionMode, which is
// internal. We reflect through it by name so the test doesn't have
// to make the enum public just to satisfy the assertion.
[SupportedOSPlatform("windows")]
public static class MsmqTransactionModeTest
{
    private static readonly Type s_modeType =
        typeof(System.ServiceModel.NetMsmqBinding).Assembly
            .GetType("System.ServiceModel.Channels.MsmqTransactionMode", throwOnError: true);

    private static readonly MethodInfo s_getMode =
        typeof(System.ServiceModel.NetMsmqBinding).Assembly
            .GetType("System.ServiceModel.Channels.MsmqMessagingInterop", throwOnError: true)
            .GetMethod("GetTransactionMode", BindingFlags.Static | BindingFlags.NonPublic);

    private static int Invoke(bool exactlyOnce, Transaction ambient)
    {
        object result = s_getMode.Invoke(null, new object[] { exactlyOnce, ambient });
        return (int)result;
    }

    private static int ModeValue(string name) => (int)Enum.Parse(s_modeType, name);

    [WcfFact]
    public static void NonExactlyOnce_AnyAmbient_ReturnsNone()
    {
        int none = ModeValue("None");
        using var scope = new TransactionScope(TransactionScopeOption.RequiresNew);
        Assert.Equal(none, Invoke(false, Transaction.Current));
        Assert.Equal(none, Invoke(false, null));
    }

    [WcfFact]
    public static void ExactlyOnce_NoAmbient_ReturnsSingle()
    {
        Assert.Equal(ModeValue("Single"), Invoke(true, null));
    }

    [WcfFact]
    public static void ExactlyOnce_WithAmbient_ReturnsAutomatic()
    {
        using var scope = new TransactionScope(TransactionScopeOption.RequiresNew);
        Assert.Equal(ModeValue("Automatic"), Invoke(true, Transaction.Current));
    }
}
