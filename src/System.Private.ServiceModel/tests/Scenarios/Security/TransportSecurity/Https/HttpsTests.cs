// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Security;
using Infrastructure.Common;
using Xunit;

public partial class HttpsTests : ConditionalWcfTest
{
#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(959)] // Server certificate validation not supported in NET Native
#endif
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Peer_Certificate_Installed),
               nameof(SSL_Available))]
    [Issue(1381)] // requires additional SSL configuration of new port to work
    [Issue(959, Framework = FrameworkID.NetNative)] // Server certificate validation not supported in NET Native
    [OuterLoop]

    // Asking for PeerTrust alone should succeed
    // if the certificate is in the TrustedPeople store.  For this test
    // we use a certificate we know is in the TrustedPeople store.
    public static void Https_SecModeTrans_CertValMode_PeerTrust_Succeeds_In_TrustedPeople()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool root_Certificate_Installed = Root_Certificate_Installed();
        bool client_Certificate_Installed = Client_Certificate_Installed();
        bool peer_Certificate_Installed = Peer_Certificate_Installed();
        bool ssl_Available = SSL_Available();

        if (!root_Certificate_Installed || 
            !client_Certificate_Installed ||
            !peer_Certificate_Installed ||
            !ssl_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            Console.WriteLine("Client_Certificate_Installed evaluated as {0}", client_Certificate_Installed);
            Console.WriteLine("Peer_Certificate_Installed evaluated as {0}", peer_Certificate_Installed);
            Console.WriteLine("SSL_Available evaluated as {0}", ssl_Available);
            return;
        }
#endif
        EndpointAddress endpointAddress = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpsBinding binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;

            endpointAddress = new EndpointAddress(
                                new Uri(Endpoints.Https_SecModeTrans_ClientCredTypeNone_ServerCertValModePeerTrust_Address));

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ServiceCertificate.SslCertificateAuthentication = new X509ServiceCertificateAuthentication();
            factory.Credentials.ServiceCertificate.SslCertificateAuthentication.CertificateValidationMode = X509CertificateValidationMode.PeerTrust;

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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(959)] // Server certificate validation not supported in NET Native
#endif
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Peer_Certificate_Installed),
               nameof(SSL_Available))]
    [Issue(1295, OS = OSID.AnyUnix)] // A libcurl built with OpenSSL is required.
    [Issue(959, Framework = FrameworkID.NetNative)] // Server certificate validation not supported in NET Native
    [OuterLoop]
    // Asking for PeerTrust alone should throw SecurityNegotiationException
    // if the certificate is not in the TrustedPeople store.  For this test
    // we use a valid chain-trusted certificate that we know is not in the
    // TrustedPeople store.

    public static void Https_SecModeTrans_CertValMode_PeerTrust_Fails_Not_In_TrustedPeople()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool root_Certificate_Installed = Root_Certificate_Installed();
        bool client_Certificate_Installed = Client_Certificate_Installed();
        bool peer_Certificate_Installed = Peer_Certificate_Installed();
        bool ssl_Available = SSL_Available();

        if (!root_Certificate_Installed || 
            !client_Certificate_Installed ||
            !peer_Certificate_Installed ||
            !ssl_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            Console.WriteLine("Client_Certificate_Installed evaluated as {0}", client_Certificate_Installed);
            Console.WriteLine("Peer_Certificate_Installed evaluated as {0}", peer_Certificate_Installed);
            Console.WriteLine("SSL_Available evaluated as {0}", ssl_Available);
            return;
        }
#endif
        EndpointAddress endpointAddress = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        CommunicationException communicationException = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpsBinding binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            endpointAddress = new EndpointAddress(new Uri(
                                Endpoints.Https_SecModeTrans_ClientCredTypeNone_ServerCertValModeChainTrust_Address));

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ServiceCertificate.SslCertificateAuthentication = new X509ServiceCertificateAuthentication();
            factory.Credentials.ServiceCertificate.SslCertificateAuthentication.CertificateValidationMode = X509CertificateValidationMode.PeerTrust;

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            try
            {
                serviceProxy.Echo(testString);
            }
            catch (CommunicationException ce)
            {
                communicationException = ce;
            }

            // *** VALIDATE *** \\
            Assert.True(communicationException != null, "Expected CommunicationException but no exception was thrown.");
            Assert.True(communicationException.GetType().Name == "SecurityNegotiationException",
                        String.Format("Expected SecurityNegotiationException but received {0}",
                                      communicationException.ToString()));

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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(959)] // Server certificate validation not supported in NET Native
#endif
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(SSL_Available))]
    [Issue(959, Framework = FrameworkID.NetNative)] // Server certificate validation not supported in NET Native
    [Issue(1295, OS = OSID.AnyCentOS | OSID.AnyFedora | OSID.AnyOpenSUSE | OSID.AnyOSX | OSID.AnyRHEL)] // A libcurl built with OpenSSL is required.
    [OuterLoop]
    // Asking for PeerOrChainTrust should succeed if the certificate is
    // chain-trusted, even though it is not in the TrustedPeople store.
    // So we ask for a known chain-trusted certificate that we also know
    // it not in TrustedPeople.
    public static void Https_SecModeTrans_CertValMode_PeerOrChainTrust_Succeeds_ChainTrusted()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool root_Certificate_Installed = Root_Certificate_Installed();
        bool client_Certificate_Installed = Client_Certificate_Installed();
        bool ssl_Available = SSL_Available();

        if (!root_Certificate_Installed || 
            !client_Certificate_Installed ||
            !ssl_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            Console.WriteLine("Client_Certificate_Installed evaluated as {0}", client_Certificate_Installed);
            Console.WriteLine("SSL_Available evaluated as {0}", ssl_Available);
            return;
        }
#endif
        EndpointAddress endpointAddress = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpsBinding binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            endpointAddress = new EndpointAddress(
                                new Uri(Endpoints.Https_SecModeTrans_ClientCredTypeNone_ServerCertValModeChainTrust_Address));

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ServiceCertificate.SslCertificateAuthentication = new X509ServiceCertificateAuthentication();
            factory.Credentials.ServiceCertificate.SslCertificateAuthentication.CertificateValidationMode = X509CertificateValidationMode.PeerOrChainTrust;

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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(959)] // Server certificate validation not supported in NET Native
#endif
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(SSL_Available))]
    [Issue(959, Framework = FrameworkID.NetNative)] // Server certificate validation not supported in NET Native
    [Issue(1295, OS = OSID.AnyCentOS | OSID.AnyFedora | OSID.AnyOpenSUSE | OSID.AnyOSX | OSID.AnyRHEL)] // A libcurl built with OpenSSL is required.
    [OuterLoop]
    // Asking for ChainTrust should succeed if the certificate is
    // chain-trusted.
    public static void Https_SecModeTrans_CertValMode_ChainTrust_Succeeds_ChainTrusted()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool root_Certificate_Installed = Root_Certificate_Installed();
        bool client_Certificate_Installed = Client_Certificate_Installed();
        bool ssl_Available = SSL_Available();

        if (!root_Certificate_Installed || 
            !client_Certificate_Installed ||
            !ssl_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            Console.WriteLine("Client_Certificate_Installed evaluated as {0}", client_Certificate_Installed);
            Console.WriteLine("SSL_Available evaluated as {0}", ssl_Available);
            return;
        }
#endif
        EndpointAddress endpointAddress = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpsBinding binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            endpointAddress = new EndpointAddress(new Uri(Endpoints.Https_SecModeTrans_ClientCredTypeNone_ServerCertValModeChainTrust_Address));

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ServiceCertificate.SslCertificateAuthentication = new X509ServiceCertificateAuthentication();
            factory.Credentials.ServiceCertificate.SslCertificateAuthentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;

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
