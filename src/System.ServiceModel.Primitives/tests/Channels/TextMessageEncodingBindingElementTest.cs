// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Tests.Common;
using System.Text;
using Xunit;

public static class TextMessageEncodingBindingElementTest
{
    [Fact]
    // Create a TextMessageEncodingBindingElement and check it's default encoding is as expected.
    public static void Default_Ctor_Initializes_Properties()
    {
        TextMessageEncodingBindingElement element = new TextMessageEncodingBindingElement();
        Assert.Equal<Encoding>(Encoding.UTF8, element.WriteEncoding);
    }

    [Theory]
    [MemberData("ValidEncodings", MemberType = typeof(TestData))]
    public static void WriteEncoding_Property_Sets(Encoding encoding)
    {
        TextMessageEncodingBindingElement element = new TextMessageEncodingBindingElement();
        element.WriteEncoding = encoding;
        Assert.Equal<Encoding>(encoding, element.WriteEncoding);
    }

    [Theory]
    [MemberData("InvalidEncodings", MemberType = typeof(TestData))]
    public static void WriteEncoding_Property_Set_Throws_For_Invalid_Encodings(Encoding encoding)
    {
        TextMessageEncodingBindingElement element = new TextMessageEncodingBindingElement();
        Assert.Throws<ArgumentException>(() => element.WriteEncoding = encoding);
    }

    [Theory]
    [MemberData("ValidTextMessageEncoderMessageVersions", MemberType = typeof(TestData))]
    public static void MessageVersion_Property_Sets(MessageVersion messageVersion)
    {
        TextMessageEncodingBindingElement element = new TextMessageEncodingBindingElement();
        element.MessageVersion = messageVersion;
        Assert.Equal<MessageVersion>(messageVersion, element.MessageVersion);
    }

    [Fact]
    public static void MessageVersion_Property_Set_Null_Throws()
    {
        TextMessageEncodingBindingElement element = new TextMessageEncodingBindingElement();
        Assert.Throws<ArgumentNullException>(() => element.MessageVersion = null);
    }
}
