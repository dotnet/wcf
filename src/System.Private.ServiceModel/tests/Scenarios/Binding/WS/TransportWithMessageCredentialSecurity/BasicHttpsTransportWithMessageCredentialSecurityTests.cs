using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public class BasicHttpsTransportWithMessageCredentialSecurityTests : ConditionalWcfTest
{
    [WcfFact]
    [Issue(2870, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(SSL_Available))]
    [OuterLoop]
    public static void Https_SecModeTransWithMessCred_CertClientCredential_Succeeds()
    {
        string clientCertThumb;
        EndpointAddress endpointAddress;
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpsBinding binding = new BasicHttpsBinding(BasicHttpsSecurityMode.TransportWithMessageCredential);
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.Certificate;
            endpointAddress = new EndpointAddress(new Uri(Endpoints.BasicHttpsBinding_SecModeTransWithMessCred_ClientCredTypeCert));
            clientCertThumb = ServiceUtilHelper.ClientCertificate.Thumbprint;

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
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
}
