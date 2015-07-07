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
        internal const string HttpPort = "8081";
        internal const string HttpsPort = "44285";
        internal const string TcpPort = "809";
        const string HttpProtocol = "http://localhost:" + HttpPort + "/";
        const string HttpsProtocol = "https://localhost:" + HttpsPort + "/";
        const string TcpProtocol = "net.tcp://localhost:" + TcpPort + "/";

        public object Put()
        {
            throw new NotImplementedException("Cannot PUT on this resource");
        }

        public object Get()
        {
            var http = HttpProtocol + AppDomain.CurrentDomain.FriendlyName;
            var https = HttpsProtocol + AppDomain.CurrentDomain.FriendlyName;
            var tcp = TcpProtocol + AppDomain.CurrentDomain.FriendlyName;
            return new Dictionary<string, string>()
            {
                { "HttpServerBaseAddress", HttpProtocol },
                { "HttpBaseAddress", http },
                { "HttpsBaseAddress", https },
                { "HttpsBasicBaseAddress", https },
                { "HttpsDigestBaseAddress", https },
                { "HttpsNtlmBaseAddress", https },
                { "HttpsWindowsBaseAddress", https },
                { "TcpBaseAddress", tcp }
            };
        }
    }
}
