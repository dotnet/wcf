using System;
using System.ServiceModel;
using System.Text;
using Xunit;

namespace Security.TransportSecurity.Tests.Tcp
{
    public static class IdentityTests
    {
        [Fact]
        [ActiveIssue(12)]
        [OuterLoop]
        // The product code will check the Dns identity from the server and throw if it does not match what is specified in DnsEndpointIdentity
        public static void VerifyServiceIdentityMatchDnsEndpointIdentity()
        {
            string testString = "Hello";

            NetTcpBinding binding = new NetTcpBinding();
            //SecurityMode.Transport is not supported yet, we will get an exception here, tracked by issue #81
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_VerifyDNS_Address),new DnsEndpointIdentity("localhost"));
            ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            // factory.Credentials.ServiceCertificate is not availabe currently, tracked by issue 243
            // We need to change the validation mode as we use a test certificate. It does not affect the purpose of this test
            // factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
            IWcfService serviceProxy = factory.CreateChannel();

            try
            {
                var result = serviceProxy.Echo(testString);
                Assert.Equal(testString, result);
            }
            finally
            {
                if (factory != null && factory.State != CommunicationState.Closed)
                {factory.Abort();
                }
            }
        }
    }
}
