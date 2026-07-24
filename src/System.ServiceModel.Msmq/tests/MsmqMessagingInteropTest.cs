// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Reflection;
using System.Runtime.Versioning;
using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

// Pins the native MQ_ERROR_* → WCF-exception mapping after slice 9
// dropped the MSMQ.Messaging dependency. The native send path now
// passes the raw HRESULT from mqrt!MQSendMessage straight into
// MsmqException's (string, int) ctor; previously the wrapper layer
// went through MSMQ.Messaging.MessageQueueException whose .ErrorCode
// is the generic HRESULT (0x80004005). Slice 6 had to undo that with
// MessageQueueErrorCode; slice 9 eliminates the wrapper entirely so
// we no longer have a substitute layer to test through.
//
// These tests construct MsmqException directly (the public ctor) with
// the native MQ_ERROR_* code and verify the normalization table fires
// correctly. If a future refactor reintroduces a wrapper layer, the
// new wrapper must preserve the native code or these assertions will
// detect the regression.
[SupportedOSPlatform("windows")]
public static class MsmqMessagingInteropTest
{
    private static Type s_normalizedTypeProperty;

    private static Type GetNormalizedType(MsmqException ex)
    {
        if (s_normalizedTypeProperty == null)
        {
            s_normalizedTypeProperty = typeof(MsmqException);
        }
        PropertyInfo p = s_normalizedTypeProperty.GetProperty("NormalizedType",
            BindingFlags.Instance | BindingFlags.NonPublic);
        return (Type)p.GetValue(ex);
    }

    // Sanity check: the native MQ_ERROR_* code passed to MsmqException
    // round-trips through ErrorCode without bit munging. This is the
    // critical invariant — if it ever changes, every normalized
    // exception type silently becomes a bare MsmqException because the
    // TuneBehavior switch in MsmqException keys off the value.
    [WcfTheory]
    [InlineData(0xC00E0003u)] // MQ_ERROR_QUEUE_NOT_FOUND
    [InlineData(0xC00E001Bu)] // MQ_ERROR_IO_TIMEOUT
    [InlineData(0xC00E001Eu)] // MQ_ERROR_ILLEGAL_FORMATNAME
    [InlineData(0xC00E0025u)] // MQ_ERROR_ACCESS_DENIED
    public static void MsmqException_PreservesNativeMQErrorCode(uint nativeCode)
    {
        var ex = new MsmqException("native send failed", unchecked((int)nativeCode));
        Assert.Equal(unchecked((int)nativeCode), ex.ErrorCode);
    }

    // End-to-end pin of the post-slice-9 send-error path: passing the
    // native MQ_ERROR_* code from mqrt!MQSendMessage's return value
    // into MsmqException, then asking MsmqException.NormalizedType,
    // yields the right WCF exception type.
    [WcfTheory]
    [InlineData(0xC00E0003u, typeof(EndpointNotFoundException))] // QueueNotFound
    [InlineData(0xC00E001Bu, typeof(TimeoutException))]          // IOTimeout
    [InlineData(0xC00E001Eu, typeof(ArgumentException))]         // IllegalFormatName
    [InlineData(0xC00E0025u, typeof(AddressAccessDeniedException))]    // AccessDenied
    [InlineData(0xC00E000Eu, typeof(EndpointNotFoundException))] // RemoteMachineNotAvailable
    public static void MsmqException_NormalizesToWcfException(uint nativeCode, Type expected)
    {
        var ex = new MsmqException("native send failed", unchecked((int)nativeCode));
        Assert.Equal(expected, GetNormalizedType(ex));
    }
}
