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
    public static void DefaultSettings_Echo_RoundTrips_String()
    {
        string variationDetails = "Client:: BasicHttpBinding/DefaultValues\nServer:: BasicHttpBinding/DefaultValues";
        StringBuilder errorBuilder = new StringBuilder();

        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
        ScenarioTestHelpers.RunBasicEchoTest(binding, Endpoints.HttpBaseAddress_Basic, variationDetails, errorBuilder);

        Assert.True(errorBuilder.Length == 0, "Test case FAILED with errors: " + errorBuilder.ToString());
    }
}