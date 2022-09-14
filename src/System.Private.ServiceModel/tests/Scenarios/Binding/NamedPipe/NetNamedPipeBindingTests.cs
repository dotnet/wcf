// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public partial class Binding_NamedPipe_NetNamedPipeBindingTests : ConditionalWcfTest
{
    // Simple echo of a string using NetNamedPipeBinding on both client and server with SecurityMode=None
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
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.NamedPipe_NoSecurity_Address));
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

    // Simple echo of a string using NetNamedPipeBinding on both client and server with all default settings.
    // Default settings are:
    //                         - SecurityMode = Transport
    //                         - ClientCredentialType = Windows
    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available),
               nameof(WindowsOrSelfHosted))]
    [OuterLoop]
    public static void DefaultSettings_Echo_RoundTrips_String()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetNamedPipeBinding binding = new NetNamedPipeBinding();
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.NamedPipe_DefaultBinding_Address));
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

    // Simple echo of a string using NetNamedPipeBinding on both client and server with SecurityMode=Transport
    // By default ClientCredentialType will be 'Windows'
    // SecurityMode is Transport by default with NetNamedPipeBinding, this test explicitly sets it.
    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available),
               nameof(WindowsOrSelfHosted))]
    [OuterLoop]
    public static void SecurityModeTransport_Echo_RoundTrips_String()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.NamedPipe_DefaultBinding_Address));
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
}
