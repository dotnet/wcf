// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.IO
{
    [Serializable]
    public class PipeException : IOException
    {
        public PipeException() { }
        public PipeException(string message) { }
        public PipeException(string message, int errorCode) { }
        public PipeException(string message, Exception inner) { }
        protected PipeException(Runtime.Serialization.SerializationInfo info, Runtime.Serialization.StreamingContext context) { }
        public virtual int ErrorCode => default;
    }
}
namespace System.ServiceModel
{
    [Serializable]
    public class AddressAccessDeniedException : CommunicationException
    {
        public AddressAccessDeniedException() { }
        public AddressAccessDeniedException(string message) { }
        public AddressAccessDeniedException(string message, Exception innerException) { }
        protected AddressAccessDeniedException(Runtime.Serialization.SerializationInfo info, Runtime.Serialization.StreamingContext context){ }
    }
    public sealed class NamedPipeTransportSecurity
    {
        public NamedPipeTransportSecurity() { }
        public Net.Security.ProtectionLevel ProtectionLevel { get { return default; } set { } }
    }
    public class NetNamedPipeBinding : Channels.Binding
    {
        public NetNamedPipeBinding() { }
        public NetNamedPipeBinding(NetNamedPipeSecurityMode securityMode) { }
        public TransferMode TransferMode { get { return default; } set { } }
        public long MaxBufferPoolSize { get { return default; } set { } }
        public int MaxBufferSize { get { return default; } set { } }
        public int MaxConnections { get { return default; } set { } }
        public long MaxReceivedMessageSize { get { return default; } set { } }
        public Xml.XmlDictionaryReaderQuotas ReaderQuotas { get { return default; } set { } }
        public override string Scheme => default;
        public EnvelopeVersion EnvelopeVersion => default;
        public NetNamedPipeSecurity Security { get { return default; } set { } }
        public override Channels.BindingElementCollection CreateBindingElements() => default;
    }
    public sealed class NetNamedPipeSecurity
    {
        public NetNamedPipeSecurity() { }
        public NetNamedPipeSecurityMode Mode { get { return default; } set { } }
        public NamedPipeTransportSecurity Transport { get { return default; } set { } }
    }
    public enum NetNamedPipeSecurityMode
    {
        None,
        Transport
    }
}
namespace System.ServiceModel.Channels
{
    public sealed class NamedPipeConnectionPoolSettings
    {
        internal NamedPipeConnectionPoolSettings() { }
        public string GroupName { get { return default; } set { } }
        public TimeSpan IdleTimeout { get { return default; } set { } }
        public int MaxOutboundConnectionsPerEndpoint { get { return default; } set { } }
    }
    public class NamedPipeTransportBindingElement : ConnectionOrientedTransportBindingElement
    {
        public NamedPipeTransportBindingElement() { }
        protected NamedPipeTransportBindingElement(NamedPipeTransportBindingElement elementToBeCloned) { }
        public NamedPipeConnectionPoolSettings ConnectionPoolSettings => default;
        public override string Scheme => default;
        public override BindingElement Clone() => default;
        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context) => default;
        public override T GetProperty<T>(BindingContext context) => default;
    }
}
