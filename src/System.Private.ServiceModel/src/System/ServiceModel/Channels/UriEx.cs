// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Channels
{
    // Normally part of Uri.cs but not available in contract
    internal class UriEx
    {
        public static readonly string UriSchemeHttp = "http";
        public static readonly string UriSchemeHttps = "https";
        public static readonly string UriSchemeNetTcp = "net.tcp";
        public static readonly string UriSchemeNetPipe = "net.pipe";
    }
}
