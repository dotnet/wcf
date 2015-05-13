// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

public static partial class Constants
{
    public static class BaseAddress
    {
        // Base address for HTTP endpoints
        public const string HttpBaseAddress = "http://wcfprojectnserver.redmond.corp.microsoft.com:80/WindowsCommunicationFoundation";

        // Base address for HTTPS endpoints
        public const string HttpsBaseAddress = "https://wcfprojectnserver.redmond.corp.microsoft.com:443/WindowsCommunicationFoundation";

        // Base address for HTTPS endpoints with Basic Authentication 
        public const string HttpsBasicBaseAddress = "https://wcfprojectnserver.redmond.corp.microsoft.com:443/WindowsCommunicationFoundation";

        // Base address for HTTPS endpoints with Digest Authentication
        public const string HttpsDigestBaseAddress = "https://wcfprojectnserver.redmond.corp.microsoft.com:443/WindowsCommunicationFoundation";

        // Base address for HTTPS endpoints with NT Authentication
        public const string HttpsNtlmBaseAddress = "https://wcfprojectnserver.redmond.corp.microsoft.com:443/WindowsCommunicationFoundation";

        // Base address for HTTPS endpoints with Windows Authentication
        public const string HttpsWindowsBaseAddress = "https://wcfprojectnserver.redmond.corp.microsoft.com:443/WindowsCommunicationFoundation";

        // Base address for Net.TCP endpoints
        public const string TcpBaseAddress = "net.tcp://wcfprojectnserver.redmond.corp.microsoft.com:809/WindowsCommunicationFoundation";
    }
}
