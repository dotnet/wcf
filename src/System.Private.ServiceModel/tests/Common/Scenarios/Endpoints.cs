// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Infrastructure.Common;

public static partial class Endpoints
{
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

    public static string NetHttpWebSocketTransport_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.WebSocketTransportResource"); }
    }

    public static string NetHttpDuplexWebSocket_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.DuplexWebSocketResource"); }
    }
    
    public static string HttpProtocolError_Address
    {
        get { return Endpoints.DefaultCustomHttp_Address + "/UnknownProtocolUrl.htm"; }
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

    // net.tcp Addresses
    public static string Tcp_DefaultBinding_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpDefaultResource"); }
    }

    public static string Tcp_NoSecurity_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpNoSecurityResource"); }
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
}
