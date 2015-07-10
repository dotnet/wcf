// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
public static partial class Endpoints
{
    // HTTP Addresses
    public static string HttpBaseAddress_Basic
    {
        get { return BaseAddress.HttpBaseAddress + "/Basic"; }
    }

    public static string HttpBaseAddress_NetHttp
    {
        get { return BaseAddress.HttpBaseAddress + "/NetHttp"; }
    }

    public static string HttpSoap11_Address
    {
        get { return BaseAddress.HttpBaseAddress + "/http-soap11"; }
    }

    public static string HttpSoap12_Address
    {
        get { return BaseAddress.HttpBaseAddress + "/http-soap12"; }
    }

    public static string HttpBinary_Address
    {
        get { return BaseAddress.HttpBaseAddress + "/http-binary"; }
    }

    // HTTPS Addresses
    public static string Https_BasicAuth_Address
    {
        get { return BaseAddress.HttpsBasicBaseAddress + "/CustomerUserName/https-basic"; }
    }

    public static string Https_DigestAuth_Address
    {
        get { return BaseAddress.HttpsDigestBaseAddress + "/https-digest"; }
    }

    public static string Https_NtlmAuth_Address
    {
        get { return BaseAddress.HttpsNtlmBaseAddress + "/https-ntlm"; }
    }

    public static string Https_WindowsAuth_Address
    {
        get { return BaseAddress.HttpsWindowsBaseAddress + "/https-windows"; }
    }

    public static string Https_DefaultBinding_Address
    {
        get { return BaseAddress.HttpsBaseAddress + "/basicHttps"; }
    }

    public static string HttpsSoap11_Address
    {
        get { return BaseAddress.HttpsBaseAddress + "/https-soap11"; }
    }

    public static string HttpsSoap12_Address
    {
        get { return BaseAddress.HttpsBaseAddress + "/https-soap12"; }
    }

    public static string HttpUrlNotFound_Address
    {
        get { return BaseAddress.HttpServerBaseAddress + "/UnknownUrl.htm"; }
    }

    public static string HttpProtocolError_Address
    {
        get { return BaseAddress.HttpBaseAddress + "/UnknownProtocolUrl.htm"; }
    }

    // net.tcp Addresses
    public static string Tcp_DefaultBinding_Address
    {
        get { return BaseAddress.TcpBaseAddress + "/tcp-default"; }
    }

    public static string Tcp_NoSecurity_Address
    {
        get { return BaseAddress.TcpBaseAddress + "/tcp-nosecurity"; }
    }

    public static string Tcp_NoSecurity_Callback_Address
    {
        get { return BaseAddress.TcpDuplexAddress + "/tcp-nosecurity-callback"; }
    }

    public static string Tcp_CustomBinding_NoSecurity_Text_Address
    {
        get { return BaseAddress.TcpBaseAddress + "/tcp-custombinding-nosecurity-text"; }
    }

    public static string Tcp_NoSecurity_TaskReturn_Address
    {
        get { return BaseAddress.TcpDuplexTaskReturnAddress + "/tcp-nosecurity-taskreturn"; }
    }

    public static string Tcp_NoSecurity_DuplexCallback_Address
    {
        get { return BaseAddress.TcpDuplexCallbackAddress + "/tcp-nosecurity-typedproxy-duplexcallback"; }
    }
}
