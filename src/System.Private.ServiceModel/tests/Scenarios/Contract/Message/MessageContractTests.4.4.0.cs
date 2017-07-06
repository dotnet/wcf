// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using Infrastructure.Common;
using WcfService;
using Xunit;

public static class MessageContractTests_4_4_0
{
    [WcfFact]
    [OuterLoop]
    public static void Message_With_MessageHeaders_RoundTrips()
    {
        BasicHttpBinding binding = null;
        IWcfService_4_4_0 clientProxy = null;
        RequestBankingData_4_4_0 requestData = null;
        ChannelFactory<IWcfService_4_4_0> factory = null;

        // *** SETUP *** \\
        try
        {
            binding = new BasicHttpBinding();
            factory = new ChannelFactory<IWcfService_4_4_0>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_4_4_0_Basic));
            clientProxy = factory.CreateChannel();

            requestData = new RequestBankingData_4_4_0();
            requestData.accountName = "Michael Jordan";
            requestData.transactionDate = DateTime.Now;
            requestData.amount = 100.0M;

            // post-1.1.0 features
            requestData.requestSingleValue = "test single value";
            requestData.requestMultipleValues = "test,multiple,value".Split(',');
            requestData.requestArrayMultipleValues = "test,array,multiple,value".Split(',');

            // *** EXECUTE *** \\
            ReplyBankingData_4_4_0 replyData = clientProxy.MessageContractRequestReply(requestData);

            // *** VALIDATE *** \\
            Assert.True(String.Equals(requestData.accountName, replyData.accountName),
                        String.Format("Expected Customer = '{0}', actual = '{1}'",
                                      requestData.accountName, replyData.accountName));

            Assert.True(requestData.amount == replyData.amount,
                        String.Format("Expected Amount = '{0}', actual = '{1}'",
                                      requestData.amount, requestData.amount));

            Assert.True(String.Equals(requestData.requestSingleValue, replyData.replySingleValue),
                        String.Format("Expected RequestSingleValue = '{0}', actual = '{1}",
                                      requestData.requestSingleValue, replyData.replySingleValue));

            ValidateArray("MultipleValue", requestData.requestMultipleValues, replyData.replyMultipleValues);
            ValidateArray("ArrayMultipleValue", requestData.requestArrayMultipleValues, replyData.replyArrayMultipleValues);

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)clientProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)clientProxy, factory);
        }
    }

    private static void ValidateArray(string elementName, string[] array1, string[] array2)
    {
        Assert.True(array2 != null,
                    String.Format("The {0} element returned a null array", elementName));

        Assert.True(array1.Length == array2.Length,
                    String.Format("The {0} element was expected to return {1} items, actual = {2}",
                                    elementName, array1.Length, array2.Length));

        for (int i = 0; i < array1.Length; ++i)
        {
            Assert.True(array1[i] == array2[i],
                        String.Format("Array item {0} of element {1} was expected to be {2}, actual was {3}",
                                        i, elementName, array1[i], array2[i]));
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void Message_With_XmlElementMessageHeader_RoundTrip()
    {
        BasicHttpBinding binding = null;
        IWcfService_4_4_0 clientProxy = null;
        ChannelFactory<IWcfService_4_4_0> factory = null;

        // *** SETUP *** \\
        try
        {
            binding = new BasicHttpBinding();
            factory = new ChannelFactory<IWcfService_4_4_0>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_4_4_0_Basic));
            clientProxy = factory.CreateChannel();

            string testString = "test string";
            var header = new XmlElementMessageHeader() { HeaderValue = testString };
            var request = new XmlElementMessageHeaderRequest(header);

            // *** EXECUTE *** \\
            XmlElementMessageHeaderResponse response = clientProxy.SendRequestWithXmlElementMessageHeader(request);

            // *** VALIDATE *** \\
            Assert.True(response != null,
                        $"Expected {nameof(response)} not to be null , but it was null");

            Assert.True(String.Equals(testString, response.TestResult),
                        $"Expected {nameof(response.TestResult)} = {testString}, actual was {response.TestResult}");

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)clientProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)clientProxy, factory);
        }
    }
}
