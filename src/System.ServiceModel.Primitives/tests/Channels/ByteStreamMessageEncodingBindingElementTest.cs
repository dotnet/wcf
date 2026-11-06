// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public static class ByteStreamMessageEncodingBindingElementTest
{
    [WcfFact]
    public static void Defaults_CreateMessageVersionNoneEncoder()
    {
        ByteStreamMessageEncodingBindingElement element = new ByteStreamMessageEncodingBindingElement();
        MessageEncoderFactory factory = element.CreateMessageEncoderFactory();

        Assert.Equal(MessageVersion.None, element.MessageVersion);
        Assert.NotNull(element.ReaderQuotas);
        Assert.Equal(MessageVersion.None, factory.MessageVersion);
        Assert.Equal(MessageVersion.None, factory.Encoder.MessageVersion);
    }

    [WcfFact]
    public static void MessageVersion_OnlyAcceptsNone()
    {
        ByteStreamMessageEncodingBindingElement element = new ByteStreamMessageEncodingBindingElement();

        element.MessageVersion = MessageVersion.None;

        Assert.Throws<ArgumentException>(() => element.MessageVersion = MessageVersion.Soap11);
    }

    [WcfFact]
    public static void Constructor_CopiesReaderQuotas()
    {
        XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas
        {
            MaxDepth = 7,
            MaxArrayLength = 128
        };

        ByteStreamMessageEncodingBindingElement element = new ByteStreamMessageEncodingBindingElement(quotas);
        quotas.MaxDepth = 8;
        quotas.MaxArrayLength = 256;

        Assert.Equal(7, element.ReaderQuotas.MaxDepth);
        Assert.Equal(128, element.ReaderQuotas.MaxArrayLength);
    }

    [WcfFact]
    public static void ReaderQuotas_SetterCopiesValueAndRejectsNull()
    {
        ByteStreamMessageEncodingBindingElement element = new ByteStreamMessageEncodingBindingElement();
        XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas
        {
            MaxDepth = 7,
            MaxArrayLength = 128
        };

        element.ReaderQuotas = quotas;
        quotas.MaxDepth = 8;
        quotas.MaxArrayLength = 256;

        Assert.Equal(7, element.ReaderQuotas.MaxDepth);
        Assert.Equal(128, element.ReaderQuotas.MaxArrayLength);
        Assert.Throws<ArgumentNullException>(() => element.ReaderQuotas = null);
    }

    [WcfFact]
    public static void Clone_CopiesReaderQuotasIndependently()
    {
        ByteStreamMessageEncodingBindingElement element = new ByteStreamMessageEncodingBindingElement();
        element.ReaderQuotas.MaxDepth = 7;
        element.ReaderQuotas.MaxArrayLength = 128;

        ByteStreamMessageEncodingBindingElement clone = (ByteStreamMessageEncodingBindingElement)element.Clone();
        element.ReaderQuotas.MaxDepth = 8;
        element.ReaderQuotas.MaxArrayLength = 256;

        Assert.NotSame(element.ReaderQuotas, clone.ReaderQuotas);
        Assert.Equal(7, clone.ReaderQuotas.MaxDepth);
        Assert.Equal(128, clone.ReaderQuotas.MaxArrayLength);
    }

    [WcfFact]
    public static void Clone_PreservesBodyReaderPositioningMode()
    {
        ByteStreamMessageEncodingBindingElement element = new ByteStreamMessageEncodingBindingElement();
        MethodInfo enablePositioning = typeof(ByteStreamMessageEncodingBindingElement).GetMethod(
            "EnableBodyReaderMoveToContent",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(enablePositioning);
        enablePositioning.Invoke(element, null);

        ByteStreamMessageEncodingBindingElement clone = (ByteStreamMessageEncodingBindingElement)element.Clone();
        MessageEncoder encoder = clone.CreateMessageEncoderFactory().Encoder;
        using (MemoryStream stream = new MemoryStream(new byte[] { 0x10 }, false))
        using (Message message = encoder.ReadMessage(stream, int.MaxValue, null))
        {
            XmlDictionaryReader reader = message.GetReaderAtBodyContents();

            Assert.Equal(XmlNodeType.Element, reader.NodeType);
            Assert.Equal("Binary", reader.LocalName);
        }
    }
}
