// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel.Tests.Common;

public static class BaseAddress
{
    // base address never used for end-to-end communication
    public const string FakeServerBaseAddress = "http://localhost/fakeservice.svc";

#if USE_FIDDLER
    public const string HttpServerBaseAddress = "http://localhost.fiddler:8081/";

    // Base address for HTTP endpoints
    public const string HttpBaseAddress = "http://localhost.fiddler:8081/WindowsCommunicationFoundation";

    // Base address for HTTPS endpoints
    public const string HttpsBaseAddress = "https://localhost.fiddler:44285/WindowsCommunicationFoundation";

    // Base address for HTTPS endpoints with Basic Authentication 
    public const string HttpsBasicBaseAddress = "https://localhost.fiddler:44285/WindowsCommunicationFoundation";

    // Base address for HTTPS endpoints with Digest Authentication
    public const string HttpsDigestBaseAddress = "https://localhost.fiddler:44285/WindowsCommunicationFoundation";

    // Base address for HTTPS endpoints with NT Authentication
    public const string HttpsNtlmBaseAddress = "https://localhost.fiddler:44285/WindowsCommunicationFoundation";

    // Base address for HTTPS endpoints with Windows Authentication
    public const string HttpsWindowsBaseAddress = "https://localhost.fiddler:44285/WindowsCommunicationFoundation";
#else

    public static string HttpServerBaseAddress { get { return BridgeTestFixture.GetBaseAddress("WcfService.TestResources.BaseAddressResource", "HttpServerBaseAddress"); } } //"http://localhost:8081/";

    // Base address for HTTP endpoints
    public static string HttpBaseAddress { get { return BridgeTestFixture.GetBaseAddress("WcfService.TestResources.BaseAddressResource", "HttpBaseAddress"); } } //= "http://localhost:8081/WindowsCommunicationFoundation";

    // Base address for HTTPS endpoints
    public static string HttpsBaseAddress { get { return BridgeTestFixture.GetBaseAddress("WcfService.TestResources.BaseAddressResource", "HttpsBaseAddress"); } } //= "https://localhost:44285/WindowsCommunicationFoundation";

    // Base address for HTTPS endpoints with Basic Authentication 
    public static string HttpsBasicBaseAddress { get { return BridgeTestFixture.GetBaseAddress("WcfService.TestResources.BaseAddressResource", "HttpsBasicBaseAddress"); } } //= "https://localhost:44285/WindowsCommunicationFoundation";

    // Base address for HTTPS endpoints with Digest Authentication
    public static string HttpsDigestBaseAddress { get { return BridgeTestFixture.GetBaseAddress("WcfService.TestResources.BaseAddressResource", "HttpsDigestBaseAddress"); } } //= "https://localhost:44285/WindowsCommunicationFoundation";

    // Base address for HTTPS endpoints with NT Authentication
    public static string HttpsNtlmBaseAddress { get { return BridgeTestFixture.GetBaseAddress("WcfService.TestResources.BaseAddressResource", "HttpsNtlmBaseAddress"); } } //= "https://localhost:44285/WindowsCommunicationFoundation";

    // Base address for HTTPS endpoints with Windows Authentication
    public static string HttpsWindowsBaseAddress { get { return BridgeTestFixture.GetBaseAddress("WcfService.TestResources.BaseAddressResource", "HttpsWindowsBaseAddress"); } } //= "https://localhost:44285/WindowsCommunicationFoundation";
#endif

    // Base address for Net.TCP endpoints
    public const string TcpDuplexAddress = "net.tcp://localhost:810/WindowsCommunicationFoundation";
    public static string TcpBaseAddress { get { return BridgeTestFixture.GetBaseAddress("WcfService.TestResources.BaseAddressResource", "TcpBaseAddress"); } } //= "net.tcp://localhost:809/WindowsCommunicationFoundation";
}


