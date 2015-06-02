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

public static class Binding_Http_NetHttpBindingTests
{
    [Fact]
    [OuterLoop]
    public static void DefaultSettings_Echo_RoundTrips_String()
    {
        string testString = "Hello";

        StringBuilder errorBuilder = new StringBuilder();
        NetHttpBinding binding = new NetHttpBinding();

        bool success = false;
        ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_NetHttp));
        IWcfService serviceProxy = factory.CreateChannel();
        string result = serviceProxy.Echo(testString);

        Assert.True(string.Equals(result, testString), 
            string.Format("Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));
    }
}