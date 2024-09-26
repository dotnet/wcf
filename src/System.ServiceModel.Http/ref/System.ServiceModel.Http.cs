//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     GenAPI Version: 8.0.10.36005
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace System.ServiceModel
{
    public partial class BasicHttpBinding : System.ServiceModel.HttpBindingBase
    {
        public BasicHttpBinding() { }
        public BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode securityMode) { }
        public System.ServiceModel.WSMessageEncoding MessageEncoding { get { throw null; } set { } }
        public System.ServiceModel.BasicHttpSecurity Security { get { throw null; } set { } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingParameterCollection parameters) { throw null; }
        public override System.ServiceModel.Channels.BindingElementCollection CreateBindingElements() { throw null; }
    }
    public enum BasicHttpMessageCredentialType
    {
        UserName = 0,
        Certificate = 1,
    }
    public sealed partial class BasicHttpMessageSecurity
    {
        public BasicHttpMessageSecurity() { }
        public System.ServiceModel.Security.SecurityAlgorithmSuite AlgorithmSuite { get { throw null; } set { } }
        public System.ServiceModel.BasicHttpMessageCredentialType ClientCredentialType { get { throw null; } set { } }
    }
    public partial class BasicHttpsBinding : System.ServiceModel.HttpBindingBase
    {
        public BasicHttpsBinding() { }
        public BasicHttpsBinding(System.ServiceModel.BasicHttpsSecurityMode securityMode) { }
        public System.ServiceModel.WSMessageEncoding MessageEncoding { get { throw null; } set { } }
        public System.ServiceModel.BasicHttpsSecurity Security { get { throw null; } set { } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingParameterCollection parameters) { throw null; }
        public override System.ServiceModel.Channels.BindingElementCollection CreateBindingElements() { throw null; }
    }
    public sealed partial class BasicHttpSecurity
    {
        public BasicHttpSecurity() { }
        public System.ServiceModel.BasicHttpMessageSecurity Message { get { throw null; } set { } }
        public System.ServiceModel.BasicHttpSecurityMode Mode { get { throw null; } set { } }
        public System.ServiceModel.HttpTransportSecurity Transport { get { throw null; } set { } }
    }
    public enum BasicHttpSecurityMode
    {
        None = 0,
        Transport = 1,
        Message = 2,
        TransportWithMessageCredential = 3,
        TransportCredentialOnly = 4,
    }
    public sealed partial class BasicHttpsSecurity
    {
        internal BasicHttpsSecurity() { }
        public System.ServiceModel.BasicHttpMessageSecurity Message { get { throw null; } set { } }
        public System.ServiceModel.BasicHttpsSecurityMode Mode { get { throw null; } set { } }
        public System.ServiceModel.HttpTransportSecurity Transport { get { throw null; } set { } }
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
        public bool AllowCookies { get { throw null; } set { } }
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool BypassProxyOnLocal { get { throw null; } set { } }
        public System.ServiceModel.EnvelopeVersion EnvelopeVersion { get { throw null; } }
        [System.ComponentModel.DefaultValueAttribute((long)524288)]
        public long MaxBufferPoolSize { get { throw null; } set { } }
        [System.ComponentModel.DefaultValueAttribute(65536)]
        public int MaxBufferSize { get { throw null; } set { } }
        [System.ComponentModel.DefaultValueAttribute((long)65536)]
        public long MaxReceivedMessageSize { get { throw null; } set { } }
        [System.ComponentModel.DefaultValueAttribute(null)]
        [System.ComponentModel.TypeConverterAttribute(typeof(System.UriTypeConverter))]
        public System.Uri ProxyAddress { get { throw null; } set { } }
        public System.Xml.XmlDictionaryReaderQuotas ReaderQuotas { get { throw null; } set { } }
        public override string Scheme { get { throw null; } }
        public System.Text.Encoding TextEncoding { get { throw null; } set { } }
        [System.ComponentModel.DefaultValueAttribute(System.ServiceModel.TransferMode.Buffered)]
        public System.ServiceModel.TransferMode TransferMode { get { throw null; } set { } }
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool UseDefaultWebProxy { get { throw null; } set { } }
    }
    public enum HttpClientCredentialType
    {
        None = 0,
        Basic = 1,
        Digest = 2,
        Ntlm = 3,
        Windows = 4,
        Certificate = 5,
        InheritedFromHost = 6,
    }
    public enum HttpProxyCredentialType
    {
        None = 0,
        Basic = 1,
        Digest = 2,
        Ntlm = 3,
        Windows = 4,
    }
    public sealed partial class HttpTransportSecurity
    {
        public HttpTransportSecurity() { }
        public System.ServiceModel.HttpClientCredentialType ClientCredentialType { get { throw null; } set { } }
        public System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy ExtendedProtectionPolicy { get { throw null; } set { } }
        public System.ServiceModel.HttpProxyCredentialType ProxyCredentialType { get { throw null; } set { } }
    }
    public partial class MessageSecurityOverHttp
    {
        public MessageSecurityOverHttp() { }
        public System.ServiceModel.Security.SecurityAlgorithmSuite AlgorithmSuite { get { throw null; } set { } }
        public System.ServiceModel.MessageCredentialType ClientCredentialType { get { throw null; } set { } }
        public bool NegotiateServiceCredential { get { throw null; } set { } }
        protected virtual bool IsSecureConversationEnabled() { throw null; }
    }
    public partial class NetHttpBinding : System.ServiceModel.HttpBindingBase
    {
        public NetHttpBinding() { }
        public NetHttpBinding(System.ServiceModel.BasicHttpSecurityMode securityMode) { }
        public NetHttpBinding(System.ServiceModel.BasicHttpSecurityMode securityMode, bool reliableSessionEnabled) { }
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public NetHttpBinding(string configurationName) { }
        [System.ComponentModel.DefaultValueAttribute(System.ServiceModel.NetHttpMessageEncoding.Binary)]
        public System.ServiceModel.NetHttpMessageEncoding MessageEncoding { get { throw null; } set { } }
        public System.ServiceModel.OptionalReliableSession ReliableSession { get { throw null; } set { } }
        public System.ServiceModel.BasicHttpSecurity Security { get { throw null; } set { } }
        public System.ServiceModel.Channels.WebSocketTransportSettings WebSocketSettings { get { throw null; } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingParameterCollection parameters) { throw null; }
        public override System.ServiceModel.Channels.BindingElementCollection CreateBindingElements() { throw null; }
    }
    public enum NetHttpMessageEncoding
    {
        Binary = 0,
        Text = 1,
        Mtom = 2,
    }
    public partial class NetHttpsBinding : System.ServiceModel.HttpBindingBase
    {
        public NetHttpsBinding() { }
        public NetHttpsBinding(System.ServiceModel.BasicHttpsSecurityMode securityMode) { }
        public NetHttpsBinding(System.ServiceModel.BasicHttpsSecurityMode securityMode, bool reliableSessionEnabled) { }
        [System.ComponentModel.DefaultValueAttribute(System.ServiceModel.NetHttpMessageEncoding.Binary)]
        public System.ServiceModel.NetHttpMessageEncoding MessageEncoding { get { throw null; } set { } }
        public System.ServiceModel.OptionalReliableSession ReliableSession { get { throw null; } set { } }
        public System.ServiceModel.BasicHttpsSecurity Security { get { throw null; } set { } }
        public System.ServiceModel.Channels.WebSocketTransportSettings WebSocketSettings { get { throw null; } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingParameterCollection parameters) { throw null; }
        public override System.ServiceModel.Channels.BindingElementCollection CreateBindingElements() { throw null; }
    }
    public sealed partial class NonDualMessageSecurityOverHttp : System.ServiceModel.MessageSecurityOverHttp
    {
        public NonDualMessageSecurityOverHttp() { }
        public bool EstablishSecurityContext { get { throw null; } set { } }
        protected override bool IsSecureConversationEnabled() { throw null; }
    }
    public partial class WS2007HttpBinding : System.ServiceModel.WSHttpBinding
    {
        public WS2007HttpBinding() { }
        public WS2007HttpBinding(System.ServiceModel.SecurityMode securityMode) { }
        public WS2007HttpBinding(System.ServiceModel.SecurityMode securityMode, bool reliableSessionEnabled) { }
        protected override System.ServiceModel.Channels.SecurityBindingElement CreateMessageSecurity() { throw null; }
    }
    public partial class WSHttpBinding : System.ServiceModel.WSHttpBindingBase
    {
        public WSHttpBinding() { }
        public WSHttpBinding(System.ServiceModel.SecurityMode securityMode) { }
        public WSHttpBinding(System.ServiceModel.SecurityMode securityMode, bool reliableSessionEnabled) { }
        public bool AllowCookies { get { throw null; } set { } }
        public System.ServiceModel.WSHttpSecurity Security { get { throw null; } set { } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingParameterCollection parameters) { throw null; }
        public override System.ServiceModel.Channels.BindingElementCollection CreateBindingElements() { throw null; }
        protected override System.ServiceModel.Channels.SecurityBindingElement CreateMessageSecurity() { throw null; }
        protected override System.ServiceModel.Channels.TransportBindingElement GetTransport() { throw null; }
    }
    public abstract partial class WSHttpBindingBase : System.ServiceModel.Channels.Binding
    {
        protected WSHttpBindingBase() { }
        protected WSHttpBindingBase(bool reliableSessionEnabled) { }
        public bool BypassProxyOnLocal { get { throw null; } set { } }
        public System.ServiceModel.EnvelopeVersion EnvelopeVersion { get { throw null; } set { } }
        public long MaxBufferPoolSize { get { throw null; } set { } }
        public long MaxReceivedMessageSize { get { throw null; } set { } }
        public System.ServiceModel.WSMessageEncoding MessageEncoding { get { throw null; } set { } }
        public System.Uri ProxyAddress { get { throw null; } set { } }
        public System.Xml.XmlDictionaryReaderQuotas ReaderQuotas { get { throw null; } set { } }
        public System.ServiceModel.OptionalReliableSession ReliableSession { get { throw null; } set { } }
        public override string Scheme { get { throw null; } }
        public System.Text.Encoding TextEncoding { get { throw null; } set { } }
        public bool TransactionFlow { get { throw null; } set { } }
        public bool UseDefaultWebProxy { get { throw null; } set { } }
        public override System.ServiceModel.Channels.BindingElementCollection CreateBindingElements() { throw null; }
        protected abstract System.ServiceModel.Channels.SecurityBindingElement CreateMessageSecurity();
        protected abstract System.ServiceModel.Channels.TransportBindingElement GetTransport();
    }
    public sealed partial class WSHttpSecurity
    {
        public WSHttpSecurity() { }
        public System.ServiceModel.NonDualMessageSecurityOverHttp Message { get { throw null; } set { } }
        public System.ServiceModel.SecurityMode Mode { get { throw null; } set { } }
        public System.ServiceModel.HttpTransportSecurity Transport { get { throw null; } set { } }
    }
    public enum WSMessageEncoding
    {
        Text = 0,
        Mtom = 1,
    }
}
namespace System.ServiceModel.Channels
{
    public sealed partial class HttpRequestMessageProperty : System.ServiceModel.Channels.IMessageProperty
    {
        public HttpRequestMessageProperty() { }
        public System.Net.WebHeaderCollection Headers { get { throw null; } }
        public string Method { get { throw null; } set { } }
        public static string Name { get { throw null; } }
        public string QueryString { get { throw null; } set { } }
        public bool SuppressEntityBody { get { throw null; } set { } }
        System.ServiceModel.Channels.IMessageProperty System.ServiceModel.Channels.IMessageProperty.CreateCopy() { throw null; }
    }
    public sealed partial class HttpResponseMessageProperty : System.ServiceModel.Channels.IMessageProperty
    {
        public HttpResponseMessageProperty() { }
        public System.Net.WebHeaderCollection Headers { get { throw null; } }
        public static string Name { get { throw null; } }
        public System.Net.HttpStatusCode StatusCode { get { throw null; } set { } }
        public string StatusDescription { get { throw null; } set { } }
        System.ServiceModel.Channels.IMessageProperty System.ServiceModel.Channels.IMessageProperty.CreateCopy() { throw null; }
    }
    public partial class HttpsTransportBindingElement : System.ServiceModel.Channels.HttpTransportBindingElement
    {
        public HttpsTransportBindingElement() { }
        protected HttpsTransportBindingElement(System.ServiceModel.Channels.HttpsTransportBindingElement elementToBeCloned) { }
        public bool RequireClientCertificate { get { throw null; } set { } }
        public override string Scheme { get { throw null; } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { throw null; }
        public override System.ServiceModel.Channels.BindingElement Clone() { throw null; }
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { throw null; }
    }
    public partial class HttpTransportBindingElement : System.ServiceModel.Channels.TransportBindingElement
    {
        public HttpTransportBindingElement() { }
        protected HttpTransportBindingElement(System.ServiceModel.Channels.HttpTransportBindingElement elementToBeCloned) { }
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool AllowCookies { get { throw null; } set { } }
        [System.ComponentModel.DefaultValueAttribute(System.Net.AuthenticationSchemes.Anonymous)]
        public System.Net.AuthenticationSchemes AuthenticationScheme { get { throw null; } set { } }
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool BypassProxyOnLocal { get { throw null; } set { } }
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool DecompressionEnabled { get { throw null; } set { } }
        public System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy ExtendedProtectionPolicy { get { throw null; } set { } }
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool KeepAliveEnabled { get { throw null; } set { } }
        [System.ComponentModel.DefaultValueAttribute(65536)]
        public int MaxBufferSize { get { throw null; } set { } }
        public System.Net.IWebProxy Proxy { get { throw null; } set { } }
        [System.ComponentModel.DefaultValueAttribute(null)]
        [System.ComponentModel.TypeConverterAttribute(typeof(System.UriTypeConverter))]
        public System.Uri ProxyAddress { get { throw null; } set { } }
        [System.ComponentModel.DefaultValueAttribute(System.Net.AuthenticationSchemes.Anonymous)]
        public System.Net.AuthenticationSchemes ProxyAuthenticationScheme { get { throw null; } set { } }
        public override string Scheme { get { throw null; } }
        [System.ComponentModel.DefaultValueAttribute(System.ServiceModel.TransferMode.Buffered)]
        public System.ServiceModel.TransferMode TransferMode { get { throw null; } set { } }
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool UseDefaultWebProxy { get { throw null; } set { } }
        public System.ServiceModel.Channels.WebSocketTransportSettings WebSocketSettings { get { throw null; } set { } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { throw null; }
        public override bool CanBuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { throw null; }
        public override System.ServiceModel.Channels.BindingElement Clone() { throw null; }
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { throw null; }
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
        public bool DisablePayloadMasking { get { throw null; } set { } }
        [System.ComponentModel.DefaultValueAttribute(typeof(System.TimeSpan), "00:00:00")]
        public System.TimeSpan KeepAliveInterval { get { throw null; } set { } }
        [System.ComponentModel.DefaultValueAttribute(null)]
        public string SubProtocol { get { throw null; } set { } }
        [System.ComponentModel.DefaultValueAttribute(System.ServiceModel.Channels.WebSocketTransportUsage.Never)]
        public System.ServiceModel.Channels.WebSocketTransportUsage TransportUsage { get { throw null; } set { } }
        public override bool Equals(object obj) { throw null; }
        public bool Equals(System.ServiceModel.Channels.WebSocketTransportSettings other) { throw null; }
        public override int GetHashCode() { throw null; }
    }
    public enum WebSocketTransportUsage
    {
        WhenDuplex = 0,
        Always = 1,
        Never = 2,
    }
}