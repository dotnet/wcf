// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Runtime.Versioning;
using System.ServiceModel.MsmqIntegration;
using Infrastructure.Common;
using Xunit;

// Pins the underlying integer value of every member of the
// MSMQ-Integration enums (AcknowledgeTypes, Acknowledgment,
// MessageType, MessagePriority). The values must match the native
// MQMSG_* / MQMSG_CLASS_* constants and the corresponding members
// in netfx System.Messaging exactly so that:
//
//   * round-tripping a property through the MSMQ ABI loses no
//     information,
//   * source-compatible callers porting from netfx see the same
//     numeric values they always saw, and
//   * an inadvertent rename or renumber during a future refactor is
//     caught at test time rather than at deployment time.
//
// Treat these as contract tests — only change them if you also
// intend to change the public enum surface, and only after
// confirming the native MSMQ constants haven't moved.
[SupportedOSPlatform("windows")]
public static class MsmqIntegrationEnumValueTest
{
    [WcfTheory]
    [InlineData(AcknowledgeTypes.None, 0)]
    [InlineData(AcknowledgeTypes.PositiveArrival, 1)]
    [InlineData(AcknowledgeTypes.PositiveReceive, 2)]
    [InlineData(AcknowledgeTypes.NegativeArrival, 4)]
    [InlineData(AcknowledgeTypes.FullReachQueue, 5)]
    [InlineData(AcknowledgeTypes.NegativeReceive, 8)]
    [InlineData(AcknowledgeTypes.NotAcknowledgeReceive, 12)]
    [InlineData(AcknowledgeTypes.FullReceive, 14)]
    public static void AcknowledgeTypes_Values(AcknowledgeTypes value, int expected)
    {
        Assert.Equal(expected, (int)value);
    }

    [WcfFact]
    public static void AcknowledgeTypes_NotAcknowledgeReachQueue_IsAliasFor_NegativeArrival()
    {
        // Both members are defined by netfx with the same underlying
        // value (4). Encoded as a fact rather than an InlineData pair
        // because xunit's xUnit1025 analyzer rejects two theory rows
        // that decay to the same boxed-int signature.
        Assert.Equal((int)AcknowledgeTypes.NegativeArrival, (int)AcknowledgeTypes.NotAcknowledgeReachQueue);
        Assert.Equal(4, (int)AcknowledgeTypes.NotAcknowledgeReachQueue);
    }

    [WcfFact]
    public static void AcknowledgeTypes_FlagComposition()
    {
        Assert.Equal(
            AcknowledgeTypes.PositiveArrival | AcknowledgeTypes.NegativeArrival,
            AcknowledgeTypes.FullReachQueue);
        Assert.Equal(
            AcknowledgeTypes.PositiveReceive | AcknowledgeTypes.NegativeReceive | AcknowledgeTypes.NegativeArrival,
            AcknowledgeTypes.FullReceive);
        Assert.Equal(
            AcknowledgeTypes.NegativeArrival | AcknowledgeTypes.NegativeReceive,
            AcknowledgeTypes.NotAcknowledgeReceive);
    }

    [WcfFact]
    public static void AcknowledgeTypes_HasFlagsAttribute()
    {
        Assert.NotNull(typeof(AcknowledgeTypes).GetCustomAttributes(typeof(FlagsAttribute), inherit: false));
        Assert.NotEmpty(typeof(AcknowledgeTypes).GetCustomAttributes(typeof(FlagsAttribute), inherit: false));
    }

    [WcfTheory]
    [InlineData(Acknowledgment.None, 0x0000)]
    [InlineData(Acknowledgment.ReachQueue, 0x0002)]
    [InlineData(Acknowledgment.Receive, 0x4000)]
    [InlineData(Acknowledgment.QueuePurged, 0x4001)]
    [InlineData(Acknowledgment.ReceiveTimeout, 0x4002)]
    [InlineData(Acknowledgment.ReachQueueTimeout, 0x4003)]
    [InlineData(Acknowledgment.ReceiveRejected, 0x4005)]
    [InlineData(Acknowledgment.BadDestinationQueue, 0x8000)]
    [InlineData(Acknowledgment.Purged, 0x8001)]
    [InlineData(Acknowledgment.QueueExceedMaximumSize, 0x8002)]
    [InlineData(Acknowledgment.AccessDenied, 0x8004)]
    [InlineData(Acknowledgment.HopCountExceeded, 0x8005)]
    [InlineData(Acknowledgment.BadSignature, 0x8006)]
    [InlineData(Acknowledgment.BadEncryption, 0x8007)]
    [InlineData(Acknowledgment.CouldNotEncrypt, 0x8008)]
    [InlineData(Acknowledgment.NotTransactionalQueue, 0x8009)]
    [InlineData(Acknowledgment.NotTransactionalMessage, 0x800A)]
    public static void Acknowledgment_Values(Acknowledgment value, int expected)
    {
        Assert.Equal(expected, (int)value);
    }

    [WcfTheory]
    [InlineData(MessageType.Normal, 1)]
    [InlineData(MessageType.Response, 2)]
    [InlineData(MessageType.Report, 3)]
    [InlineData(MessageType.Acknowledgment, 6)]
    public static void MessageType_Values(MessageType value, int expected)
    {
        Assert.Equal(expected, (int)value);
    }

    [WcfTheory]
    [InlineData(MessagePriority.Lowest, 0)]
    [InlineData(MessagePriority.VeryLow, 1)]
    [InlineData(MessagePriority.Low, 2)]
    [InlineData(MessagePriority.Normal, 3)]
    [InlineData(MessagePriority.AboveNormal, 4)]
    [InlineData(MessagePriority.High, 5)]
    [InlineData(MessagePriority.VeryHigh, 6)]
    [InlineData(MessagePriority.Highest, 7)]
    public static void MessagePriority_Values(MessagePriority value, int expected)
    {
        Assert.Equal(expected, (int)value);
    }

    // CLS compliance: ours-only enums (no longer flowing through
    // MSMQ.Messaging's [CLSCompliant(false)] surface) means the
    // MsmqMessage<T> / MsmqIntegrationMessageProperty members that
    // expose them no longer need member-level CLS exclusions.
    [WcfTheory]
    [InlineData(nameof(MsmqMessage<string>.AcknowledgeType))]
    [InlineData(nameof(MsmqMessage<string>.Acknowledgment))]
    [InlineData(nameof(MsmqMessage<string>.MessageType))]
    [InlineData(nameof(MsmqMessage<string>.Priority))]
    public static void MsmqMessageT_EnumMembers_NoCLSCompliantFalseAttribute(string memberName)
    {
        var prop = typeof(MsmqMessage<string>).GetProperty(memberName);
        Assert.NotNull(prop);
        var attrs = prop.GetCustomAttributes(typeof(CLSCompliantAttribute), inherit: false);
        Assert.Empty(attrs);
    }
}
