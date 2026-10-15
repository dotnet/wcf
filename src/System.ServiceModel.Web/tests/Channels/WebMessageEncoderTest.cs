// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public static class WebMessageEncoderTest
{
    private sealed class FixedContentTypeMapper : WebContentTypeMapper
    {
        private readonly WebContentFormat _format;

        public FixedContentTypeMapper(WebContentFormat format) => _format = format;

        public override WebContentFormat GetMessageFormatForContentType(string contentType) => _format;
    }

    private sealed class NullReturningContentTypeMapper : WebContentTypeMapper
    {
        public override WebContentFormat GetMessageFormatForContentType(string contentType) => WebContentFormat.Default;
    }

    private static MessageEncoder CreateEncoder(WebContentTypeMapper mapper = null, Encoding writeEncoding = null)
    {
        WebMessageEncodingBindingElement element = writeEncoding == null
            ? new WebMessageEncodingBindingElement()
            : new WebMessageEncodingBindingElement(writeEncoding);
        element.ContentTypeMapper = mapper;
        return element.CreateMessageEncoderFactory().Encoder;
    }

    private static Message ReadFrom(MessageEncoder encoder, string payload, string contentType)
    {
        return ReadFrom(encoder, Encoding.UTF8.GetBytes(payload), contentType);
    }

    private static Message ReadFrom(MessageEncoder encoder, byte[] payload, string contentType)
    {
        return encoder.ReadMessage(new MemoryStream(payload), int.MaxValue, contentType);
    }

    private static byte[] WriteToArray(MessageEncoder encoder, Message message)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            encoder.WriteMessage(message, stream);
            return stream.ToArray();
        }
    }

    private static byte[] WriteToBuffer(MessageEncoder encoder, Message message)
    {
        BufferManager bufferManager = BufferManager.CreateBufferManager(64 * 1024, 64 * 1024);
        ArraySegment<byte> written = default;

        try
        {
            written = encoder.WriteMessage(message, int.MaxValue, bufferManager, 0);
            byte[] result = new byte[written.Count];
            Array.Copy(written.Array, written.Offset, result, 0, written.Count);
            return result;
        }
        finally
        {
            if (written.Array != null)
            {
                bufferManager.ReturnBuffer(written.Array);
            }

            bufferManager.Clear();
        }
    }

    private static WebContentFormat GetFormat(Message message)
    {
        Assert.True(message.Properties.TryGetValue(WebBodyFormatMessageProperty.Name, out object value));
        return ((WebBodyFormatMessageProperty)value).Format;
    }

    [WcfFact]
    public static void Encoder_Factory_Is_Internal()
    {
        Type encoderFactoryType = typeof(WebMessageEncodingBindingElement).Assembly.GetType("System.ServiceModel.Channels.WebMessageEncoderFactory");

        Assert.NotNull(encoderFactoryType);
        Assert.False(encoderFactoryType.IsPublic);
    }

    [WcfFact]
    public static void Encoder_Advertises_Xml_Media_Type_And_Utf8_Charset()
    {
        MessageEncoder encoder = CreateEncoder();

        Assert.Equal("application/xml", encoder.MediaType);
        Assert.Equal("application/xml; charset=utf-8", encoder.ContentType);
        Assert.Equal(MessageVersion.None, encoder.MessageVersion);
    }

    [WcfFact]
    public static void Encoder_ContentType_Reflects_WriteEncoding()
    {
        MessageEncoder encoder = CreateEncoder(writeEncoding: Encoding.Unicode);

        Assert.Equal("application/xml; charset=utf-16LE", encoder.ContentType);
    }

    [WcfFact]
    public static void IsContentTypeSupported_Accepts_Xml_Json_And_Raw()
    {
        MessageEncoder encoder = CreateEncoder();

        Assert.True(encoder.IsContentTypeSupported("application/xml"));
        Assert.True(encoder.IsContentTypeSupported("text/xml"));
        Assert.True(encoder.IsContentTypeSupported("application/xml; charset=utf-8"));
        Assert.True(encoder.IsContentTypeSupported("application/json"));
        Assert.True(encoder.IsContentTypeSupported("application/json; charset=utf-8"));
        Assert.True(encoder.IsContentTypeSupported("application/octet-stream"));
        Assert.True(encoder.IsContentTypeSupported("*/*"));
    }

    [WcfFact]
    public static void IsContentTypeSupported_Accepts_Arbitrary_Types_Via_Raw_Encoder()
    {
        MessageEncoder encoder = CreateEncoder();

        // The raw (byte stream) encoder accepts anything, so an unknown media type is still readable.
        Assert.True(encoder.IsContentTypeSupported("application/pdf"));
    }

    [WcfFact]
    public static void IsContentTypeSupported_Null_Throws()
    {
        MessageEncoder encoder = CreateEncoder();

        Assert.Throws<ArgumentNullException>(() => encoder.IsContentTypeSupported(null));
    }

    [WcfFact]
    public static void IsContentTypeSupported_Honors_ContentTypeMapper()
    {
        MessageEncoder encoder = CreateEncoder(new FixedContentTypeMapper(WebContentFormat.Json));

        Assert.True(encoder.IsContentTypeSupported("application/whatever"));
    }

    [WcfFact]
    public static void ReadMessage_Xml_Produces_Xml_Format_And_Readable_Body()
    {
        MessageEncoder encoder = CreateEncoder();

        using (Message message = ReadFrom(encoder, "<Value>hello</Value>", "application/xml"))
        {
            Assert.Equal(WebContentFormat.Xml, GetFormat(message));
            Assert.Equal(MessageVersion.None, message.Version);
            Assert.Equal("hello", message.GetReaderAtBodyContents().ReadElementContentAsString());
        }
    }

    [WcfFact]
    public static void WriteMessage_Xml_RoundTrips()
    {
        MessageEncoder encoder = CreateEncoder();
        byte[] written;

        using (Message message = ReadFrom(encoder, "<Value>hello</Value>", "application/xml"))
        {
            written = WriteToArray(encoder, message);
        }

        Assert.Contains("hello", Encoding.UTF8.GetString(written));

        using (Message roundTripped = ReadFrom(encoder, written, "application/xml"))
        {
            Assert.Equal("hello", roundTripped.GetReaderAtBodyContents().ReadElementContentAsString());
        }
    }

    [WcfFact]
    public static void ReadMessage_Xml_Honors_Charset()
    {
        MessageEncoder encoder = CreateEncoder();
        // Non-UTF-8 XML must carry a declaration so the reader can validate the declared encoding.
        byte[] payload = Encoding.Unicode.GetBytes("<?xml version=\"1.0\" encoding=\"utf-16\"?><Value>caf\u00e9</Value>");

        using (Message message = ReadFrom(encoder, payload, "application/xml; charset=utf-16LE"))
        {
            Assert.Equal("caf\u00e9", message.GetReaderAtBodyContents().ReadElementContentAsString());
        }
    }

    [WcfFact]
    public static void ReadMessage_Xml_With_Unsupported_Charset_Falls_Back_To_Raw()
    {
        MessageEncoder encoder = CreateEncoder();
        byte[] payload = Encoding.UTF8.GetBytes("<Value>hello</Value>");

        // utf-32 is not a supported text charset, so the content type no longer maps to the
        // text encoder and the raw encoder takes over rather than the read failing outright.
        using (Message message = ReadFrom(encoder, payload, "application/xml; charset=utf-32"))
        {
            Assert.Equal(WebContentFormat.Raw, GetFormat(message));
            Assert.Equal(payload, WriteToArray(encoder, message));
        }
    }

    [WcfFact]
    public static void ReadMessage_Json_Produces_Json_Format()
    {
        MessageEncoder encoder = CreateEncoder();

        using (Message message = ReadFrom(encoder, "\"hello\"", "application/json"))
        {
            Assert.Equal(WebContentFormat.Json, GetFormat(message));
            Assert.Equal(MessageVersion.None, message.Version);
        }
    }

    [WcfFact]
    public static void ReadMessage_Json_Body_Is_Readable_As_Json_Infoset()
    {
        MessageEncoder encoder = CreateEncoder();

        using (Message message = ReadFrom(encoder, "\"hello\"", "application/json"))
        {
            XmlDictionaryReader reader = message.GetReaderAtBodyContents();

            // The JSON reader surfaces JSON as an XML infoset rooted at <root type="string">.
            Assert.Equal("root", reader.LocalName);
            Assert.Equal("string", reader.GetAttribute("type"));
            Assert.Equal("hello", reader.ReadElementContentAsString());
        }
    }

    [WcfFact]
    public static void Json_RoundTrips_Scalar_Unchanged()
    {
        MessageEncoder encoder = CreateEncoder();
        byte[] written;

        using (Message message = ReadFrom(encoder, "\"hello\"", "application/json"))
        {
            written = WriteToArray(encoder, message);
        }

        Assert.Equal("\"hello\"", Encoding.UTF8.GetString(written));
    }

    [WcfFact]
    public static void Json_RoundTrips_Object_Unchanged()
    {
        MessageEncoder encoder = CreateEncoder();
        const string Payload = "{\"name\":\"contoso\",\"count\":3}";
        byte[] written;

        using (Message message = ReadFrom(encoder, Payload, "application/json"))
        {
            written = WriteToArray(encoder, message);
        }

        Assert.Equal(Payload, Encoding.UTF8.GetString(written));
    }

    [WcfFact]
    public static void Json_RoundTrips_Array_Unchanged()
    {
        MessageEncoder encoder = CreateEncoder();
        const string Payload = "[1,2,3]";
        byte[] written;

        using (Message message = ReadFrom(encoder, Payload, "application/json"))
        {
            written = WriteToArray(encoder, message);
        }

        Assert.Equal(Payload, Encoding.UTF8.GetString(written));
    }

    [WcfFact]
    public static void ReadMessage_OctetStream_Produces_Raw_Format()
    {
        MessageEncoder encoder = CreateEncoder();
        byte[] payload = { 1, 2, 3, 4, 5 };

        using (Message message = ReadFrom(encoder, payload, "application/octet-stream"))
        {
            Assert.Equal(WebContentFormat.Raw, GetFormat(message));
        }
    }

    [WcfFact]
    public static void ReadMessage_RawBodyReaderStartsOnBinaryElement()
    {
        MessageEncoder encoder = CreateEncoder();

        using (Message message = ReadFrom(encoder, new byte[] { 1, 2, 3 }, "application/octet-stream"))
        {
            XmlDictionaryReader reader = message.GetReaderAtBodyContents();

            Assert.Equal(XmlNodeType.Element, reader.NodeType);
            Assert.Equal("Binary", reader.LocalName);
        }
    }

    [WcfFact]
    public static void Raw_RoundTrips_Bytes_Unchanged()
    {
        MessageEncoder encoder = CreateEncoder();
        byte[] payload = { 0, 1, 2, 250, 251, 252, 253, 254, 255 };
        byte[] written;

        using (Message message = ReadFrom(encoder, payload, "application/octet-stream"))
        {
            written = WriteToArray(encoder, message);
        }

        Assert.Equal(payload, written);
    }

    [WcfFact]
    public static void Raw_RoundTrips_Empty_Payload()
    {
        MessageEncoder encoder = CreateEncoder();
        byte[] written;

        using (Message message = ReadFrom(encoder, Array.Empty<byte>(), "application/octet-stream"))
        {
            written = WriteToArray(encoder, message);
        }

        Assert.Empty(written);
    }

    [WcfFact]
    public static void Unknown_Content_Type_Falls_Back_To_Raw()
    {
        MessageEncoder encoder = CreateEncoder();
        byte[] payload = { 9, 8, 7 };

        using (Message message = ReadFrom(encoder, payload, "application/vnd.contoso.thing"))
        {
            Assert.Equal(WebContentFormat.Raw, GetFormat(message));
            Assert.Equal(payload, WriteToArray(encoder, message));
        }
    }

    [WcfFact]
    public static void ContentTypeMapper_Overrides_Format_Selection()
    {
        // The payload is XML but the mapper insists everything is raw.
        MessageEncoder encoder = CreateEncoder(new FixedContentTypeMapper(WebContentFormat.Raw));
        byte[] payload = Encoding.UTF8.GetBytes("<Value>hello</Value>");

        using (Message message = ReadFrom(encoder, payload, "application/xml"))
        {
            Assert.Equal(WebContentFormat.Raw, GetFormat(message));
            Assert.Equal(payload, WriteToArray(encoder, message));
        }
    }

    [WcfFact]
    public static void ContentTypeMapper_Returning_Default_Falls_Back_To_Builtin_Detection()
    {
        MessageEncoder encoder = CreateEncoder(new NullReturningContentTypeMapper());

        using (Message message = ReadFrom(encoder, "<Value>hello</Value>", "application/xml"))
        {
            Assert.Equal(WebContentFormat.Xml, GetFormat(message));
        }
    }

    [WcfFact]
    public static void ReadMessage_From_Buffer_Matches_Stream_Path()
    {
        MessageEncoder encoder = CreateEncoder();
        byte[] payload = Encoding.UTF8.GetBytes("<Value>hello</Value>");
        BufferManager bufferManager = BufferManager.CreateBufferManager(64 * 1024, 64 * 1024);

        byte[] buffer = bufferManager.TakeBuffer(payload.Length);
        Array.Copy(payload, buffer, payload.Length);

        using (Message message = encoder.ReadMessage(new ArraySegment<byte>(buffer, 0, payload.Length), bufferManager, "application/xml"))
        {
            Assert.Equal(WebContentFormat.Xml, GetFormat(message));
            Assert.Equal("hello", message.GetReaderAtBodyContents().ReadElementContentAsString());
        }
    }

    [WcfFact]
    public static void ReadMessage_Json_From_Buffer_Matches_Stream_Path()
    {
        MessageEncoder encoder = CreateEncoder();
        byte[] payload = Encoding.UTF8.GetBytes("\"hello\"");
        BufferManager bufferManager = BufferManager.CreateBufferManager(64 * 1024, 64 * 1024);
        byte[] buffer = bufferManager.TakeBuffer(payload.Length + 4);
        Array.Copy(payload, 0, buffer, 2, payload.Length);

        using (Message message = encoder.ReadMessage(
            new ArraySegment<byte>(buffer, 2, payload.Length),
            bufferManager,
            "application/json"))
        {
            Assert.Equal(WebContentFormat.Json, GetFormat(message));
            XmlDictionaryReader reader = message.GetReaderAtBodyContents();
            Assert.Equal("hello", reader.ReadElementContentAsString());
        }
    }

    [WcfFact]
    public static void ReadMessage_Raw_From_Buffer_Matches_Stream_Path()
    {
        MessageEncoder encoder = CreateEncoder();
        byte[] payload = { 0, 1, 2, 250, 251, 252, 253, 254, 255 };
        BufferManager bufferManager = BufferManager.CreateBufferManager(64 * 1024, 64 * 1024);
        byte[] buffer = bufferManager.TakeBuffer(payload.Length + 4);
        Array.Copy(payload, 0, buffer, 2, payload.Length);

        using (Message message = encoder.ReadMessage(
            new ArraySegment<byte>(buffer, 2, payload.Length),
            bufferManager,
            "application/octet-stream"))
        {
            Assert.Equal(WebContentFormat.Raw, GetFormat(message));
            Assert.Equal(payload, WriteToArray(encoder, message));
        }
    }

    [WcfFact]
    public static void WriteMessage_To_Buffer_Matches_Stream_Path()
    {
        MessageEncoder encoder = CreateEncoder();
        BufferManager bufferManager = BufferManager.CreateBufferManager(64 * 1024, 64 * 1024);
        byte[] payload = { 1, 2, 3 };

        using (Message message = ReadFrom(encoder, payload, "application/octet-stream"))
        {
            ArraySegment<byte> written = encoder.WriteMessage(message, int.MaxValue, bufferManager, 0);

            byte[] actual = new byte[written.Count];
            Array.Copy(written.Array, written.Offset, actual, 0, written.Count);
            Assert.Equal(payload, actual);

            bufferManager.ReturnBuffer(written.Array);
        }
    }

    [WcfFact]
    public static void WriteMessage_Xml_To_Buffer_Matches_Stream_Path()
    {
        MessageEncoder encoder = CreateEncoder();
        const string Payload = "<Value>hello</Value>";
        byte[] streamed;
        byte[] buffered;

        using (Message message = ReadFrom(encoder, Payload, "application/xml"))
        {
            streamed = WriteToArray(encoder, message);
        }

        using (Message message = ReadFrom(encoder, Payload, "application/xml"))
        {
            buffered = WriteToBuffer(encoder, message);
        }

        Assert.Equal(streamed, buffered);
    }

    [WcfFact]
    public static void WriteMessage_Json_To_Buffer_Matches_Stream_Path()
    {
        MessageEncoder encoder = CreateEncoder();
        const string Payload = "{\"name\":\"contoso\",\"count\":3}";
        byte[] streamed;
        byte[] buffered;

        using (Message message = ReadFrom(encoder, Payload, "application/json"))
        {
            streamed = WriteToArray(encoder, message);
        }

        using (Message message = ReadFrom(encoder, Payload, "application/json"))
        {
            buffered = WriteToBuffer(encoder, message);
        }

        Assert.Equal(streamed, buffered);
    }

}
