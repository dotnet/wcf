// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Tests.Common;
using Infrastructure.Common;
using Xunit;

public static class MessageTest
{
    private const string s_action = "http://tempuri.org/someserviceendpoint";

    [WcfTheory]
    [MemberData(nameof(TestData.MessageVersionsWithEnvelopeAndAddressingVersions), MemberType = typeof(TestData))]
    public static void MessageVersion_Verify_AddressingVersions_And_EnvelopeVersions(MessageVersion messageVersion, EnvelopeVersion envelopeVersion, AddressingVersion addressingVersion)
    {
        Assert.Equal<EnvelopeVersion>(envelopeVersion, messageVersion.Envelope);
        Assert.Equal<AddressingVersion>(addressingVersion, messageVersion.Addressing);
    }

    [WcfFact]
    public static void CreateMessageWithSoap12WSAddressing10_WithNoBody()
    {
        var message = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, s_action);
        Assert.Equal<MessageVersion>(MessageVersion.Soap12WSAddressing10, message.Version);
        Assert.Equal(s_action, message.Headers.Action);
        Assert.True(message.IsEmpty);
    }

    [WcfFact]
    public static void CreateMessageWithSoap12WSAddressing10_WithBody()
    {
        string content = "This is what goes in the body of the message.";
        object body = content;
        var message = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, s_action, body);

        Assert.Equal<MessageVersion>(MessageVersion.Soap12WSAddressing10, message.Version);
        Assert.Equal(s_action, message.Headers.Action);
        Assert.False(message.IsEmpty);

        var reader = message.GetReaderAtBodyContents();
        var messageBody = reader.ReadElementContentAsString();

        Assert.Equal(content, messageBody);
    }

    [WcfFact]
    public static void CreateMessageWithSoap12WSAddressing10_WithCustomBodyWriter()
    {
        var message = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, s_action, new CustomBodyWriter());

        Assert.Equal<MessageVersion>(MessageVersion.Soap12WSAddressing10, message.Version);
        Assert.Equal(s_action, message.Headers.Action);
        Assert.False(message.IsEmpty);

        var reader = message.GetReaderAtBodyContents();
        var messageBody = reader.ReadContentAsString();

        Assert.Equal(string.Empty, messageBody);
    }

    [WcfFact]
    // Get the MessageVersion from a Custom binding
    public static void GetMessageVersion()
    {
        MessageVersion version = null;
        BindingElement[] bindingElements = new BindingElement[2];
        bindingElements[0] = new TextMessageEncodingBindingElement();
        bindingElements[1] = new HttpTransportBindingElement();
        CustomBinding binding = new CustomBinding(bindingElements);
        version = binding.MessageVersion;

        string expected = "Soap12 (http://www.w3.org/2003/05/soap-envelope) Addressing10 (http://www.w3.org/2005/08/addressing)";
        string actual = version.ToString();
        Assert.Equal(expected, actual);
    }

    [WcfFact]
    public static void CreateMessageWithFaultCode()
    {
        FaultCode faultCode = new FaultCode("fName");
        string faultReason = "fault reason";
        object faultDetail = new FaultDetail("fault details");

        //create message without fault detail
        var message = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, faultCode, faultReason, s_action);
        Assert.Equal(MessageVersion.Soap12WSAddressing10, message.Version);
        Assert.Equal(s_action, message.Headers.Action);
        Assert.False(message.IsEmpty);
        Assert.True(message.IsFault);

        var msgFault = MessageFault.CreateFault(message, int.MaxValue);
        Assert.Equal(faultReason, msgFault.Reason.GetMatchingTranslation().Text);

        //create message with fault detail
        message = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, faultCode, faultReason, faultDetail, s_action);
        Assert.Equal(MessageVersion.Soap12WSAddressing10, message.Version);
        Assert.Equal(s_action, message.Headers.Action);
        Assert.False(message.IsEmpty);
        Assert.True(message.IsFault);

        msgFault = MessageFault.CreateFault(message, int.MaxValue);
        Assert.Equal(faultReason, msgFault.Reason.GetMatchingTranslation().Text);
        Assert.True(msgFault.HasDetail);
        var msgFDetail = msgFault.GetDetail<FaultDetail>();
        Assert.NotNull(msgFDetail);
        Assert.Equal(((FaultDetail)faultDetail).Message, msgFDetail.Message);
    }
}
