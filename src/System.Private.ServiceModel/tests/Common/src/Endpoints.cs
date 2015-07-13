﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel.Tests.Common;

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

    public static string HttpUrlNotFound_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.EndpointNotFoundResource") + "/UnknownUrl.htm"; } 
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

    public static string Tcp_NoSecurity_Callback_Address
    {
        get { return BaseAddress.TcpDuplexAddress + "/tcp-nosecurity-callback"; }
    }

    public static string Tcp_CustomBinding_NoSecurity_Text_Address
    {
        get { return BridgeClient.GetResourceAddress("WcfService.TestResources.TcpNoSecurityTextResource"); }
    }
}
