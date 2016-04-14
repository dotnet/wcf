// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Infrastructure.Common;

public static partial class Endpoints
{
    #region HTTP Addresses
    // HTTP Addresses
    public static string DefaultCustomHttp_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.DefaultCustomHttpResource"); }
    }

    public static string HttpBaseAddress_Basic
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.BasicHttpResource"); }
    }

    public static string HttpBaseAddress_NetHttp
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.NetHttpResource"); }
    }

    public static string HttpSoap11_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.HttpSoap11Resource"); }
    }

    public static string HttpSoap12_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.HttpSoap12Resource"); }
    }

    public static string HttpBinary_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.HttpBinaryResource"); }
    }

    public static string HttpProtocolError_Address
    {
        get { return Endpoints.DefaultCustomHttp_Address + "/UnknownProtocolUrl.htm"; }
    }

    #region WebSocket Addresses
    public static string NetHttpWebSocketTransport_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.WebSocketTransportResource"); }
    }

    public static string NetHttpDuplexWebSocket_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.DuplexWebSocketResource"); }
    }

    public static string WebSocketHttpDuplexBinaryStreamed_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.WebSocketHttpDuplexBinaryStreamedResource"); }
    }

    public static string WebSocketHttpRequestReplyBinaryStreamed_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.WebSocketHttpRequestReplyBinaryStreamedResource"); }
    }

    public static string WebSocketHttpRequestReplyTextStreamed_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.WebSocketHttpRequestReplyTextStreamedResource"); }
    }

    public static string WebSocketHttpDuplexTextStreamed_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.WebSocketHttpDuplexTextStreamedResource"); }
    }

    public static string WebSocketHttpRequestReplyTextBuffered_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.WebSocketHttpRequestReplyTextBufferedResource"); }
    }

    public static string WebSocketHttpRequestReplyBinaryBuffered_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.WebSocketHttpRequestReplyBinaryBufferedResource"); }
    }

    public static string WebSocketHttpDuplexTextBuffered_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.WebSocketHttpDuplexTextBufferedResource"); }
    }

    public static string WebSocketHttpDuplexBinaryBuffered_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.WebSocketHttpDuplexBinaryBufferedResource"); }
    }
    #endregion WebSocket Addresses

    #region Service Contract Addresses
    public static string ServiceContractAsyncIntOut_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.ServiceContractAsyncIntOutResource"); }
    }

    public static string ServiceContractAsyncUniqueTypeOut_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.ServiceContractAsyncUniqueTypeOutResource"); }
    }

    public static string ServiceContractAsyncIntRef_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.ServiceContractAsyncIntRefResource"); }
    }

    public static string ServiceContractAsyncUniqueTypeRef_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.ServiceContractAsyncUniqueTypeRefResource"); }
    }

    public static string ServiceContractSyncUniqueTypeOut_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.ServiceContractSyncUniqueTypeOutResource"); }
    }

    public static string ServiceContractSyncUniqueTypeRef_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.ServiceContractSyncUniqueTypeRefResource"); }
    }
    #endregion Service Contract Addresses

    #region Custom Message Encoder Addresses

    public static string CustomTextEncoderBuffered_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.CustomTextEncoderBufferedResource"); }
    }

    public static string CustomTextEncoderStreamed_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.CustomTextEncoderStreamedResource"); }
    }
    #endregion Custom Message Encoder Addresses
    #endregion HTTP Addresses

    #region HTTPS Addresses
    // HTTPS Addresses
    public static string Https_BasicAuth_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallRootCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.BasicAuthResource");
        } 
    }

    public static string Https_DigestAuth_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallRootCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.HttpsDigestResource");
        }
    }

    public static string Http_DigestAuth_NoDomain_Address
    {
        get
        {
            return BridgeClient.GetResourceAddress("WcfService.TestResources.HttpDigestNoDomainResource");
        }
    }

    public static string Https_NtlmAuth_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallRootCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.HttpsNtlmResource");
        }
    }

    public static string Https_WindowsAuth_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallRootCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.HttpsWindowsResource");
        }
    }

    public static string Https_ClientCertificateAuth_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallRootCertificateFromBridge();
            BridgeClientCertificateManager.InstallLocalCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.HttpsClientCertificateResource");
        }
    }

    public static string Http_WindowsAuth_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallRootCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.HttpWindowsResource");
        }
    }

    public static string Https_DefaultBinding_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallRootCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.BasicHttpsResource");
        }
    }

    public static string HttpsSoap11_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallRootCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.HttpsSoap11Resource");
        }
    }

    public static string HttpsSoap12_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallRootCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.HttpsSoap12Resource");
        }
    }

    #region Secure WebSocket Addresses
    public static string WebSocketHttpsDuplexBinaryStreamed_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallRootCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.WebSocketHttpsDuplexBinaryStreamedResource");
        }
    }

    public static string WebSocketHttpsDuplexTextStreamed_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallRootCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.WebSocketHttpsDuplexTextStreamedResource");
        }
    }

    public static string WebSocketHttpsRequestReplyBinaryBuffered_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallRootCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.WebSocketHttpsRequestReplyBinaryBufferedResource");
        }
    }

    public static string WebSocketHttpsRequestReplyTextBuffered_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallRootCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.WebSocketHttpsRequestReplyTextBufferedResource");
        }
    }

    public static string WebSocketHttpsDuplexBinaryBuffered_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallRootCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.WebSocketHttpsDuplexBinaryBufferedResource");
        }
    }

    public static string WebSocketHttpsDuplexTextBuffered_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallRootCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.WebSocketHttpsDuplexTextBufferedResource");
        }
    }
    #endregion Secure WebSocket Addresses
    #endregion  HTTPS Addresses

    #region net.tcp Addresses
    // net.tcp Addresses
    public static string Tcp_DefaultBinding_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpDefaultResource"); }
    }

    public static string Tcp_NoSecurity_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpNoSecurityResource"); }
    }

    public static string Tcp_Streamed_NoSecurity_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpStreamedNoSecurityResource"); }
    }

    public static string Tcp_VerifyDNS_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallLocalCertificateFromBridge(); 
            return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpVerifyDNSResource");
        }
    }

    public static string Tcp_VerifyDNS_HostName
    {
        get
        {
            return BridgeClient.GetResourceHostName("WcfService.TestResources.TcpVerifyDNSResource");
        }
    }

    public static string Tcp_ExpiredServerCertResource_Address
    {
        get
        {
            return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpExpiredServerCertResource");
        }
    }

    public static string Tcp_RevokedServerCertResource_HostName
    {
        get
        {
            return BridgeClient.GetResourceHostName("WcfService.TestResources.TcpRevokedServerCertResource");
        }
    }

    public static string Tcp_RevokedServerCertResource_Address
    {
        get
        {
            return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpRevokedServerCertResource");
        }
    }

    public static string Tcp_ExpiredServerCertResource_HostName
    {
        get
        {
            return BridgeClient.GetResourceHostName("WcfService.TestResources.TcpExpiredServerCertResource");
        }
    }


    public static string Tcp_NoSecurity_Callback_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.DuplexResource"); }
    }

    public static string Tcp_CustomBinding_NoSecurity_Text_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpNoSecurityTextResource"); }
    }

    public static string Tcp_NoSecurity_TaskReturn_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.DuplexChannelCallbackReturnResource"); }
    }

    public static string Tcp_NoSecurity_DuplexCallback_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.DuplexCallbackResource"); }
    }

    public static string Tcp_NoSecurity_DataContractDuplexCallback_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.DuplexCallbackDataContractComplexTypeResource"); }
    }

    public static string Tcp_NoSecurity_XmlDuplexCallback_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.DuplexCallbackXmlComplexTypeResource"); }
    }


    public static string Tcp_CustomBinding_SslStreamSecurity_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallLocalCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpTransportSecurityWithSslResource");
        }
    }

    public static string Tcp_CustomBinding_SslStreamSecurity_HostName
    {
        get
        {
            return BridgeClient.GetResourceHostName("WcfService.TestResources.TcpTransportSecurityWithSslResource");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallLocalCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpTransportSecuritySslClientCredentialTypeCertificate");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_CustomValidation_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallLocalCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpTransportSecuritySslCustomCertValidationResource");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_With_ServerAltName_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallLocalCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpCertificateWithServerAltNameResource");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_With_CanonicalName_Localhost_Address
    {
        get
        {
            return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpCertificateWithSubjectCanonicalNameLocalhostResource");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_With_CanonicalName_DomainName_Address
    {
        get
        {
            return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpCertificateWithSubjectCanonicalNameDomainNameResource");
        }
    }

    public static string Tcp_ClientCredentialType_Certificate_With_CanonicalName_Fqdn_Address
    {
        get
        {
            return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpCertificateWithSubjectCanonicalNameFqdnResource");
        }
    }

    public static string Tcp_Transport_Security_Streamed_Address
    {
        get
        {
            BridgeClientCertificateManager.InstallLocalCertificateFromBridge();
            return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpTransportSecurityStreamedResource");
        }
    }
    #endregion net.tcp Addresses
}
