// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public static class WebMessageEncodingBindingElementTest
{
    private sealed class FixedContentTypeMapper : WebContentTypeMapper
    {
        private readonly WebContentFormat _format;

        public FixedContentTypeMapper(WebContentFormat format) => _format = format;

        public override WebContentFormat GetMessageFormatForContentType(string contentType) => _format;
    }

    [WcfFact]
    public static void Default_Ctor_Uses_Utf8()
    {
        WebMessageEncodingBindingElement element = new WebMessageEncodingBindingElement();

        Assert.Equal(Encoding.UTF8.WebName, element.WriteEncoding.WebName);
    }

    [WcfFact]
    public static void Default_Ctor_Sets_Expected_Defaults()
    {
        WebMessageEncodingBindingElement element = new WebMessageEncodingBindingElement();

        Assert.Null(element.ContentTypeMapper);
        Assert.False(element.CrossDomainScriptAccessEnabled);
        Assert.Equal(MessageVersion.None, element.MessageVersion);
        Assert.NotNull(element.ReaderQuotas);
        Assert.True(element.MaxReadPoolSize > 0);
        Assert.True(element.MaxWritePoolSize > 0);
    }

    [WcfFact]
    public static void Ctor_Null_Encoding_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new WebMessageEncodingBindingElement(null));
    }

    [WcfFact]
    public static void Ctor_Accepts_Supported_Encodings()
    {
        Assert.Equal(Encoding.Unicode.WebName, new WebMessageEncodingBindingElement(Encoding.Unicode).WriteEncoding.WebName);
        Assert.Equal(Encoding.BigEndianUnicode.WebName, new WebMessageEncodingBindingElement(Encoding.BigEndianUnicode).WriteEncoding.WebName);
    }

    [WcfFact]
    public static void Ctor_Rejects_Unsupported_Encoding()
    {
        Assert.ThrowsAny<ArgumentException>(() => new WebMessageEncodingBindingElement(Encoding.UTF32));
    }

    [WcfFact]
    public static void WriteEncoding_Null_Throws()
    {
        WebMessageEncodingBindingElement element = new WebMessageEncodingBindingElement();

        Assert.Throws<ArgumentNullException>(() => element.WriteEncoding = null);
    }

    [WcfFact]
    public static void MessageVersion_Getter_Is_Always_None()
    {
        WebMessageEncodingBindingElement element = new WebMessageEncodingBindingElement
        {
            MessageVersion = MessageVersion.None
        };

        Assert.Equal(MessageVersion.None, element.MessageVersion);
    }

    [WcfFact]
    public static void MessageVersion_Rejects_Null_And_Soap_Versions()
    {
        WebMessageEncodingBindingElement element = new WebMessageEncodingBindingElement();

        Assert.Throws<ArgumentNullException>(() => element.MessageVersion = null);
        Assert.Throws<ArgumentException>(() => element.MessageVersion = MessageVersion.Soap11);
        Assert.Throws<ArgumentException>(() => element.MessageVersion = MessageVersion.Soap12WSAddressing10);
    }

    [WcfFact]
    public static void Pool_Sizes_Reject_Non_Positive_Values()
    {
        WebMessageEncodingBindingElement element = new WebMessageEncodingBindingElement();

        Assert.Throws<ArgumentOutOfRangeException>(() => element.MaxReadPoolSize = 0);
        Assert.Throws<ArgumentOutOfRangeException>(() => element.MaxReadPoolSize = -1);
        Assert.Throws<ArgumentOutOfRangeException>(() => element.MaxWritePoolSize = 0);
        Assert.Throws<ArgumentOutOfRangeException>(() => element.MaxWritePoolSize = -1);
    }

    [WcfFact]
    public static void Properties_RoundTrip()
    {
        WebContentTypeMapper mapper = new FixedContentTypeMapper(WebContentFormat.Json);
        WebMessageEncodingBindingElement element = new WebMessageEncodingBindingElement
        {
            ContentTypeMapper = mapper,
            CrossDomainScriptAccessEnabled = true,
            MaxReadPoolSize = 7,
            MaxWritePoolSize = 11,
            WriteEncoding = Encoding.Unicode
        };

        Assert.Same(mapper, element.ContentTypeMapper);
        Assert.True(element.CrossDomainScriptAccessEnabled);
        Assert.Equal(7, element.MaxReadPoolSize);
        Assert.Equal(11, element.MaxWritePoolSize);
        Assert.Equal(Encoding.Unicode.WebName, element.WriteEncoding.WebName);
    }

    [WcfFact]
    public static void Clone_Copies_All_Settings()
    {
        WebContentTypeMapper mapper = new FixedContentTypeMapper(WebContentFormat.Raw);
        WebMessageEncodingBindingElement element = new WebMessageEncodingBindingElement
        {
            ContentTypeMapper = mapper,
            CrossDomainScriptAccessEnabled = true,
            MaxReadPoolSize = 7,
            MaxWritePoolSize = 11,
            WriteEncoding = Encoding.Unicode
        };
        element.ReaderQuotas.MaxStringContentLength = 12345;

        WebMessageEncodingBindingElement clone = Assert.IsType<WebMessageEncodingBindingElement>(element.Clone());

        Assert.NotSame(element, clone);
        Assert.Same(mapper, clone.ContentTypeMapper);
        Assert.True(clone.CrossDomainScriptAccessEnabled);
        Assert.Equal(7, clone.MaxReadPoolSize);
        Assert.Equal(11, clone.MaxWritePoolSize);
        Assert.Equal(Encoding.Unicode.WebName, clone.WriteEncoding.WebName);
        Assert.Equal(12345, clone.ReaderQuotas.MaxStringContentLength);
    }

    [WcfFact]
    public static void Clone_Does_Not_Alias_ReaderQuotas()
    {
        WebMessageEncodingBindingElement element = new WebMessageEncodingBindingElement();
        element.ReaderQuotas.MaxStringContentLength = 100;

        WebMessageEncodingBindingElement clone = (WebMessageEncodingBindingElement)element.Clone();
        clone.ReaderQuotas.MaxStringContentLength = 200;

        Assert.NotSame(element.ReaderQuotas, clone.ReaderQuotas);
        Assert.Equal(100, element.ReaderQuotas.MaxStringContentLength);
    }

    [WcfFact]
    public static void GetProperty_Returns_ReaderQuotas()
    {
        WebMessageEncodingBindingElement element = new WebMessageEncodingBindingElement();
        BindingContext context = new BindingContext(new CustomBinding(element), new BindingParameterCollection());

        Assert.Same(element.ReaderQuotas, element.GetProperty<XmlDictionaryReaderQuotas>(context));
    }

    [WcfFact]
    public static void GetProperty_Null_Context_Throws()
    {
        WebMessageEncodingBindingElement element = new WebMessageEncodingBindingElement();

        Assert.Throws<ArgumentNullException>(() => element.GetProperty<XmlDictionaryReaderQuotas>(null));
    }

    [WcfFact]
    public static void CreateMessageEncoderFactory_Produces_MessageVersion_None_Encoder()
    {
        WebMessageEncodingBindingElement element = new WebMessageEncodingBindingElement();

        MessageEncoderFactory factory = element.CreateMessageEncoderFactory();

        Assert.NotNull(factory);
        Assert.Equal(MessageVersion.None, factory.MessageVersion);
        Assert.Equal(MessageVersion.None, factory.Encoder.MessageVersion);
    }

    [WcfFact]
    public static void CanBuildChannelFactory_Supports_Request_Channels()
    {
        WebMessageEncodingBindingElement element = new WebMessageEncodingBindingElement();
        // The context must describe only the elements *below* the encoder, otherwise the encoder
        // would recurse into itself when it delegates to the rest of the stack.
        CustomBinding binding = new CustomBinding(new HttpTransportBindingElement());

        BindingContext context = new BindingContext(binding, new BindingParameterCollection());

        Assert.True(element.CanBuildChannelFactory<IRequestChannel>(context));
    }
}
