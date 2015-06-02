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

public static class Binding_Http_Tests_CustomBindingWithHttpTransportTests
{
    // Client and Server bindings setup exactly the same using default settings.
    [Fact]
    [OuterLoop]
    public static void SameBinding_Https_Soap12_Echo_RoundTrips_String()
    {
        string variationDetails = "Client:: CustomBinding/DefaultValues\nServer:: CustomBinding/DefaultValues";
        StringBuilder errorBuilder = new StringBuilder();

        CustomBinding binding = new CustomBinding(new TextMessageEncodingBindingElement(), new HttpsTransportBindingElement());
        ScenarioTestHelpers.RunBasicEchoTest(binding, Endpoints.HttpsSoap12_Address, variationDetails, errorBuilder);

        Assert.True(errorBuilder.Length == 0, "Test case FAILED with errors: " + errorBuilder.ToString());
    }
}