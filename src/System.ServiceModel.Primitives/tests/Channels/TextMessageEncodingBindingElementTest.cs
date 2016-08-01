// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Tests.Common;
using System.Text;
using Infrastructure.Common;
using Xunit;

public static class TextMessageEncodingBindingElementTest
{
#if FULLXUNIT_NOTSUPPORTED
    [Fact]
#endif
    [WcfFact]
    // Create a TextMessageEncodingBindingElement and check it's default encoding is as expected.
    public static void Default_Ctor_Initializes_Properties()
    {
        TextMessageEncodingBindingElement element = new TextMessageEncodingBindingElement();
        Assert.Equal<Encoding>(Encoding.UTF8, element.WriteEncoding);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [MemberData("ValidEncodings", MemberType = typeof(TestData))]
    [ActiveIssue(1450)]
    public static void WriteEncoding_Property_Sets(Encoding encoding)
    {
        TextMessageEncodingBindingElement element = new TextMessageEncodingBindingElement();
        element.WriteEncoding = encoding;
        Assert.Equal<Encoding>(encoding, element.WriteEncoding);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [MemberData("InvalidEncodings", MemberType = typeof(TestData))]
    [ActiveIssue(1450)]
    public static void WriteEncoding_Property_Set_Throws_For_Invalid_Encodings(Encoding encoding)
    {
        TextMessageEncodingBindingElement element = new TextMessageEncodingBindingElement();
        Assert.Throws<ArgumentException>(() => element.WriteEncoding = encoding);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [MemberData("ValidTextMessageEncoderMessageVersions", MemberType = typeof(TestData))]
    public static void MessageVersion_Property_Sets(MessageVersion messageVersion)
    {
        TextMessageEncodingBindingElement element = new TextMessageEncodingBindingElement();
        element.MessageVersion = messageVersion;
        Assert.Equal<MessageVersion>(messageVersion, element.MessageVersion);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
#endif
    [WcfFact]
    public static void MessageVersion_Property_Set_Null_Throws()
    {
        TextMessageEncodingBindingElement element = new TextMessageEncodingBindingElement();
        Assert.Throws<ArgumentNullException>(() => element.MessageVersion = null);
    }
}
