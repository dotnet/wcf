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

public static class Http_TransportChannel_BasicBindings_NetHttpBindingTests
{
    [Fact]
    [OuterLoop]
    public static void DefaultSettings_EchoString_RoundTrips()
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