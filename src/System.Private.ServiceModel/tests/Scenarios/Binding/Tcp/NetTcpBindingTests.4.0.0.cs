// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public partial class Binding_Tcp_NetTcpBindingTests : ConditionalWcfTest
{
    // Simple echo of a string using NetTcpBinding on both client and server with SecurityMode=None
    [WcfFact]
    [OuterLoop]
    public static void SecurityModeNone_Echo_RoundTrips_String()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_Address));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    // Test for https://github.com/dotnet/wcf/issues/4946
    [WcfFact]
    [OuterLoop]
    public static async Task SecurityModeNone_Echo_RoundTrips_String_SyncAfterAsync()
    {
        string testString = "Hello";
        ChannelFactory<IWcfServiceGenerated> factory = null;
        IWcfServiceGenerated serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            factory = new ChannelFactory<IWcfServiceGenerated>(binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_Address));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);
            Assert.Equal(testString, result);
            // Without the ConfigureAwait, the xunit sync context will hop threads from the
            // completing thread to their sync context. ConfigureAwait means it continues
            // executing on the completing thread and triggers the bug (without the fix).
            result = await serviceProxy.EchoAsync(testString).ConfigureAwait(false);
            Assert.Equal(testString, result);
            result = serviceProxy.Echo(testString);
            Assert.Equal(testString, result);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    // Test for https://github.com/dotnet/wcf/issues/5134
    [WcfFact]
    [OuterLoop]
    public static void ReceiveTimeout_Applied()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            binding.OpenTimeout = TimeSpan.FromSeconds(10);
            binding.SendTimeout = binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_Address));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            ((IClientChannel)serviceProxy).Open();
            Thread.Sleep(binding.OpenTimeout + TimeSpan.FromSeconds(5));
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}
