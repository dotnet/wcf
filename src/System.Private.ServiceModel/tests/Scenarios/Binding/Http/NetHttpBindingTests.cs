// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Tests.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestTypes;
using Xunit;

public static class Binding_Http_NetHttpBindingTests 
{
    [Fact]
    [OuterLoop]
    public static void DefaultSettings_Echo_RoundTrips_String()
    {
        string variationDetails = "Client:: NetHttpBinding/DefaultValues\nServer:: NetHttpBinding/DefaultValues";
        string testString = "Hello";
        StringBuilder errorBuilder = new StringBuilder();
        bool success = false;

        NetHttpBinding binding = new NetHttpBinding();

        try
        {
            ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_NetHttp));
            IWcfService serviceProxy = factory.CreateChannel();
            string result = serviceProxy.Echo(testString);
            success = string.Equals(result, testString);

            if (!success)
            {
                errorBuilder.AppendLine(String.Format("    Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));
            }
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("    Error: Unexpected exception was caught while doing the basic echo test for variation...\n'{0}'\nException: {1}", variationDetails, ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, "Test case FAILED with errors: " + errorBuilder.ToString());
    }
}