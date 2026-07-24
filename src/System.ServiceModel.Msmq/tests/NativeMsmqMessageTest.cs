// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Reflection;
using System.Runtime.Versioning;
using Infrastructure.Common;
using Xunit;

// Marshaling-layout tests for the post-slice-9 native MSMQ message
// builder. The full send path needs a running MSMQ to verify, but
// the per-slot byte layout written into the MQPROPVARIANT buffer is
// pure logic and can be pinned cheaply with reflection + byte-level
// assertions.
//
// If any of these tests start failing it means the in-memory layout
// stopped matching what mqrt.dll expects, and the next live send
// will either return a property-validation error (best case) or
// silently corrupt the message body (worst case).
[SupportedOSPlatform("windows")]
public static class NativeMsmqMessageTest
{
    private static readonly Type s_native =
        typeof(System.ServiceModel.NetMsmqBinding).Assembly
            .GetType("System.ServiceModel.Channels.NativeMsmqMessage", throwOnError: true);

    private static object NewMessage() => Activator.CreateInstance(s_native, nonPublic: true);

    private static void InvokeSetter(object message, string name, params object[] args)
    {
        // Resolve overload by name + parameter count to avoid having
        // to spell out the full parameter type list.
        MethodInfo m = null;
        foreach (var candidate in s_native.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (candidate.Name == name && candidate.GetParameters().Length == args.Length)
            {
                m = candidate;
                break;
            }
        }
        Assert.NotNull(m);
        m.Invoke(message, args);
    }

    private static byte[] GetSlot(object message, int index)
        => (byte[])s_native.GetMethod("GetSlotForTests", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(message, new object[] { index });

    private static uint GetPropId(object message, int index)
        => (uint)s_native.GetMethod("GetPropIdForTests", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(message, new object[] { index });

    private static int GetSlotCount(object message)
        => (int)s_native.GetMethod("get_SlotCountForTests", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(message, null);

    private static int VectorOffset
        => (int)s_native.GetMethod("get_VectorElementsOffsetForTests", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);

    private static int PropVarSize
        => (int)s_native.GetMethod("get_PropVariantSizeForTests", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);

    // ----- Layout invariants -----

    [WcfFact]
    public static void PropVariantSize_Is_24Bytes()
    {
        // Treat the whole struct as a fixed 24-byte slot regardless of
        // arch. The native union's largest member (DECIMAL / CAUB on x64)
        // sets the upper bound, and the 24-byte assumption is what every
        // slot writer in NativeMsmqMessage allocates against.
        Assert.Equal(24, PropVarSize);
    }

    [WcfFact]
    public static void VectorElementsOffset_MatchesPointerWidth()
    {
        // Vector members store { cElems @ 8, pElems @ vectorOffset }.
        // On x64 (8-byte pointer with 8-byte alignment) the pointer
        // slot sits at offset 16; on x86 at offset 12. Anything else
        // means the union layout was miscalculated.
        Assert.Equal(IntPtr.Size == 8 ? 16 : 12, VectorOffset);
    }

    // ----- Per-setter slot content -----

    [WcfFact]
    public static void SetByte_Writes_VT_UI1_AndValue()
    {
        object msg = NewMessage();
        InvokeSetter(msg, "SetByte", (uint)4u /* PROPID_M_PRIORITY */, (byte)5);
        Assert.Equal(1, GetSlotCount(msg));
        Assert.Equal(4u, GetPropId(msg, 0));

        byte[] slot = GetSlot(msg, 0);
        Assert.Equal(24, slot.Length);
        Assert.Equal(17, slot[0]);  // VT_UI1 little-endian low byte
        Assert.Equal(0, slot[1]);   // VT_UI1 little-endian high byte
        Assert.Equal(5, slot[8]);   // value at union offset
    }

    [WcfFact]
    public static void SetUInt32_Writes_VT_UI4_AndLittleEndianValue()
    {
        object msg = NewMessage();
        InvokeSetter(msg, "SetUInt32", 14u /* PROPID_M_TIME_TO_BE_RECEIVED */, 0xCAFEBABEu);
        byte[] slot = GetSlot(msg, 0);
        Assert.Equal(19, slot[0]);   // VT_UI4
        Assert.Equal(0, slot[1]);
        Assert.Equal(0xBE, slot[8]);
        Assert.Equal(0xBA, slot[9]);
        Assert.Equal(0xFE, slot[10]);
        Assert.Equal(0xCA, slot[11]);
    }

    [WcfFact]
    public static void SetByteVector_Writes_VT_VECTOR_BIT_AndLengthAtUnionStart()
    {
        object msg = NewMessage();
        InvokeSetter(msg, "SetByteVector", 24u /* PROPID_M_EXTENSION */, new byte[] { 1, 2, 3, 4 }, 0u);
        byte[] slot = GetSlot(msg, 0);
        // VT_VECTOR (0x1000) | VT_UI1 (17) = 0x1011 = 4113
        Assert.Equal(0x11, slot[0]);
        Assert.Equal(0x10, slot[1]);
        // cElems at offset 8
        Assert.Equal(4, slot[8]);
        Assert.Equal(0, slot[9]);
        Assert.Equal(0, slot[10]);
        Assert.Equal(0, slot[11]);
        // pElems pointer occupies VectorElementsOffset..VectorElementsOffset+IntPtr.Size.
        // We can't assert its absolute value, but it must be non-zero.
        bool anyNonZero = false;
        for (int i = VectorOffset; i < VectorOffset + IntPtr.Size; i++) anyNonZero |= slot[i] != 0;
        Assert.True(anyNonZero, "pElems pointer slot must be non-zero after SetByteVector");
    }

    [WcfFact]
    public static void SetByteVector_LengthPropId_AppendsSecondSlot()
    {
        object msg = NewMessage();
        InvokeSetter(msg, "SetByteVector", 24u /* PROPID_M_EXTENSION */, new byte[] { 7, 8 }, 25u /* PROPID_M_EXTENSION_LEN */);
        Assert.Equal(2, GetSlotCount(msg));
        Assert.Equal(24u, GetPropId(msg, 0));
        Assert.Equal(25u, GetPropId(msg, 1));
        // Length slot is VT_UI4 with value 2
        byte[] len = GetSlot(msg, 1);
        Assert.Equal(19, len[0]);   // VT_UI4
        Assert.Equal(2, len[8]);
    }

    [WcfFact]
    public static void SetWideString_WritesVT_LPWSTR_AndAppendsLength()
    {
        object msg = NewMessage();
        InvokeSetter(msg, "SetWideString", 11u /* PROPID_M_LABEL */, "demo", 12u /* PROPID_M_LABEL_LEN */);
        Assert.Equal(2, GetSlotCount(msg));
        byte[] slot = GetSlot(msg, 0);
        Assert.Equal(31, slot[0]); // VT_LPWSTR
        // Length slot includes terminating null: 4 + 1 = 5
        byte[] len = GetSlot(msg, 1);
        Assert.Equal(19, len[0]);
        Assert.Equal(5, len[8]);
    }

    [WcfFact]
    public static void SetBody_AppendsBodySlot_PlusBodySize()
    {
        object msg = NewMessage();
        InvokeSetter(msg, "SetBody", new byte[] { 0xA0, 0xA1, 0xA2 }, 0, 3);
        Assert.Equal(2, GetSlotCount(msg));
        Assert.Equal(9u, GetPropId(msg, 0));  // PROPID_M_BODY
        Assert.Equal(10u, GetPropId(msg, 1)); // PROPID_M_BODY_SIZE

        byte[] body = GetSlot(msg, 0);
        Assert.Equal(0x11, body[0]); // VT_VECTOR | VT_UI1 low byte
        Assert.Equal(0x10, body[1]); // VT_VECTOR | VT_UI1 high byte
        Assert.Equal(3, body[8]);    // cElems

        byte[] size = GetSlot(msg, 1);
        Assert.Equal(19, size[0]);   // VT_UI4
        Assert.Equal(3, size[8]);
    }

    // ----- Message-id parser (used for CorrelationId) -----

    [WcfFact]
    public static void ParseMessageId_Valid_Returns20Bytes()
    {
        MethodInfo m = s_native.GetMethod("ParseMessageId", BindingFlags.Static | BindingFlags.NonPublic);
        byte[] result = (byte[])m.Invoke(null, new object[] { "11111111-2222-3333-4444-555555555555\\7" });
        Assert.Equal(20, result.Length);
        // Counter is the trailing 4 bytes, little-endian.
        Assert.Equal(7, result[16]);
        Assert.Equal(0, result[17]);
        Assert.Equal(0, result[18]);
        Assert.Equal(0, result[19]);
    }

    [WcfTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("no-backslash")]
    [InlineData("not-a-guid\\1")]
    [InlineData("11111111-2222-3333-4444-555555555555\\not-a-number")]
    [InlineData("11111111-2222-3333-4444-555555555555\\")]
    public static void ParseMessageId_Invalid_Throws(string input)
    {
        MethodInfo m = s_native.GetMethod("ParseMessageId", BindingFlags.Static | BindingFlags.NonPublic);
        TargetInvocationException ex = Assert.Throws<TargetInvocationException>(() => m.Invoke(null, new object[] { input }));
        Assert.IsType<FormatException>(ex.InnerException);
    }
}
