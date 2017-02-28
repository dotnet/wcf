// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace SharedPoolsOfWCFObjects
{
    public static class TestHelpers
    {
        private static TestBinding UseBinding { get; set; }
        private static string s_hostName;
        private static bool s_debugMode;

        private static int s_selfHostPorts;
        private static int s_shPortIdx = 1;

        private static SecurityMode s_bindingSecurityMode = SecurityMode.None;
        private static HttpClientCredentialType s_httpClientCredentialType = HttpClientCredentialType.None;
        private static TcpClientCredentialType s_tcpClientCredentialType = TcpClientCredentialType.None;
        private static string s_serverDnsEndpointIdentity;
        private static string s_clientCertThumbprint;
        private static StoreName s_clientCertStoreName;
        private static StoreLocation s_clientCertStoreLocation;

        // a negative value signifies "do not use custom connection pool size"
        private static int s_customConnectionPoolSize;

        // 0 signifies "do not change the default timeout"
        private static int s_openTimeoutMSeconds;
        private static int s_closeTimeoutMSeconds;
        private static int s_receiveTimeoutMSeconds;
        private static int s_sendTimeoutMSeconds;

        private static string s_httpHelloUrl;
        private static string[] s_sh_httpHelloUrls;
        private static string s_httpsHelloUrl;
        private static string[] s_sh_httpsHelloUrls;
        private static string s_httpStreamingUrl;
        private static string[] s_sh_httpStreamingUrls;
        private static string s_httpsStreamingUrl;
        private static string[] s_sh_httpsStreamingUrls;
        private static string s_httpDuplexUrl;

        private static string s_tcpHelloUrl;
        private static string[] s_sh_tcpHelloUrls;
        private static string s_tcpStreamingUrl;
        private static string[] s_sh_tcpStreamingUrls;
        private static string s_tcpDupUrl;
        private static string[] s_sh_tcpDupUrls;

        private static string s_netHttpHelloUrl;
        private static string[] s_sh_netHttpHelloUrl;
        private static string s_netHttpDupUrl;
        private static string[] s_sh_netHttpDupUrl;
        private static string s_netHttpStreamingUrl;
        private static string[] s_sh_netHttpStreamingUrl;
        private static string s_netHttpDupStreamingUrl;
        private static string[] s_sh_netHttpDupStreamingUrl;

        // address modifiers for Http, NetTcp, and NetHttp bindings
        private const string HttpCert = "/httpCert";
        private const string HttpWind = "/httpWind";
        private const string NetTcpCert = "/netTcpCert";
        private const string NetTcpWind = "/netTcpWind";

        // This is one-stop setup for TestHelpers methods to start doing the desired work.
        public static void SetHelperParameters(
            TestBinding useBinding,
            string hostName,
            string appName,
            int selfHostPortStartingNumber,
            int numSelfHostPorts,
            // security-related parameters
            SecurityMode bindingSecurityMode,
            HttpClientCredentialType httpClientCredentialType,
            TcpClientCredentialType tcpClientCredentialType,
            string serverDnsEndpointIdentity,
            string clientCertThumbprint,
            StoreName clientCertStoreName,
            StoreLocation clientCertStoreLocation,
            int openTimeoutMSecs, int closeTimeoutMSecs, int receiveTimeoutMSecs, int sendTimeoutMSecs,
            int customConnectionPoolSize,
            bool debugMode
            )
        {
            UseBinding = useBinding;
            s_debugMode = debugMode;
            s_hostName = hostName;
            s_selfHostPorts = numSelfHostPorts;

            s_bindingSecurityMode = bindingSecurityMode;
            s_httpClientCredentialType = httpClientCredentialType;
            s_tcpClientCredentialType = tcpClientCredentialType;
            s_serverDnsEndpointIdentity = serverDnsEndpointIdentity;
            s_clientCertThumbprint = clientCertThumbprint;
            s_clientCertStoreLocation = clientCertStoreLocation;
            s_clientCertStoreName = clientCertStoreName;

            s_openTimeoutMSeconds = openTimeoutMSecs;
            s_closeTimeoutMSeconds = closeTimeoutMSecs;
            s_receiveTimeoutMSeconds = receiveTimeoutMSecs;
            s_sendTimeoutMSeconds = sendTimeoutMSecs;

            s_customConnectionPoolSize = customConnectionPoolSize;

            //
            // validate security parameters to make it easier to identify incorrect ones
            //
            if (bindingSecurityMode == SecurityMode.Transport)
            {
                // For cert auth we need:
                if (httpClientCredentialType == HttpClientCredentialType.Certificate || tcpClientCredentialType == TcpClientCredentialType.Certificate)
                {
                    // a thumbprint
                    if (string.IsNullOrEmpty(clientCertThumbprint))
                    {
                        throw new ArgumentException("Client certificate thumbprint must be provided for certificate authentication");
                    }
                    // server's DNS endpoint identity string
                    if (string.IsNullOrEmpty(serverDnsEndpointIdentity))
                    {
                        throw new ArgumentException("Server's DNS endpoint identity must be provided for certificate authentication");
                    }

                    // WCF authentication errors are not always informative so we place some diagnostics here
                    if (debugMode)
                    {
                        using (X509Store store = new X509Store(s_clientCertStoreName, s_clientCertStoreLocation))
                        {
                            Console.WriteLine(String.Format("Looking for cert in store: {0} location:{1}",
                                s_clientCertStoreName.ToString(), s_clientCertStoreLocation.ToString()));
                            store.Open(OpenFlags.ReadOnly);
                            X509Certificate2Collection foundCertificates = store.Certificates.Find(X509FindType.FindByThumbprint, clientCertThumbprint, false);
                            foreach (var cert in foundCertificates)
                            {
                                string hasPrivateKey, dnsName = "", altDnsName = "", privateKeys = "";
                                try
                                {
                                    dnsName = cert.GetNameInfo(X509NameType.DnsName, false);
                                }
                                catch { }
                                try
                                {
                                    altDnsName = cert.GetNameInfo(X509NameType.DnsFromAlternativeName, false);
                                }
                                catch { }
                                try
                                {
                                    hasPrivateKey = cert.HasPrivateKey.ToString();
                                }
                                catch (Exception e)
                                {
                                    hasPrivateKey = e.Message;
                                }
                                try
                                {
                                    privateKeys += "RSA key: " + (cert.GetRSAPrivateKey()?.KeySize).ToString();
                                }
                                catch (Exception ee)
                                {
                                    privateKeys = ee.Message;
                                }
                                Console.WriteLine(String.Format("Found cert with Name: {0}, Authority: {1}, Subject {2}, Has private key: {3}, privateKeyLength: {4} dnsName: {5}, altDnsName: {6} .",
                                    cert.FriendlyName, cert.Issuer, cert.Subject, hasPrivateKey, privateKeys, dnsName, altDnsName));
                            }
                        }
                    }
                }
            }
            else if (bindingSecurityMode == SecurityMode.None)
            {
                // Reject incompatible parameters
                if (s_httpClientCredentialType != HttpClientCredentialType.None || s_tcpClientCredentialType != TcpClientCredentialType.None)
                {
                    throw new ArgumentException(String.Format("Client credential types ({0} {1}) are incompatible with binding security mode {2}",
                        s_httpClientCredentialType, s_tcpClientCredentialType, bindingSecurityMode));
                }
            }
            else
            {
                throw new ArgumentException("Only SecurityMode.None and SecurityMode.Transport are supported", "bindingSecurityMode");
            }

            // pre-set urls
            s_httpHelloUrl = "http://" + hostName + "/" + appName + "/Service1.svc";
            MultiPortUrlHelper(selfHostPortStartingNumber, numSelfHostPorts, hostName, "http://{0}:{1}/Service1.svc", out s_sh_httpHelloUrls);
            s_httpsHelloUrl = "https://" + hostName + "/" + appName + "/Service1.svc";
            MultiPortUrlHelper(selfHostPortStartingNumber, numSelfHostPorts, hostName, "https://{0}:{1}/Service1.svc", out s_sh_httpsHelloUrls);
            s_httpStreamingUrl = "http://" + hostName + "/" + appName + "/StreamingService.svc";
            MultiPortUrlHelper(selfHostPortStartingNumber, numSelfHostPorts, hostName, "http://{0}:{1}/StreamingService.svc", out s_sh_httpStreamingUrls);
            s_httpsStreamingUrl = "https://" + hostName + "/" + appName + "/StreamingService.svc";
            MultiPortUrlHelper(selfHostPortStartingNumber, numSelfHostPorts, hostName, "https://{0}:{1}/StreamingService.svc", out s_sh_httpsStreamingUrls);
            s_httpDuplexUrl = "http://" + hostName + "/" + appName + "/DuplexService.svc";

            s_tcpHelloUrl = "net.tcp://" + hostName + ":808/" + appName + "/Service1.svc";
            MultiPortUrlHelper(selfHostPortStartingNumber, numSelfHostPorts, hostName, "net.tcp://{0}:{1}/Service1.svc", out s_sh_tcpHelloUrls);
            s_tcpDupUrl = "net.tcp://" + hostName + ":808/" + appName + "/DuplexService.svc";
            MultiPortUrlHelper(selfHostPortStartingNumber, numSelfHostPorts, hostName, "net.tcp://{0}:{1}/DuplexService.svc", out s_sh_tcpDupUrls);
            s_tcpStreamingUrl = "net.tcp://" + hostName + ":808/" + appName + "/StreamingService.svc";
            MultiPortUrlHelper(selfHostPortStartingNumber, numSelfHostPorts, hostName, "net.tcp://{0}:{1}/StreamingService.svc", out s_sh_tcpStreamingUrls);

            s_netHttpHelloUrl = "ws://" + hostName + "/" + appName + "/Service1.svc/websocket";
            MultiPortUrlHelper(selfHostPortStartingNumber, numSelfHostPorts, hostName, "net.tcp://{0}:{1}/Service1.svc/nethttp", out s_sh_netHttpHelloUrl);
            s_netHttpDupUrl = "ws://" + hostName + "/" + appName + "/DuplexService.svc/websocket";
            MultiPortUrlHelper(selfHostPortStartingNumber, numSelfHostPorts, hostName, "net.tcp://{0}:{1}/DuplexService.svc/websocket", out s_sh_netHttpDupUrl);
            s_netHttpStreamingUrl = "ws://" + hostName + "/" + appName + "/StreamingService.svc/websocket";
            MultiPortUrlHelper(selfHostPortStartingNumber, numSelfHostPorts, hostName, "net.tcp://{0}:{1}/StreamingService.svc", out s_sh_netHttpStreamingUrl);
            s_netHttpDupStreamingUrl = "ws://" + hostName + "/" + appName + "/DuplexStreamingService.svc";
            MultiPortUrlHelper(selfHostPortStartingNumber, numSelfHostPorts, hostName, "net.tcp://{0}:{1}/DuplexStreamingService.svc", out s_sh_netHttpDupStreamingUrl);
        }

        private static void MultiPortUrlHelper(int selfHostPortStartingNumber, int numSelfHostPorts, string hostName, string urlTemplate, out String[] sh_urls)
        {
            sh_urls = null;
            if (selfHostPortStartingNumber != 0)
            {
                if (numSelfHostPorts <= 0)
                {
                    throw new ArgumentException("Must specify non zero numSelfHostPorts when selfHostPortNumber is set", "numSelfHostPorts");
                }

                Console.WriteLine(String.Format("Using {0} services starting from port {1}", numSelfHostPorts, selfHostPortStartingNumber));
                sh_urls = new string[numSelfHostPorts];
                for (int p = 0; p < numSelfHostPorts; p++)
                {
                    sh_urls[p] = String.Format(urlTemplate, hostName, selfHostPortStartingNumber + p);
#if DEBUG
                    Console.WriteLine(sh_urls[p]);
#endif
                }
            }
        }

        public static EndpointAddress CreateEndPointHelloAddress()
        {
            switch (UseBinding)
            {
                case TestBinding.Http:
                    return CreateHttpEndpointHelloAddress();
                case TestBinding.Https:
                    return CreateHttpsEndpointHelloAddress();
                case TestBinding.NetHttpBinding:
                    return CreateNetHttpEndpointHelloAddress();
                case TestBinding.NetTcp:
                    return CreateNetTcpEndpointHelloAddress();
                default:
                    return null;
            }
        }
        public static EndpointAddress CreateHttpEndpointHelloAddress()
        {
            if (s_selfHostPorts == 0)
            {
                return CreateHttpEndpointAddressImpl(s_httpHelloUrl);
            }
            else
            {
                var index = Interlocked.Increment(ref s_shPortIdx) % s_selfHostPorts;
                return CreateHttpEndpointAddressImpl(s_sh_httpHelloUrls[index]);
            }
        }
        public static EndpointAddress CreateHttpsEndpointHelloAddress()
        {
            if (s_selfHostPorts == 0)
            {
                return CreateHttpEndpointAddressImpl(s_httpsHelloUrl);
            }
            else
            {
                var index = Interlocked.Increment(ref s_shPortIdx) % s_selfHostPorts;
                return CreateHttpEndpointAddressImpl(s_sh_httpsHelloUrls[index]);
            }
        }
        public static EndpointAddress CreateNetHttpEndpointHelloAddress()
        {
            if (s_debugMode)
            {
                Console.WriteLine(s_netHttpHelloUrl);
            }
            return new EndpointAddress(s_netHttpHelloUrl);
        }
        public static EndpointAddress CreateNetTcpEndpointHelloAddress()
        {
            if (s_selfHostPorts == 0)
            {
                return CreateNetTcpEndpointAddressImpl(s_tcpHelloUrl);
            }
            else
            {
                var index = Interlocked.Increment(ref s_shPortIdx) % s_selfHostPorts;
                return CreateNetTcpEndpointAddressImpl(s_sh_tcpHelloUrls[index]);
            }
        }
        private static EndpointAddress CreateHttpEndpointAddressImpl(string url)
        {
            if (s_httpClientCredentialType == HttpClientCredentialType.Certificate)
            {
                if (String.IsNullOrEmpty(s_serverDnsEndpointIdentity))
                {
                    throw new NotSupportedException("Server DnsEndpointIdentity is required for certificate authentication");
                }
                url += HttpCert;
                if (s_debugMode)
                {
                    Console.WriteLine(String.Format("URL: {0} DNSIdentity: {1} ", url, s_serverDnsEndpointIdentity));
                }
                return new EndpointAddress(new Uri(url), new DnsEndpointIdentity(s_serverDnsEndpointIdentity));
            }
            else if (s_httpClientCredentialType == HttpClientCredentialType.Windows)
            {
                url += HttpWind;
            }
            if (s_debugMode)
            {
                Console.WriteLine(String.Format("URL: {0}", url));
            }
            return new EndpointAddress(url);
        }
        private static EndpointAddress CreateNetTcpEndpointAddressImpl(string url)
        {
            if (s_tcpClientCredentialType == TcpClientCredentialType.Certificate)
            {
                if (String.IsNullOrEmpty(s_serverDnsEndpointIdentity))
                {
                    throw new NotSupportedException("Server DnsEndpointIdentity is required for certificate authentication");
                }
                url = url + NetTcpCert;
                if (s_debugMode)
                {
                    Console.WriteLine(String.Format("URL: {0} DNSIdentity: {1} ", url, s_serverDnsEndpointIdentity));
                }
                return new EndpointAddress(new Uri(url), new DnsEndpointIdentity(s_serverDnsEndpointIdentity));
            }
            else if (s_tcpClientCredentialType == TcpClientCredentialType.Windows)
            {
                url = url + NetTcpWind;
            }

            if (s_debugMode)
            {
                Console.WriteLine(String.Format("URL: {0}", url));
            }
            return new EndpointAddress(url);
        }

        public static EndpointAddress CreateEndPointDuplexAddress()
        {
            switch (UseBinding)
            {
                case TestBinding.Http:
                    return CreateHttpEndpointDuplexAddress();
                case TestBinding.NetHttpBinding:
                    return CreateNetHttpEndpointDuplexAddress();
                case TestBinding.NetTcp:
                    return CreateNetTcpEndpointDuplexAddress();
                default:
                    return null;
            }
        }
        public static EndpointAddress CreateHttpEndpointDuplexAddress()
        {
            return new EndpointAddress(s_httpDuplexUrl);
        }
        public static EndpointAddress CreateNetHttpEndpointDuplexAddress()
        {
            if (s_debugMode)
            {
                Console.WriteLine(s_netHttpDupUrl);
            }
            return new EndpointAddress(s_netHttpDupUrl);
        }
        public static EndpointAddress CreateNetTcpEndpointDuplexAddress()
        {
            if (s_selfHostPorts == 0)
            {
                return CreateNetTcpEndpointAddressImpl(s_tcpDupUrl);
            }
            else
            {
                var index = Interlocked.Increment(ref s_shPortIdx) % s_selfHostPorts;
                return CreateNetTcpEndpointAddressImpl(s_sh_tcpDupUrls[index]);
            }
        }

        public static EndpointAddress CreateEndPointStreamingAddress()
        {
            switch (UseBinding)
            {
                case TestBinding.Http:
                    return CreateHttpEndpointStreamingAddress();
                case TestBinding.Https:
                    return CreateHttpsEndpointStreamingAddress();
                case TestBinding.NetHttpBinding:
                    return CreateNetHttpEndpointStreamingAddress();
                case TestBinding.NetTcp:
                    return CreateNetTcpEndpointStreamingAddress();
                default:
                    return null;
            }
        }
        public static EndpointAddress CreateHttpEndpointStreamingAddress()
        {
            if (s_selfHostPorts == 0)
            {
                return CreateHttpEndpointAddressImpl(s_httpStreamingUrl);
            }
            else
            {
                var index = Interlocked.Increment(ref s_shPortIdx) % s_selfHostPorts;
                return CreateHttpEndpointAddressImpl(s_sh_httpStreamingUrls[index]);
            }
        }
        public static EndpointAddress CreateHttpsEndpointStreamingAddress()
        {
            if (s_selfHostPorts == 0)
            {
                return CreateHttpEndpointAddressImpl(s_httpsStreamingUrl);
            }
            else
            {
                var index = Interlocked.Increment(ref s_shPortIdx) % s_selfHostPorts;
                return CreateHttpEndpointAddressImpl(s_sh_httpsStreamingUrls[index]);
            }
        }
        public static EndpointAddress CreateNetHttpEndpointStreamingAddress()
        {
            if (s_debugMode)
            {
                Console.WriteLine(s_netHttpStreamingUrl);
            }
            return new EndpointAddress(s_netHttpStreamingUrl);
        }
        public static EndpointAddress CreateNetTcpEndpointStreamingAddress()
        {
            if (s_selfHostPorts == 0)
            {
                return CreateNetTcpEndpointAddressImpl(s_tcpStreamingUrl);
            }
            else
            {
                var index = Interlocked.Increment(ref s_shPortIdx) % s_selfHostPorts;
                return CreateNetTcpEndpointAddressImpl(s_sh_tcpStreamingUrls[index]);
            }
        }

        public static EndpointAddress CreateEndPointDuplexStreamingAddress()
        {
            switch (UseBinding)
            {
                case TestBinding.NetHttpBinding:
                    return CreateNetHttpEndpointDuplexStreamingAddress();
                default:
                    throw new NotImplementedException("Duplex streaming is only supported for NetHttp binding");
            }
        }
        public static EndpointAddress CreateNetHttpEndpointDuplexStreamingAddress()
        {
            if (s_debugMode)
            {
                Console.WriteLine(s_netHttpDupStreamingUrl);
            }
            return new EndpointAddress(s_netHttpDupStreamingUrl);
        }


        public static Binding CreateBinding()
        {
            switch (UseBinding)
            {
                case TestBinding.Http:
                    return CreateHttpBinding();
                case TestBinding.Https:
                    return CreateHttpsBinding();
                case TestBinding.NetHttpBinding:
                    return CreateNetHttpBinding();
                case TestBinding.NetTcp:
                    return CreateNetTcpBinding();
                default:
                    return null;
            }
        }
        public static Binding CreateHttpBinding()
        {
            return SetupBindingTimeouts(new BasicHttpBinding());
        }
        public static Binding CreateHttpsBinding()
        {
            var b = new BasicHttpsBinding();
            SetupHttpsBindingSecurity(b);
            return SetupBindingTimeouts(b);
        }

        private static void SetupHttpsBindingSecurity(BasicHttpsBinding binding)
        {
            binding.Security.Mode = (s_bindingSecurityMode == SecurityMode.Transport) ? BasicHttpsSecurityMode.Transport : BasicHttpsSecurityMode.TransportWithMessageCredential;
            binding.Security.Transport = new HttpTransportSecurity();
            binding.Security.Transport.ClientCredentialType = s_httpClientCredentialType;
            if (s_debugMode)
            {
                Console.WriteLine(String.Format(
                    "SetupHttpsBindingSecurity: binding.Security.Mode: {0}, binding.Security.Transport.ClientCredentialType: {1}",
                    binding.Security.Mode, binding.Security.Transport.ClientCredentialType));
            }
        }

        public static Binding CreateNetHttpBinding()
        {
            NetHttpBinding netHttp = new NetHttpBinding();
            netHttp.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            return SetupBindingTimeouts(netHttp);
        }

        public static Binding CreateNetTcpBinding()
        {
            return (s_customConnectionPoolSize >= 0)
                ? CreateCustomNetTcpBinding()
                : CreateNetTcpBindingImpl();
        }
        public static Binding CreateNetTcpBindingImpl()
        {
            var binding = new NetTcpBinding();
            SetupNetTcpBindingSecurity(binding);
            return SetupBindingTimeouts(binding);
        }
        public static Binding CreateCustomNetTcpBinding()
        {
            CustomBinding binding = new CustomBinding(CreateNetTcpBindingImpl());
            binding.Elements.Find<TcpTransportBindingElement>().ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint = s_customConnectionPoolSize;
            return binding;
        }

        public static Binding CreateStreamingBinding(int maxStreamSize)
        {
            switch (UseBinding)
            {
                case TestBinding.Http:
                    return CreateHttpStreamingBinding(maxStreamSize);
                case TestBinding.Https:
                    return CreateHttpsStreamingBinding(maxStreamSize);
                case TestBinding.NetHttpBinding:
                    return CreateNetHttpStreamingBinding(maxStreamSize);
                case TestBinding.NetTcp:
                    return CreateNetTcpStreamingBinding(maxStreamSize);
                default:
                    return null;
            }
        }
        public static Binding CreateHttpStreamingBinding(int maxStreamSize)
        {
            var binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = maxStreamSize * 2;
            binding.TransferMode = TransferMode.Streamed;
            return SetupBindingTimeouts(binding);
        }
        public static Binding CreateHttpsStreamingBinding(int maxStreamSize)
        {
            var binding = new BasicHttpsBinding();
            binding.MaxReceivedMessageSize = maxStreamSize * 2;
            binding.TransferMode = TransferMode.Streamed;
            SetupHttpsBindingSecurity(binding);
            return SetupBindingTimeouts(binding);
        }

        public static Binding CreateNetTcpStreamingBinding(int maxStreamSize)
        {
            return (s_customConnectionPoolSize >= 0)
                ? CreateCustomNetTcpStreamingBinding(maxStreamSize)
                : CreateNetTcpStreamingBindingImpl(maxStreamSize);
        }

        public static Binding CreateCustomNetTcpStreamingBinding(int maxStreamSize)
        {
            CustomBinding binding = new CustomBinding(CreateNetTcpStreamingBindingImpl(maxStreamSize));
            binding.Elements.Find<TcpTransportBindingElement>().ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint = s_customConnectionPoolSize;
            return binding;
        }
        public static Binding CreateNetTcpStreamingBindingImpl(int maxStreamSize)
        {
            NetTcpBinding binding = new NetTcpBinding();
            SetupNetTcpBindingSecurity(binding);
            binding.MaxReceivedMessageSize = maxStreamSize * 2;
            binding.TransferMode = TransferMode.Streamed;
            return SetupBindingTimeouts(binding);
        }
        public static Binding CreateNetHttpStreamingBinding(int maxStreamSize)
        {
            var binding = new NetHttpBinding();

            binding.MaxReceivedMessageSize = maxStreamSize * 2;
            binding.TransferMode = TransferMode.Streamed;
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            return SetupBindingTimeouts(binding);
        }

        private static T SetupBindingTimeouts<T>(T binding) where T : Binding
        {
            if (s_receiveTimeoutMSeconds > 0) binding.ReceiveTimeout = TimeSpan.FromMilliseconds(s_receiveTimeoutMSeconds);
            if (s_sendTimeoutMSeconds > 0) binding.SendTimeout = TimeSpan.FromMilliseconds(s_sendTimeoutMSeconds);
            if (s_openTimeoutMSeconds > 0) binding.OpenTimeout = TimeSpan.FromMilliseconds(s_openTimeoutMSeconds);
            if (s_closeTimeoutMSeconds > 0) binding.CloseTimeout = TimeSpan.FromMilliseconds(s_closeTimeoutMSeconds);
            return binding;
        }
        private static void SetupNetTcpBindingSecurity(NetTcpBinding binding)
        {
            binding.Security = new NetTcpSecurity();
            binding.Security.Mode = s_bindingSecurityMode;
            binding.Security.Transport.ClientCredentialType = s_tcpClientCredentialType;
        }

        public static ChannelFactory<C> CreateChannelFactory<C>(EndpointAddress a, Binding b)
        {
            var factory = new ChannelFactory<C>(b, a);
            SetupChannelFactorySecurity(factory);
            new CommunicationObjectEventVerifier(factory);
            return factory;
        }

        public static DuplexChannelFactory<C> CreateDuplexChannelFactory<C>(EndpointAddress a, Binding b, object duplexCallback)
        {
            var instanceContext = new InstanceContext(duplexCallback);
            var factory = new DuplexChannelFactory<C>(instanceContext, b, a);
            SetupChannelFactorySecurity(factory);

            new CommunicationObjectEventVerifier(factory);
            new CommunicationObjectEventVerifier(instanceContext);
            return factory;
        }

        private static void SetupChannelFactorySecurity<C>(ChannelFactory<C> factory)
        {
            if (s_bindingSecurityMode == SecurityMode.Transport &&
                !String.IsNullOrEmpty(s_clientCertThumbprint) &&
                (s_httpClientCredentialType == HttpClientCredentialType.Certificate || s_tcpClientCredentialType == TcpClientCredentialType.Certificate))
            {
                factory.Credentials.ClientCertificate.SetCertificate(s_clientCertStoreLocation, s_clientCertStoreName, X509FindType.FindByThumbprint, s_clientCertThumbprint);
                factory.Credentials.ServiceCertificate.SslCertificateAuthentication = new System.ServiceModel.Security.X509ServiceCertificateAuthentication();

                // We won't repeatedly check cert revocation for stress as this could take a significant hit on performance.
                // This could be a coverage hole though as this ommits a valid code path.
                factory.Credentials.ServiceCertificate.SslCertificateAuthentication.RevocationMode = X509RevocationMode.NoCheck;
                // use default CertificateValidationMode for now
            }
        }

        public static void CloseFactory<C>(ChannelFactory<C> factory)
        {
            factory.Close();
        }

        public static Task CloseFactoryAsync<C>(ChannelFactory<C> factory)
        {
            return Task.Factory.FromAsync(factory.BeginClose, factory.EndClose, TaskCreationOptions.None);
        }

        private static int s_factoryPreemptiveTimeoutsFailed = 0;
        public static async Task CloseFactoryPreemptiveTimeoutAsync<C>(ChannelFactory<C> factory, int timeoutms, bool failOnPreemptiveTimeout)
        {
            var closeTask = CloseFactoryAsync(factory);
            var timeoutTask = Task.Delay(timeoutms);
            var taskThatCompleted = await Task.WhenAny(closeTask, timeoutTask);
            if (taskThatCompleted == timeoutTask)
            {
                if (failOnPreemptiveTimeout)
                {
                    TestUtils.ReportFailure("CloseFactoryAsync timed out! " + factory.State);
                    GC.KeepAlive(factory);
                }
                else
                {
                    Console.WriteLine("CloseFactoryAsync preemptive timeout: #" + Interlocked.Increment(ref s_factoryPreemptiveTimeoutsFailed));
                }
                // don't return until we actually do close it
                await closeTask;
            }
        }

        public static C CreateChannel<C>(ChannelFactory<C> factory)
        {
            var channel = factory.CreateChannel();
            new CommunicationObjectEventVerifier(channel as ICommunicationObject);
            return channel;
        }

        public class CommunicationObjectEventVerifier
        {
            private int _openedFired;
            private int _openingFired;
            private int _closedFired;
            private int _closingFired;
            private int _faultedFired;

            private static long s_commObjectsCreated = 0;
            private static long s_commObjectsOpened = 0;
            private static long s_commObjectsClosed = 0;
            private static long s_commObjectsFailed = 0;


            public CommunicationObjectEventVerifier(ICommunicationObject communicationObj)
            {
                Interlocked.Increment(ref s_commObjectsCreated);

                communicationObj.Opened += (_, __) =>
                {
                    Interlocked.Increment(ref s_commObjectsOpened);
                    if (Interlocked.CompareExchange(ref _openedFired, 1, 0) != 0)
                    {
                        TestUtils.ReportFailure("ICommunicationObject.Opened event fired more than once");
                    }
                };

                communicationObj.Opening += (_, __) =>
                {
                    if (Interlocked.CompareExchange(ref _openingFired, 1, 0) != 0)
                    {
                        TestUtils.ReportFailure("ICommunicationObject.Opening event fired more than once");
                    }
                };

                communicationObj.Closed += (_, __) =>
                {
                    Interlocked.Increment(ref s_commObjectsClosed);
                    if (Interlocked.CompareExchange(ref _closedFired, 1, 0) != 0)
                    {
                        TestUtils.ReportFailure("ICommunicationObject.Closed event fired more than once");
                    }
                };

                communicationObj.Closing += (_, __) =>
                {
                    if (Interlocked.CompareExchange(ref _closingFired, 1, 0) != 0)
                    {
                        TestUtils.ReportFailure("ICommunicationObject.Closing event fired more than once");
                    }
                };

                communicationObj.Faulted += (_, __) =>
                {
                    Interlocked.Increment(ref s_commObjectsFailed);
                    if (Interlocked.CompareExchange(ref _faultedFired, 1, 0) != 0)
                    {
                        TestUtils.ReportFailure("ICommunicationObject.Faulted event fired more than once");
                    }
                };
            }
        }


        public static void OpenChannel<C>(C channel)
        {
            // Getting a null would indicate an issue in harness
            if (channel == null)
            {
                TestUtils.ReportFailure("channel == null");
            }
            var cc = channel as ICommunicationObject;
            // Getting a null would indicate an issue in tests
            if (cc == null)
            {
                TestUtils.ReportFailure("channel is not ICommunicationObject");
            }

            cc.Open();
        }

        public static async Task OpenChannelAsync<C>(C channel)
        {
            // Getting a null would indicate an issue in harness
            if (channel == null)
            {
                TestUtils.ReportFailure("channel == null");
            }
            var cc = (channel as IClientChannel);
            // Getting a null would indicate an issue in tests
            if (cc == null)
            {
                TestUtils.ReportFailure("channel is not IClientChannel");
            }
            await Task.Factory.FromAsync(cc.BeginOpen, cc.EndOpen, TaskCreationOptions.None);
        }

        public static void CloseChannel<C>(C channel)
        {
            // Getting a null would indicate an issue in harness
            if (channel == null)
            {
                TestUtils.ReportFailure("channel == null");
            }
            var cc = channel as ICommunicationObject;
            // Getting a null would indicate an issue in tests
            if (cc == null)
            {
                TestUtils.ReportFailure("channel is not ICommunicationObject");
            }
            cc.Close();
        }

        public static Task CloseChannelAsync<C>(C channel)
        {
            // Getting a null would indicate an issue in harness
            if (channel == null)
            {
                TestUtils.ReportFailure("channel == null");
            }
            var cc = (channel as IClientChannel);
            // Getting a null would indicate an issue in tests
            if (cc == null)
            {
                TestUtils.ReportFailure("channel is not IClientChannel");
            }
            return Task.Factory.FromAsync(cc.BeginClose, cc.EndClose, TaskCreationOptions.None);
        }

        private static int s_channelPreemptiveTimeoutsFailed = 0;

        public static async Task CloseChannelPreemptiveTimeoutAsync<C>(C channel, int timeout, bool failOnPreemptiveTimeout)
        {
            var closeTask = CloseChannelAsync(channel);
            var timeoutTask = Task.Delay(timeout);
            var taskThatCompleted = await Task.WhenAny(closeTask, timeoutTask);
            if (taskThatCompleted == timeoutTask)
            {
                if (failOnPreemptiveTimeout)
                {
                    TestUtils.ReportFailure("CloseChannelAsync time out! " + (channel as IClientChannel).State);
                    GC.KeepAlive(channel);
                }
                else
                {
                    Console.WriteLine("CloseChannelAsync preemptive timeout: #" + Interlocked.Increment(ref s_channelPreemptiveTimeoutsFailed));
                }
                // don't return until we actually do close it
                await closeTask;
            }
        }

        public static bool IsCommunicationObjectUsable<C>(Object obj)
        {
            var cObj = obj as ICommunicationObject;
            if (cObj != null)
            {
                if (cObj.State < CommunicationState.Closing)
                {
                    return true;
                }
            }
            return false;
        }

        public static int SendTimeoutMs
        {
            get
            {
                return s_sendTimeoutMSeconds;
            }
        }
    }


    public static class TestUtils
    {
        private static Action<string, bool> s_logger = null;
        public static void SetFailureLogger(Action<string, bool> logger)
        {
            s_logger = logger;
        }
        public static bool ReportFailureDontEatException(Exception e)
        {
            ReportFailure(e.ToString());
            GC.KeepAlive(e);
            return false;
        }

        public static bool ReportFailureNoBreakEatException(Exception e)
        {
            ReportFailure(e.ToString(), debugBreak: false);
            return true;
        }

        private static int s_exceptionsEaten = 0;
        public static bool ReportFailureAndBreakIfNotTimeout(Exception e)
        {
            bool eatException = (e is System.TimeoutException);
            if (eatException)
            {
                Console.WriteLine("Eating timeout #" + Interlocked.Increment(ref s_exceptionsEaten));
            }
            ReportFailure(e.ToString(), debugBreak: eatException ? false : true);
            return eatException;
        }

        public static void ReportFailure(string message, bool debugBreak = true)
        {
            Console.WriteLine(message);
            var logger = s_logger;
            logger?.Invoke(message, debugBreak);
            if (debugBreak)
            {
                Debugger.Break();
                GC.KeepAlive(message);
            }
        }
    }
}
