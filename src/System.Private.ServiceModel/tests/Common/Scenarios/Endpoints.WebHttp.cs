// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Endpoints addresses for WebHttp (REST) test services. Partial extension to
// Endpoints (declared partial in tests/Common/Scenarios/Endpoints.cs).
public static partial class Endpoints
{
    #region WebHttp Addresses
    public static string HttpBaseAddress_WebHttp
        => GetEndpointAddress("WebHttp.svc/");

    public static string HttpBaseAddress_WebHttp_EchoGet
        => GetEndpointAddress("WebHttp.svc/EchoWithGet");

    public static string HttpBaseAddress_WebHttp_EchoGetJson
        => GetEndpointAddress("WebHttp.svc/EchoWithGetJson");

    public static string HttpBaseAddress_WebHttp_EchoPost
        => GetEndpointAddress("WebHttp.svc/EchoWithPost");

    public static string HttpBaseAddress_WebHttp_EchoPostJson
        => GetEndpointAddress("WebHttp.svc/EchoWithPostJson");

    public static string HttpBaseAddress_WebHttp_EchoGetPath
        => GetEndpointAddress("WebHttp.svc/EchoWithGetPath");
    #endregion
}
