// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Tests.Common;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public static class BinaryMessageEncodingBindingElementTest
{
    [WcfFact]
    public static void Default_Ctor_Initializes_Properties()
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();

        Assert.Equal<CompressionFormat>(CompressionFormat.None, bindingElement.CompressionFormat);
        Assert.Equal<int>(2048, bindingElement.MaxSessionSize);
        Assert.Equal<MessageVersion>(MessageVersion.Default, bindingElement.MessageVersion);

        Assert.True(TestHelpers.XmlDictionaryReaderQuotasAreEqual(bindingElement.ReaderQuotas, new XmlDictionaryReaderQuotas()),
            "BinaryEncodingBindingElement_DefaultCtor: Assert property 'XmlDictionaryReaderQuotas' == default value failed.");
    }

    [WcfTheory]
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

    [WcfTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public static void MaxSessionSize_Property_Sets(int value)
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();
        bindingElement.MaxSessionSize = value;
        Assert.Equal<int>(value, bindingElement.MaxSessionSize);
    }

    [WcfTheory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public static void MaxSessionSize_Property_Set_Invalid_Value_Throws(int value)
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();
        Assert.Throws<ArgumentOutOfRangeException>(() => bindingElement.MaxSessionSize = value);
    }

    [WcfTheory]
    [MemberData(nameof(TestData.ValidBinaryMessageEncoderMessageVersions), MemberType = typeof(TestData))]
    public static void MessageVersion_Property_Sets(MessageVersion version)
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();
        bindingElement.MessageVersion = version;
        Assert.Equal<MessageVersion>(version, bindingElement.MessageVersion);
    }

    [WcfTheory]
    [MemberData(nameof(TestData.InvalidBinaryMessageEncoderMessageVersions), MemberType = typeof(TestData))]
    public static void MessageVersion_Property_Set_Invalid_Value_Throws(MessageVersion version)
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();
        Assert.Throws<InvalidOperationException>(() => bindingElement.MessageVersion = version);
    }

    [WcfFact]
    public static void MessageVersion_Property_Set_Null_Value_Throws()
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();
        Assert.Throws<ArgumentNullException>(() => bindingElement.MessageVersion = null);
    }

    [WcfTheory]
    [InlineData(64)]
    public static void MaxReadPoolSize_DefaultValueIsSameAsNetFrameWork(int value)
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();
        Assert.Equal<int>(value, bindingElement.MaxReadPoolSize);
    }

    [WcfTheory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public static void MaxReadPoolSize_Property_Sets(int value)
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();
        bindingElement.MaxReadPoolSize = value;
        Assert.Equal<int>(value, bindingElement.MaxReadPoolSize);
    }

    [WcfTheory]
    [InlineData(0)]
    [InlineData(int.MinValue)]
    public static void MaxReadPoolSize_Property_Set_Invalid_Value_Throws(int value)
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();
        Assert.Throws<ArgumentOutOfRangeException>(() => bindingElement.MaxReadPoolSize = value);
    }

    [WcfTheory]
    [InlineData(16)]
    public static void MaxWritePoolSize_DefaultValueIsSameAsNetFrameWork(int value)
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();
        Assert.Equal<int>(value, bindingElement.MaxWritePoolSize);
    }

    [WcfTheory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public static void MaxWritePoolSize_Property_Sets(int value)
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();
        bindingElement.MaxWritePoolSize = value;
        Assert.Equal<int>(value, bindingElement.MaxWritePoolSize);
    }

    [WcfTheory]
    [InlineData(0)]
    [InlineData(int.MinValue)]
    public static void MaxWritePoolSize_Property_Set_Invalid_Value_Throws(int value)
    {
        BinaryMessageEncodingBindingElement bindingElement = new BinaryMessageEncodingBindingElement();
        Assert.Throws<ArgumentOutOfRangeException>(() => bindingElement.MaxWritePoolSize = value);
    }
}
