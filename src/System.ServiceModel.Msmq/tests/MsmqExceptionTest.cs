// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Reflection;
using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public static class MsmqExceptionTest
{
    private static (bool faultSender, bool faultReceiver, Type normalizedType) MapErrorCode(uint code)
    {
        var ex = new MsmqException("test", unchecked((int)code));
        Type t = typeof(MsmqException);
        bool faultSender = (bool)t.GetProperty("FaultSender", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ex);
        bool faultReceiver = (bool)t.GetProperty("FaultReceiver", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ex);
        Type outer = (Type)t.GetProperty("NormalizedType", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ex);
        return (faultSender, faultReceiver, outer);
    }

    [WcfTheory]
    [InlineData(0xC00E0003u, typeof(EndpointNotFoundException))] // QueueNotFound
    [InlineData(0xC00E001Eu, typeof(ArgumentException))]         // IllegalFormatName
    [InlineData(0xC00E0014u, typeof(ArgumentException))]         // IllegalQueuePathName
    [InlineData(0xC00E001Bu, typeof(TimeoutException))]          // IOTimeout
    [InlineData(0xC00E0026u, typeof(EndpointNotFoundException))] // QueueNotAvailable
    [InlineData(0xC00E000Eu, typeof(EndpointNotFoundException))] // RemoteMachineNotAvailable
    [InlineData(0xC00E000Bu, typeof(EndpointNotFoundException))] // ServiceNotAvailable
    [InlineData(0xC00E0050u, typeof(InvalidOperationException))] // TransactionUsage
    [InlineData(0xC00E0006u, typeof(InvalidOperationException))] // StaleHandle
    [InlineData(0xC00E0025u, typeof(AddressAccessDeniedException))]   // AccessDenied — now lives in Primitives (slice 10)
    [InlineData(0xC00E0009u, typeof(AddressAccessDeniedException))]   // SharingViolation
    [InlineData(0xC00E0027u, typeof(CommunicationException))]    // InsufficientResources
    public static void Normalized_MapsErrorCodeToWcfExceptionType(uint code, Type expected)
    {
        var (_, _, outer) = MapErrorCode(code);
        Assert.Equal(expected, outer);
    }

    [WcfFact]
    public static void Normalized_UnknownErrorCode_ReturnsSelf()
    {
        var ex = new MsmqException("test", 0x12345678);
        Type t = typeof(MsmqException);
        Type outer = (Type)t.GetProperty("NormalizedType", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ex);
        Assert.Null(outer);
        Exception normalized = (Exception)t.GetProperty("Normalized", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ex);
        Assert.Same(ex, normalized);
    }

    [WcfTheory]
    [InlineData(0xC00E001Bu, false, false)] // IOTimeout: neither faulted
    [InlineData(0xC00E001Eu, false, false)] // IllegalFormatName: neither
    [InlineData(0xC00E0026u, false, true)]  // QueueNotAvailable: receiver only
    [InlineData(0xC00E0003u, true, true)]   // QueueNotFound: both
    public static void FaultSenderReceiver_MatchesNetFxTable(uint code, bool expectedSender, bool expectedReceiver)
    {
        var (sender, receiver, _) = MapErrorCode(code);
        Assert.Equal(expectedSender, sender);
        Assert.Equal(expectedReceiver, receiver);
    }

    [WcfFact]
    public static void MsmqException_Default_ErrorCodeIsExternalExceptionDefault()
    {
        // ExternalException() uses E_FAIL (0x80004005) as the default
        // HResult on .NET (Core+). The default ctor is rarely useful for
        // MsmqException since callers normally have an actual MSMQ error
        // code; we just lock the current behavior so it doesn't change
        // silently.
        Assert.Equal(unchecked((int)0x80004005), new MsmqException().ErrorCode);
    }

    [WcfFact]
    public static void MsmqException_PreservesErrorCode()
    {
        var ex = new MsmqException("hi", unchecked((int)0xC00E0003u));
        Assert.Equal(unchecked((int)0xC00E0003u), ex.ErrorCode);
        Assert.Equal("hi", ex.Message);
    }
}
