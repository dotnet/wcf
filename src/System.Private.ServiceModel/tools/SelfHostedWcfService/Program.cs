using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using WcfService;

namespace SelfHostedWCFService
{
    public class SelfHostedWCFService
    {
        static int httpPort = 8081;
        static int httpsPort = 44285;
        static int tcpPort = 809;
        static int websocketPort = 8083;
        static int websocketsPort = 8084;

        static private void GetPorts()
        {
            httpPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("httpPort")) ? httpPort : int.Parse(Environment.GetEnvironmentVariable("httpPort"));
            httpsPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("httpsPort")) ? httpsPort : int.Parse(Environment.GetEnvironmentVariable("httpsPort"));
            tcpPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("tcpPort")) ? tcpPort : int.Parse(Environment.GetEnvironmentVariable("tcpPort"));
            websocketPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("websocketPort")) ? websocketPort : int.Parse(Environment.GetEnvironmentVariable("websocketPort"));
            websocketsPort = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("websocketsPort")) ? websocketsPort : int.Parse(Environment.GetEnvironmentVariable("websocketsPort"));
        }

        static void Main()
        {
            GetPorts();

            string httpBaseAddress = string.Format(@"http://localhost:{0}", httpPort);
            string httpsBaseAddress = string.Format(@"https://localhost:{0}", httpsPort);
            string tcpBaseAddress = string.Format(@"net.tcp://localhost:{0}", tcpPort);
            string websocketBaseAddress = string.Format(@"http://localhost:{0}", websocketPort);
            string websocketsBaseAddress = string.Format(@"https://localhost:{0}", websocketsPort);

            //Do not need to catch exceptions and dispose service hosts as the process will terminate
            Uri crlUrl = new Uri(string.Format("http://localhost/CrlService.svc", httpPort));
            WebServiceHost host = new WebServiceHost(typeof(CrlService), crlUrl);
            WebHttpBinding binding = new WebHttpBinding();
            host.AddServiceEndpoint(typeof(ICrlService), binding, "");
            ServiceDebugBehavior stp = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            stp.HttpHelpPageEnabled = false;
            host.Open();

            Uri[] utilTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/Util.svc", httpBaseAddress))};
            UtilTestServiceHost utilTestServiceHostServiceHost = new UtilTestServiceHost(typeof(WcfService.Util), utilTestServiceHostbaseAddress);
            utilTestServiceHostServiceHost.Open();

            Uri[] basicAuthTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/BasicAuth.svc", httpsBaseAddress))};
            BasicAuthTestServiceHost basicAuthTestServiceHostServiceHost = new BasicAuthTestServiceHost(typeof(WcfService.WcfUserNameService), basicAuthTestServiceHostbaseAddress);
            basicAuthTestServiceHostServiceHost.Open();

            Uri[] basicHttpsTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/BasicHttps.svc", httpsBaseAddress))};
            BasicHttpsTestServiceHost basicHttpsTestServiceHostServiceHost = new BasicHttpsTestServiceHost(typeof(WcfService.WcfService), basicHttpsTestServiceHostbaseAddress);
            basicHttpsTestServiceHostServiceHost.Open();

            Uri[] basicHttpTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/BasicHttp.svc", httpBaseAddress))};
            BasicHttpTestServiceHost basicHttpTestServiceHostServiceHost = new BasicHttpTestServiceHost(typeof(WcfService.WcfService), basicHttpTestServiceHostbaseAddress);
            basicHttpTestServiceHostServiceHost.Open();

            Uri[] defaultCustomHttpTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/DefaultCustomHttp.svc", httpBaseAddress))};
            DefaultCustomHttpTestServiceHost defaultCustomHttpTestServiceHostServiceHost = new DefaultCustomHttpTestServiceHost(typeof(WcfService.WcfService), defaultCustomHttpTestServiceHostbaseAddress);
            defaultCustomHttpTestServiceHostServiceHost.Open();

            Uri[] duplexTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/Duplex.svc", tcpBaseAddress))};
            DuplexTestServiceHost duplexTestServiceHostServiceHost = new DuplexTestServiceHost(typeof(WcfService.WcfDuplexService), duplexTestServiceHostbaseAddress);
            duplexTestServiceHostServiceHost.Open();

            Uri[] duplexCallbackTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/DuplexCallback.svc", tcpBaseAddress))};
            DuplexCallbackTestServiceHost duplexCallbackTestServiceHostServiceHost = new DuplexCallbackTestServiceHost(typeof(WcfService.DuplexCallbackService), duplexCallbackTestServiceHostbaseAddress);
            duplexCallbackTestServiceHostServiceHost.Open();

            Uri[] duplexChannelCallbackReturnTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/DuplexChannelCallbackReturn.svc", tcpBaseAddress))};
            DuplexChannelCallbackReturnTestServiceHost duplexChannelCallbackReturnTestServiceHostServiceHost = new DuplexChannelCallbackReturnTestServiceHost(typeof(WcfService.DuplexChannelCallbackReturnService), duplexChannelCallbackReturnTestServiceHostbaseAddress);
            duplexChannelCallbackReturnTestServiceHostServiceHost.Open();

            Uri[] duplexCallbackDataContractComplexTypeTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/DuplexCallbackDataContractComplexType.svc", tcpBaseAddress))};
            DuplexCallbackDataContractComplexTypeTestServiceHost duplexCallbackDataContractComplexTypeTestServiceHostServiceHost = new DuplexCallbackDataContractComplexTypeTestServiceHost(typeof(WcfService.WcfDuplexService), duplexCallbackDataContractComplexTypeTestServiceHostbaseAddress);
            duplexCallbackDataContractComplexTypeTestServiceHostServiceHost.Open();

            Uri[] duplexCallbackXmlComplexTypeTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/DuplexCallbackXmlComplexType.svc", tcpBaseAddress))};
            DuplexCallbackXmlComplexTypeTestServiceHost duplexCallbackXmlComplexTypeTestServiceHostServiceHost = new DuplexCallbackXmlComplexTypeTestServiceHost(typeof(WcfService.WcfDuplexService), duplexCallbackXmlComplexTypeTestServiceHostbaseAddress);
            duplexCallbackXmlComplexTypeTestServiceHostServiceHost.Open();

            Uri[] httpBinaryTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/HttpBinary.svc", httpBaseAddress))};
            HttpBinaryTestServiceHost httpBinaryTestServiceHostServiceHost = new HttpBinaryTestServiceHost(typeof(WcfService.WcfService), httpBinaryTestServiceHostbaseAddress);
            httpBinaryTestServiceHostServiceHost.Open();

            Uri[] httpDigestNoDomainTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/HttpDigestNoDomain.svc", httpBaseAddress))};
            HttpDigestNoDomainTestServiceHost httpDigestNoDomainTestServiceHostServiceHost = new HttpDigestNoDomainTestServiceHost(typeof(WcfService.WcfService), httpDigestNoDomainTestServiceHostbaseAddress);
            httpDigestNoDomainTestServiceHostServiceHost.Open();

            Uri[] httpsClientCertificateTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/HttpsClientCertificate.svc", httpsBaseAddress))};
            HttpsClientCertificateTestServiceHost httpsClientCertificateTestServiceHostServiceHost = new HttpsClientCertificateTestServiceHost(typeof(WcfService.WcfService), httpsClientCertificateTestServiceHostbaseAddress);
            httpsClientCertificateTestServiceHostServiceHost.Open();

            Uri[] httpsDigestTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/HttpsDigest.svc", httpsBaseAddress))};
            HttpsDigestTestServiceHost httpsDigestTestServiceHostServiceHost = new HttpsDigestTestServiceHost(typeof(WcfService.WcfService), httpsDigestTestServiceHostbaseAddress);
            httpsDigestTestServiceHostServiceHost.Open();

            Uri[] httpsNtlmTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/HttpsNtlm.svc", httpsBaseAddress))};
            HttpsNtlmTestServiceHost httpsNtlmTestServiceHostServiceHost = new HttpsNtlmTestServiceHost(typeof(WcfService.WcfService), httpsNtlmTestServiceHostbaseAddress);
            httpsNtlmTestServiceHostServiceHost.Open();

            Uri[] httpSoap11TestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/HttpSoap11.svc", httpBaseAddress))};
            HttpSoap11TestServiceHost httpSoap11TestServiceHostServiceHost = new HttpSoap11TestServiceHost(typeof(WcfService.WcfService), httpSoap11TestServiceHostbaseAddress);
            httpSoap11TestServiceHostServiceHost.Open();

            Uri[] httpsSoap11TestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/HttpsSoap11.svc", httpsBaseAddress))};
            HttpsSoap11TestServiceHost httpsSoap11TestServiceHostServiceHost = new HttpsSoap11TestServiceHost(typeof(WcfService.WcfService), httpsSoap11TestServiceHostbaseAddress);
            httpsSoap11TestServiceHostServiceHost.Open();

            Uri[] httpsSoap12TestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/HttpsSoap12.svc", httpsBaseAddress))};
            HttpsSoap12TestServiceHost httpsSoap12TestServiceHostServiceHost = new HttpsSoap12TestServiceHost(typeof(WcfService.WcfService), httpsSoap12TestServiceHostbaseAddress);
            httpsSoap12TestServiceHostServiceHost.Open();

            Uri[] httpSoap12TestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/HttpSoap12.svc", httpBaseAddress))};
            HttpSoap12TestServiceHost httpSoap12TestServiceHostServiceHost = new HttpSoap12TestServiceHost(typeof(WcfService.WcfService), httpSoap12TestServiceHostbaseAddress);
            httpSoap12TestServiceHostServiceHost.Open();

            Uri[] httpsWindowsTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/HttpsWindows.svc", httpsBaseAddress))};
            HttpsWindowsTestServiceHost httpsWindowsTestServiceHostServiceHost = new HttpsWindowsTestServiceHost(typeof(WcfService.WcfService), httpsWindowsTestServiceHostbaseAddress);
            httpsWindowsTestServiceHostServiceHost.Open();

            Uri[] httpWindowsTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/HttpWindows.svc", httpBaseAddress))};
            HttpWindowsTestServiceHost httpWindowsTestServiceHostServiceHost = new HttpWindowsTestServiceHost(typeof(WcfService.WcfService), httpWindowsTestServiceHostbaseAddress);
            httpWindowsTestServiceHostServiceHost.Open();

            Uri[] netHttpTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/NetHttp.svc", httpBaseAddress))};
            NetHttpTestServiceHost netHttpTestServiceHostServiceHost = new NetHttpTestServiceHost(typeof(WcfService.WcfService), netHttpTestServiceHostbaseAddress);
            netHttpTestServiceHostServiceHost.Open();

            Uri[] serviceContractAsyncIntOutTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/ServiceContractAsyncIntOut.svc", httpBaseAddress))};
            ServiceContractAsyncIntOutTestServiceHost serviceContractAsyncIntOutTestServiceHostServiceHost = new ServiceContractAsyncIntOutTestServiceHost(typeof(WcfService.ServiceContractIntOutService), serviceContractAsyncIntOutTestServiceHostbaseAddress);
            serviceContractAsyncIntOutTestServiceHostServiceHost.Open();

            Uri[] serviceContractAsyncUniqueTypeOutTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/ServiceContractAsyncUniqueTypeOut.svc", httpBaseAddress))};
            ServiceContractAsyncUniqueTypeOutTestServiceHost serviceContractAsyncUniqueTypeOutTestServiceHostServiceHost = new ServiceContractAsyncUniqueTypeOutTestServiceHost(typeof(WcfService.ServiceContractUniqueTypeOutService), serviceContractAsyncUniqueTypeOutTestServiceHostbaseAddress);
            serviceContractAsyncUniqueTypeOutTestServiceHostServiceHost.Open();

            Uri[] serviceContractAsyncIntRefTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/ServiceContractAsyncIntRef.svc", httpBaseAddress))};
            ServiceContractAsyncIntRefTestServiceHost serviceContractAsyncIntRefTestServiceHostServiceHost = new ServiceContractAsyncIntRefTestServiceHost(typeof(WcfService.ServiceContractIntRefService), serviceContractAsyncIntRefTestServiceHostbaseAddress);
            serviceContractAsyncIntRefTestServiceHostServiceHost.Open();

            Uri[] serviceContractAsyncUniqueTypeRefTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/ServiceContractAsyncUniqueTypeRef.svc", httpBaseAddress))};
            ServiceContractAsyncUniqueTypeRefTestServiceHost serviceContractAsyncUniqueTypeRefTestServiceHostServiceHost = new ServiceContractAsyncUniqueTypeRefTestServiceHost(typeof(WcfService.ServiceContractUniqueTypeRefService), serviceContractAsyncUniqueTypeRefTestServiceHostbaseAddress);
            serviceContractAsyncUniqueTypeRefTestServiceHostServiceHost.Open();

            Uri[] serviceContractSyncUniqueTypeOutTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/ServiceContractSyncUniqueTypeOut.svc", httpBaseAddress))};
            ServiceContractSyncUniqueTypeOutTestServiceHost serviceContractSyncUniqueTypeOutTestServiceHostServiceHost = new ServiceContractSyncUniqueTypeOutTestServiceHost(typeof(WcfService.ServiceContractUniqueTypeOutSyncService), serviceContractSyncUniqueTypeOutTestServiceHostbaseAddress);
            serviceContractSyncUniqueTypeOutTestServiceHostServiceHost.Open();

            Uri[] serviceContractSyncUniqueTypeRefTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/ServiceContractSyncUniqueTypeRef.svc", httpBaseAddress))};
            ServiceContractSyncUniqueTypeRefTestServiceHost serviceContractSyncUniqueTypeRefTestServiceHostServiceHost = new ServiceContractSyncUniqueTypeRefTestServiceHost(typeof(WcfService.ServiceContractUniqueTypeRefSyncService), serviceContractSyncUniqueTypeRefTestServiceHostbaseAddress);
            serviceContractSyncUniqueTypeRefTestServiceHostServiceHost.Open();

            Uri[] tcpCertificateWithServerAltNameTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/TcpCertificateWithServerAltName.svc", tcpBaseAddress))};
            TcpCertificateWithServerAltNameTestServiceHost tcpCertificateWithServerAltNameTestServiceHostServiceHost = new TcpCertificateWithServerAltNameTestServiceHost(typeof(WcfService.WcfService), tcpCertificateWithServerAltNameTestServiceHostbaseAddress);
            tcpCertificateWithServerAltNameTestServiceHostServiceHost.Open();

            Uri[] tcpCertificateWithSubjectCanonicalNameDomainNameTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/TcpCertificateWithSubjectCanonicalNameDomainName.svc", tcpBaseAddress))};
            TcpCertificateWithSubjectCanonicalNameDomainNameTestServiceHost tcpCertificateWithSubjectCanonicalNameDomainNameTestServiceHostServiceHost = new TcpCertificateWithSubjectCanonicalNameDomainNameTestServiceHost(typeof(WcfService.WcfService), tcpCertificateWithSubjectCanonicalNameDomainNameTestServiceHostbaseAddress);
            tcpCertificateWithSubjectCanonicalNameDomainNameTestServiceHostServiceHost.Open();

            Uri[] tcpCertificateWithSubjectCanonicalNameFqdnTestServiceHostbaseAddress = new Uri[] { new Uri(string.Format("{0}/TcpCertificateWithSubjectCanonicalNameFqdn.svc", tcpBaseAddress)) };
            TcpCertificateWithSubjectCanonicalNameFqdnTestServiceHost tcpCertificateWithSubjectCanonicalNameFqdnTestServiceHostServiceHost = new TcpCertificateWithSubjectCanonicalNameFqdnTestServiceHost(typeof(WcfService.WcfService), tcpCertificateWithSubjectCanonicalNameFqdnTestServiceHostbaseAddress);
            tcpCertificateWithSubjectCanonicalNameFqdnTestServiceHostServiceHost.Open();

            Uri[] tcpCertificateWithSubjectCanonicalNameLocalhostTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/TcpCertificateWithSubjectCanonicalNameLocalhost.svc", tcpBaseAddress))};
            TcpCertificateWithSubjectCanonicalNameLocalhostTestServiceHost tcpCertificateWithSubjectCanonicalNameLocalhostTestServiceHostServiceHost = new TcpCertificateWithSubjectCanonicalNameLocalhostTestServiceHost(typeof(WcfService.WcfService), tcpCertificateWithSubjectCanonicalNameLocalhostTestServiceHostbaseAddress);
            tcpCertificateWithSubjectCanonicalNameLocalhostTestServiceHostServiceHost.Open();

            Uri[] tcpExpiredServerCertTestServiceHostbaseAddress = new Uri[] { new Uri(string.Format("{0}/TcpExpiredServerCert.svc", tcpBaseAddress)) };
            TcpExpiredServerCertTestServiceHost tcpExpiredServerCertTestServiceHostServiceHost = new TcpExpiredServerCertTestServiceHost(typeof(WcfService.WcfService), tcpExpiredServerCertTestServiceHostbaseAddress);
            tcpExpiredServerCertTestServiceHostServiceHost.Open();

            Uri[] tcpNoSecurityTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/TcpNoSecurity.svc", tcpBaseAddress))};
            TcpNoSecurityTestServiceHost tcpNoSecurityTestServiceHostServiceHost = new TcpNoSecurityTestServiceHost(typeof(WcfService.WcfService), tcpNoSecurityTestServiceHostbaseAddress);
            tcpNoSecurityTestServiceHostServiceHost.Open();

            Uri[] tcpDefaultTestServiceHostbaseAddress = new Uri[] { new Uri(string.Format("{0}/TcpDefault.svc", tcpBaseAddress)) };
            TcpDefaultResourceTestServiceHost tcpDefaultTestServiceHostServiceHost = new TcpDefaultResourceTestServiceHost(typeof(WcfService.WcfService), tcpDefaultTestServiceHostbaseAddress);
            tcpDefaultTestServiceHostServiceHost.Open();

            Uri[] tcpNoSecurityTextTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/TcpNoSecurityText.svc", tcpBaseAddress))};
            TcpNoSecurityTextTestServiceHost tcpNoSecurityTextTestServiceHostServiceHost = new TcpNoSecurityTextTestServiceHost(typeof(WcfService.WcfService), tcpNoSecurityTextTestServiceHostbaseAddress);
            tcpNoSecurityTextTestServiceHostServiceHost.Open();

            Uri[] tcpRevokedServerCertTestServiceHostbaseAddress = new Uri[] { new Uri(string.Format("{0}/TcpRevokedServerCert.svc", tcpBaseAddress)) };
            TcpRevokedServerCertTestServiceHost tcpRevokedServerCertTestServiceHostServiceHost = new TcpRevokedServerCertTestServiceHost(typeof(WcfService.WcfService), tcpRevokedServerCertTestServiceHostbaseAddress);
            tcpRevokedServerCertTestServiceHostServiceHost.Open();

            Uri[] tcpStreamedNoSecurityTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/TcpStreamedNoSecurity.svc", tcpBaseAddress))};
            TcpStreamedNoSecurityTestServiceHost tcpStreamedNoSecurityTestServiceHostServiceHost = new TcpStreamedNoSecurityTestServiceHost(typeof(WcfService.WcfService), tcpStreamedNoSecurityTestServiceHostbaseAddress);
            tcpStreamedNoSecurityTestServiceHostServiceHost.Open();

            Uri[] tcpTransportSecuritySslCustomCertValidationTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/TcpTransportSecuritySslCustomCertValidation.svc", tcpBaseAddress))};
            TcpTransportSecuritySslCustomCertValidationTestServiceHost tcpTransportSecuritySslCustomCertValidationTestServiceHostServiceHost = new TcpTransportSecuritySslCustomCertValidationTestServiceHost(typeof(WcfService.WcfService), tcpTransportSecuritySslCustomCertValidationTestServiceHostbaseAddress);
            tcpTransportSecuritySslCustomCertValidationTestServiceHostServiceHost.Open();

            Uri[] tcpTransportSecurityStreamedTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/TcpTransportSecurityStreamed.svc", tcpBaseAddress))};
            TcpTransportSecurityStreamedTestServiceHost tcpTransportSecurityStreamedTestServiceHostServiceHost = new TcpTransportSecurityStreamedTestServiceHost(typeof(WcfService.WcfService), tcpTransportSecurityStreamedTestServiceHostbaseAddress);
            tcpTransportSecurityStreamedTestServiceHostServiceHost.Open();

            Uri[] tcpTransportSecurityWithSslTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/TcpTransportSecurityWithSsl.svc", tcpBaseAddress))};
            TcpTransportSecurityWithSslTestServiceHost tcpTransportSecurityWithSslTestServiceHostServiceHost = new TcpTransportSecurityWithSslTestServiceHost(typeof(WcfService.WcfService), tcpTransportSecurityWithSslTestServiceHostbaseAddress);
            tcpTransportSecurityWithSslTestServiceHostServiceHost.Open();

            Uri[] tcpVerifyDNSTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/TcpVerifyDNS.svc", tcpBaseAddress))};
            TcpVerifyDNSTestServiceHost tcpVerifyDNSTestServiceHostServiceHost = new TcpVerifyDNSTestServiceHost(typeof(WcfService.WcfService), tcpVerifyDNSTestServiceHostbaseAddress);
            tcpVerifyDNSTestServiceHostServiceHost.Open();

            Uri[] duplexWebSocketTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/DuplexWebSocket.svc", websocketBaseAddress))};
            DuplexWebSocketTestServiceHost duplexWebSocketTestServiceHostServiceHost = new DuplexWebSocketTestServiceHost(typeof(WcfService.WcfWebSocketService), duplexWebSocketTestServiceHostbaseAddress);
            duplexWebSocketTestServiceHostServiceHost.Open();

            Uri[] webSocketTransportTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/WebSocketTransport.svc", websocketBaseAddress))};
            WebSocketTransportTestServiceHost webSocketTransportTestServiceHostServiceHost = new WebSocketTransportTestServiceHost(typeof(WcfService.WcfWebSocketTransportUsageAlwaysService), webSocketTransportTestServiceHostbaseAddress);
            webSocketTransportTestServiceHostServiceHost.Open();

            Uri[] webSocketHttpDuplexBinaryStreamedTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/WebSocketHttpDuplexBinaryStreamed.svc", websocketBaseAddress))};
            WebSocketHttpDuplexBinaryStreamedTestServiceHost webSocketHttpDuplexBinaryStreamedTestServiceHostServiceHost = new WebSocketHttpDuplexBinaryStreamedTestServiceHost(typeof(WcfService.WSDuplexService), webSocketHttpDuplexBinaryStreamedTestServiceHostbaseAddress);
            webSocketHttpDuplexBinaryStreamedTestServiceHostServiceHost.Open();

            Uri[] webSocketHttpRequestReplyBinaryStreamedTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/WebSocketHttpRequestReplyBinaryStreamed.svc", websocketBaseAddress))};
            WebSocketHttpRequestReplyBinaryStreamedTestServiceHost webSocketHttpRequestReplyBinaryStreamedTestServiceHostServiceHost = new WebSocketHttpRequestReplyBinaryStreamedTestServiceHost(typeof(WcfService.WSRequestReplyService), webSocketHttpRequestReplyBinaryStreamedTestServiceHostbaseAddress);
            webSocketHttpRequestReplyBinaryStreamedTestServiceHostServiceHost.Open();

            Uri[] webSocketHttpsDuplexBinaryStreamedTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/WebSocketHttpsDuplexBinaryStreamed.svc", websocketsBaseAddress))};
            WebSocketHttpsDuplexBinaryStreamedTestServiceHost webSocketHttpsDuplexBinaryStreamedTestServiceHostServiceHost = new WebSocketHttpsDuplexBinaryStreamedTestServiceHost(typeof(WcfService.WSDuplexService), webSocketHttpsDuplexBinaryStreamedTestServiceHostbaseAddress);
            webSocketHttpsDuplexBinaryStreamedTestServiceHostServiceHost.Open();

            Uri[] webSocketHttpsDuplexTextStreamedTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/WebSocketHttpsDuplexTextStreamed.svc", websocketsBaseAddress))};
            WebSocketHttpsDuplexTextStreamedTestServiceHost webSocketHttpsDuplexTextStreamedTestServiceHostServiceHost = new WebSocketHttpsDuplexTextStreamedTestServiceHost(typeof(WcfService.WSDuplexService), webSocketHttpsDuplexTextStreamedTestServiceHostbaseAddress);
            webSocketHttpsDuplexTextStreamedTestServiceHostServiceHost.Open();

            Uri[] webSocketHttpRequestReplyTextStreamedTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/WebSocketHttpRequestReplyTextStreamed.svc", websocketBaseAddress))};
            WebSocketHttpRequestReplyTextStreamedTestServiceHost webSocketHttpRequestReplyTextStreamedTestServiceHostServiceHost = new WebSocketHttpRequestReplyTextStreamedTestServiceHost(typeof(WcfService.WSRequestReplyService), webSocketHttpRequestReplyTextStreamedTestServiceHostbaseAddress);
            webSocketHttpRequestReplyTextStreamedTestServiceHostServiceHost.Open();

            Uri[] webSocketHttpDuplexTextStreamedTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/WebSocketHttpDuplexTextStreamed.svc", websocketBaseAddress))};
            WebSocketHttpDuplexTextStreamedTestServiceHost webSocketHttpDuplexTextStreamedTestServiceHostServiceHost = new WebSocketHttpDuplexTextStreamedTestServiceHost(typeof(WcfService.WSDuplexService), webSocketHttpDuplexTextStreamedTestServiceHostbaseAddress);
            webSocketHttpDuplexTextStreamedTestServiceHostServiceHost.Open();

            Uri[] webSocketHttpRequestReplyTextBufferedTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/WebSocketHttpRequestReplyTextBuffered.svc", websocketBaseAddress))};
            WebSocketHttpRequestReplyTextBufferedTestServiceHost webSocketHttpRequestReplyTextBufferedTestServiceHostServiceHost = new WebSocketHttpRequestReplyTextBufferedTestServiceHost(typeof(WcfService.WSRequestReplyService), webSocketHttpRequestReplyTextBufferedTestServiceHostbaseAddress);
            webSocketHttpRequestReplyTextBufferedTestServiceHostServiceHost.Open();

            Uri[] webSocketHttpRequestReplyBinaryBufferedTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/WebSocketHttpRequestReplyBinaryBuffered.svc", websocketBaseAddress))};
            WebSocketHttpRequestReplyBinaryBufferedTestServiceHost webSocketHttpRequestReplyBinaryBufferedTestServiceHostServiceHost = new WebSocketHttpRequestReplyBinaryBufferedTestServiceHost(typeof(WcfService.WSRequestReplyService), webSocketHttpRequestReplyBinaryBufferedTestServiceHostbaseAddress);
            webSocketHttpRequestReplyBinaryBufferedTestServiceHostServiceHost.Open();

            Uri[] webSocketHttpDuplexTextBufferedTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/WebSocketHttpDuplexTextBuffered.svc", websocketBaseAddress))};
            WebSocketHttpDuplexTextBufferedTestServiceHost webSocketHttpDuplexTextBufferedTestServiceHostServiceHost = new WebSocketHttpDuplexTextBufferedTestServiceHost(typeof(WcfService.WSDuplexService), webSocketHttpDuplexTextBufferedTestServiceHostbaseAddress);
            webSocketHttpDuplexTextBufferedTestServiceHostServiceHost.Open();

            Uri[] webSocketHttpDuplexBinaryBufferedTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/WebSocketHttpDuplexBinaryBuffered.svc", websocketBaseAddress))};
            WebSocketHttpDuplexBinaryBufferedTestServiceHost webSocketHttpDuplexBinaryBufferedTestServiceHostServiceHost = new WebSocketHttpDuplexBinaryBufferedTestServiceHost(typeof(WcfService.WSDuplexService), webSocketHttpDuplexBinaryBufferedTestServiceHostbaseAddress);
            webSocketHttpDuplexBinaryBufferedTestServiceHostServiceHost.Open();

            Uri[] webSocketHttpsRequestReplyBinaryBufferedTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/WebSocketHttpsRequestReplyBinaryBuffered.svc", websocketsBaseAddress))};
            WebSocketHttpsRequestReplyBinaryBufferedTestServiceHost webSocketHttpsRequestReplyBinaryBufferedTestServiceHostServiceHost = new WebSocketHttpsRequestReplyBinaryBufferedTestServiceHost(typeof(WcfService.WSRequestReplyService), webSocketHttpsRequestReplyBinaryBufferedTestServiceHostbaseAddress);
            webSocketHttpsRequestReplyBinaryBufferedTestServiceHostServiceHost.Open();

            Uri[] webSocketHttpsRequestReplyTextBufferedTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/WebSocketHttpsRequestReplyTextBuffered.svc", websocketsBaseAddress))};
            WebSocketHttpsRequestReplyTextBufferedTestServiceHost webSocketHttpsRequestReplyTextBufferedTestServiceHostServiceHost = new WebSocketHttpsRequestReplyTextBufferedTestServiceHost(typeof(WcfService.WSRequestReplyService), webSocketHttpsRequestReplyTextBufferedTestServiceHostbaseAddress);
            webSocketHttpsRequestReplyTextBufferedTestServiceHostServiceHost.Open();

            Uri[] webSocketHttpsDuplexBinaryBufferedTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/WebSocketHttpsDuplexBinaryBuffered.svc", websocketsBaseAddress))};
            WebSocketHttpsDuplexBinaryBufferedTestServiceHost webSocketHttpsDuplexBinaryBufferedTestServiceHostServiceHost = new WebSocketHttpsDuplexBinaryBufferedTestServiceHost(typeof(WcfService.WSDuplexService), webSocketHttpsDuplexBinaryBufferedTestServiceHostbaseAddress);
            webSocketHttpsDuplexBinaryBufferedTestServiceHostServiceHost.Open();

            Uri[] webSocketHttpsDuplexTextBufferedTestServiceHostbaseAddress = new Uri[]  { new Uri(string.Format("{0}/WebSocketHttpsDuplexTextBuffered.svc", websocketsBaseAddress))};
            WebSocketHttpsDuplexTextBufferedTestServiceHost webSocketHttpsDuplexTextBufferedTestServiceHostServiceHost = new WebSocketHttpsDuplexTextBufferedTestServiceHost(typeof(WcfService.WSDuplexService), webSocketHttpsDuplexTextBufferedTestServiceHostbaseAddress);
            webSocketHttpsDuplexTextBufferedTestServiceHostServiceHost.Open();

            Console.WriteLine("All service hosts have started.");
            Console.WriteLine("Press <ENTER> to terminate the self service Host.");
            Console.ReadLine();
        }
    }
}
