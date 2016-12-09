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
        get { return GetEndpointAddress("DefaultCustomHttp.svc//default-custom-http"); }
    }

    public static string HttpBaseAddress_Basic
    {
        get { return GetEndpointAddress("BasicHttp.svc//Basic"); }
    }

    // Endpoint that relies on post-1.1.0 features
    public static string HttpBaseAddress_4_4_0_Basic
    {
        get { return GetEndpointAddress("BasicHttp_4_4_0.svc//Basic"); }
    }

    public static string HttpBaseAddress_NetHttp
    {
        get { return GetEndpointAddress("NetHttp.svc//NetHttp"); }
    }

    public static string HttpBaseAddress_NetHttpWebSockets
    {
        get { return GetEndpointAddress("NetHttpWebSockets.svc//NetHttpWebSockets"); }
    }

    public static string HttpSoap11_Address
    {
        get { return GetEndpointAddress("HttpSoap11.svc//http-soap11"); }
    }

    public static string HttpSoap12_Address
    {
        get { return GetEndpointAddress("HttpSoap12.svc//http-soap12"); }
    }

    public static string HttpBinary_Address
    {
        get { return GetEndpointAddress("HttpBinary.svc//http-binary"); }
    }

    public static string HttpProtocolError_Address
    {
        get { return Endpoints.DefaultCustomHttp_Address + "/UnknownProtocolUrl.htm"; }
    }

    public static string HttpBaseAddress_ChannelExtensibility
    {
        get { return GetEndpointAddress("ChannelExtensibility.svc//ChannelExtensibility"); }
    }

    #region WebSocket Addresses
    public static string NetHttpWebSocketTransport_Address
    {
        get { return GetEndpointAddress("WebSocketTransport.svc//http-requestreplywebsockets-transportusagealways", protocol: "ws"); }
    }

    public static string NetHttpDuplexWebSocket_Address
    {
        get { return GetEndpointAddress("DuplexWebSocket.svc//http-defaultduplexwebsockets", protocol: "ws"); }
    }

    public static string WebSocketHttpDuplexBinaryStreamed_Address
    {
        get { return GetEndpointAddress("WebSocketHttpDuplexBinaryStreamed.svc//WebSocketHttpDuplexBinaryStreamedResource", protocol: "ws"); }
    }

    public static string WebSocketHttpRequestReplyBinaryStreamed_Address
    {
        get { return GetEndpointAddress("WebSocketHttpRequestReplyBinaryStreamed.svc//WebSocketHttpRequestReplyBinaryStreamedResource", protocol: "ws"); }
    }

    public static string WebSocketHttpRequestReplyTextStreamed_Address
    {
        get { return GetEndpointAddress("WebSocketHttpRequestReplyTextStreamed.svc//WebSocketHttpRequestReplyTextStreamedResource", protocol: "ws"); }
    }

    public static string WebSocketHttpDuplexTextStreamed_Address
    {
        get { return GetEndpointAddress("WebSocketHttpDuplexTextStreamed.svc//WebSocketHttpDuplexTextStreamedResource", protocol: "ws"); }
    }

    public static string WebSocketHttpRequestReplyTextBuffered_Address
    {
        get { return GetEndpointAddress("WebSocketHttpRequestReplyTextBuffered.svc//WebSocketHttpRequestReplyTextBufferedResource", protocol: "ws"); }
    }

    public static string WebSocketHttpRequestReplyBinaryBuffered_Address
    {
        get { return GetEndpointAddress("WebSocketHttpRequestReplyBinaryBuffered.svc//WebSocketHttpRequestReplyBinaryBufferedResource", protocol: "ws"); }
    }

    public static string WebSocketHttpDuplexTextBuffered_Address
    {
        get { return GetEndpointAddress("WebSocketHttpDuplexTextBuffered.svc//WebSocketHttpDuplexTextBufferedResource", protocol: "ws"); }
    }

    public static string WebSocketHttpDuplexBinaryBuffered_Address
    {
        get { return GetEndpointAddress("WebSocketHttpDuplexBinaryBuffered.svc//WebSocketHttpDuplexBinaryBufferedResource", protocol: "ws"); }
    }

    public static string WebSocketHttpVerifyWebSocketsUsed_Address
    {
        get { return GetEndpointAddress("WebSocketHttpVerifyWebSocketsUsed.svc//WebSocketHttpVerifyWebSocketsUsed", protocol: "ws"); }
    }
    #endregion WebSocket Addresses

    #region Service Contract Addresses
    public static string ServiceContractAsyncIntOut_Address
    {
        get { return GetEndpointAddress("ServiceContractAsyncIntOut.svc//ServiceContractIntOut"); }
    }

    public static string ServiceContractAsyncUniqueTypeOut_Address
    {
        get { return GetEndpointAddress("ServiceContractAsyncUniqueTypeOut.svc//ServiceContractUniqueTypeOut"); }
    }

    public static string ServiceContractAsyncIntRef_Address
    {
        get { return GetEndpointAddress("ServiceContractAsyncIntRef.svc//ServiceContractIntRef"); }
    }

    public static string ServiceContractAsyncUniqueTypeRef_Address
    {
        get { return GetEndpointAddress("ServiceContractAsyncUniqueTypeRef.svc//ServiceContractAsyncUniqueTypeRef"); }
    }

    public static string ServiceContractSyncUniqueTypeOut_Address
    {
        get { return GetEndpointAddress("ServiceContractSyncUniqueTypeOut.svc//ServiceContractUniqueTypeOutSync"); }
    }

    public static string ServiceContractSyncUniqueTypeRef_Address
    {
        get { return GetEndpointAddress("ServiceContractSyncUniqueTypeRef.svc//ServiceContractUniqueTypeRefSync"); }
    }

    public static string DataContractResolver_Address
    {
        get { return GetEndpointAddress("DataContractResolver.svc//DataContractResolver"); }
    }

    #endregion Service Contract Addresses

    #region Custom Message Encoder Addresses

    public static string CustomTextEncoderBuffered_Address
    {
        get { return GetEndpointAddress("CustomTextEncoderBuffered.svc//http-customtextencoder"); }
    }

    public static string CustomTextEncoderStreamed_Address
    {
        get { return GetEndpointAddress("CustomTextEncoderStreamed.svc//http-customtextencoder-streamed"); }
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
            return GetEndpointAddress("WindowAuthenticationNtlm/HttpsNtlm.svc//https-ntlm", protocol: "https");
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

    public static string Https_DefaultBinding_Address
    {
        get
        {
            return GetEndpointAddress("BasicHttps.svc//basicHttps", protocol: "https");
        }
    }

    public static string HttpsSoap11_Address
    {
        get
        {
            return GetEndpointAddress("HttpsSoap11.svc//https-soap11", protocol: "https");
        }
    }

    public static string HttpsSoap12_Address
    {
        get
        {
            return GetEndpointAddress("HttpsSoap12.svc//https-soap12", protocol: "https");
        }
    }

    public static string HttpBaseAddress_NetHttps
    {
        get
        {
            return GetEndpointAddress("NetHttps.svc//NetHttps", protocol: "https");
        }
    }

    public static string HttpBaseAddress_NetHttpsWebSockets
    {
        get
        {
            return GetEndpointAddress("NetHttpsWebSockets.svc//NetHttpsWebSockets", protocol: "https");
        }
    }

    public static string Https_SecModeTrans_ClientCredTypeNone_ServerCertValModePeerTrust_Address
    {
        get
        {
            return GetEndpointAddress("HttpsCertValModePeerTrust.svc//https-server-cert-valmode-peertrust", protocol: "https");
        }
    }

    public static string Https_SecModeTrans_ClientCredTypeNone_ServerCertValModeChainTrust_Address
    {
        get
        {
            return GetEndpointAddress("HttpsCertValModeChainTrust.svc//https-server-cert-valmode-chaintrust", protocol: "https");
        }
    }

    #region Secure WebSocket Addresses
    public static string WebSocketHttpsDuplexBinaryStreamed_Address
    {
        get
        {
            return GetEndpointAddress("WebSocketHttpsDuplexBinaryStreamed.svc//WebSocketHttpsDuplexBinaryStreamedResource", protocol: "wss");
        }
    }

    public static string WebSocketHttpsDuplexTextStreamed_Address
    {
        get
        {
            return GetEndpointAddress("WebSocketHttpsDuplexTextStreamed.svc//WebSocketHttpsDuplexTextStreamedResource", protocol: "wss");
        }
    }

    public static string WebSocketHttpsRequestReplyBinaryBuffered_Address
    {
        get
        {
            return GetEndpointAddress("WebSocketHttpsRequestReplyBinaryBuffered.svc//WebSocketHttpsRequestReplyBinaryBufferedResource", protocol: "wss");
        }
    }

    public static string WebSocketHttpsRequestReplyTextBuffered_Address
    {
        get
        {
            return GetEndpointAddress("WebSocketHttpsRequestReplyTextBuffered.svc//WebSocketHttpsRequestReplyTextBufferedResource", protocol: "wss");
        }
    }

    public static string WebSocketHttpsDuplexBinaryBuffered_Address
    {
        get
        {
            return GetEndpointAddress("WebSocketHttpsDuplexBinaryBuffered.svc//WebSocketHttpsDuplexBinaryBufferedResource", protocol: "wss");
        }
    }

    public static string WebSocketHttpsDuplexTextBuffered_Address
    {
        get
        {
            return GetEndpointAddress("WebSocketHttpsDuplexTextBuffered.svc//WebSocketHttpsDuplexTextBufferedResource", protocol: "wss");
        }
    }
    #endregion Secure WebSocket Addresses
    #endregion  HTTPS Addresses

    #region net.tcp Addresses
    // net.tcp Addresses
    public static string Tcp_DefaultBinding_Address
    {
        get { return GetEndpointAddress("TcpDefault.svc//tcp-default", protocol: "net.tcp"); }
    }

    public static string Tcp_NoSecurity_Address
    {
        get { return GetEndpointAddress("TcpNoSecurity.svc//tcp-nosecurity", protocol: "net.tcp"); }
    }

    public static string Tcp_Streamed_NoSecurity_Address
    {
        get { return GetEndpointAddress("TcpStreamedNoSecurity.svc//tcp-streamed-nosecurity", protocol: "net.tcp"); }
    }

    public static string Tcp_VerifyDNS_Address
    {
        get
        {
            return GetEndpointAddress("TcpVerifyDNS.svc//tcp-VerifyDNS", protocol: "net.tcp");
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
            return GetEndpointAddress("TcpExpiredServerCert.svc//tcp-ExpiredServerCert", protocol: "net.tcp");
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
            return GetEndpointAddress("TcpRevokedServerCert.svc//tcp-RevokedServerCert", protocol: "net.tcp");
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
        get { return GetEndpointAddress("Duplex.svc//tcp-nosecurity-callback", protocol: "net.tcp"); }
    }

    public static string Tcp_CustomBinding_NoSecurity_Text_Address
    {
        get { return GetEndpointAddress("TcpNoSecurityText.svc//tcp-custombinding-nosecurity-text", protocol: "net.tcp"); }
    }

    public static string Tcp_NoSecurity_TaskReturn_Address
    {
        get { return GetEndpointAddress("DuplexChannelCallbackReturn.svc//tcp-nosecurity-taskreturn", protocol: "net.tcp"); }
    }

    public static string Tcp_NoSecurity_DuplexCallback_Address
    {
        get { return GetEndpointAddress("DuplexCallback.svc//tcp-nosecurity-typedproxy-duplexcallback", protocol: "net.tcp"); }
    }

    public static string Tcp_NoSecurity_DataContractDuplexCallback_Address
    {
        get { return GetEndpointAddress("DuplexCallbackDataContractComplexType.svc//tcp-nosecurity-callback", protocol: "net.tcp"); }
    }

    public static string Tcp_NoSecurity_XmlDuplexCallback_Address
    {
        get { return GetEndpointAddress("DuplexCallbackXmlComplexType.svc//tcp-nosecurity-callback", protocol: "net.tcp"); }
    }


    public static string Tcp_CustomBinding_SslStreamSecurity_Address
    {
        get
        {
            return GetEndpointAddress("TcpTransportSecurityWithSsl.svc//tcp-server-ssl-security", protocol: "net.tcp");
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
            return GetEndpointAddress("TcpTransportSecuritySslClientCredentialTypeCertificate.svc//tcp-server-ssl-security-clientcredentialtype-certificate", protocol: "net.tcp");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_CustomValidation_Address
    {
        get
        {
            return GetEndpointAddress("TcpTransportSecuritySslCustomCertValidation.svc//tcp-server-ssl-security-clientcredentialtype-certificate-customvalidator", protocol: "net.tcp");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_With_ServerAltName_Address
    {
        get
        {
            return GetEndpointAddress("TcpCertificateWithServerAltName.svc//tcp-server-alt-name-cert", protocol: "net.tcp");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_With_CanonicalName_Localhost_Address
    {
        get
        {
            return GetEndpointAddress("TcpCertificateWithSubjectCanonicalNameLocalhost.svc//tcp-server-subject-cn-localhost-cert", protocol: "net.tcp");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_With_CanonicalName_DomainName_Address
    {
        get
        {
            return GetEndpointAddress("TcpCertificateWithSubjectCanonicalNameDomainName.svc//tcp-server-subject-cn-domainname-cert", protocol: "net.tcp");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_With_CanonicalName_Fqdn_Address
    {
        get
        {
            return GetEndpointAddress("TcpCertificateWithSubjectCanonicalNameFqdn.svc//tcp-server-subject-cn-fqdn-cert", protocol: "net.tcp");
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
            return GetEndpointAddress("NetTcpCertValModePeerTrust.svc//nettcp-server-cert-valmode-peertrust", protocol: "net.tcp");
        }
    }

    public static string Tcp_Certificate_Duplex_Address
    {
        get
        {
            return GetEndpointAddress("DuplexCallbackTcpCertificateCredential.svc/tcp-certificate-callback", protocol: "net.tcp");
        }
    }

    #endregion net.tcp Addresses
}

