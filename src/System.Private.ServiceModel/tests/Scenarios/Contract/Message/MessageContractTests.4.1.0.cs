// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel.Channels;
using System.Xml;
using Infrastructure.Common;
using MessageContractCommon;
using Xunit;

public static class MessageContractTests_4_1_0
{
    [WcfFact]
    [OuterLoop]
    public static void MessageContract_IsWrapped_True()
    {
        MessageContractHelpers.IMessageContract clientProxy;
        MessageContractTypes.RequestBankingData requestData;
        MyInspector inspector = MessageContractHelpers.SetupMessageContractTests(out clientProxy, out requestData);
        clientProxy.MessageContractRequestReply(requestData);
        XmlDictionaryReader reader = MessageContractHelpers.GetResponseBodyReader(inspector);

        string wrapperName = "ReplyBankingDataWrapper";
        Assert.True(reader.LocalName.Equals(wrapperName),
            string.Format("reader.LocalName - Expected: {0}, Actual: {1}", wrapperName, reader.LocalName));

        Assert.True(reader.NamespaceURI.Equals(MessageContractConstants.wrapperNamespace),
            string.Format("reader.NamespaceURI - Expected: {0}, Actual: {1}", MessageContractConstants.wrapperNamespace, reader.NamespaceURI));
    }

    [WcfFact]
    [OuterLoop]
    public static void MessageContract_IsWrapped_False()
    {
        MessageContractHelpers.IMessageContract clientProxy;
        MessageContractTypes.RequestBankingData requestData;
        MyInspector inspector = MessageContractHelpers.SetupMessageContractTests(out clientProxy, out requestData);
        clientProxy.MessageContractRequestReplyNotWrapped(requestData);
        XmlDictionaryReader reader = MessageContractHelpers.GetResponseBodyReader(inspector);

        string wrapperName = "ReplyBankingDataWrapper";
        Assert.False(reader.LocalName.Equals(wrapperName),
            "When IsWrapped set to false, the message body should not be wrapped with an extra element.");
    }

    [WcfFact]
    [OuterLoop]
    public static void MessageBody_Elements_Ordered()
    {
        MessageContractHelpers.IMessageContract clientProxy;
        MessageContractTypes.RequestBankingData requestData;
        MyInspector inspector = MessageContractHelpers.SetupMessageContractTests(out clientProxy, out requestData);
        clientProxy.MessageContractRequestReply(requestData);
        XmlDictionaryReader reader = MessageContractHelpers.GetResponseBodyReader(inspector);

        string wrapperName = "ReplyBankingDataWrapper";
        Assert.True(reader.LocalName.Equals(wrapperName),
            string.Format("Unexpected element order (1/5). Expected {0}, Actual: {1}", wrapperName, reader.LocalName));

        reader.Read();

        Assert.True(reader.LocalName.Equals(MessageContractConstants.dateElementName),
            string.Format("Unexpected element order (2/5). Expected {0}, Actual: {1}", MessageContractConstants.dateElementName, reader.LocalName));

        reader.Read(); // Move to Value node
        reader.Read(); // Move to the end tag
        reader.ReadEndElement(); // Checks that the current content node is an end tag and advances the reader to the next node.

        Assert.True(reader.LocalName.Equals(MessageContractConstants.transactionElementName),
            string.Format("Unexpected element order (3/5). Expected: {0}, Actual: {1}", MessageContractConstants.transactionElementName, reader.LocalName));

        reader.Read(); // Move to Value node
        reader.Read(); // Move to the end tag
        reader.ReadEndElement(); // Checks that the current content node is an end tag and advances the reader to the next node.

        Assert.True(reader.LocalName.Equals(MessageContractConstants.customerElementName),
            string.Format("Unexpected element order (4/5). Expected: {0}, Actual: {1}", MessageContractConstants.customerElementName, reader.LocalName));


        reader.Read(); // Move to Value node
        reader.Read(); // Move to the end tag
        reader.ReadEndElement(); // Checks that the current content node is an end tag and advances the reader to the next node.

        Assert.True(reader.IsStartElement() == false && reader.LocalName.Equals(wrapperName),
            string.Format("Unexpected element order (5/5). Expected: {0}, Actual: {1}", wrapperName, reader.LocalName));
    }

    [WcfFact]
    [OuterLoop]
    public static void MessageBody_Elements_CustomerElement_Value_Matches()
    {
        MessageContractHelpers.IMessageContract clientProxy;
        MessageContractTypes.RequestBankingData requestData;
        MyInspector inspector = MessageContractHelpers.SetupMessageContractTests(out clientProxy, out requestData);
        clientProxy.MessageContractRequestReply(requestData);
        XmlDictionaryReader reader = MessageContractHelpers.GetResponseBodyReader(inspector);

        bool elementFound = false;
        while (reader.Read())
        {
            if (reader.LocalName.Equals(MessageContractConstants.customerElementName) && reader.NamespaceURI.Equals(MessageContractConstants.customerElementNamespace))
            {
                elementFound = true;
                reader.ReadStartElement();
                Assert.Equal(MessageContractConstants.customerElementValue, reader.Value);
                break;
            }
            else
            {
                // Continue checking remaining nodes.
            }
        }

        Assert.True(elementFound,
            string.Format("Expected element not found. Looking For: {0} && {1}", MessageContractConstants.customerElementName, MessageContractConstants.customerElementNamespace));
    }

    [WcfFact]
    [OuterLoop]
    public static void MessageHeader_MustUnderstand_True()
    {
        MessageContractHelpers.IMessageContract clientProxy;
        MessageContractTypes.RequestBankingData requestData;
        MyInspector inspector = MessageContractHelpers.SetupMessageContractTests(out clientProxy, out requestData);
        clientProxy.MessageContractRequestReplyWithMessageHeader(requestData);
        MessageHeaders headers = MessageContractHelpers.GetHeaders(inspector);

        string extraValue = "ReplyBankingDataWithMessageHeaderExtraValues";
        int index = headers.FindHeader(extraValue, MessageContractConstants.extraValuesNamespace);
        var header = headers[index];

        Assert.True(header != null, "There's no header in the message.");
        Assert.True(header.MustUnderstand, "Expected MustUnderstand to be true, but it was false.");
    }

    [WcfFact]
    [OuterLoop]
    public static void MessageHeader_MustUnderstand_False()
    {
        MessageContractHelpers.IMessageContract clientProxy;
        MessageContractTypes.RequestBankingData requestData;
        MyInspector inspector = MessageContractHelpers.SetupMessageContractTests(out clientProxy, out requestData);
        clientProxy.MessageContractRequestReplyWithMessageHeaderNotNecessaryUnderstood(requestData);
        MessageHeaders headers = MessageContractHelpers.GetHeaders(inspector);

        string extraValue = "ReplyBankingDataWithMessageHeaderNotNecessaryUnderstoodExtraValue";
        int index = headers.FindHeader(extraValue, MessageContractConstants.extraValuesNamespace);
        var header = headers[index];

        Assert.True(header != null, "There's no header in the message.");
        Assert.False(header.MustUnderstand, "Expected MustUnderstand to be false, but it was true.");
    }
}
