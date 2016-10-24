// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.Text;
using Infrastructure.Common;
using Xunit;

public static class MessageContractTest_4_4_0
{
    [WcfTheory]
    [InlineData(true)]
    [InlineData(false)]
    public static void MessageHeaderArray_MustUnderStand_Sets(bool mustUnderstand)
    {
        MessageHeaderArrayAttribute attribute = new MessageHeaderArrayAttribute();
        attribute.MustUnderstand = mustUnderstand;
        Assert.Equal(mustUnderstand, attribute.MustUnderstand);
    }

    [WcfTheory]
    [InlineData(new object[] { null })]
    [InlineData("")]
    [InlineData("testName")]
    public static void MessageProperty_Name_Sets(string name)
    {
        MessagePropertyAttribute attribute = new MessagePropertyAttribute();
        attribute.Name = name;
        Assert.True(String.Equals(name, attribute.Name),
                    String.Format("Set Name to '{0}' but getter returned '{1}'", name, attribute.Name));
    }

    [WcfFact]
    public static void MessageContract_ContractDescription_MessageHeaders()
    {
        // -----------------------------------------------------------------------------------------------
        // IMessageContract_4_4_0:
        //    This service exposes a single operation that uses MessageContract.
        //    This contract uses attributes added to the public API only after 1.1.0
        //    (MessageHeaderArrayAttribute and MessageProperty)
        // -----------------------------------------------------------------------------------------------
        string results = ContractDescriptionTestHelper.ValidateContractDescription<IMessageContract_4_4_0>(new ContractDescriptionData
        {
            Operations = new OperationDescriptionData[]
            {
                new OperationDescriptionData
                {
                    Name = TestTypeConstants_4_4_0.MessageContract_RequestReply_OperationName,
                    IsOneWay = false,
                    HasTask = false,
                    Messages = new MessageDescriptionData[]
                    {
                        // The request message description
                        new MessageDescriptionData
                        {
                            Action = TestTypeConstants_4_4_0.MessageContract_Request_Action,
                            Direction = MessageDirection.Input,
                            MessageType = typeof(RequestBankingData_4_4_0),
                            Body = new PartDescriptionData[]
                            {
                                new PartDescriptionData { Name = TestTypeConstants_4_4_0.MessageContract_Request_TransactionDateName,
                                                          Type = typeof(DateTime),
                                                          Multiple = false },
                                new PartDescriptionData { Name = TestTypeConstants_4_4_0.MessageContract_Request_CustomerName,
                                                          Type = typeof(string),
                                                          Multiple = false }
                            },
                            Headers = new PartDescriptionData[]
                            {
                                new PartDescriptionData { Name = TestTypeConstants_4_4_0.MessageContract_Request_SingleValueName,
                                                          Type = typeof(string),
                                                          Multiple = false },
                                new PartDescriptionData { Name = TestTypeConstants_4_4_0.MessageContract_Request_MultipleValueName,
                                                          Type = typeof(string[]),
                                                          Multiple = false },
                                new PartDescriptionData { Name = TestTypeConstants_4_4_0.MessageContract_Request_MultipleArrayValueName,
                                                          Type = typeof(string),
                                                          Multiple = true },
                            },
                            Properties = new PartDescriptionData[]
                            {
                                new PartDescriptionData { Name = TestTypeConstants_4_4_0.MessageContract_Request_PropertyName,
                                                          Type = null,
                                                          Multiple = false },
                            }
                        },

                        // The reply message description
                        new MessageDescriptionData
                        {
                            Action = TestTypeConstants_4_4_0.MessageContract_Reply_Action,
                            Direction = MessageDirection.Output,
                            MessageType = typeof(ReplyBankingData_4_4_0),
                            Body = new PartDescriptionData[]
                            {
                                new PartDescriptionData { Name = TestTypeConstants_4_4_0.MessageContract_Reply_TransactionDateName,
                                                          Type = typeof(DateTime),
                                                          Multiple = false },
                                new PartDescriptionData { Name = TestTypeConstants_4_4_0.MessageContract_Reply_CustomerName,
                                                          Type = typeof(string),
                                                          Multiple = false },
                                new PartDescriptionData { Name = TestTypeConstants_4_4_0.MessageContract_Reply_TransactionAmountName,
                                                          Type = typeof(decimal),
                                                          Multiple = false },
                            },
                            Headers = new PartDescriptionData[]
                            {
                                new PartDescriptionData { Name = TestTypeConstants_4_4_0.MessageContract_Reply_SingleValueName,
                                                          Type = typeof(string),
                                                          Multiple = false },
                                new PartDescriptionData { Name = TestTypeConstants_4_4_0.MessageContract_Reply_MultipleValueName,
                                                          Type = typeof(string[]),
                                                          Multiple = false },
                                new PartDescriptionData { Name = TestTypeConstants_4_4_0.MessageContract_Reply_MultipleArrayValueName,
                                                          Type = typeof(string),
                                                          Multiple = true },
                            },
                            Properties = new PartDescriptionData[]
                            {
                                new PartDescriptionData { Name = TestTypeConstants_4_4_0.MessageContract_Reply_PropertyName,
                                                          Type = null,
                                                          Multiple = false },
                            }
                        }
                    }
               }
            }
        });

        // Assert.True because results contains informative error failure
        Assert.True(results == null, results);
    }

    [WcfFact]
    public static void MessageContract_Builds_Correct_Request_Message_Properties_And_Headers()
    {
        MockChannelFactory<IRequestChannel> mockChannelFactory = null;
        MockRequestChannel mockRequestChannel = null;
        RequestBankingData_4_4_0 request = null;
        Dictionary<string, string> properties = new Dictionary<string, string>();
        Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();

        // *** SETUP *** \\
        // Intercept the creation of the factory so we can intercept creation of the channel
        Func<Type, BindingContext, IChannelFactory> buildFactoryAction = (Type type, BindingContext context) =>
        {
            mockChannelFactory = new MockChannelFactory<IRequestChannel>(context, new TextMessageEncodingBindingElement().CreateMessageEncoderFactory());
            mockChannelFactory.OnCreateChannelOverride = (EndpointAddress endpoint, Uri via) =>
            {
                // When the channel is created, override the Request method to capture properties and headers
                mockRequestChannel = (MockRequestChannel)mockChannelFactory.DefaultOnCreateChannel(endpoint, via);
                mockRequestChannel.RequestOverride = (msg, timeout) =>
                {
                    // Capture properties and headers before the message is read and closed
                    foreach (var property in msg.Properties)
                    {
                        properties[property.Key] = property.Value.ToString();
                    }

                    foreach (MessageHeader header in msg.Headers)
                    {
                        List<string> values = null;
                        if (!headers.TryGetValue(header.Name, out values))
                        {
                            values = new List<string>();
                            headers[header.Name] = values;
                        }

                        string headerValue = header.ToString().Replace(Environment.NewLine, String.Empty).Replace("  ", String.Empty);
                        values.Add(headerValue);
                    }

                    // Echo back a fresh copy of the request
                    MessageBuffer buffer = msg.CreateBufferedCopy(Int32.MaxValue);
                    return buffer.CreateMessage();
                };

                return mockRequestChannel;
            };

            return mockChannelFactory;
        };

        MockTransportBindingElement mockBindingElement = new MockTransportBindingElement();
        mockBindingElement.BuildChannelFactoryOverride = buildFactoryAction;

        CustomBinding binding = new CustomBinding(mockBindingElement);
        EndpointAddress address = new EndpointAddress("myprotocol://localhost:5000");
        var factory = new ChannelFactory<IMessageContractRoundTrip_4_4_0>(binding, address);
        IMessageContractRoundTrip_4_4_0 channel = factory.CreateChannel();

        // Prepare a strongly-typed request
        request = new RequestBankingData_4_4_0();
        request.accountName = "My account";
        request.transactionDate = DateTime.Now;
        request.requestTestProperty = "test property";
        request.testValue = "test value";
        request.testValues = new string[] { "test", "values" };
        request.testValuesArray = new string[] { "test", "values", "array"};

        // *** EXECUTE *** \\
        channel.MessageContractRoundTrip(request);

        // *** VALIDATE *** \\

        string propertyValue = null;
        bool hasProperty = properties.TryGetValue(TestTypeConstants_4_4_0.MessageContract_Request_PropertyName, out propertyValue);
        Assert.True(hasProperty, String.Format("Expected message property '{0}'", TestTypeConstants_4_4_0.MessageContract_Request_PropertyName));
        Assert.True(String.Equals(request.requestTestProperty, propertyValue),
                    String.Format("Message property value for '{0}' expected = '{1}', actual = '{2}'",
                                  TestTypeConstants_4_4_0.MessageContract_Request_PropertyName, request.requestTestProperty, propertyValue));

        ValidateHeader(headers, TestTypeConstants_4_4_0.MessageContract_Request_SingleValueName,
@"<?xml version=""1.0"" encoding=""utf-16""?>" +
@"<h:Request_SingleRequestValue xmlns:h=""http://www.contoso.com"">test value</h:Request_SingleRequestValue>");

        ValidateHeader(headers, TestTypeConstants_4_4_0.MessageContract_Request_MultipleValueName,
@"<?xml version=""1.0"" encoding=""utf-16""?>" + 
@"<h:Request_MultipleRequestValue xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:h=""http://www.contoso.com"">" + 
@"<string xmlns=""http://schemas.microsoft.com/2003/10/Serialization/Arrays"">test</string>" + 
@"<string xmlns=""http://schemas.microsoft.com/2003/10/Serialization/Arrays"">values</string>" +
@"</h:Request_MultipleRequestValue>");

        ValidateHeader(headers, TestTypeConstants_4_4_0.MessageContract_Request_MultipleArrayValueName,
@"<?xml version=""1.0"" encoding=""utf-16""?>" + 
@"<h:Request_MultipleArrayRequestValue xmlns:h=""http://www.contoso.com"">test</h:Request_MultipleArrayRequestValue>",

@"<?xml version=""1.0"" encoding=""utf-16""?>" + 
@"<h:Request_MultipleArrayRequestValue xmlns:h=""http://www.contoso.com"">values</h:Request_MultipleArrayRequestValue>",

@"<?xml version=""1.0"" encoding=""utf-16""?>" + 
@"<h:Request_MultipleArrayRequestValue xmlns:h=""http://www.contoso.com"">array</h:Request_MultipleArrayRequestValue>");
    }

    [WcfFact]
    public static void MessageContract_RoundTrips_Properties_And_Headers()
    {
        MockChannelFactory<IRequestChannel> mockChannelFactory = null;
        MockRequestChannel mockRequestChannel = null;
        RequestBankingData_4_4_0 request = null;
        RequestBankingData_4_4_0 reply = null;

        // *** SETUP *** \\
        // Intercept the creation of the factory so we can intercept creation of the channel
        Func<Type, BindingContext, IChannelFactory> buildFactoryAction = (Type type, BindingContext context) =>
        {
            mockChannelFactory = new MockChannelFactory<IRequestChannel>(context, new TextMessageEncodingBindingElement().CreateMessageEncoderFactory());
            mockChannelFactory.OnCreateChannelOverride = (EndpointAddress endpoint, Uri via) =>
            {
                // When the channel is created, override the Request method to capture properties and headers
                mockRequestChannel = (MockRequestChannel)mockChannelFactory.DefaultOnCreateChannel(endpoint, via);
                mockRequestChannel.RequestOverride = (msg, timeout) =>
                {
                    // Echo back a fresh copy of the request
                    MessageBuffer buffer = msg.CreateBufferedCopy(Int32.MaxValue);
                    Message replyMsg = buffer.CreateMessage();
                    return replyMsg;
                };

                return mockRequestChannel;
            };

            return mockChannelFactory;
        };

        MockTransportBindingElement mockBindingElement = new MockTransportBindingElement();
        mockBindingElement.BuildChannelFactoryOverride = buildFactoryAction;

        CustomBinding binding = new CustomBinding(mockBindingElement);
        EndpointAddress address = new EndpointAddress("myprotocol://localhost:5000");
        var factory = new ChannelFactory<IMessageContractRoundTrip_4_4_0>(binding, address);
        IMessageContractRoundTrip_4_4_0 channel = factory.CreateChannel();

        // Prepare a strongly-typed request
        request = new RequestBankingData_4_4_0();
        request.accountName = "My account";
        request.transactionDate = DateTime.Now;
        request.requestTestProperty = "test property";
        request.testValue = "test value";
        request.testValues = new string[] { "test", "values" };
        request.testValuesArray = new string[] { "test", "values", "array" };

        // *** EXECUTE *** \\
        reply = channel.MessageContractRoundTrip(request);

        // *** VALIDATE *** \\
        Assert.True(String.Equals(request.requestTestProperty, reply.requestTestProperty),
                    String.Format("TestProperty expected = '{0}' actual = '{1}'",
                                  request.requestTestProperty, reply.requestTestProperty));

        ValidateArray("TestValues", request.testValues, reply.testValues);
        ValidateArray("TestValuesArray", request.testValuesArray, reply.testValuesArray);
    }

    private static void ValidateHeader(Dictionary<string, List<string>> headers, string name, params string[] values)
    {
        List<string> headerValues;
        bool hasHeader = headers.TryGetValue(name, out headerValues);
        Assert.True(hasHeader, String.Format("Expected header '{0}'", name));

        StringBuilder errorBuilder = new StringBuilder();
        for (int i = 0; i < headerValues.Count; ++i)
        {
            errorBuilder.AppendLine(String.Format("---- Header[{0}] ----", i));
            errorBuilder.AppendLine(String.Format("{0}", headerValues[i]));
        }

        Assert.True(values.Length == headerValues.Count,
                    String.Format("Expected header '{0}' to have {1} values, actual = {2}", name, values.Length, headerValues.Count));

        foreach (string value in values)
        {
            Assert.True(headerValues.Contains(value),
                        String.Format("Expected header '{0}' to contain value:{1}{2}{1} but it contained these:{1}{3}", 
                                      name, Environment.NewLine, value, errorBuilder.ToString()));
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
}
