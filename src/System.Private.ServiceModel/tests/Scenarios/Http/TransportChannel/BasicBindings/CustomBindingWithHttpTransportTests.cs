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

public static class Http_TransportChannel_BasicBindings_CustomBindingWithHttpTransportTests
{
    // Client and Server bindings setup exactly the same using default settings.
    [Fact]
    [OuterLoop]
    public static void SameBinding_Https_Soap12_EchoString_RoundTrips()
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
}