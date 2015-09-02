// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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

    // HTTPS Addresses
    public static string Https_BasicAuth_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.BasicAuthResource"); } 
    }

    public static string Https_DigestAuth_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.HttpsDigestResource"); }
    }

    public static string Https_NtlmAuth_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.HttpsNtlmResource"); }
    }

    public static string Https_WindowsAuth_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.HttpsWindowsResource"); }
    }

    public static string Http_WindowsAuth_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.HttpWindowsResource"); }
    }

    public static string Https_DefaultBinding_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.BasicHttpsResource"); }
    }

    public static string HttpsSoap11_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.HttpsSoap11Resource"); }
    }

    public static string HttpsSoap12_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.HttpsSoap12Resource"); }
    }

    public static string HttpProtocolError_Address
    {
        get { return Endpoints.DefaultCustomHttp_Address + "/UnknownProtocolUrl.htm"; } 
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
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpVerifyDNSResource"); }
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
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpTransportSecurityWithSslResource"); }
    }
}
