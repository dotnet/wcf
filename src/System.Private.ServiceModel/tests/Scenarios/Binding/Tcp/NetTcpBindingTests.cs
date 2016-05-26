// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Infrastructure.Common;
using System.ServiceModel;
using Xunit;

public partial class Binding_Tcp_NetTcpBindingTests : ConditionalWcfTest
{
    // Simple echo of a string using NetTcpBinding on both client and server with all default settings.
    // Default settings are:
    //                         - SecurityMode = Transport
    //                         - ClientCredentialType = Windows
    [ConditionalFact(nameof(Root_Certificate_Installed), nameof(Windows_Authentication_Available))]
#if !FEATURE_NETNATIVE
    [ActiveIssue(945, PlatformID.AnyUnix)] // NegotiateStream works on Windows and Linux, but we have not yet automated ambient credential configuration in Linux
#else
    [ActiveIssue(832)] // Windows Stream Security is not supported in NET Native
#endif
    [OuterLoop]
    public static void DefaultSettings_Echo_RoundTrips_String()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding();
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_DefaultBinding_Address));
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

    // Simple echo of a string using NetTcpBinding on both client and server with SecurityMode=Transport
    // By default ClientCredentialType will be 'Windows'
    // SecurityMode is Transport by default with NetTcpBinding, this test explicitly sets it.
    [ConditionalFact(nameof(Root_Certificate_Installed), nameof(Windows_Authentication_Available))]
#if !FEATURE_NETNATIVE
    [ActiveIssue(945, PlatformID.AnyUnix)] // NegotiateStream works on Windows and Linux, but we have not yet automated ambient credential configuration in Linux
#else
    [ActiveIssue(832)] // Windows Stream Security is not supported in NET Native
#endif
    [OuterLoop]
    public static void SecurityModeTransport_Echo_RoundTrips_String()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_DefaultBinding_Address));
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