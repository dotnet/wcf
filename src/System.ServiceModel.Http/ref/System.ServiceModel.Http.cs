// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.ServiceModel
{
    public partial class BasicHttpBinding : System.ServiceModel.HttpBindingBase
    {
        public BasicHttpBinding() { }
        public BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode securityMode) { }
        public System.ServiceModel.BasicHttpSecurity Security { get { return default(System.ServiceModel.BasicHttpSecurity); } set { } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingParameterCollection parameters) { return default(System.ServiceModel.Channels.IChannelFactory<TChannel>); }
        public override System.ServiceModel.Channels.BindingElementCollection CreateBindingElements() { return default(System.ServiceModel.Channels.BindingElementCollection); }
    }
    public enum BasicHttpMessageCredentialType
    {
        Certificate = 1,
        UserName = 0,
    }
    public sealed partial class BasicHttpMessageSecurity
    {
        public BasicHttpMessageSecurity() { }
        public System.ServiceModel.BasicHttpMessageCredentialType ClientCredentialType { get { return default(System.ServiceModel.BasicHttpMessageCredentialType); } set { } }
        public System.ServiceModel.Security.SecurityAlgorithmSuite AlgorithmSuite { get { return default(System.ServiceModel.Security.SecurityAlgorithmSuite); } set { } }
    }
    public sealed partial class BasicHttpSecurity
    {
        public BasicHttpSecurity() { }
        public System.ServiceModel.BasicHttpSecurityMode Mode { get { return default(System.ServiceModel.BasicHttpSecurityMode); } set { } }
        public System.ServiceModel.HttpTransportSecurity Transport { get { return default(System.ServiceModel.HttpTransportSecurity); } set { } }
        public System.ServiceModel.BasicHttpMessageSecurity Message { get { return default(System.ServiceModel.BasicHttpMessageSecurity); } set { } }
    }
    public enum BasicHttpSecurityMode
    {
        None = 0,
        Transport = 1,
        Message = 2,
        TransportWithMessageCredential = 3,
        TransportCredentialOnly = 4,
    }
    public partial class BasicHttpsBinding : System.ServiceModel.HttpBindingBase
    {
        public BasicHttpsBinding() { }
        public BasicHttpsBinding(System.ServiceModel.BasicHttpsSecurityMode securityMode) { }
        public System.ServiceModel.BasicHttpsSecurity Security { get { return default(System.ServiceModel.BasicHttpsSecurity); } set { } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingParameterCollection parameters) { return default(System.ServiceModel.Channels.IChannelFactory<TChannel>); }
        public override System.ServiceModel.Channels.BindingElementCollection CreateBindingElements() { return default(System.ServiceModel.Channels.BindingElementCollection); }
    }
    public sealed partial class BasicHttpsSecurity
    {
        internal BasicHttpsSecurity() { }
        public System.ServiceModel.BasicHttpsSecurityMode Mode { get { return default(System.ServiceModel.BasicHttpsSecurityMode); } set { } }
        public System.ServiceModel.HttpTransportSecurity Transport { get { return default(System.ServiceModel.HttpTransportSecurity); } set { } }
    }
     public enum BasicHttpsSecurityMode
    {
        Transport = 0,
        TransportWithMessageCredential = 1,
    }
    public abstract partial class HttpBindingBase : System.ServiceModel.Channels.Binding
    {
        internal HttpBindingBase() { }
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool AllowCookies { get { return default(bool); } set { } }
        [System.ComponentModel.DefaultValue(false)]
        public bool BypassProxyOnLocal { get { return default(bool); } set { } }
        public System.ServiceModel.EnvelopeVersion EnvelopeVersion { get { return default(System.ServiceModel.EnvelopeVersion); } }
        [System.ComponentModel.DefaultValueAttribute((long)524288)]
        public long MaxBufferPoolSize { get { return default(long); } set { } }
        [System.ComponentModel.DefaultValueAttribute(65536)]
        public int MaxBufferSize { get { return default(int); } set { } }
        [System.ComponentModel.DefaultValueAttribute((long)65536)]
        public long MaxReceivedMessageSize { get { return default(long); } set { } }
        [System.ComponentModel.DefaultValueAttribute(null)]
        [System.ComponentModel.TypeConverter(typeof(System.UriTypeConverter))]
        public System.Uri ProxyAddress { get { return default(System.Uri); } set { } }
        public System.Xml.XmlDictionaryReaderQuotas ReaderQuotas { get { return default(System.Xml.XmlDictionaryReaderQuotas); } set { } }
        public override string Scheme { get { return default(string); } }
        public System.Text.Encoding TextEncoding { get { return default(System.Text.Encoding); } set { } }
        [System.ComponentModel.DefaultValueAttribute((System.ServiceModel.TransferMode)(0))]
        public System.ServiceModel.TransferMode TransferMode { get { return default(System.ServiceModel.TransferMode); } set { } }
        [System.ComponentModel.DefaultValue(true)]
        public bool UseDefaultWebProxy { get { return default(bool); } set { } }
    }
    public enum HttpClientCredentialType
    {
        Basic = 1,
        Certificate = 5,
        Digest = 2,
        InheritedFromHost = 6,
        None = 0,
        Ntlm = 3,
        Windows = 4,
    }
    public enum HttpProxyCredentialType
    {
        None,
        Basic,
        Digest,
        Ntlm,
        Windows,
    }
    public sealed partial class HttpTransportSecurity
    {
        public HttpTransportSecurity() { }
        public System.ServiceModel.HttpClientCredentialType ClientCredentialType { get { return default(System.ServiceModel.HttpClientCredentialType); } set { } }
        public System.ServiceModel.HttpProxyCredentialType ProxyCredentialType { get { return default(System.ServiceModel.HttpProxyCredentialType); } set { } }
        public System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy ExtendedProtectionPolicy { get { return default(System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy); } set { } }
    }
    public partial class NetHttpBinding : System.ServiceModel.HttpBindingBase
    {
        public NetHttpBinding() { }
        public NetHttpBinding(System.ServiceModel.BasicHttpSecurityMode securityMode) { }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public NetHttpBinding(string configurationName) { }
        [System.ComponentModel.DefaultValueAttribute((System.ServiceModel.NetHttpMessageEncoding)(0))]
        public System.ServiceModel.NetHttpMessageEncoding MessageEncoding { get { return default(System.ServiceModel.NetHttpMessageEncoding); } set { } }
        public System.ServiceModel.BasicHttpSecurity Security { get { return default(System.ServiceModel.BasicHttpSecurity); } set { } }
        public System.ServiceModel.Channels.WebSocketTransportSettings WebSocketSettings { get { return default(System.ServiceModel.Channels.WebSocketTransportSettings); } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingParameterCollection parameters) { return default(System.ServiceModel.Channels.IChannelFactory<TChannel>); }
        public override System.ServiceModel.Channels.BindingElementCollection CreateBindingElements() { return default(System.ServiceModel.Channels.BindingElementCollection); }
    }
    public partial class NetHttpsBinding : System.ServiceModel.HttpBindingBase
    {
        public NetHttpsBinding() { }
        public NetHttpsBinding(System.ServiceModel.BasicHttpsSecurityMode securityMode) { }
        [System.ComponentModel.DefaultValueAttribute((System.ServiceModel.NetHttpMessageEncoding)(0))]
        public System.ServiceModel.NetHttpMessageEncoding MessageEncoding { get { return default(System.ServiceModel.NetHttpMessageEncoding); } set { } }
        public System.ServiceModel.BasicHttpsSecurity Security { get { return default(System.ServiceModel.BasicHttpsSecurity); } set { } }
        public System.ServiceModel.Channels.WebSocketTransportSettings WebSocketSettings { get { return default(System.ServiceModel.Channels.WebSocketTransportSettings); } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingParameterCollection parameters) { return default(System.ServiceModel.Channels.IChannelFactory<TChannel>); }
        public override System.ServiceModel.Channels.BindingElementCollection CreateBindingElements() { return default(System.ServiceModel.Channels.BindingElementCollection); }
    }
    public enum NetHttpMessageEncoding
    {
        Binary = 0,
        Text = 1,
        Mtom = 2,
    }
    public abstract partial class WSHttpBindingBase : System.ServiceModel.Channels.Binding
    {
        protected WSHttpBindingBase() { }
        protected WSHttpBindingBase(bool reliableSessionEnabled) { }
        public bool BypassProxyOnLocal { get { return default(bool); } set { } }
        public bool TransactionFlow { get { return default(bool); } set { } }
        //public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return default(System.ServiceModel.HostNameComparisonMode); } set { } }
        public long MaxBufferPoolSize { get { return default(long); } set { } }
        public long MaxReceivedMessageSize { get { return default(long); } set { } }
        //public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return default(System.ServiceModel.WSMessageEncoding); } set { } }
        public Uri ProxyAddress { get { return default(Uri); } set { } }
        public System.Xml.XmlDictionaryReaderQuotas ReaderQuotas { get { return default(System.Xml.XmlDictionaryReaderQuotas); } set { } }
        //public System.ServiceModel.OptionalReliableSession ReliableSession { get { return default(System.ServiceModel.OptionalReliableSession); } set { } }
        public override string Scheme { get { return default(string); } }
        public System.ServiceModel.EnvelopeVersion EnvelopeVersion { get { return default(System.ServiceModel.EnvelopeVersion); } set { } }
        public System.Text.Encoding TextEncoding { get { return default(System.Text.Encoding); } set { } }
        public bool UseDefaultWebProxy { get { return default(bool); } set { } }
        public override System.ServiceModel.Channels.BindingElementCollection CreateBindingElements() { return default(System.ServiceModel.Channels.BindingElementCollection); }
        protected abstract System.ServiceModel.Channels.TransportBindingElement GetTransport();
        protected abstract System.ServiceModel.Channels.SecurityBindingElement CreateMessageSecurity();
    }
    public partial class WSHttpBinding : System.ServiceModel.WSHttpBindingBase
    {
        public WSHttpBinding() { }
        public WSHttpBinding(System.ServiceModel.SecurityMode securityMode) { }
        public WSHttpBinding(System.ServiceModel.SecurityMode securityMode, bool reliableSessionEnabled) { }
        public bool AllowCookies { get { return default(bool); } set { } }
        public System.ServiceModel.WSHttpSecurity Security { get { return default(System.ServiceModel.WSHttpSecurity); } set { } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingParameterCollection parameters) { return default(System.ServiceModel.Channels.IChannelFactory<TChannel>); }
        public override System.ServiceModel.Channels.BindingElementCollection CreateBindingElements() { return default(System.ServiceModel.Channels.BindingElementCollection); }
        protected override System.ServiceModel.Channels.TransportBindingElement GetTransport() { return default(System.ServiceModel.Channels.TransportBindingElement); }
        protected override System.ServiceModel.Channels.SecurityBindingElement CreateMessageSecurity() { return default(System.ServiceModel.Channels.SecurityBindingElement); }
    }
    public sealed partial class WSHttpSecurity
    {
        public WSHttpSecurity() { }
        public System.ServiceModel.SecurityMode Mode { get { return default(System.ServiceModel.SecurityMode); } set { } }
        public System.ServiceModel.HttpTransportSecurity Transport { get { return default(System.ServiceModel.HttpTransportSecurity); } set { } }
        public System.ServiceModel.NonDualMessageSecurityOverHttp Message { get { return default(System.ServiceModel.NonDualMessageSecurityOverHttp); } set { } }
    }
    public sealed class NonDualMessageSecurityOverHttp : System.ServiceModel.MessageSecurityOverHttp
    {
        public NonDualMessageSecurityOverHttp() { }
        public bool EstablishSecurityContext { get { return default(bool); } set { } }
        protected override bool IsSecureConversationEnabled() { return default(bool); }
    }
    public class MessageSecurityOverHttp
    {
        public MessageSecurityOverHttp() { }
        public System.ServiceModel.MessageCredentialType ClientCredentialType { get { return default(System.ServiceModel.MessageCredentialType); } set { } }
        public bool NegotiateServiceCredential { get { return default(bool); } set { } }
        public System.ServiceModel.Security.SecurityAlgorithmSuite AlgorithmSuite { get { return default(System.ServiceModel.Security.SecurityAlgorithmSuite); } set { } }
        protected virtual bool IsSecureConversationEnabled() { return default(bool); }
    }
    //public enum WSMessageEncoding
    //{
    //    Text = 0,
    //    Mtom,
    //}
}
namespace System.ServiceModel.Channels
{
    public sealed partial class HttpRequestMessageProperty : System.ServiceModel.Channels.IMessageProperty
    {
        public HttpRequestMessageProperty() { }
        public System.Net.WebHeaderCollection Headers { get { return default(System.Net.WebHeaderCollection); } }
        public string Method { get { return default(string); } set { } }
        public static string Name { get { return default(string); } }
        public string QueryString { get { return default(string); } set { } }
        public bool SuppressEntityBody { get { return default(bool); } set { } }
        System.ServiceModel.Channels.IMessageProperty System.ServiceModel.Channels.IMessageProperty.CreateCopy() { return default(System.ServiceModel.Channels.IMessageProperty); }
    }
    public sealed partial class HttpResponseMessageProperty : System.ServiceModel.Channels.IMessageProperty
    {
        public HttpResponseMessageProperty() { }
        public System.Net.WebHeaderCollection Headers { get { return default(System.Net.WebHeaderCollection); } }
        public static string Name { get { return default(string); } }
        public System.Net.HttpStatusCode StatusCode { get { return default(System.Net.HttpStatusCode); } set { } }
        public string StatusDescription { get { return default(string); } set { } }
        System.ServiceModel.Channels.IMessageProperty System.ServiceModel.Channels.IMessageProperty.CreateCopy() { return default(System.ServiceModel.Channels.IMessageProperty); }
    }
    public partial class HttpsTransportBindingElement : System.ServiceModel.Channels.HttpTransportBindingElement
    {
        public HttpsTransportBindingElement() { }
        protected HttpsTransportBindingElement(System.ServiceModel.Channels.HttpsTransportBindingElement elementToBeCloned) { }
        public bool RequireClientCertificate { get { return default(bool); } set { } }
        public override string Scheme { get { return default(string); } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default(System.ServiceModel.Channels.IChannelFactory<TChannel>); }
        public override System.ServiceModel.Channels.BindingElement Clone() { return default(System.ServiceModel.Channels.BindingElement); }
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { return default(T); }
    }
    public partial class HttpTransportBindingElement : System.ServiceModel.Channels.TransportBindingElement
    {
        public HttpTransportBindingElement() { }
        protected HttpTransportBindingElement(System.ServiceModel.Channels.HttpTransportBindingElement elementToBeCloned) { }
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool AllowCookies { get { return default(bool); } set { } }
        [System.ComponentModel.DefaultValueAttribute((System.Net.AuthenticationSchemes)(32768))]
        public System.Net.AuthenticationSchemes AuthenticationScheme { get { return default(System.Net.AuthenticationSchemes); } set { } }
        [System.ComponentModel.DefaultValue(false)]
        public bool BypassProxyOnLocal { get { return default(bool); } set { } }
        public System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy ExtendedProtectionPolicy { get; set; }
        [System.ComponentModel.DefaultValueAttribute(65536)]
        public int MaxBufferSize { get { return default(int); } set { } }
        [System.ComponentModel.DefaultValue(null)]
        [System.ComponentModel.TypeConverter(typeof(System.UriTypeConverter))]
        public System.Uri ProxyAddress { get { return default(System.Uri); } set { } }
        [System.ComponentModel.DefaultValueAttribute((System.Net.AuthenticationSchemes)(32768))]
        public System.Net.AuthenticationSchemes ProxyAuthenticationScheme { get { return default(System.Net.AuthenticationSchemes); } set { } }
        public override string Scheme { get { return default(string); } }
        [System.ComponentModel.DefaultValueAttribute((System.ServiceModel.TransferMode)(0))]
        public System.ServiceModel.TransferMode TransferMode { get { return default(System.ServiceModel.TransferMode); } set { } }
        public System.ServiceModel.Channels.WebSocketTransportSettings WebSocketSettings { get { return default(System.ServiceModel.Channels.WebSocketTransportSettings); } set { } }
        [System.ComponentModel.DefaultValue(true)]
        public bool UseDefaultWebProxy { get { return default(bool); } set { } }
        [System.ComponentModel.DefaultValue(true)]
        public bool KeepAliveEnabled { get { return default(bool); } set { } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default(System.ServiceModel.Channels.IChannelFactory<TChannel>); }
        public override bool CanBuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default(bool); }
        public override System.ServiceModel.Channels.BindingElement Clone() { return default(System.ServiceModel.Channels.BindingElement); }
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { return default(T); }
    }
    public partial interface IHttpCookieContainerManager
    {
        System.Net.CookieContainer CookieContainer { get; set; }
    }
    public sealed partial class WebSocketTransportSettings : System.IEquatable<System.ServiceModel.Channels.WebSocketTransportSettings>
    {
        public const string BinaryMessageReceivedAction = "http://schemas.microsoft.com/2011/02/websockets/onbinarymessage";
        public const string TextMessageReceivedAction = "http://schemas.microsoft.com/2011/02/websockets/ontextmessage";
        public WebSocketTransportSettings() { }
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool DisablePayloadMasking { get { return default(bool); } set { } }
        [System.ComponentModel.DefaultValueAttribute(typeof(System.TimeSpan), "00:00:00")]
        public System.TimeSpan KeepAliveInterval { get { return default(System.TimeSpan); } set { } }
        [System.ComponentModel.DefaultValueAttribute(null)]
        public string SubProtocol { get { return default(string); } set { } }
        [System.ComponentModel.DefaultValueAttribute((System.ServiceModel.Channels.WebSocketTransportUsage)(2))]
        public System.ServiceModel.Channels.WebSocketTransportUsage TransportUsage { get { return default(System.ServiceModel.Channels.WebSocketTransportUsage); } set { } }
        public override bool Equals(object obj) { return default(bool); }
        public bool Equals(System.ServiceModel.Channels.WebSocketTransportSettings other) { return default(bool); }
        public override int GetHashCode() { return default(int); }
    }
    public enum WebSocketTransportUsage
    {
        Always = 1,
        Never = 2,
        WhenDuplex = 0,
    }
}
