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

public static class Http_TransportChannel_BasicBindings_BasicHttpBindingTests
{

    [Fact]
    [OuterLoop]
    public static void DefaultSettings_EchoString_RoundTrips()
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
}