// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Runtime;
using System.Security.Authentication.ExtendedProtection;

namespace System.ServiceModel.Channels
{
    public class HttpTransportBindingElement
        : TransportBindingElement
    {
        private HostNameComparisonMode _hostNameComparisonMode;
        private bool _inheritBaseAddressSettings;
        private int _maxBufferSize;
        private bool _maxBufferSizeInitialized;
        private string _method;
        private AuthenticationSchemes _proxyAuthenticationScheme;
        private string _realm;
        private TimeSpan _requestInitializationTimeout;
        private TransferMode _transferMode;
        private bool _useDefaultWebProxy;
        private WebSocketTransportSettings _webSocketSettings;
        private ExtendedProtectionPolicy _extendedProtectionPolicy;
        private int _maxPendingAccepts;
        private MruCache<string, HttpClient> _httpClientCache;

        public HttpTransportBindingElement()
            : base()
        {
            AllowCookies = HttpTransportDefaults.AllowCookies;
            AuthenticationScheme = HttpTransportDefaults.AuthenticationScheme;
            BypassProxyOnLocal = HttpTransportDefaults.BypassProxyOnLocal;
            DecompressionEnabled = HttpTransportDefaults.DecompressionEnabled;
            _hostNameComparisonMode = HttpTransportDefaults.HostNameComparisonMode;
            KeepAliveEnabled = HttpTransportDefaults.KeepAliveEnabled;
            _maxBufferSize = TransportDefaults.MaxBufferSize;
            _maxPendingAccepts = HttpTransportDefaults.DefaultMaxPendingAccepts;
            _method = string.Empty;
            _proxyAuthenticationScheme = HttpTransportDefaults.ProxyAuthenticationScheme;
            Proxy = HttpTransportDefaults.Proxy;
            ProxyAddress = HttpTransportDefaults.ProxyAddress;
            _realm = HttpTransportDefaults.Realm;
            _requestInitializationTimeout = HttpTransportDefaults.RequestInitializationTimeout;
            _transferMode = HttpTransportDefaults.TransferMode;
            UnsafeConnectionNtlmAuthentication = HttpTransportDefaults.UnsafeConnectionNtlmAuthentication;
            _useDefaultWebProxy = HttpTransportDefaults.UseDefaultWebProxy;
            _webSocketSettings = HttpTransportDefaults.GetDefaultWebSocketTransportSettings();
        }

        protected HttpTransportBindingElement(HttpTransportBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            AllowCookies = elementToBeCloned.AllowCookies;
            AuthenticationScheme = elementToBeCloned.AuthenticationScheme;
            BypassProxyOnLocal = elementToBeCloned.BypassProxyOnLocal;
            DecompressionEnabled = elementToBeCloned.DecompressionEnabled;
            _hostNameComparisonMode = elementToBeCloned._hostNameComparisonMode;
            _inheritBaseAddressSettings = elementToBeCloned.InheritBaseAddressSettings;
            KeepAliveEnabled = elementToBeCloned.KeepAliveEnabled;
            _maxBufferSize = elementToBeCloned._maxBufferSize;
            _maxBufferSizeInitialized = elementToBeCloned._maxBufferSizeInitialized;
            _maxPendingAccepts = elementToBeCloned._maxPendingAccepts;
            _method = elementToBeCloned._method;
            Proxy = elementToBeCloned.Proxy;
            ProxyAddress = elementToBeCloned.ProxyAddress;
            _proxyAuthenticationScheme = elementToBeCloned._proxyAuthenticationScheme;
            _realm = elementToBeCloned._realm;
            _requestInitializationTimeout = elementToBeCloned._requestInitializationTimeout;
            _transferMode = elementToBeCloned._transferMode;
            UnsafeConnectionNtlmAuthentication = elementToBeCloned.UnsafeConnectionNtlmAuthentication;
            _useDefaultWebProxy = elementToBeCloned._useDefaultWebProxy;
            _webSocketSettings = elementToBeCloned._webSocketSettings.Clone();
            _extendedProtectionPolicy = elementToBeCloned.ExtendedProtectionPolicy;
            MessageHandlerFactory = elementToBeCloned.MessageHandlerFactory;
        }

        [DefaultValue(HttpTransportDefaults.AllowCookies)]
        public bool AllowCookies { get; set; }

        [DefaultValue(HttpTransportDefaults.AuthenticationScheme)]
        public AuthenticationSchemes AuthenticationScheme { get; set; }

        [DefaultValue(HttpTransportDefaults.BypassProxyOnLocal)]
        public bool BypassProxyOnLocal { get; set; }

        [DefaultValue(HttpTransportDefaults.DecompressionEnabled)]
        public bool DecompressionEnabled { get; set; }

        [DefaultValue(HttpTransportDefaults.HostNameComparisonMode)]
        public HostNameComparisonMode HostNameComparisonMode
        {
            get
            {
                return _hostNameComparisonMode;
            }
            set
            {
                HostNameComparisonModeHelper.Validate(value);
                _hostNameComparisonMode = value;
            }
        }

        public HttpMessageHandlerFactory MessageHandlerFactory { get; set; }

        public ExtendedProtectionPolicy ExtendedProtectionPolicy
        {
            get
            {
                return _extendedProtectionPolicy;
            }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }

                if (value.PolicyEnforcement == PolicyEnforcement.Always &&
                    !System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy.OSSupportsExtendedProtection)
                {
                    ExceptionHelper.PlatformNotSupported(SR.ExtendedProtectionNotSupported);
                }

                _extendedProtectionPolicy = value;
            }
        }

        // MB#26970: used by MEX to ensure that we don't conflict on base-address scoped settings
        internal bool InheritBaseAddressSettings
        {
            get
            {
                return _inheritBaseAddressSettings;
            }
            set
            {
                _inheritBaseAddressSettings = value;
            }
        }

        [DefaultValue(HttpTransportDefaults.KeepAliveEnabled)]
        public bool KeepAliveEnabled { get; set; }

        // client
        // server
        [DefaultValue(TransportDefaults.MaxBufferSize)]
        public int MaxBufferSize
        {
            get
            {
                if (_maxBufferSizeInitialized || TransferMode != TransferMode.Buffered)
                {
                    return _maxBufferSize;
                }

                long maxReceivedMessageSize = MaxReceivedMessageSize;
                if (maxReceivedMessageSize > int.MaxValue)
                {
                    return int.MaxValue;
                }
                else
                {
                    return (int)maxReceivedMessageSize;
                }
            }
            set
            {
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SR.ValueMustBePositive));
                }

                _maxBufferSizeInitialized = true;
                _maxBufferSize = value;
            }
        }

        // server
        [DefaultValue(HttpTransportDefaults.DefaultMaxPendingAccepts)]
        public int MaxPendingAccepts
        {
            get
            {
                return _maxPendingAccepts;
            }
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SR.ValueMustBeNonNegative));
                }

                if (value > HttpTransportDefaults.MaxPendingAcceptsUpperLimit)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SR.Format(SR.HttpMaxPendingAcceptsTooLargeError, HttpTransportDefaults.MaxPendingAcceptsUpperLimit)));
                }

                _maxPendingAccepts = value;
            }
        }

        // string.Empty == wildcard
        internal string Method
        {
            get
            {
                return _method;
            }

            set
            {
                _method = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
            }
        }

        // fully specified proxy by client
        [DefaultValue(HttpTransportDefaults.Proxy)]
        public IWebProxy Proxy { get; set; }
        
        [DefaultValue(HttpTransportDefaults.ProxyAddress)]
        [TypeConverter(typeof(UriTypeConverter))]
        public Uri ProxyAddress { get; set; }

        [DefaultValue(HttpTransportDefaults.ProxyAuthenticationScheme)]
        public AuthenticationSchemes ProxyAuthenticationScheme
        {
            get
            {
                return _proxyAuthenticationScheme;
            }

            set
            {
                if (!value.IsSingleton())
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(nameof(value), SR.Format(SR.HttpProxyRequiresSingleAuthScheme,
                        value));
                }

                _proxyAuthenticationScheme = value;
            }
        }

        [DefaultValue(HttpTransportDefaults.Realm)]
        public string Realm
        {
            get
            {
                return _realm;
            }
            set
            {
                _realm = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
            }
        }

        [DefaultValue(typeof(TimeSpan), HttpTransportDefaults.RequestInitializationTimeoutString)]
        public TimeSpan RequestInitializationTimeout
        {
            get
            {
                return _requestInitializationTimeout;
            }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value, SR.SFxTimeoutOutOfRange0));
                }
                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value, SR.SFxTimeoutOutOfRangeTooBig));
                }

                _requestInitializationTimeout = value;
            }
        }

        public override string Scheme { get { return "http"; } }

        // client
        // server
        [DefaultValue(HttpTransportDefaults.TransferMode)]
        public TransferMode TransferMode
        {
            get
            {
                return _transferMode;
            }
            set
            {
                TransferModeHelper.Validate(value);
                _transferMode = value;
            }
        }

        public WebSocketTransportSettings WebSocketSettings
        {
            get
            {
                return _webSocketSettings;
            }
            set
            {
                _webSocketSettings = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
            }
        }

        internal virtual bool GetSupportsClientAuthenticationImpl(AuthenticationSchemes effectiveAuthenticationSchemes)
        {
            return effectiveAuthenticationSchemes != AuthenticationSchemes.None &&
                effectiveAuthenticationSchemes.IsNotSet(AuthenticationSchemes.Anonymous);
        }

        internal virtual bool GetSupportsClientWindowsIdentityImpl(AuthenticationSchemes effectiveAuthenticationSchemes)
        {
            return effectiveAuthenticationSchemes != AuthenticationSchemes.None &&
                effectiveAuthenticationSchemes.IsNotSet(AuthenticationSchemes.Anonymous);
        }

        [DefaultValue(HttpTransportDefaults.UnsafeConnectionNtlmAuthentication)]
        public bool UnsafeConnectionNtlmAuthentication { get; set; }

        [DefaultValue(HttpTransportDefaults.UseDefaultWebProxy)]
        public bool UseDefaultWebProxy
        {
            get
            {
                return _useDefaultWebProxy;
            }
            set
            {
                _useDefaultWebProxy = value;
            }
        }

        internal string GetWsdlTransportUri(bool useWebSocketTransport)
        {
            if (useWebSocketTransport)
            {
                return TransportPolicyConstants.WebSocketTransportUri;
            }

            return TransportPolicyConstants.HttpTransportUri;
        }

        public override BindingElement Clone()
        {
            return new HttpTransportBindingElement(this);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                AuthenticationSchemes effectiveAuthenticationSchemes = AuthenticationScheme;
                // Desktop: HttpTransportBindingElement.GetEffectiveAuthenticationSchemes(this.AuthenticationScheme, context.BindingParameters);

                return (T)(object)new SecurityCapabilities(GetSupportsClientAuthenticationImpl(effectiveAuthenticationSchemes),
                    effectiveAuthenticationSchemes == AuthenticationSchemes.Negotiate,
                    GetSupportsClientWindowsIdentityImpl(effectiveAuthenticationSchemes),
                    ProtectionLevel.None,
                    ProtectionLevel.None);
            }
            else if (typeof(T) == typeof(IBindingDeliveryCapabilities))
            {
                return (T)(object)new BindingDeliveryCapabilitiesHelper();
            }
            else if (typeof(T) == typeof(ExtendedProtectionPolicy))
            {
                return (T)(object)ExtendedProtectionPolicy;
            }
            else if (typeof(T) == typeof(ITransportCompressionSupport))
            {
                return (T)(object)new TransportCompressionSupportHelper();
            }
            else if (typeof(T) == typeof(MruCache<string, HttpClient>))
            {
                EnsureHttpClientCache();
                return (T)(object)_httpClientCache;
            }
            else if (typeof(T) == typeof(Tuple<TransferMode>))
            {
                // Work around for ReliableSessionBindingElement.VerifyTransportMode not being able to
                // reference HttpTransportBindingElement to fetch the TransferMode.
                return (T)(object)Tuple.Create(TransferMode);
            }
            else
            {
                Contract.Assert(context.BindingParameters != null);
                if (context.BindingParameters.Find<MessageEncodingBindingElement>() == null)
                {
                    context.BindingParameters.Add(new TextMessageEncodingBindingElement());
                }
                return base.GetProperty<T>(context);
            }
        }

        private MruCache<string, HttpClient> EnsureHttpClientCache()
        {
            if (_httpClientCache == null || !_httpClientCache.AddRef())
            {
                _httpClientCache = new MruCache<string, HttpClient>(10);
            }

            return _httpClientCache;
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (typeof(TChannel) == typeof(IRequestChannel))
            {
                return WebSocketSettings.TransportUsage != WebSocketTransportUsage.Always;
            }
            else if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return WebSocketSettings.TransportUsage != WebSocketTransportUsage.Never;
            }
            return false;
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }

            if (MessageHandlerFactory != null)
            {
                throw FxTrace.Exception.AsError(new InvalidOperationException(SR.Format(SR.HttpPipelineNotSupportedOnClientSide, "MessageHandlerFactory")));
            }

            if (!CanBuildChannelFactory<TChannel>(context))
            {
                Contract.Assert(context.Binding != null);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("TChannel", SR.Format(SR.CouldnTCreateChannelForChannelType2, context.Binding.Name, typeof(TChannel)));
            }

            if (AuthenticationScheme == AuthenticationSchemes.None)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", SR.Format(SR.HttpAuthSchemeCannotBeNone,
                    AuthenticationScheme));
            }
            else if (!AuthenticationScheme.IsSingleton() && AuthenticationScheme != AuthenticationSchemes.IntegratedWindowsAuthentication)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", SR.Format(SR.HttpRequiresSingleAuthScheme,
                    AuthenticationScheme));
            }

            return (IChannelFactory<TChannel>)(object)new HttpChannelFactory<TChannel>(this, context);
        }

        internal override bool IsMatch(BindingElement b)
        {
            if (!base.IsMatch(b))
            {
                return false;
            }

            HttpTransportBindingElement http = b as HttpTransportBindingElement;
            if (http == null)
            {
                return false;
            }

            if (AllowCookies != http.AllowCookies)
            {
                return false;
            }

            if (AuthenticationScheme != http.AuthenticationScheme)
            {
                return false;
            }

            if (DecompressionEnabled != http.DecompressionEnabled)
            {
                return false;
            }

            if (_hostNameComparisonMode != http._hostNameComparisonMode)
            {
                return false;
            }

            if (_inheritBaseAddressSettings != http._inheritBaseAddressSettings)
            {
                return false;
            }

            if (KeepAliveEnabled != http.KeepAliveEnabled)
            {
                return false;
            }

            if (_maxBufferSize != http._maxBufferSize)
            {
                return false;
            }

            if (_method != http._method)
            {
                return false;
            }

            if (_realm != http._realm)
            {
                return false;
            }

            if (_transferMode != http._transferMode)
            {
                return false;
            }

            if (UnsafeConnectionNtlmAuthentication != http.UnsafeConnectionNtlmAuthentication)
            {
                return false;
            }

            if (_useDefaultWebProxy != http._useDefaultWebProxy)
            {
                return false;
            }

            if (!WebSocketSettings.Equals(http.WebSocketSettings))
            {
                return false;
            }

            return true;
        }

        private MessageEncodingBindingElement FindMessageEncodingBindingElement(BindingElementCollection bindingElements, out bool createdNew)
        {
            createdNew = false;
            MessageEncodingBindingElement encodingBindingElement = bindingElements.Find<MessageEncodingBindingElement>();
            if (encodingBindingElement == null)
            {
                createdNew = true;
                encodingBindingElement = new TextMessageEncodingBindingElement();
            }
            return encodingBindingElement;
        }

        private class BindingDeliveryCapabilitiesHelper : IBindingDeliveryCapabilities
        {
            internal BindingDeliveryCapabilitiesHelper()
            {
            }
            bool IBindingDeliveryCapabilities.AssuresOrderedDelivery
            {
                get { return false; }
            }

            bool IBindingDeliveryCapabilities.QueuedDelivery
            {
                get { return false; }
            }
        }

        private class TransportCompressionSupportHelper : ITransportCompressionSupport
        {
            public bool IsCompressionFormatSupported(CompressionFormat compressionFormat)
            {
                return true;
            }
        }
    }
}
