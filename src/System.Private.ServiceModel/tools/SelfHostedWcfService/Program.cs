// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using WcfService;

namespace SelfHostedWCFService
{
    public class SelfHostedWCFService
    {
        private static int s_httpPort = 8081;
        private static int s_httpsPort = 44285;
        private static int s_tcpPort = 809;
        private static int s_websocketPort = 8083;
        private static int s_websocketsPort = 8084;

        static private void GetPorts()
        {
            s_httpPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("httpPort")) ? s_httpPort : int.Parse(Environment.GetEnvironmentVariable("httpPort"));
            s_httpsPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("httpsPort")) ? s_httpsPort : int.Parse(Environment.GetEnvironmentVariable("httpsPort"));
            s_tcpPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("tcpPort")) ? s_tcpPort : int.Parse(Environment.GetEnvironmentVariable("tcpPort"));
            s_websocketPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("websocketPort")) ? s_websocketPort : int.Parse(Environment.GetEnvironmentVariable("websocketPort"));
            s_websocketsPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("websocketsPort")) ? s_websocketsPort : int.Parse(Environment.GetEnvironmentVariable("websocketsPort"));
        }

        private static void Main()
        {
            GetPorts();

            string httpBaseAddress = string.Format(@"http://localhost:{0}", s_httpPort);
            string httpsBaseAddress = string.Format(@"https://localhost:{0}", s_httpsPort);
            string tcpBaseAddress = string.Format(@"net.tcp://localhost:{0}", s_tcpPort);
            string websocketBaseAddress = string.Format(@"http://localhost:{0}", s_websocketPort);
            string websocketsBaseAddress = string.Format(@"https://localhost:{0}", s_websocketsPort);

            Console.WriteLine("Starting all service hosts...");

            CreateHost<BasicAuthTestServiceHost, WcfUserNameService>("BasicAuth.svc", httpsBaseAddress);
            CreateHost<BasicHttpsTestServiceHost, WcfService.WcfService>("BasicHttps.svc", httpsBaseAddress);
            CreateHost<BasicHttpTestServiceHost, WcfService.WcfService>("BasicHttp.svc", httpBaseAddress);
            CreateHost<BasicHttpTestServiceHost_4_4_0, WcfService_4_4_0>("BasicHttp_4_4_0.svc", httpBaseAddress);
            CreateHost<CustomTextEncoderBufferedTestServiceHost,WcfService.WcfService>("CustomTextEncoderBuffered.svc", httpBaseAddress);
            CreateHost<CustomTextEncoderStreamedTestServiceHost, WcfService.WcfService>("CustomTextEncoderStreamed.svc", httpBaseAddress);
            CreateHost<DefaultCustomHttpTestServiceHost, WcfService.WcfService>("DefaultCustomHttp.svc", httpBaseAddress);
            CreateHost<DuplexTestServiceHost,WcfDuplexService>("Duplex.svc", tcpBaseAddress);
            CreateHost<DuplexCallbackTestServiceHost, DuplexCallbackService>("DuplexCallback.svc", tcpBaseAddress);
            CreateHost<DuplexChannelCallbackReturnTestServiceHost, DuplexChannelCallbackReturnService>("DuplexChannelCallbackReturn.svc", tcpBaseAddress);
            CreateHost<DuplexCallbackDataContractComplexTypeTestServiceHost, WcfDuplexService>("DuplexCallbackDataContractComplexType.svc", tcpBaseAddress);
            CreateHost<DuplexCallbackXmlComplexTypeTestServiceHost, WcfDuplexService>("DuplexCallbackXmlComplexType.svc", tcpBaseAddress);
            CreateHost<DuplexCallbackTcpCertificateCredentialTestServiceHost, WcfDuplexService>("DuplexCallbackTcpCertificateCredential.svc", tcpBaseAddress);
            CreateHost<HttpBinaryTestServiceHost, WcfService.WcfService>("HttpBinary.svc", httpBaseAddress);
            CreateHost<HttpDigestNoDomainTestServiceHost, WcfService.WcfService>("HttpDigestNoDomain.svc", httpBaseAddress);
            CreateHost<HttpsClientCertificateTestServiceHost, WcfService.WcfService>("ClientCertificateAccepted/HttpsClientCertificate.svc", httpsBaseAddress);
            CreateHost<HttpsDigestTestServiceHost, WcfService.WcfService>("DigestAuthentication/HttpsDigest.svc", httpsBaseAddress);
            CreateHost<HttpsNtlmTestServiceHost, WcfService.WcfService>("WindowAuthenticationNtlm/HttpsNtlm.svc", httpsBaseAddress);
            CreateHost<HttpSoap11TestServiceHost, WcfService.WcfService>("HttpSoap11.svc", httpBaseAddress);
            CreateHost<HttpsSoap11TestServiceHost, WcfService.WcfService>("HttpsSoap11.svc", httpsBaseAddress);
            CreateHost<HttpsSoap12TestServiceHost, WcfService.WcfService>("HttpsSoap12.svc", httpsBaseAddress);
            CreateHost<HttpSoap12TestServiceHost, WcfService.WcfService>("HttpSoap12.svc", httpBaseAddress);
            CreateHost<HttpsWindowsTestServiceHost, WcfService.WcfService>("WindowAuthenticationNegotiate/HttpsWindows.svc", httpsBaseAddress);
            CreateHost<HttpWindowsTestServiceHost, WcfService.WcfService>("WindowAuthenticationNegotiate/HttpWindows.svc", httpBaseAddress);
            CreateHost<NetHttpTestServiceHost, WcfService.WcfService>("NetHttp.svc", httpBaseAddress);
            CreateHost<NetHttpTestServiceHostUsingWebSockets, WcfService.WcfService>("NetHttpWebSockets.svc", httpBaseAddress);
            CreateHost<NetHttpsTestServiceHost, WcfService.WcfService>("NetHttps.svc", httpsBaseAddress);
            CreateHost<NetHttpsTestServiceHostUsingWebSockets, WcfService.WcfService>("NetHttpsWebSockets.svc", httpsBaseAddress);
            CreateHost<HttpsCertificateValidationPeerTrustTestServiceHost, WcfService.WcfService>("HttpsCertValModePeerTrust.svc", httpsBaseAddress);
            CreateHost<HttpsCertificateValidationChainTrustTestServiceHost, WcfService.WcfService>("HttpsCertValModeChainTrust.svc", httpsBaseAddress);
            CreateHost<ServiceContractAsyncIntOutTestServiceHost, ServiceContractIntOutService>("ServiceContractAsyncIntOut.svc", httpBaseAddress);
            CreateHost<ServiceContractAsyncUniqueTypeOutTestServiceHost, ServiceContractUniqueTypeOutService>("ServiceContractAsyncUniqueTypeOut.svc", httpBaseAddress);
            CreateHost<ServiceContractAsyncIntRefTestServiceHost, ServiceContractIntRefService>("ServiceContractAsyncIntRef.svc", httpBaseAddress);
            CreateHost<ServiceContractAsyncUniqueTypeRefTestServiceHost, ServiceContractUniqueTypeRefService>("ServiceContractAsyncUniqueTypeRef.svc", httpBaseAddress);
            CreateHost<ServiceContractSyncUniqueTypeOutTestServiceHost, ServiceContractUniqueTypeOutSyncService>("ServiceContractSyncUniqueTypeOut.svc", httpBaseAddress);
            CreateHost<ServiceContractSyncUniqueTypeRefTestServiceHost, ServiceContractUniqueTypeRefSyncService>("ServiceContractSyncUniqueTypeRef.svc", httpBaseAddress);
            CreateHost<TcpCertificateWithServerAltNameTestServiceHost, WcfService.WcfService>("TcpCertificateWithServerAltName.svc", tcpBaseAddress);
            CreateHost<TcpCertificateWithSubjectCanonicalNameDomainNameTestServiceHost, WcfService.WcfService>("TcpCertificateWithSubjectCanonicalNameDomainName.svc", tcpBaseAddress);
            CreateHost<TcpCertificateWithSubjectCanonicalNameFqdnTestServiceHost, WcfService.WcfService>("TcpCertificateWithSubjectCanonicalNameFqdn.svc", tcpBaseAddress);
            CreateHost<TcpCertificateWithSubjectCanonicalNameLocalhostTestServiceHost, WcfService.WcfService>("TcpCertificateWithSubjectCanonicalNameLocalhost.svc", tcpBaseAddress);
            CreateHost<TcpExpiredServerCertTestServiceHost, WcfService.WcfService>("TcpExpiredServerCert.svc", tcpBaseAddress);
            CreateHost<TcpNoSecurityTestServiceHost, WcfService.WcfService>("TcpNoSecurity.svc", tcpBaseAddress);
            CreateHost<TcpDefaultResourceTestServiceHost, WcfService.WcfService>("TcpDefault.svc", tcpBaseAddress);
            CreateHost<TcpNoSecurityTextTestServiceHost, WcfService.WcfService>("TcpNoSecurityText.svc", tcpBaseAddress);
            CreateHost<TcpRevokedServerCertTestServiceHost, WcfService.WcfService>("TcpRevokedServerCert.svc", tcpBaseAddress);
            CreateHost<TcpStreamedNoSecurityTestServiceHost, WcfService.WcfService>("TcpStreamedNoSecurity.svc", tcpBaseAddress);
            CreateHost<TcpTransportSecuritySslCustomCertValidationTestServiceHost, WcfService.WcfService>("TcpTransportSecuritySslCustomCertValidation.svc", tcpBaseAddress);
            CreateHost<TcpTransportSecurityStreamedTestServiceHost, WcfService.WcfService>("WindowAuthenticationNegotiate/TcpTransportSecurityStreamed.svc", tcpBaseAddress);
            CreateHost<TcpTransportSecurityWithSslTestServiceHost, WcfService.WcfService>("TcpTransportSecurityWithSsl.svc", tcpBaseAddress);
            CreateHost<TcpTransportSecuritySslClientCredentialTypeCertificateTestServiceHost, WcfService.WcfService>("TcpTransportSecuritySslClientCredentialTypeCertificate.svc", tcpBaseAddress);
            CreateHost<TcpVerifyDNSTestServiceHost, WcfService.WcfService>("TcpVerifyDNS.svc", tcpBaseAddress);
            CreateHost<TcpCertificateValidationPeerTrustTestServiceHost, WcfService.WcfService>("NetTcpCertValModePeerTrust.svc", tcpBaseAddress);
            CreateHost<DuplexWebSocketTestServiceHost, WcfWebSocketService>("DuplexWebSocket.svc", websocketBaseAddress);
            CreateHost<WebSocketTransportTestServiceHost, WcfWebSocketTransportUsageAlwaysService>("WebSocketTransport.svc", websocketBaseAddress);
            CreateHost<WebSocketHttpDuplexBinaryStreamedTestServiceHost, WSDuplexService>("WebSocketHttpDuplexBinaryStreamed.svc", websocketBaseAddress);
            CreateHost<WebSocketHttpRequestReplyBinaryStreamedTestServiceHost, WSRequestReplyService>("WebSocketHttpRequestReplyBinaryStreamed.svc", websocketBaseAddress);
            CreateHost<WebSocketHttpsDuplexBinaryStreamedTestServiceHost, WSDuplexService>("WebSocketHttpsDuplexBinaryStreamed.svc", websocketsBaseAddress);
            CreateHost<WebSocketHttpsDuplexTextStreamedTestServiceHost, WSDuplexService>("WebSocketHttpsDuplexTextStreamed.svc", websocketsBaseAddress);
            CreateHost<WebSocketHttpRequestReplyTextStreamedTestServiceHost, WSRequestReplyService>("WebSocketHttpRequestReplyTextStreamed.svc", websocketBaseAddress);
            CreateHost<WebSocketHttpDuplexTextStreamedTestServiceHost, WSDuplexService>("WebSocketHttpDuplexTextStreamed.svc", websocketBaseAddress);
            CreateHost<WebSocketHttpRequestReplyTextBufferedTestServiceHost, WSRequestReplyService>("WebSocketHttpRequestReplyTextBuffered.svc", websocketBaseAddress);
            CreateHost<WebSocketHttpRequestReplyBinaryBufferedTestServiceHost, WSRequestReplyService>("WebSocketHttpRequestReplyBinaryBuffered.svc", websocketBaseAddress);
            CreateHost<WebSocketHttpDuplexTextBufferedTestServiceHost, WSDuplexService>("WebSocketHttpDuplexTextBuffered.svc", websocketBaseAddress);
            CreateHost<WebSocketHttpDuplexBinaryBufferedTestServiceHost, WSDuplexService>("WebSocketHttpDuplexBinaryBuffered.svc", websocketBaseAddress);
            CreateHost<WebSocketHttpsRequestReplyBinaryBufferedTestServiceHost, WSRequestReplyService>("WebSocketHttpsRequestReplyBinaryBuffered.svc", websocketsBaseAddress);
            CreateHost<WebSocketHttpsRequestReplyTextBufferedTestServiceHost, WSRequestReplyService>("WebSocketHttpsRequestReplyTextBuffered.svc", websocketsBaseAddress);
            CreateHost<WebSocketHttpsDuplexBinaryBufferedTestServiceHost, WSDuplexService>("WebSocketHttpsDuplexBinaryBuffered.svc", websocketsBaseAddress);
            CreateHost<WebSocketHttpsDuplexTextBufferedTestServiceHost, WSDuplexService>("WebSocketHttpsDuplexTextBuffered.svc", websocketsBaseAddress);
            CreateHost<ChannelExtensibilityServiceHost, WcfChannelExtensiblityService>("ChannelExtensibility.svc", httpBaseAddress);
            CreateHost<WebSocketHttpVerifyWebSocketsUsedTestServiceHost, VerifyWebSockets>("WebSocketHttpVerifyWebSocketsUsed.svc", websocketBaseAddress);
            CreateHost<DataContractResolverTestServiceHost, WcfService.DataContractResolverService>("DataContractResolver.svc", httpBaseAddress);

            //Start the crlUrl service last as the client use it to ensure all services have been started
            Uri testHostUrl = new Uri(string.Format("http://localhost/TestHost.svc", s_httpPort));
            WebServiceHost host = new WebServiceHost(typeof(TestHost), testHostUrl);
            WebHttpBinding binding = new WebHttpBinding();
            host.AddServiceEndpoint(typeof(ITestHost), binding, "");
            ServiceDebugBehavior serviceDebugBehavior = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            serviceDebugBehavior.HttpHelpPageEnabled = false;
            host.Open();

            Console.WriteLine("All service hosts have started.");
            do
            {
                Console.WriteLine("Type <Exit> to terminate the self service Host.");
                string input = Console.ReadLine();
                if (string.Compare(input, "exit", true) == 0)
                {
                    return;
                }
            } while (true);
        }

        // Instantiates and opens a ServiceHost of type 'THost' that exposes a service of type 'TService'
        // The endpoint address is formed by combining 'baseAddress' with 'serviceAddress'.
        private static THost CreateHost<THost, TService>(string serviceAddress, string baseAddress) where THost : ServiceHost
        {
            string address = String.Format("{0}/{1}", baseAddress, serviceAddress);
            THost host = (THost)Activator.CreateInstance(typeof(THost), new object[] { typeof(TService), new Uri[] { new Uri(address) } });
            Console.WriteLine("  {0} at {1}", typeof(THost).Name, address);
            host.Open();
            return host;
        }
    }
}
