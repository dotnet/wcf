// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;

using Infrastructure.Common;

public static partial class Endpoints
{
    private static X509Certificate2 rootCert;
    private static X509Certificate2 clientCert;
    private static object rootCertLock = new object();
    private static object clientCertLock = new object();
    private static string serviceHostName = string.Empty;
    //Install Root certificate, this is required for all https test
    private static void EnsureRootCertificateInstalled()
    {
        lock (rootCertLock)
        {
            if (rootCert == null)
            {
                BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
                ChannelFactory<IUtil> factory = null;
                IUtil serviceProxy = null;
                try
                {
                    factory = new ChannelFactory<IUtil>(basicHttpBinding, new EndpointAddress(ServiceUtil_Address));
                    serviceProxy = factory.CreateChannel();
                    rootCert = new X509Certificate2(serviceProxy.GetRootCert(false));

                    if (rootCert == null)
                    {
                        //throw
                        throw new Exception("Failed to obtain root cert from the server");
                    }

                    BridgeClientCertificateManager.InstallCertificateToRootStore(rootCert);
                }
                finally
                {
                    ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
                }
            }
        }
    }

    //Install client certificate
    private static void EnsureLocalClientCertifciateInstalled()
    {
        EnsureRootCertificateInstalled();
        lock (clientCertLock)
        {
            if (clientCert == null)
            {
                BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
                ChannelFactory<IUtil> factory = null;
                IUtil serviceProxy = null;
                try
                {
                    factory = new ChannelFactory<IUtil>(basicHttpBinding, new EndpointAddress(ServiceUtil_Address));
                    serviceProxy = factory.CreateChannel();
                    clientCert = new X509Certificate2(serviceProxy.GetClientCert(false), "test", X509KeyStorageFlags.PersistKeySet);
                    if (clientCert == null)
                    {
                        //throw
                        throw new Exception("Failed to obtain client cert from the server");
                    }

                    BridgeClientCertificateManager.AddToStoreIfNeeded(StoreName.My, StoreLocation.CurrentUser, clientCert);
                    BridgeClientCertificateManager.LocalCertThumbprint = clientCert.Thumbprint;
                }
                finally
                {
                    ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
                }
            }
        }
    }

    private static string ServiceHostName
    {
        get
        {
            //Get the host name from server
            if (string.IsNullOrEmpty(serviceHostName))
            {
                BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
                ChannelFactory<IUtil> factory = null;
                IUtil serviceProxy = null;
                try
                {
                    factory = new ChannelFactory<IUtil>(basicHttpBinding, new EndpointAddress(ServiceUtil_Address));
                    serviceProxy = factory.CreateChannel();
                    serviceHostName = serviceProxy.GetFQDN();
                }
                finally
                {
                    ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
                }
            }

            return serviceHostName;
        }
    }

    private static bool IISHosted
    {
        get
        {
            // We assume the self hosted service does not have test service base addess, only the host name passed
            // This will satisfy all current requirements
            if (TestProperties.GetProperty(TestProperties.ServiceUri_PropertyName).Contains("/"))
            {
                return true;
            };

            return false;
        }
    }

    private static Uri BuildBaseUri(string protocol)
    {
        var builder = new UriBuilder();
        builder.Host = TestProperties.GetProperty(TestProperties.ServiceUri_PropertyName);
        builder.Scheme = protocol;

        if (!IISHosted)
        {
            switch (protocol)
            {
                case "http":
                    builder.Port = int.Parse(TestProperties.GetProperty(TestProperties.BridgeHttpPort_PropertyName));
                    break;
                case "ws":
                    builder.Port = int.Parse(TestProperties.GetProperty(TestProperties.BridgeWebSocketPort_PropertyName));
                    builder.Scheme = "http";
                    break;
                case "https":
                     builder.Port = int.Parse(TestProperties.GetProperty(TestProperties.BridgeHttpsPort_PropertyName));
                    break;
                case "wss":
                    builder.Port = int.Parse(TestProperties.GetProperty(TestProperties.BridgeSecureWebSocketPort_PropertyName));
                    builder.Scheme = "https";
                    break;
                case "net.tcp":
                    builder.Port = int.Parse(TestProperties.GetProperty(TestProperties.BridgeTcpPort_PropertyName));
                    break;
                default:
                    break;
            }
        }

        return builder.Uri;
    }
    public static string GetEndpointAdddress(string endpoint, string protocol = "http")
    {
        return string.Format(@"{0}/{1}", BuildBaseUri(protocol), endpoint);
    }

    public static string ServiceUtil_Address
    {
        get { return GetEndpointAdddress("Util.svc//Util"); }
    }
    // HTTP Addresses
    public static string DefaultCustomHttp_Address
    {
        get { return GetEndpointAdddress("DefaultCustomHttp.svc//default-custom-http"); }
    }

    public static string HttpBaseAddress_Basic
    {
        get { return GetEndpointAdddress("BasicHttp.svc//Basic"); }
    }

    public static string HttpBaseAddress_NetHttp
    {
        get { return GetEndpointAdddress("NetHttp.svc//NetHttp"); }
    }

    public static string HttpSoap11_Address
    {
        get { return GetEndpointAdddress("HttpSoap11.svc//http-soap11"); }
    }

    public static string HttpSoap12_Address
    {
        get { return GetEndpointAdddress("HttpSoap12.svc//http-soap12"); }
    }

    public static string HttpBinary_Address
    {
        get { return GetEndpointAdddress("HttpBinary.svc//http-binary"); }
    }

    public static string NetHttpWebSocketTransport_Address
    {
        get { return GetEndpointAdddress("WebSocketTransport.svc//http-requestreplywebsockets-transportusagealways", protocol:"ws"); }
    }

    public static string NetHttpDuplexWebSocket_Address
    {
        get { return GetEndpointAdddress("DuplexWebSocket.svc//http-defaultduplexwebsockets", protocol: "ws"); }
    }
    
    public static string HttpProtocolError_Address
    {
        get { return Endpoints.DefaultCustomHttp_Address + "/UnknownProtocolUrl.htm"; }
    }

    public static string WebSocketHttpDuplexBinaryStreamed_Address
    {
        get { return GetEndpointAdddress("WebSocketHttpDuplexBinaryStreamed.svc//WebSocketHttpDuplexBinaryStreamedResource", protocol: "ws"); }
    }

    public static string WebSocketHttpRequestReplyBinaryStreamed_Address
    {
        get { return GetEndpointAdddress("WebSocketHttpRequestReplyBinaryStreamed.svc//WebSocketHttpRequestReplyBinaryStreamedResource", protocol: "ws");}
    }

    public static string WebSocketHttpRequestReplyTextStreamed_Address
    {
        get { return GetEndpointAdddress("WebSocketHttpRequestReplyTextStreamed.svc//WebSocketHttpRequestReplyTextStreamedResource", protocol: "ws"); }
    }

    public static string WebSocketHttpDuplexTextStreamed_Address
    {
        get { return GetEndpointAdddress("WebSocketHttpDuplexTextStreamed.svc//WebSocketHttpDuplexTextStreamedResource", protocol: "ws"); }
    }

    public static string WebSocketHttpRequestReplyTextBuffered_Address
    {
        get { return GetEndpointAdddress("WebSocketHttpRequestReplyTextBuffered.svc//WebSocketHttpRequestReplyTextBufferedResource", protocol: "ws"); }
    }

    public static string WebSocketHttpRequestReplyBinaryBuffered_Address
    {
        get { return GetEndpointAdddress("WebSocketHttpRequestReplyBinaryBuffered.svc//WebSocketHttpRequestReplyBinaryBufferedResource", protocol: "ws"); }
    }

    public static string WebSocketHttpDuplexTextBuffered_Address
    {
        get { return GetEndpointAdddress("WebSocketHttpDuplexTextBuffered.svc//WebSocketHttpDuplexTextBufferedResource", protocol: "ws"); }
    }

    public static string WebSocketHttpDuplexBinaryBuffered_Address
    {
        get { return GetEndpointAdddress("WebSocketHttpDuplexBinaryBuffered.svc//WebSocketHttpDuplexBinaryBufferedResource", protocol: "ws"); }
    }

    public static string ServiceContractAsyncIntOut_Address
    {
        get { return GetEndpointAdddress("ServiceContractAsyncIntOut.svc//ServiceContractIntOut"); }
    }

    public static string ServiceContractAsyncUniqueTypeOut_Address
    {
        get { return GetEndpointAdddress("ServiceContractAsyncUniqueTypeOut.svc//ServiceContractUniqueTypeOut"); }
    }

    public static string ServiceContractAsyncIntRef_Address
    {
        get { return GetEndpointAdddress("ServiceContractAsyncIntRef.svc//ServiceContractIntRef"); }
    }

    public static string ServiceContractAsyncUniqueTypeRef_Address
    {
        get { return GetEndpointAdddress("ServiceContractAsyncUniqueTypeRef.svc//ServiceContractAsyncUniqueTypeRef"); }
    }

    public static string ServiceContractSyncUniqueTypeOut_Address
    {
        get { return GetEndpointAdddress("ServiceContractSyncUniqueTypeOut.svc//ServiceContractUniqueTypeOutSync"); }
    }

    public static string ServiceContractSyncUniqueTypeRef_Address
    {
        get { return GetEndpointAdddress("ServiceContractSyncUniqueTypeRef.svc//ServiceContractUniqueTypeRefSync"); }
    }

    // HTTPS Addresses
    public static string Https_BasicAuth_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("BasicAuth.svc//https-basic", protocol:"https");
        } 
    }

    public static string Https_DigestAuth_Address
    {
        get
        {
            EnsureRootCertificateInstalled();
            return GetEndpointAdddress("HttpsDigest.svc//https-digest", protocol: "https");
        }
    }

    public static string Http_DigestAuth_NoDomain_Address
    {
        get
        {
            return GetEndpointAdddress("HttpDigestNoDomain.svc//");
        }
    }

    public static string Https_NtlmAuth_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("HttpsNtlm.svc//https-ntlm.svc//");
        }
    }

    public static string Https_WindowsAuth_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("HttpsWindows.svc//https-windows", protocol: "https");
        }
    }

    public static string Https_ClientCertificateAuth_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("HttpsClientCertificate.svc//https-client-certificate", protocol: "https");
        }
    }

    public static string Http_WindowsAuth_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("HttpWindows.svc//http-windows");
        }
    }

    public static string Https_DefaultBinding_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("BasicHttps.svc//basicHttps", protocol: "https");
        }
    }

    public static string HttpsSoap11_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("HttpsSoap11.svc//https-soap11", protocol: "https");
        }
    }

    public static string HttpsSoap12_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("HttpsSoap12.svc//https-soap12", protocol: "https");
        }
    }

    public static string WebSocketHttpsDuplexBinaryStreamed_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("WebSocketHttpsDuplexBinaryStreamed.svc//WebSocketHttpsDuplexBinaryStreamedResource", protocol: "wss");
        }
    }

    public static string WebSocketHttpsDuplexTextStreamed_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("WebSocketHttpsDuplexTextStreamed.svc//WebSocketHttpsDuplexTextStreamedResource", protocol: "wss");
        }
    }

    public static string WebSocketHttpsRequestReplyBinaryBuffered_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("WebSocketHttpsRequestReplyBinaryBuffered.svc//WebSocketHttpsRequestReplyBinaryBufferedResource", protocol: "wss");
        }
    }

    public static string WebSocketHttpsRequestReplyTextBuffered_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("WebSocketHttpsRequestReplyTextBuffered.svc//WebSocketHttpsRequestReplyTextBufferedResource", protocol: "wss");
        }
    }

    public static string WebSocketHttpsDuplexBinaryBuffered_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("WebSocketHttpsDuplexBinaryBuffered.svc//WebSocketHttpsDuplexBinaryBufferedResource", protocol: "wss");
        }
    }

    public static string WebSocketHttpsDuplexTextBuffered_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("WebSocketHttpsDuplexTextBuffered.svc//WebSocketHttpsDuplexTextBufferedResource", protocol: "wss");
        }
    }

    // net.tcp Addresses
    public static string Tcp_DefaultBinding_Address
    {
        get { return GetEndpointAdddress("TcpDefault.svc//tcp-default", protocol: "net.tcp"); }
    }

    public static string Tcp_NoSecurity_Address
    {
        get { return GetEndpointAdddress("TcpNoSecurity.svc//tcp-nosecurity", protocol: "net.tcp"); }
    }

    public static string Tcp_Streamed_NoSecurity_Address
    {
        get { return GetEndpointAdddress("TcpStreamedNoSecurity.svc//tcp-streamed-nosecurity", protocol: "net.tcp"); }
    }

    public static string Tcp_VerifyDNS_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("TcpVerifyDNS.svc//tcp-VerifyDNS", protocol: "net.tcp");
        }
    }

    public static string Tcp_VerifyDNS_HostName
    {
        get
        {
            return ServiceHostName;
        }
    }

    public static string Tcp_ExpiredServerCertResource_Address
    {
        get
        {
            EnsureRootCertificateInstalled();
            return GetEndpointAdddress("TcpExpiredServerCert.svc//tcp-ExpiredServerCert", protocol: "net.tcp");
        }
    }

    public static string Tcp_RevokedServerCertResource_HostName
    {
        get
        {
            return ServiceHostName;
        }
    }

    public static string Tcp_RevokedServerCertResource_Address
    {
        get
        {
            EnsureRootCertificateInstalled();
            return GetEndpointAdddress("TcpRevokedServerCert.svc//tcp-RevokedServerCert", protocol: "net.tcp");
        }
    }

    public static string Tcp_ExpiredServerCertResource_HostName
    {
        get
        {
            return ServiceHostName;
        }
    }


    public static string Tcp_NoSecurity_Callback_Address
    {
        get { return GetEndpointAdddress("Duplex.svc//tcp-nosecurity-callback", protocol: "net.tcp"); }
    }

    public static string Tcp_CustomBinding_NoSecurity_Text_Address
    {
        get { return GetEndpointAdddress("TcpNoSecurityText.svc//tcp-custombinding-nosecurity-text", protocol: "net.tcp"); }
    }

    public static string Tcp_NoSecurity_TaskReturn_Address
    {
        get { return GetEndpointAdddress("DuplexChannelCallbackReturn.svc//tcp-nosecurity-taskreturn", protocol: "net.tcp"); }
    }

    public static string Tcp_NoSecurity_DuplexCallback_Address
    {
        get { return GetEndpointAdddress("DuplexCallback.svc//tcp-nosecurity-typedproxy-duplexcallback", protocol: "net.tcp"); }
    }

    public static string Tcp_NoSecurity_DataContractDuplexCallback_Address
    {
        get { return GetEndpointAdddress("DuplexCallbackDataContractComplexType.svc//tcp-nosecurity-callback", protocol: "net.tcp"); }
    }

    public static string Tcp_NoSecurity_XmlDuplexCallback_Address
    {
        get { return GetEndpointAdddress("DuplexCallbackXmlComplexType.svc//tcp-nosecurity-callback", protocol: "net.tcp"); }
    }


    public static string Tcp_CustomBinding_SslStreamSecurity_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("TcpTransportSecurityWithSsl.svc//tcp-server-ssl-security", protocol: "net.tcp");
        }
    }

    public static string Tcp_CustomBinding_SslStreamSecurity_HostName
    {
        get
        {
            return ServiceHostName;
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("TcpTransportSecuritySslClientCredentialTypeCertificate.svc//tcp-server-ssl-security-clientcredentialtype-certificate", protocol: "net.tcp");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_CustomValidation_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("TcpTransportSecuritySslCustomCertValidation.svc//tcp-server-ssl-security-clientcredentialtype-certificate-customvalidator", protocol: "net.tcp");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_With_ServerAltName_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("TcpCertificateWithServerAltName.svc//tcp-server-alt-name-cert", protocol: "net.tcp");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_With_CanonicalName_Localhost_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("TcpCertificateWithSubjectCanonicalNameLocalhost.svc//tcp-server-subject-cn-localhost-cert", protocol: "net.tcp");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_With_CanonicalName_DomainName_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("TcpCertificateWithSubjectCanonicalNameDomainName.svc//tcp-server-subject-cn-domainname-cert", protocol: "net.tcp");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_With_CanonicalName_Fqdn_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("TcpCertificateWithSubjectCanonicalNameFqdn.svc//tcp-server-subject-cn-fqdn-cert", protocol: "net.tcp");
        }
    }

    public static string Tcp_Transport_Security_Streamed_Address
    {
        get
        {
            EnsureLocalClientCertifciateInstalled();
            return GetEndpointAdddress("TcpTransportSecurityStreamed.svc//tcp-transport-security-streamed", protocol: "net.tcp");
        }
    }
}
