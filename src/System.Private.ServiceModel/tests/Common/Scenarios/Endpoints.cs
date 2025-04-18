// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;

using Infrastructure.Common;

public static partial class Endpoints
{
    private static string GetEndpointAddress(string endpoint, string protocol = "http")
    {
        return ServiceUtilHelper.GetEndpointAddress(endpoint, protocol);
    }

    #region HTTP Addresses
    // HTTP Addresses
    public static string DefaultCustomHttp_Address
    {
        get { return GetEndpointAddress("DefaultCustomHttp.svc/default-custom-http"); }
    }

    public static string HttpBaseAddress_Basic_Text
    {
        get { return GetEndpointAddress("BasicHttp.svc/Text"); }
    }

    public static string HttpBaseAddress_Basic
    {
        get { return GetEndpointAddress("BasicHttp.svc/"); }
    }

    public static string HttpBaseAddress_BasicDecomp
    {
        get { return GetEndpointAddress("BasicHttpWcfDecomp.svc/TestDecompressionEnabled"); }
    }

    public static string HttpBaseAddress_BasicService1
    {
        get { return GetEndpointAddress("BasicService1.svc/Service1"); }
    }

    // Endpoint that relies on post-1.1.0 features
    public static string HttpBaseAddress_4_4_0_Basic
    {
        get { return GetEndpointAddress("BasicHttp_4_4_0.svc/Basic"); }
    }

    public static string HttpBaseAddress_Basic_Soap
    {
        get { return GetEndpointAddress("BasicHttpSoap.svc/Basic"); }
    }

    public static string HttpBaseAddress_NetHttp
    {
        get { return GetEndpointAddress("NetHttp.svc/"); }
    }

    public static string HttpSoap11_Address
    {
        get { return GetEndpointAddress("HttpSoap11.svc/http-soap11"); }
    }

    public static string HttpSoap11WSA10_Address
    {
        get { return GetEndpointAddress("HttpSoap11WSA10.svc/http-Soap11WSA10"); }
    }

    public static string HttpSoap11WSA2004_Address
	{
		get { return GetEndpointAddress("HttpSoap11WSA2004.svc/http-Soap11WSA2004"); }
	}

    public static string HttpSoap12_Address
    {
        get { return GetEndpointAddress("HttpSoap12.svc/http-soap12"); }
    }

    public static string HttpSoap12WSANone_Address
    {
        get { return GetEndpointAddress("HttpSoap12WSANone.svc/http-Soap12WSANone"); }
    }

    public static string HttpSoap12WSA2004_Address
	{
		get { return GetEndpointAddress("HttpSoap12WSA2004.svc/http-Soap12WSA2004"); }
	}

    public static string HttpBinary_Address
    {
        get { return GetEndpointAddress("HttpBinary.svc/http-binary"); }
    }

    public static string HttpProtocolError_Address
    {
        get { return Endpoints.DefaultCustomHttp_Address + "/UnknownProtocolUrl.htm"; }
    }

    public static string HttpBaseAddress_ChannelExtensibility
    {
        get { return GetEndpointAddress("ChannelExtensibility.svc/ChannelExtensibility"); }
    }

    public static string UnderstoodHeaders
    {
        get { return GetEndpointAddress("UnderstoodHeaders.svc/UnderstoodHeaders"); }
    }

    public static string XmlSFAttribute_Address
    {
        get { return GetEndpointAddress("XmlSFAttribute.svc/XmlSFAttribute"); }
    }

    public static string BasicHttpRpcEncSingleNs_Address
    {
        get { return GetEndpointAddress("BasicHttpRpcEncSingleNs.svc/Basic"); }
    }

    public static string BasicHttpRpcLitSingleNs_Address
    {
        get { return GetEndpointAddress("BasicHttpRpcLitSingleNs.svc/Basic"); }
    }

    public static string BasicHttpDocLitSingleNs_Address
    {
        get { return GetEndpointAddress("BasicHttpDocLitSingleNs.svc/Basic"); }
    }

    public static string BasicHttpRpcEncDualNs_Address
    {
        get { return GetEndpointAddress("BasicHttpRpcEncDualNs.svc/Basic"); }
    }

    public static string BasicHttpRpcLitDualNs_Address
    {
        get { return GetEndpointAddress("BasicHttpRpcLitDualNs.svc/Basic"); }
    }

    public static string BasicHttpDocLitDualNs_Address
    {
        get { return GetEndpointAddress("BasicHttpDocLitDualNs.svc/Basic"); }
    }

    public static string BasicHttpRpcEncWithHeaders_Address
    {
        get { return GetEndpointAddress("BasicHttpRpcEncWithHeaders.svc/Basic"); }
    }

    public static string ReliableSession_NetHttp
    {
        get { return GetEndpointAddress("ReliableSessionService.svc/NetHttp"); }
    }

    public static string ReliableOneWaySession_NetHttp
    {
        get { return GetEndpointAddress("ReliableSessionOneWayService.svc/NetHttp"); }
    }

    public static string ReliableSession_WSHttp
    {
        get { return GetEndpointAddress("ReliableSessionService.svc/WSHttp"); }
    }

    public static string ReliableOneWaySession_WSHttp
    {
        get { return GetEndpointAddress("ReliableSessionOneWayService.svc/WSHttp"); }
    }

    public static string WSHttpBindingBaseAddress
    {
        get { return GetEndpointAddress("WSHttp.svc/"); }
    }

    #region WebSocket Addresses
    public static string HttpBaseAddress_NetHttpWebSockets
    {
        get { return GetEndpointAddress("NetHttpWebSockets.svc/NetHttpWebSockets", "ws"); }
    }

    public static string HttpBaseAddress_NetHttpsWebSockets
    {
        get { return GetEndpointAddress("NetHttpsWebSockets.svc/NetHttpsWebSockets", protocol: "wss"); }
    }

    public static string NetHttpWebSocketTransport_Address
    {
        get { return GetEndpointAddress("WebSocketTransport.svc/http-requestreplywebsockets-transportusagealways", protocol: "ws"); }
    }

    public static string NetHttpDuplexWebSocket_Address
    {
        get { return GetEndpointAddress("DuplexWebSocket.svc/http-defaultduplexwebsockets", protocol: "ws"); }
    }

    public static string WebSocketHttpDuplexStreamed_Address
    {
        get { return GetEndpointAddress("WebSocketHttpDuplex.svc/Streamed", protocol: "ws"); }
    }

    public static string WebSocketHttpRequestReplyStreamed_Address
    {
        get { return GetEndpointAddress("WebSocketHttpRequestReply.svc/Streamed", protocol: "ws"); }
    }

    public static string WebSocketHttpRequestReplyBuffered_Address
    {
        get { return GetEndpointAddress("WebSocketHttpRequestReply.svc/Buffered", protocol: "ws"); }
    }

    public static string WebSocketHttpDuplexBuffered_Address
    {
        get { return GetEndpointAddress("WebSocketHttpDuplex.svc/Buffered", protocol: "ws"); }
    }

    public static string WebSocketHttpVerifyWebSocketsUsed_Address
    {
        get { return GetEndpointAddress("WebSocketHttpVerifyWebSocketsUsed.svc/WebSocketHttpVerifyWebSocketsUsed", protocol: "ws"); }
    }
    #endregion WebSocket Addresses

    #region Service Contract Addresses
    public static string ServiceContractAsyncIntOut_Address
    {
        get { return GetEndpointAddress("ServiceContractAsyncIntOut.svc/ServiceContractIntOut"); }
    }

    public static string ServiceContractAsyncUniqueTypeOut_Address
    {
        get { return GetEndpointAddress("ServiceContractAsyncUniqueTypeOut.svc/ServiceContractUniqueTypeOut"); }
    }

    public static string ServiceContractAsyncIntRef_Address
    {
        get { return GetEndpointAddress("ServiceContractAsyncIntRef.svc/ServiceContractIntRef"); }
    }

    public static string ServiceContractAsyncUniqueTypeRef_Address
    {
        get { return GetEndpointAddress("ServiceContractAsyncUniqueTypeRef.svc/ServiceContractAsyncUniqueTypeRef"); }
    }

    public static string ServiceContractSyncUniqueTypeOut_Address
    {
        get { return GetEndpointAddress("ServiceContractSyncUniqueTypeOut.svc/ServiceContractUniqueTypeOutSync"); }
    }

    public static string ServiceContractSyncUniqueTypeRef_Address
    {
        get { return GetEndpointAddress("ServiceContractSyncUniqueTypeRef.svc/ServiceContractUniqueTypeRefSync"); }
    }

    public static string DataContractResolver_Address
    {
        get { return GetEndpointAddress("DataContractResolver.svc/DataContractResolver"); }
    }

    #endregion Service Contract Addresses

    #region Custom Message Encoder Addresses

    public static string CustomTextEncoderBuffered_Address
    {
        get { return GetEndpointAddress("CustomTextEncoderBuffered.svc/http-customtextencoder"); }
    }

    public static string CustomTextEncoderStreamed_Address
    {
        get { return GetEndpointAddress("CustomTextEncoderStreamed.svc/http-customtextencoder-streamed"); }
    }
    #endregion Custom Message Encoder Addresses
    #endregion HTTP Addresses

    #region HTTPS Addresses
    // HTTPS Addresses
    public static string Https_BasicAuth_Address
    {
        get
        {
            return GetEndpointAddress("BasicAuth.svc/https-basic", protocol: "https");
        }
    }

    public static string Https_DigestAuth_Address
    {
        get
        {
            return GetEndpointAddress("DigestAuthentication/HttpsDigest.svc/https-digest", protocol: "https");
        }
    }

    public static string Http_DigestAuth_NoDomain_Address
    {
        get
        {
            return GetEndpointAddress("HttpDigestNoDomain.svc/http-digest-nodomain");
        }
    }

    public static string Https_NtlmAuth_Address
    {
        get
        {
            return GetEndpointAddress("WindowAuthenticationNtlm/HttpsNtlm.svc/https-ntlm", protocol: "https");
        }
    }

    public static string Https_WindowsAuth_Address
    {
        get
        {
            return GetEndpointAddress("WindowAuthenticationNegotiate/HttpsWindows.svc/https-windows", protocol: "https");
        }
    }

    public static string Https_ClientCertificateAuth_Address
    {
        get
        {
            return GetEndpointAddress("ClientCertificateAccepted/HttpsClientCertificate.svc/https-client-certificate", protocol: "https");
        }
    }

    public static string Http_WindowsAuth_Address
    {
        get
        {
            return GetEndpointAddress("WindowAuthenticationNegotiate/HttpWindows.svc/http-windows");
        }
    }

    public static string Https_DefaultBinding_Address_Text
    {
        get
        {
            return GetEndpointAddress("BasicHttps.svc/Text", protocol: "https");
        }
    }

    public static string Https_DefaultBinding_Address
    {
        get
        {
            return GetEndpointAddress("BasicHttps.svc/", protocol: "https");
        }
    }

    public static string HttpsSoap11_Address
    {
        get
        {
            return GetEndpointAddress("HttpsSoap11.svc/https-soap11", protocol: "https");
        }
    }

    public static string HttpsSoap12_Address
    {
        get
        {
            return GetEndpointAddress("HttpsSoap12.svc/https-soap12", protocol: "https");
        }
    }

    public static string HttpBaseAddress_NetHttps_Binary
    {
        get
        {
            return GetEndpointAddress("NetHttps.svc/Binary", protocol: "https");
        }
    }

    public static string HttpBaseAddress_NetHttps
    {
        get
        {
            return GetEndpointAddress("NetHttps.svc/", protocol: "https");
        }
    }

    public static string Https_SecModeTrans_ClientCredTypeNone_ServerCertValModePeerTrust_Address
    {
        get
        {
            return GetEndpointAddress("HttpsCertValModePeerTrust.svc/https-server-cert-valmode-peertrust", protocol: "https");
        }
    }

    public static string Https_SecModeTrans_ClientCredTypeNone_ServerCertValModeChainTrust_Address
    {
        get
        {
            return GetEndpointAddress("HttpsCertValModeChainTrust.svc/https-server-cert-valmode-chaintrust", protocol: "https");
        }
    }

    public static string Https_SecModeTransWithMessCred_ClientCredTypeCert
    {
        get
        {
            return GetEndpointAddress("HttpsTransSecMessCredsCert.svc/https-message-credentials-cert", protocol: "https");
        }
    }

    public static string Https_SecModeTransWithMessCred_ClientCredTypeUserName
    {
        get
        {
            return GetEndpointAddress("HttpsTransSecMessCredsUserName.svc/https-message-credentials-username", protocol: "https");
        }
    }

    public static string Https2007_SecModeTransWithMessCred_ClientCredTypeCert
    {
        get
        {
            return GetEndpointAddress("HttpsTransSecMessCredsCert.svc/https2007-message-credentials-cert", protocol: "https");
        }
    }

    public static string Https2007_SecModeTransWithMessCred_ClientCredTypeUserName
    {
        get
        {
            return GetEndpointAddress("HttpsTransSecMessCredsUserName.svc/https2007-message-credentials-username", protocol: "https");
        }
    }

    public static string BasicHttps_SecModeTransWithMessCred_ClientCredTypeCert
    {
        get
        {
            return GetEndpointAddress("BasicHttpsTransSecMessCredsCert.svc/https-message-credentials-cert", protocol: "https");
        }
    }

    public static string BasicHttpsBinding_SecModeTransWithMessCred_ClientCredTypeCert
    {
        get
        {
            return GetEndpointAddress("BasicHttpsBindingTransSecMessCredsCert.svc/https-transwithmessage-credentials-cert", protocol: "https");
        }
    }

    public static string BasicHttps_SecModeTransWithMessCred_ClientCredTypeUserName
    {
        get
        {
            return GetEndpointAddress("BasicHttpsTransSecMessCredsUserName.svc/https-message-credentials-username", protocol: "https");
        }
    }

    public static string WSFederationAuthorityLocalSTS
    {
        get
        {
            return GetEndpointAddress("LocalSTS.svc/", protocol: "https");
        }
    }

    public static string Https_SecModeTransWithMessCred_ClientCredTypeIssuedTokenSaml2
    {
        get
        {
            return GetEndpointAddress("Saml2IssuedToken.svc/issued-token-using-tls/", protocol: "https");
        }
    }

    #region Secure WebSocket Addresses
    public static string WebSocketHttpsDuplexStreamed_Address
    {
        get
        {
            return GetEndpointAddress("WebSocketHttpsDuplex.svc/Streamed", protocol: "wss");
        }
    }

    public static string WebSocketHttpsRequestReplyBuffered_Address
    {
        get
        {
            return GetEndpointAddress("WebSocketHttpsRequestReply.svc/Buffered", protocol: "wss");
        }
    }

    public static string WebSocketHttpsRequestReplyClientCertAuth_Address
    {
        get
        {
            return GetEndpointAddress("ClientCertificateAccepted/HttpsClientCertificate.svc/WebSocket-client-certificate", protocol: "https");
        }
    }

    public static string WebSocketHttpsDuplexBuffered_Address
    {
        get
        {
            return GetEndpointAddress("WebSocketHttpsDuplex.svc/Buffered", protocol: "wss");
        }
    }
    #endregion Secure WebSocket Addresses
    #endregion  HTTPS Addresses

    #region net.tcp Addresses
    // net.tcp Addresses
    public static string Tcp_DefaultBinding_Address
    {
        get { return GetEndpointAddress("TcpDefault.svc/tcp-default", protocol: "net.tcp"); }
    }

    public static string Tcp_NoSecurity_Address
    {
        get { return GetEndpointAddress("TcpNoSecurity.svc/tcp-nosecurity", protocol: "net.tcp"); }
    }

    public static string Tcp_Streamed_NoSecurity_Address
    {
        get { return GetEndpointAddress("TcpStreamedNoSecurity.svc/tcp-streamed-nosecurity", protocol: "net.tcp"); }
    }

    public static string Tcp_VerifyDNS_Address
    {
        get
        {
            return GetEndpointAddress("TcpVerifyDNS.svc/tcp-VerifyDNS", protocol: "net.tcp");
        }
    }

    public static string Tcp_VerifyDNS_HostName
    {
        get
        {
            return ServiceUtilHelper.ServiceHostName;
        }
    }

    public static string Tcp_ExpiredServerCertResource_Address
    {
        get
        {
            return GetEndpointAddress("TcpExpiredServerCert.svc/tcp-ExpiredServerCert", protocol: "net.tcp");
        }
    }

    public static string Tcp_InvalidEkuServerCertResource_HostName
    {
        get
        {
            return ServiceUtilHelper.ServiceHostName;
        }
    }

    public static string Tcp_InvalidEkuServerCertResource_Address
    {
        get
        {
            return GetEndpointAddress("TcpInvalidEkuServerCert.svc/tcp-InvalidEkuServerCert", protocol: "net.tcp");
        }
    }

    public static string Tcp_RevokedServerCertResource_HostName
    {
        get
        {
            return ServiceUtilHelper.ServiceHostName;
        }
    }

    public static string Tcp_RevokedServerCertResource_Address
    {
        get
        {
            return GetEndpointAddress("TcpRevokedServerCert.svc/tcp-RevokedServerCert", protocol: "net.tcp");
        }
    }

    public static string Tcp_ExpiredServerCertResource_HostName
    {
        get
        {
            return ServiceUtilHelper.ServiceHostName;
        }
    }


    public static string Tcp_NoSecurity_Callback_Address
    {
        get { return GetEndpointAddress("Duplex.svc/tcp-nosecurity-callback", protocol: "net.tcp"); }
    }

    public static string Tcp_CustomBinding_NoSecurity_Text_Address
    {
        get { return GetEndpointAddress("TcpNoSecurityText.svc/tcp-custombinding-nosecurity-text", protocol: "net.tcp"); }
    }

    public static string Tcp_NoSecurity_TaskReturn_Address
    {
        get { return GetEndpointAddress("DuplexChannelCallbackReturn.svc/tcp-nosecurity-taskreturn", protocol: "net.tcp"); }
    }

    public static string Tcp_NoSecurity_DuplexCallback_Address
    {
        get { return GetEndpointAddress("DuplexCallback.svc/tcp-nosecurity-typedproxy-duplexcallback", protocol: "net.tcp"); }
    }

    public static string Tcp_NoSecurity_DataContractDuplexCallback_Address
    {
        get { return GetEndpointAddress("DuplexCallbackDataContractComplexType.svc/tcp-nosecurity-callback", protocol: "net.tcp"); }
    }

    public static string Tcp_NoSecurity_XmlDuplexCallback_Address
    {
        get { return GetEndpointAddress("DuplexCallbackXmlComplexType.svc/tcp-nosecurity-callback", protocol: "net.tcp"); }
    }


    public static string Tcp_CustomBinding_SslStreamSecurity_Address
    {
        get
        {
            return GetEndpointAddress("TcpTransportSecurityWithSsl.svc/tcp-server-ssl-security", protocol: "net.tcp");
        }
    }

    public static string Tcp_CustomBinding_SslStreamSecurity_HostName
    {
        get
        {
            return ServiceUtilHelper.ServiceHostName;
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_Address
    {
        get
        {
            return GetEndpointAddress("TcpTransportSecuritySslClientCredentialTypeCertificate.svc/tcp-server-ssl-security-clientcredentialtype-certificate", protocol: "net.tcp");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_CustomValidation_Address
    {
        get
        {
            return GetEndpointAddress("TcpTransportSecuritySslCustomCertValidation.svc/tcp-server-ssl-security-clientcredentialtype-certificate-customvalidator", protocol: "net.tcp");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_With_ServerAltName_Address
    {
        get
        {
            return GetEndpointAddress("TcpCertificateWithServerAltName.svc/tcp-server-alt-name-cert", protocol: "net.tcp");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_With_CanonicalName_Localhost_Address
    {
        get
        {
            return GetEndpointAddress("TcpCertificateWithSubjectCanonicalNameLocalhost.svc/tcp-server-subject-cn-localhost-cert", protocol: "net.tcp");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_With_CanonicalName_DomainName_Address
    {
        get
        {
            return GetEndpointAddress("TcpCertificateWithSubjectCanonicalNameDomainName.svc/tcp-server-subject-cn-domainname-cert", protocol: "net.tcp");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_With_CanonicalName_Fqdn_Address
    {
        get
        {
            return GetEndpointAddress("TcpCertificateWithSubjectCanonicalNameFqdn.svc/tcp-server-subject-cn-fqdn-cert", protocol: "net.tcp");
        }
    }

    public static string Tcp_Transport_Security_Streamed_Address
    {
        get
        {
            return GetEndpointAddress("WindowAuthenticationNegotiate/TcpTransportSecurityStreamed.svc/tcp-transport-security-streamed", protocol: "net.tcp");
        }
    }

    public static string NetTcp_SecModeTrans_ClientCredTypeNone_ServerCertValModePeerTrust_Address
    {
        get
        {
            return GetEndpointAddress("NetTcpCertValModePeerTrust.svc/nettcp-server-cert-valmode-peertrust", protocol: "net.tcp");
        }
    }

    public static string Tcp_Certificate_Duplex_Address
    {
        get
        {
            return GetEndpointAddress("DuplexCallbackTcpCertificateCredential.svc/tcp-certificate-callback", protocol: "net.tcp");
        }
    }

    public static string Tcp_Session_Tests_Default_Service
    {
        get
        {
            return GetEndpointAddress("SessionTestsDefaultService.svc/tcp-sessions", protocol: "net.tcp");
        }
    }

    public static string Tcp_Session_Tests_Short_Timeout_Service
    {
        get
        {
            return GetEndpointAddress("SessionTestsShortTimeoutService.svc", protocol: "net.tcp");
        }
    }

    public static string Tcp_Session_Tests_Duplex_Service
    {
        get
        {
            return GetEndpointAddress("SessionTestsDuplexService.svc", protocol: "net.tcp");
        }
    }

    public static string TcpSoap11WSA10_Address
    {
        get { return GetEndpointAddress("TcpSoap11WSA10.svc/tcp-Soap11WSA10", protocol: "net.tcp"); }
    }

    public static string Tcp_SecModeTransWithMessCred_ClientCredTypeCert
    {
        get { return GetEndpointAddress("TcpTransSecMessCredsCert.svc/tcp-message-credentials-cert", protocol: "net.tcp"); }
    }

    public static string Tcp_SecModeTransWithMessCred_ClientCredTypeUserName
    {
        get { return GetEndpointAddress("TcpTransSecMessCredsUserName.svc/tcp-message-credentials-username", protocol: "net.tcp"); }
    }

    public static string ReliableSession_NetTcp
    {
        get { return GetEndpointAddress("ReliableSessionService.svc/NetTcp", protocol: "net.tcp"); }
    }

    public static string ReliableOneWaySession_NetTcp
    {
        get { return GetEndpointAddress("ReliableSessionOneWayService.svc/NetTcp", protocol: "net.tcp"); }
    }

    public static string ReliableDuplexSession_NetTcp
    {
        get { return GetEndpointAddress("ReliableSessionDuplexService.svc/NetTcp", protocol: "net.tcp"); }
    }

    public static string DuplexCallbackConcurrencyMode_Address
    {
        get { return GetEndpointAddress("DuplexCallbackConcurrencyMode.svc/tcp", protocol: "net.tcp"); }
    }

    public static string DuplexCallbackDebugBehavior_Address
    {
        get { return GetEndpointAddress("DuplexCallbackDebugBehavior.svc/tcp", protocol: "net.tcp"); }
    }

    public static string DuplexCallbackErrorHandler_Address
    {
        get { return GetEndpointAddress("DuplexCallbackErrorHandler.svc/tcp", protocol: "net.tcp"); }
    }
    #endregion net.tcp Addresses

    #region net.pipe Addresses
    // net.pipe addresses
    public static string NamedPipe_NoSecurity_Address
    {
        get { return GetEndpointAddress("TcpNoSecurity.svc/namedpipe-nosecurity", protocol: "net.pipe"); }
    }

    public static string NamedPipe_DefaultBinding_Address
    {
        get { return GetEndpointAddress("TcpDefault.svc/namedpipe-default", protocol: "net.pipe"); }
    }
    #endregion
}

