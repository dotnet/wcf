// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using Infrastructure.Common;
using Xunit;

public class Tcp_ClientCredentialTypeTests : ConditionalWcfTest
{
    // Simple echo of a string using NetTcpBinding on both client and server with all default settings.
    // Default settings are:
    //                         - SecurityMode = Transport
    //                         - ClientCredentialType = Windows
    [Fact]
#if !FEATURE_NETNATIVE
    [ActiveIssue(592, PlatformID.AnyUnix)] // NegotiateStream works on Windows but is not yet supported on Unix
#else
    [ActiveIssue(832)] // Windows Stream Security is not supported in NET Native
#endif
    [OuterLoop]
    public static void SameBinding_DefaultSettings_EchoString()
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

    // Simple echo of a string using NetTcpBinding on both client and server with SecurityMode=None
    [Fact]
    [OuterLoop]
    [ActiveIssue(951, PlatformID.OSX)]
    public static void SameBinding_SecurityModeNone_EchoString()
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

    // Simple echo of a string using NetTcpBinding on both client and server with SecurityMode=Transport
    // By default ClientCredentialType will be 'Windows'
    [Fact]
#if !FEATURE_NETNATIVE
    [ActiveIssue(592, PlatformID.AnyUnix)] // NegotiateStream works on Windows but is not yet supported on Unix
#else
    [ActiveIssue(832)] // Windows Stream Security is not supported in NET Native
#endif
    [OuterLoop]
    public static void SameBinding_SecurityModeTransport_EchoString()
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

    // Simple echo of a string using a CustomBinding to mimic a NetTcpBinding with Security.Mode = TransportWithMessageCredentials
    // This does not exactly match the binding elements in a NetTcpBinding which also includes a TransportSecurityBindingElement
    [ConditionalFact(nameof(Root_Certificate_Installed), nameof(Client_Certificate_Installed))]
#if FEATURE_NETNATIVE
    [ActiveIssue(833)] // Not supported in NET Native
#endif
    [OuterLoop]
    public static void SameBinding_SecurityModeTransport_ClientCredentialTypeCertificate_EchoString()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding binding = new CustomBinding(
                new SslStreamSecurityBindingElement(), // This is the binding element used when Security.Mode  = TransportWithMessageCredentials
                new BinaryMessageEncodingBindingElement(),
                new TcpTransportBindingElement());

            var endpointIdentity = new DnsEndpointIdentity(Endpoints.Tcp_CustomBinding_SslStreamSecurity_HostName);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(new Uri(Endpoints.Tcp_CustomBinding_SslStreamSecurity_Address), endpointIdentity));
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

    [ConditionalFact(nameof(Root_Certificate_Installed), nameof(Client_Certificate_Installed))]
#if FEATURE_NETNATIVE
    [ActiveIssue(833)] // Not supported in NET Native
#else
    [ActiveIssue(951, PlatformID.OSX)]
#endif
    [OuterLoop]
    public static void TcpClientCredentialType_Certificate_EchoString()
    {
        string clientCertThumb = null;
        EndpointAddress endpointAddress = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_ClientCredentialType_Certificate_Address),
                new DnsEndpointIdentity(Endpoints.Tcp_VerifyDNS_HostName));
            clientCertThumb = BridgeClientCertificateManager.LocalCertThumbprint; // ClientCert as given by the Bridge

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
            factory.Credentials.ClientCertificate.SetCertificate(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindByThumbprint,
                clientCertThumb);

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

    [ConditionalFact(nameof(Root_Certificate_Installed), nameof(Client_Certificate_Installed))]
#if FEATURE_NETNATIVE
    [ActiveIssue(833)] // Not supported in NET Native
#else
    [ActiveIssue(951, PlatformID.OSX)]
#endif
    [OuterLoop]
    public static void TcpClientCredentialType_Certificate_CustomValidator_EchoString()
    {
        string clientCertThumb = null;
        EndpointAddress endpointAddress = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_ClientCredentialType_Certificate_CustomValidation_Address),
                new DnsEndpointIdentity(Endpoints.Tcp_VerifyDNS_HostName));
            clientCertThumb = BridgeClientCertificateManager.LocalCertThumbprint; // ClientCert as given by the Bridge

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;

            MyX509CertificateValidator myX509CertificateValidator = new MyX509CertificateValidator(ScenarioTestHelpers.CertificateIssuerName);
            factory.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = myX509CertificateValidator;
            factory.Credentials.ClientCertificate.SetCertificate(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindByThumbprint,
                clientCertThumb);

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.True(myX509CertificateValidator.validateMethodWasCalled, "The Validate method of the X509CertificateValidator was NOT called.");
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

    [ConditionalFact(nameof(Root_Certificate_Installed), nameof(Client_Certificate_Installed))]
#if FEATURE_NETNATIVE
    [ActiveIssue(833)] // Not supported in NET Native
#endif
    [OuterLoop]
    public static void TcpClientCredentialType_Certificate_With_ServerAltName_EchoString()
    {
        EndpointAddress endpointAddress = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

            endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_ClientCredentialType_Certificate_With_ServerAltName_Address));

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;

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
