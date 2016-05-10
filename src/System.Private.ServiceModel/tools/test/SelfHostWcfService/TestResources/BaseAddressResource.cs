// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal class BaseAddressResource : IResource
    {
        internal const string Http = "http";
        internal const string Https = "https";
        internal const string Tcp = "net.tcp";

        public ResourceResponse Put(ResourceRequestContext context)
        {
            throw new NotImplementedException("Cannot PUT on this resource");
        }

        public ResourceResponse Get(ResourceRequestContext context)
        {
            var http = GetHttpProtocol(context) + AppDomain.CurrentDomain.FriendlyName;
            var https = GetHttpsProtocol(context) + AppDomain.CurrentDomain.FriendlyName;
            var tcp = GetTcpProtocol(context) + AppDomain.CurrentDomain.FriendlyName;
            Dictionary<string, string> resultDictionary = new Dictionary<string, string>()
            {
                { "HttpServerBaseAddress", GetHttpProtocol(context) },
                { "HttpBaseAddress", http },
                { "HttpsBaseAddress", https },
                { "HttpsBasicBaseAddress", https },
                { "HttpsDigestBaseAddress", https },
                { "HttpsNtlmBaseAddress", https },
                { "HttpsWindowsBaseAddress", https },
                { "TcpBaseAddress", tcp }
            };

            return new ResourceResponse
            {
                Properties = resultDictionary
            };
        }

        public static string GetHttpProtocol(ResourceRequestContext context)
        {
            return string.Format("http://{0}:{1}/",
                                context.BridgeConfiguration.BridgeHost,
                                context.BridgeConfiguration.BridgeHttpPort);
        }

        public static string GetHttpsProtocol(ResourceRequestContext context)
        {
            return string.Format("https://{0}:{1}/",
                                context.BridgeConfiguration.BridgeHost,
                                context.BridgeConfiguration.BridgeHttpsPort);
        }

        public static string GetTcpProtocol(ResourceRequestContext context)
        {
            return string.Format("net.tcp://{0}:{1}/",
                                context.BridgeConfiguration.BridgeHost,
                                context.BridgeConfiguration.BridgeTcpPort);
        }
    }
}
