// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Channels
{
    internal static class TransportPolicyConstants
    {
        public const string BasicHttpAuthenticationName = "BasicAuthentication";
        public const string CompositeDuplex = "CompositeDuplex";
        public const string CompositeDuplexNamespace = "http://schemas.microsoft.com/net/2006/06/duplex";
        public const string CompositeDuplexPrefix = "cdp";
        public const string DigestHttpAuthenticationName = "DigestAuthentication";
        public const string DotNetFramingNamespace = BinaryEncodingString.NamespaceUri + "/policy";
        public const string DotNetFramingPrefix = "msf";
        public const string HttpTransportNamespace = "http://schemas.microsoft.com/ws/06/2004/policy/http";
        public const string HttpTransportPrefix = "http";
        public const string HttpTransportUri = "http://schemas.xmlsoap.org/soap/http";
        public const string NamedPipeTransportUri = "http://schemas.microsoft.com/soap/named-pipe";
        public const string NegotiateHttpAuthenticationName = "NegotiateAuthentication";
        public const string NtlmHttpAuthenticationName = "NtlmAuthentication";
        public const string PeerTransportUri = "http://schemas.microsoft.com/soap/peer";
        public const string ProtectionLevelName = "ProtectionLevel";
        public const string RequireClientCertificateName = "RequireClientCertificate";
        public const string SslTransportSecurityName = "SslTransportSecurity";
        public const string StreamedName = "Streamed";
        public const string TcpTransportUri = "http://schemas.microsoft.com/soap/tcp";
        public const string WebSocketPolicyPrefix = "mswsp";
        public const string WebSocketPolicyNamespace = "http://schemas.microsoft.com/soap/websocket/policy";
        public const string WebSocketTransportUri = "http://schemas.microsoft.com/soap/websocket";
        public const string WebSocketEnabled = "WebSocketEnabled";
        public const string WindowsTransportSecurityName = "WindowsTransportSecurity";
    }
}
