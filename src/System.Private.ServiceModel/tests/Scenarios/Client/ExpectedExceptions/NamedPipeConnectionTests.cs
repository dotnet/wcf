// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public class NamedPipeConnectionTests : ConditionalWcfTest
{
    [WcfFact]
    [Condition(nameof(Is_Windows))]
    [OuterLoop]
    public static async Task HighConcurrency_Connections_RetryUntilAvailable()
    {
        const int ConnectionCount = 100;
        Task<string>[] requests = new Task<string>[ConnectionCount];

        for (int i = 0; i < requests.Length; i++)
        {
            requests[i] = EchoAsync();
        }

        string[] results = await Task.WhenAll(requests);
        Assert.All(results, result => Assert.Equal("Hello", result));
    }

    private static async Task<string> EchoAsync()
    {
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                OpenTimeout = TimeSpan.FromSeconds(10)
            };
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.NamedPipe_NoSecurity_Address));
            serviceProxy = factory.CreateChannel();

            return await serviceProxy.EchoWithTimeoutAsync("Hello", TimeSpan.FromMilliseconds(100));
        }
        finally
        {
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}
