// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using Infrastructure.Common;
using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using Xunit;

public partial class Tcp_ClientCredentialTypeTests : ConditionalWcfTest
{
    [WcfFact]
    [Issue(1945, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Peer_Certificate_Installed),
               nameof(SSL_Available),
               nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    // Asking for PeerTrust alone should succeed
    // if the certificate is in the TrustedPeople store.  For this test
    // we use a certificate we know is in the TrustedPeople store.
    public static void NetTcp_SecModeTrans_CertValMode_PeerTrust_Succeeds_In_TrustedPeople()
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

            endpointAddress = new EndpointAddress(
                                new Uri(Endpoints.NetTcp_SecModeTrans_ClientCredTypeNone_ServerCertValModePeerTrust_Address));

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.PeerTrust;

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

    [WcfFact]
    [Issue(1945, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Peer_Certificate_Installed),
               nameof(SSL_Available))]
    [OuterLoop]
    // Asking for PeerTrust alone should throw SecurityNegotiationException
    // if the certificate is not in the TrustedPeople store.  For this test
    // we use a valid chain-trusted certificate that we know is not in the
    // TrustedPeople store.
    public static void NetTcp_SecModeTrans_CertValMode_PeerTrust_Fails_Not_In_TrustedPeople()
    {
        EndpointAddress endpointAddress = null;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        CommunicationException communicationException = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

            endpointAddress = new EndpointAddress(new Uri(
                                Endpoints.Tcp_CustomBinding_SslStreamSecurity_Address));

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.PeerTrust;

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
            // objects are in faulted state and will throw, so only use finally style cleanup
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Issue(1945, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(SSL_Available),
               nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    // Asking for PeerOrChainTrust should succeed if the certificate is
    // chain-trusted, even though it is not in the TrustedPeople store.
    // So we ask for a known chain-trusted certificate that we also know
    // it not in TrustedPeople.
    public static void NetTcp_SecModeTrans_CertValMode_PeerOrChainTrust_Succeeds_ChainTrusted()
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

            endpointAddress = new EndpointAddress(new Uri(
                Endpoints.Tcp_CustomBinding_SslStreamSecurity_Address));

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.PeerOrChainTrust;

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

    [WcfFact]
    [Issue(3572, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(SSL_Available))]
    [OuterLoop]
    // Asking for ChainTrust only should succeed if the certificate is
    // chain-trusted.
    public static void NetTcp_SecModeTrans_CertValMode_ChainTrust_Succeeds_ChainTrusted()
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

            endpointAddress = new EndpointAddress(new Uri(
                Endpoints.Tcp_CustomBinding_SslStreamSecurity_Address));

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
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

    [WcfFact]
    [Issue(1945, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(SSL_Available),
               nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    // Asking for ChainTrust only should succeed if the certificate is
    // chain-trusted.
    public static void NetTcp_SecModeTrans_Duplex_Callback_Succeeds()
    {
        string clientCertThumb = null;
        EndpointAddress endpointAddress = null;
        DuplexChannelFactory<IWcfDuplexService> factory = null;
        IWcfDuplexService serviceProxy = null;
        Guid guid = Guid.NewGuid();

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            endpointAddress = new EndpointAddress(new Uri(
                Endpoints.Tcp_Certificate_Duplex_Address));
            clientCertThumb = ServiceUtilHelper.ClientCertificate.Thumbprint;

            WcfDuplexServiceCallback callbackService = new WcfDuplexServiceCallback();
            InstanceContext context = new InstanceContext(callbackService);

            factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, endpointAddress);
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            factory.Credentials.ClientCertificate.SetCertificate(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindByThumbprint,
                clientCertThumb);
                
            serviceProxy = factory.CreateChannel();
            
            // *** EXECUTE *** \\
            // Ping on another thread.
            Task.Run(() => serviceProxy.Ping(guid));
            Guid returnedGuid = callbackService.CallbackGuid;

            // *** VALIDATE *** \\
            Assert.True(guid == returnedGuid,
                string.Format("The sent GUID does not match the returned GUID. Sent '{0}', Received: '{1}'", guid, returnedGuid));

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            ((ICommunicationObject)factory).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}
