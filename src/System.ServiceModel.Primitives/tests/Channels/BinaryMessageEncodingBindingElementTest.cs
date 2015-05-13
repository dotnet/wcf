// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Tests.Common;
using System.Xml;
using Xunit;

public static class BinaryMessageEncodingBindingElementTest
{
    [Fact]
    public static void Default_Ctor_Initializes_Properties()
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();

        Assert.Equal<CompressionFormat>(CompressionFormat.None, bindingElement.CompressionFormat);
        Assert.Equal<int>(2048, bindingElement.MaxSessionSize);
        Assert.Equal<MessageVersion>(MessageVersion.Default, bindingElement.MessageVersion);

        Assert.True(TestHelpers.XmlDictionaryReaderQuotasAreEqual(bindingElement.ReaderQuotas, new XmlDictionaryReaderQuotas()),
            "BinaryEncodingBindingElement_DefaultCtor: Assert property 'XmlDictionaryReaderQuotas' == default value failed.");
    }

    [Theory]
    [InlineData(CompressionFormat.Deflate)]
    [InlineData(CompressionFormat.GZip)]
    public static void CompressionFormat_Property_Sets(CompressionFormat format)
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();
        bindingElement.CompressionFormat = format;
        Assert.Equal(format, bindingElement.CompressionFormat);

        // Note: invalid formats can be tested once we have a transport underneath, as it's the transport that determines 
        // whether or not the CompressionFormat is valid for it. 
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public static void MaxSessionSize_Property_Sets(int value)
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();
        bindingElement.MaxSessionSize = value;
        Assert.Equal<int>(value, bindingElement.MaxSessionSize);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public static void MaxSessionSize_Property_Set_Invalid_Value_Throws(int value)
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();
        Assert.Throws<ArgumentOutOfRangeException>(() => bindingElement.MaxSessionSize = value);
    }

    [Theory]
    [MemberData("ValidBinaryMessageEncoderMessageVersions", MemberType = typeof(TestData))]
    public static void MessageVersion_Property_Sets(MessageVersion version)
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();
        bindingElement.MessageVersion = version;
        Assert.Equal<MessageVersion>(version, bindingElement.MessageVersion);
    }

    [Theory]
    [MemberData("InvalidBinaryMessageEncoderMessageVersions", MemberType = typeof(TestData))]
    public static void MessageVersion_Property_Set_Invalid_Value_Throws(MessageVersion version)
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();
        Assert.Throws<InvalidOperationException>(() => bindingElement.MessageVersion = version);
    }

    [Fact]
    public static void MessageVersion_Property_Set_Null_Value_Throws()
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();
        Assert.Throws<ArgumentNullException>(() => bindingElement.MessageVersion = null);
    }
}
