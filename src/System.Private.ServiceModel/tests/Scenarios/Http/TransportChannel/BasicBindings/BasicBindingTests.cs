// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestTypes;
using Xunit;

public static class Http_TransportChannel_BasicBindings_BasicBindingTests
{

    [Fact]
    [OuterLoop]
    public static void BasicHttpBinding_DefaultSettings_EchoString()
    {
        string testCaseName = "HttpsTransportBindingElement_ScenarioTests.SameBinding_DefaultSettings_EchoString";
        string variationDetails = "Client:: BasicHttpBinding/DefaultValues\nServer:: BasicHttpBinding/DefaultValues";

        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            ScenarioTestHelpers.RunBasicEchoTest(binding, Endpoints.HttpBaseAddress_Basic, variationDetails, errorBuilder);
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, String.Format("Test Case: {0} FAILED with the following errors: {1}", testCaseName, errorBuilder));
    }


    // Client and Server bindings setup exactly the same using default settings.
    [Fact]
    [OuterLoop]
    public static void SameBinding_DefaultSettings_EchoString()
    {
        string testCaseName = "HttpsTransportBindingElement_ScenarioTests.SameBinding_DefaultSettings_EchoString";
        string variationDetails = "Client:: CustomBinding/DefaultValues\nServer:: CustomBinding/DefaultValues";

        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            CustomBinding binding = new CustomBinding(new TextMessageEncodingBindingElement(), new HttpsTransportBindingElement());
            ScenarioTestHelpers.RunBasicEchoTest(binding, Endpoints.HttpsSoap12_Address, variationDetails, errorBuilder);
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, String.Format("Test Case: {0} FAILED with the following errors: {1}", testCaseName, errorBuilder));
    }

    [Fact]
    [OuterLoop]
    public static void NetHttpBinding_DefaultSettings_EchoString()
    {
        string testString = "Hello";

        StringBuilder errorBuilder = new StringBuilder();
        NetHttpBinding binding = new NetHttpBinding();

        bool success = false;
        ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_NetHttp));
        IWcfService serviceProxy = factory.CreateChannel();
        string result = serviceProxy.Echo(testString);
        success = string.Equals(result, testString);

        Assert.True(success, string.Format("Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));
    }
}