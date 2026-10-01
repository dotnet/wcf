// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Net.Security;
using System.Reflection;
using System.Runtime.Versioning;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

// Regression coverage for the transport properties the binding is supposed to
// push onto every outgoing message. Each of these was previously stored on the
// binding and never written to a native propid, so the message went out with
// MSMQ's default behaviour while the caller believed otherwise. These tests
// drive the real property-population path through reflection so they can run
// without a queue manager.
[SupportedOSPlatform("windows")]
public static class MsmqSendPropertiesTest
{
    private static readonly Assembly s_assembly = typeof(NetMsmqBinding).Assembly;

    private static readonly Type s_interop =
        s_assembly.GetType("System.ServiceModel.Channels.MsmqMessagingInterop", throwOnError: true);

    private static readonly Type s_nativeMessage =
        s_assembly.GetType("System.ServiceModel.Channels.NativeMsmqMessage", throwOnError: true);

    // PROPID_M_* values, verified against the Windows SDK Mq.h.
    private const uint PropIdDelivery = 5;
    private const uint PropIdJournal = 7;
    private const uint PropIdPrivLevel = 23;
    private const uint PropIdAuthLevel = 24;
    private const uint PropIdHashAlg = 26;
    private const uint PropIdEncryptionAlg = 27;
    private const uint PropIdTrace = 41;

    private const byte DeliveryExpress = 0;
    private const byte DeliveryRecoverable = 1;
    private const byte JournalNone = 0;
    private const byte DeadLetter = 1;
    private const byte Journal = 2;

    private static object BuildMessage(MsmqBindingElementBase bindingElement)
    {
        object message = Activator.CreateInstance(s_nativeMessage, nonPublic: true);
        MethodInfo populate = s_interop.GetMethod(
            "PopulateBaseProperties", BindingFlags.Static | BindingFlags.NonPublic);
        populate.Invoke(null, new object[] { message, new byte[] { 1, 2, 3 }, 0, 3, bindingElement });
        return message;
    }

    private static int SlotCount(object message)
        => (int)s_nativeMessage.GetMethod("get_SlotCountForTests", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(message, null);

    private static uint PropId(object message, int index)
        => (uint)s_nativeMessage.GetMethod("GetPropIdForTests", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(message, new object[] { index });

    private static byte[] Slot(object message, int index)
        => (byte[])s_nativeMessage.GetMethod("GetSlotForTests", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(message, new object[] { index });

    private static bool TryFindSlot(object message, uint propId, out byte[] slot)
    {
        for (int i = 0; i < SlotCount(message); i++)
        {
            if (PropId(message, i) == propId)
            {
                slot = Slot(message, i);
                return true;
            }
        }
        slot = null;
        return false;
    }

    private static byte GetByteValue(object message, uint propId)
    {
        Assert.True(TryFindSlot(message, propId, out byte[] slot), $"PROPID {propId} was never written.");
        return slot[8];
    }

    private static uint GetUInt32Value(object message, uint propId)
    {
        Assert.True(TryFindSlot(message, propId, out byte[] slot), $"PROPID {propId} was never written.");
        return BitConverter.ToUInt32(slot, 8);
    }

    // Durable=true previously never reached PROPID_M_DELIVERY, so MSMQ applied
    // its EXPRESS default and every "guaranteed" message was lost on a queue
    // manager restart.
    [WcfTheory]
    [InlineData(true, false, DeliveryRecoverable)]
    [InlineData(false, false, DeliveryExpress)]
    [InlineData(false, true, DeliveryRecoverable)]
    [InlineData(true, true, DeliveryRecoverable)]
    public static void Delivery_ReflectsDurableAndExactlyOnce(bool durable, bool exactlyOnce, byte expected)
    {
        var bindingElement = new MsmqTransportBindingElement { Durable = durable, ExactlyOnce = exactlyOnce };
        Assert.Equal(expected, GetByteValue(BuildMessage(bindingElement), PropIdDelivery));
    }

    [WcfFact]
    public static void Journal_ReflectsUseSourceJournalAndDeadLetterQueue()
    {
        var noJournal = new MsmqTransportBindingElement
        {
            UseSourceJournal = false,
            DeadLetterQueue = DeadLetterQueue.None
        };
        Assert.Equal(JournalNone, GetByteValue(BuildMessage(noJournal), PropIdJournal));

        var sourceJournal = new MsmqTransportBindingElement
        {
            UseSourceJournal = true,
            DeadLetterQueue = DeadLetterQueue.None
        };
        Assert.Equal(Journal, GetByteValue(BuildMessage(sourceJournal), PropIdJournal));

        var both = new MsmqTransportBindingElement
        {
            UseSourceJournal = true,
            DeadLetterQueue = DeadLetterQueue.System
        };
        Assert.Equal((byte)(Journal | DeadLetter), GetByteValue(BuildMessage(both), PropIdJournal));
    }

    [WcfFact]
    public static void Trace_IsWrittenOnlyWhenUseMsmqTracingIsSet()
    {
        var tracing = new MsmqTransportBindingElement { UseMsmqTracing = true };
        Assert.Equal(1, GetByteValue(BuildMessage(tracing), PropIdTrace));

        var noTracing = new MsmqTransportBindingElement { UseMsmqTracing = false };
        Assert.False(TryFindSlot(BuildMessage(noTracing), PropIdTrace, out _));
    }

    // Transport security used to mutate managed state only: the authentication
    // and privacy propids were never written, so a caller asking for Sign or
    // EncryptAndSign silently got an unauthenticated, unencrypted message.
    [WcfFact]
    public static void TransportSecurity_Sign_WritesAuthLevelAndHashAlgorithm()
    {
        var bindingElement = new MsmqTransportBindingElement();
        bindingElement.MsmqTransportSecurity.MsmqAuthenticationMode = MsmqAuthenticationMode.WindowsDomain;
        bindingElement.MsmqTransportSecurity.MsmqProtectionLevel = ProtectionLevel.Sign;
        bindingElement.MsmqTransportSecurity.MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.Sha256;

        object message = BuildMessage(bindingElement);
        Assert.Equal(1u, GetUInt32Value(message, PropIdAuthLevel));      // MQMSG_AUTH_LEVEL_ALWAYS
        Assert.Equal(0x800Cu, GetUInt32Value(message, PropIdHashAlg));   // CALG_SHA_256
        Assert.Equal(0u, GetUInt32Value(message, PropIdPrivLevel));      // MQMSG_PRIV_LEVEL_NONE
    }

    [WcfFact]
    public static void TransportSecurity_EncryptAndSign_WritesPrivacyLevelAndCipher()
    {
        var bindingElement = new MsmqTransportBindingElement();
        bindingElement.MsmqTransportSecurity.MsmqAuthenticationMode = MsmqAuthenticationMode.WindowsDomain;
        bindingElement.MsmqTransportSecurity.MsmqProtectionLevel = ProtectionLevel.EncryptAndSign;
        bindingElement.MsmqTransportSecurity.MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.Aes;

        object message = BuildMessage(bindingElement);
        Assert.Equal(1u, GetUInt32Value(message, PropIdAuthLevel));
        Assert.Equal(0x05u, GetUInt32Value(message, PropIdPrivLevel));       // MQMSG_PRIV_LEVEL_BODY_AES
        Assert.Equal(0x6611u, GetUInt32Value(message, PropIdEncryptionAlg)); // CALG_AES
    }

    [WcfFact]
    public static void TransportSecurity_Disabled_WritesExplicitNoneLevels()
    {
        var bindingElement = new MsmqTransportBindingElement();
        bindingElement.MsmqTransportSecurity.MsmqAuthenticationMode = MsmqAuthenticationMode.None;
        bindingElement.MsmqTransportSecurity.MsmqProtectionLevel = ProtectionLevel.None;

        object message = BuildMessage(bindingElement);
        Assert.Equal(0u, GetUInt32Value(message, PropIdAuthLevel));
        Assert.Equal(0u, GetUInt32Value(message, PropIdPrivLevel));
    }

    [WcfTheory]
    [InlineData(MsmqSecureHashAlgorithm.MD5, 0x8003u)]
    [InlineData(MsmqSecureHashAlgorithm.Sha1, 0x8004u)]
    [InlineData(MsmqSecureHashAlgorithm.Sha256, 0x800Cu)]
    [InlineData(MsmqSecureHashAlgorithm.Sha512, 0x800Eu)]
    public static void HashAlgorithm_MapsToWinCryptCalgValue(MsmqSecureHashAlgorithm algorithm, uint expected)
    {
        MethodInfo map = s_interop.GetMethod("ToHashAlgorithmId", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.Equal(expected, (uint)map.Invoke(null, new object[] { algorithm }));
    }

    [WcfTheory]
    [InlineData(MsmqEncryptionAlgorithm.RC4Stream, 0x6801u)]
    [InlineData(MsmqEncryptionAlgorithm.Aes, 0x6611u)]
    public static void EncryptionAlgorithm_MapsToWinCryptCalgValue(MsmqEncryptionAlgorithm algorithm, uint expected)
    {
        MethodInfo map = s_interop.GetMethod("ToEncryptionAlgorithmId", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.Equal(expected, (uint)map.Invoke(null, new object[] { algorithm }));
    }
}
