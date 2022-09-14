// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

using System.Security.Authentication;

[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ServiceModel.Channels.ConnectionOrientedTransportBindingElement))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ServiceModel.Channels.SslStreamSecurityBindingElement))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.ServiceModel.Channels.WindowsStreamSecurityBindingElement))]

namespace System.ServiceModel
{
    public sealed partial class MessageSecurityOverTcp
    {
        public MessageSecurityOverTcp() { }
        [ComponentModel.DefaultValueAttribute((ServiceModel.MessageCredentialType)(1))]
        public ServiceModel.MessageCredentialType ClientCredentialType { get { return default; } set { } }
    }
    public partial class NetTcpBinding : ServiceModel.Channels.Binding
    {
        public NetTcpBinding() { }
        public NetTcpBinding(ServiceModel.SecurityMode securityMode) { }
        public NetTcpBinding(ServiceModel.SecurityMode securityMode, bool reliableSessionEnabled) { }
        [ComponentModel.EditorBrowsableAttribute(ComponentModel.EditorBrowsableState.Never)]
        public NetTcpBinding(string configurationName) { }
        public ServiceModel.EnvelopeVersion EnvelopeVersion { get { return default; } }
        [ComponentModel.DefaultValueAttribute((long)524288)]
        public long MaxBufferPoolSize { get { return default; } set { } }
        [ComponentModel.DefaultValueAttribute(65536)]
        public int MaxBufferSize { get { return default; } set { } }
        [ComponentModel.DefaultValueAttribute((long)65536)]
        public long MaxReceivedMessageSize { get { return default; } set { } }
        public Xml.XmlDictionaryReaderQuotas ReaderQuotas { get { return default; } set { } }
        public ServiceModel.OptionalReliableSession ReliableSession { get; set; }
        public override string Scheme { get { return default; } }
        public ServiceModel.NetTcpSecurity Security { get { return default; } set { } }
        [ComponentModel.DefaultValueAttribute((ServiceModel.TransferMode)(0))]
        public ServiceModel.TransferMode TransferMode { get { return default; } set { } }
        public override ServiceModel.Channels.BindingElementCollection CreateBindingElements() { return default; }
    }
    public sealed partial class NetTcpSecurity
    {
        public NetTcpSecurity() { }
        public ServiceModel.MessageSecurityOverTcp Message { get { return default; } set { } }
        [ComponentModel.DefaultValueAttribute((ServiceModel.SecurityMode)(1))]
        public ServiceModel.SecurityMode Mode { get { return default; } set { } }
        public ServiceModel.TcpTransportSecurity Transport { get { return default; } set { } }
    }
    public enum TcpClientCredentialType
    {
        Certificate = 2,
        None = 0,
        Windows = 1,
    }
    public sealed partial class TcpTransportSecurity
    {
        public TcpTransportSecurity() { }
        [ComponentModel.DefaultValueAttribute((ServiceModel.TcpClientCredentialType)(1))]
        public ServiceModel.TcpClientCredentialType ClientCredentialType { get { return default; } set { } }
        [ComponentModel.DefaultValueAttribute((SslProtocols)(4080))]
        public SslProtocols SslProtocols { get { return default; } set { } }

    }
}
namespace System.ServiceModel.Channels
{
    public sealed partial class TcpConnectionPoolSettings
    {
        internal TcpConnectionPoolSettings() { }
        public string GroupName { get { return default; } set { } }
        public TimeSpan IdleTimeout { get { return default; } set { } }
        public TimeSpan LeaseTimeout { get { return default; } set { } }
        public int MaxOutboundConnectionsPerEndpoint { get { return default; } set { } }
    }
}
