// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using System.ServiceModel;
using System.ServiceModel.MsmqIntegration;
using System.Text;
using System.Xml.Serialization;
using Infrastructure.Common;
using Xunit;

// MsmqIntegrationBinding carries raw MSMQ payloads rather than SOAP envelopes,
// so the body is produced by MsmqIntegrationSerializer according to
// SerializationFormat. Before this was wired up, SerializationFormat was stored
// and validated but never read, and the channel factory fell back to a SOAP 1.2
// binary encoder that threw on every send.
[SupportedOSPlatform("windows")]
public static class MsmqIntegrationSerializerTest
{
    private static readonly Type s_serializer =
        typeof(NetMsmqBinding).Assembly.GetType(
            "System.ServiceModel.MsmqIntegration.MsmqIntegrationSerializer", throwOnError: true);

    private static byte[] Serialize(MsmqMessageSerializationFormat format, object body)
    {
        object serializer = Activator.CreateInstance(
            s_serializer,
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: new object[] { format },
            culture: null);
        MethodInfo serialize = s_serializer.GetMethod(
            "Serialize", BindingFlags.Instance | BindingFlags.NonPublic);
        try
        {
            return (byte[])serialize.Invoke(serializer, new object[] { new MsmqIntegrationMessageProperty { Body = body } });
        }
        catch (TargetInvocationException ex)
        {
            throw ex.InnerException;
        }
    }

    [WcfFact]
    public static void ByteArrayFormat_PassesBodyThroughUnchanged()
    {
        byte[] payload = new byte[] { 9, 8, 7, 6 };
        Assert.Equal(payload, Serialize(MsmqMessageSerializationFormat.ByteArray, payload));
    }

    [WcfFact]
    public static void ByteArrayFormat_RejectsNonByteArrayBody()
    {
        Assert.Throws<SerializationException>(
            () => Serialize(MsmqMessageSerializationFormat.ByteArray, "not a byte array"));
    }

    [WcfFact]
    public static void StreamFormat_ReadsEntireStream()
    {
        byte[] payload = Encoding.UTF8.GetBytes("hello queue");
        Assert.Equal(payload, Serialize(MsmqMessageSerializationFormat.Stream, new MemoryStream(payload)));
    }

    [WcfFact]
    public static void StreamFormat_RejectsNonStreamBody()
    {
        Assert.Throws<SerializationException>(
            () => Serialize(MsmqMessageSerializationFormat.Stream, new byte[] { 1 }));
    }

    [WcfFact]
    public static void XmlFormat_ProducesXmlSerializerOutput()
    {
        byte[] actual = Serialize(MsmqMessageSerializationFormat.Xml, "payload");

        using var expectedStream = new MemoryStream();
        new XmlSerializer(typeof(string)).Serialize(expectedStream, "payload");
        Assert.Equal(expectedStream.ToArray(), actual);
    }

    // BinaryFormatter and the ActiveX type serializer have no modern .NET
    // equivalent. Failing loudly beats emitting a payload the receiver cannot
    // interpret.
    [WcfTheory]
    [InlineData(MsmqMessageSerializationFormat.Binary)]
    [InlineData(MsmqMessageSerializationFormat.ActiveX)]
    public static void UnsupportedFormats_ThrowPlatformNotSupported(MsmqMessageSerializationFormat format)
    {
        Assert.Throws<PlatformNotSupportedException>(() => Serialize(format, "payload"));
    }

    [WcfFact]
    public static void NullBody_SerializesToEmptyPayload()
    {
        Assert.Empty(Serialize(MsmqMessageSerializationFormat.Xml, null));
    }
}
