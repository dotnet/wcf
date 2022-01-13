// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using Infrastructure.Common;
using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Xunit;

public partial class CustomBindingTests : ConditionalWcfTest
{
    // Tcp: Client and Server bindings setup exactly the same using default settings.
    [WcfFact]
    [Issue(3572, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed), nameof(Client_Certificate_Installed))]
    [OuterLoop]
    public static void DefaultSettings_Tcp_Binary_Echo_RoundTrips_String()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding binding = new CustomBinding(
                new SslStreamSecurityBindingElement(),
                new BinaryMessageEncodingBindingElement(),
                new TcpTransportBindingElement());

            var endpointIdentity = new DnsEndpointIdentity(Endpoints.Tcp_CustomBinding_SslStreamSecurity_HostName);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(new Uri(Endpoints.Tcp_CustomBinding_SslStreamSecurity_Address), endpointIdentity));
            factory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
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

    // Https: Client and Server bindings setup exactly the same using default settings.
    [WcfFact]
    [Issue(3572, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(SSL_Available))]
    [OuterLoop]
    public static void DefaultSettings_Https_Text_Echo_RoundTrips_String()
    {
        string testString = "Hello";
        CustomBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            binding = new CustomBinding(new TextMessageEncodingBindingElement(), new HttpsTransportBindingElement());
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpsSoap12_Address));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.NotNull(result);
            Assert.Equal(testString, result);

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}
