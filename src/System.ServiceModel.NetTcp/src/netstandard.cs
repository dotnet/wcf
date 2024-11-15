// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ServiceModel
{
    public sealed partial class MessageSecurityOverTcp
    {
        public MessageSecurityOverTcp() { }
        [System.ComponentModel.DefaultValueAttribute((System.ServiceModel.MessageCredentialType)(1))]
        public System.ServiceModel.MessageCredentialType ClientCredentialType { get { return default; } set { } }
    }
    public partial class NetTcpBinding : System.ServiceModel.Channels.Binding
    {
        public NetTcpBinding() { }
        public NetTcpBinding(System.ServiceModel.SecurityMode securityMode) { }
        public NetTcpBinding(System.ServiceModel.SecurityMode securityMode, bool reliableSessionEnabled) { }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public NetTcpBinding(string configurationName) { }
        public System.ServiceModel.EnvelopeVersion EnvelopeVersion { get { return default; } }
        [System.ComponentModel.DefaultValueAttribute((long)524288)]
        public long MaxBufferPoolSize { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute(65536)]
        public int MaxBufferSize { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute((long)65536)]
        public long MaxReceivedMessageSize { get { return default; } set { } }
        public System.Xml.XmlDictionaryReaderQuotas ReaderQuotas { get { return default; } set { } }
        public System.ServiceModel.OptionalReliableSession ReliableSession { get; set; }
        public override string Scheme { get { return default; } }
        public System.ServiceModel.NetTcpSecurity Security { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute((System.ServiceModel.TransferMode)(0))]
        public System.ServiceModel.TransferMode TransferMode { get { return default; } set { } }
        public override System.ServiceModel.Channels.BindingElementCollection CreateBindingElements() { return default; }
    }
    public sealed partial class NetTcpSecurity
    {
        public NetTcpSecurity() { }
        public System.ServiceModel.MessageSecurityOverTcp Message { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute((System.ServiceModel.SecurityMode)(1))]
        public System.ServiceModel.SecurityMode Mode { get { return default; } set { } }
        public System.ServiceModel.TcpTransportSecurity Transport { get { return default; } set { } }
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
        [System.ComponentModel.DefaultValueAttribute((System.ServiceModel.TcpClientCredentialType)(1))]
        public System.ServiceModel.TcpClientCredentialType ClientCredentialType { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute((System.Security.Authentication.SslProtocols)(4080))]
        public System.Security.Authentication.SslProtocols SslProtocols { get { return default; } set { } }

    }
}
namespace System.ServiceModel.Channels
{
    public abstract partial class ConnectionOrientedTransportBindingElement : System.ServiceModel.Channels.TransportBindingElement
    {
        internal ConnectionOrientedTransportBindingElement() { }
        [System.ComponentModel.DefaultValueAttribute(8192)]
        public int ConnectionBufferSize { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute(65536)]
        public int MaxBufferSize { get { return default; } set { } }
        [System.ComponentModel.DefaultValueAttribute((System.ServiceModel.TransferMode)(0))]
        public System.ServiceModel.TransferMode TransferMode { get { return default; } set { } }
        public override bool CanBuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { return default; }
    }
    public partial class SslStreamSecurityBindingElement : System.ServiceModel.Channels.BindingElement
    {
        public SslStreamSecurityBindingElement() { }
        public bool RequireClientCertificate { get { return default; } set { } }
        public System.Security.Authentication.SslProtocols SslProtocols { get { return default; } set { } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public override bool CanBuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public override System.ServiceModel.Channels.BindingElement Clone() { return default; }
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { return default; }
    }
    public sealed partial class TcpConnectionPoolSettings
    {
        internal TcpConnectionPoolSettings() { }
        public string GroupName { get { return default; } set { } }
        public System.TimeSpan IdleTimeout { get { return default; } set { } }
        public System.TimeSpan LeaseTimeout { get { return default; } set { } }
        public int MaxOutboundConnectionsPerEndpoint { get { return default; } set { } }
    }
    public partial class TcpTransportBindingElement : System.ServiceModel.Channels.ConnectionOrientedTransportBindingElement
    {
        public TcpTransportBindingElement() { }
        protected TcpTransportBindingElement(System.ServiceModel.Channels.TcpTransportBindingElement elementToBeCloned) { }
        public System.ServiceModel.Channels.TcpConnectionPoolSettings ConnectionPoolSettings { get { return default; } }
        public override string Scheme { get { return default; } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public override System.ServiceModel.Channels.BindingElement Clone() { return default; }
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { return default; }
    }
    public partial class WindowsStreamSecurityBindingElement : System.ServiceModel.Channels.BindingElement
    {
        public WindowsStreamSecurityBindingElement() { }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public override bool CanBuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public override System.ServiceModel.Channels.BindingElement Clone() { return default; }
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { return default; }
    }
}
