// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Security;
using System.Runtime;
using System.Security.Authentication.ExtendedProtection;
using System.ServiceModel.Description;
using Microsoft.Xml;

namespace System.ServiceModel.Channels
{
    public class HttpTransportBindingElement
        : TransportBindingElement, ITransportPolicyImport
    {
        private bool _allowCookies;
        private AuthenticationSchemes _authenticationScheme;
        private bool _decompressionEnabled;
        private HostNameComparisonMode _hostNameComparisonMode;
        private bool _keepAliveEnabled;
        private bool _inheritBaseAddressSettings;
        private int _maxBufferSize;
        private bool _maxBufferSizeInitialized;
        private string _method;
        private string _realm;
        private TimeSpan _requestInitializationTimeout;
        private TransferMode _transferMode;
        private bool _unsafeConnectionNtlmAuthentication;
        private bool _useDefaultWebProxy;
        private WebSocketTransportSettings _webSocketSettings;
#if !FEATURE_NETNATIVE
        private ExtendedProtectionPolicy _extendedProtectionPolicy;
#endif // !FEATURE_NETNATIVE
        private HttpMessageHandlerFactory _httpMessageHandlerFactory;
        private int _maxPendingAccepts;

        public HttpTransportBindingElement()
            : base()
        {
            _allowCookies = HttpTransportDefaults.AllowCookies;
            _authenticationScheme = HttpTransportDefaults.AuthenticationScheme;
            _decompressionEnabled = HttpTransportDefaults.DecompressionEnabled;
            _hostNameComparisonMode = HttpTransportDefaults.HostNameComparisonMode;
            _keepAliveEnabled = HttpTransportDefaults.KeepAliveEnabled;
            _maxBufferSize = TransportDefaults.MaxBufferSize;
            _maxPendingAccepts = HttpTransportDefaults.DefaultMaxPendingAccepts;
            _method = string.Empty;
            _realm = HttpTransportDefaults.Realm;
            _requestInitializationTimeout = HttpTransportDefaults.RequestInitializationTimeout;
            _transferMode = HttpTransportDefaults.TransferMode;
            _unsafeConnectionNtlmAuthentication = HttpTransportDefaults.UnsafeConnectionNtlmAuthentication;
            _useDefaultWebProxy = HttpTransportDefaults.UseDefaultWebProxy;
            _webSocketSettings = HttpTransportDefaults.GetDefaultWebSocketTransportSettings();
            _extendedProtectionPolicy = ChannelBindingUtility.DefaultPolicy;
        }

        protected HttpTransportBindingElement(HttpTransportBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _allowCookies = elementToBeCloned._allowCookies;
            _authenticationScheme = elementToBeCloned._authenticationScheme;
            _decompressionEnabled = elementToBeCloned._decompressionEnabled;
            _hostNameComparisonMode = elementToBeCloned._hostNameComparisonMode;
            _inheritBaseAddressSettings = elementToBeCloned.InheritBaseAddressSettings;
            _keepAliveEnabled = elementToBeCloned._keepAliveEnabled;
            _maxBufferSize = elementToBeCloned._maxBufferSize;
            _maxBufferSizeInitialized = elementToBeCloned._maxBufferSizeInitialized;
            _maxPendingAccepts = elementToBeCloned._maxPendingAccepts;
            _method = elementToBeCloned._method;
            _realm = elementToBeCloned._realm;
            _requestInitializationTimeout = elementToBeCloned._requestInitializationTimeout;
            _transferMode = elementToBeCloned._transferMode;
            _unsafeConnectionNtlmAuthentication = elementToBeCloned._unsafeConnectionNtlmAuthentication;
            _useDefaultWebProxy = elementToBeCloned._useDefaultWebProxy;
            _webSocketSettings = elementToBeCloned._webSocketSettings.Clone();
#if !FEATURE_NETNATIVE
            _extendedProtectionPolicy = elementToBeCloned.ExtendedProtectionPolicy;
#endif // !FEATURE_NETNATIVE
            this.MessageHandlerFactory = elementToBeCloned.MessageHandlerFactory;
        }

        [DefaultValue(HttpTransportDefaults.AllowCookies)]
        public bool AllowCookies
        {
            get
            {
                return _allowCookies;
            }
            set
            {
                _allowCookies = value;
            }
        }

        [DefaultValue(HttpTransportDefaults.AuthenticationScheme)]
        public AuthenticationSchemes AuthenticationScheme
        {
            get
            {
                return _authenticationScheme;
            }

            set
            {
                _authenticationScheme = value;
            }
        }

        [DefaultValue(HttpTransportDefaults.DecompressionEnabled)]
        public bool DecompressionEnabled
        {
            get
            {
                return _decompressionEnabled;
            }
            set
            {
                _decompressionEnabled = value;
            }
        }

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

        public HttpMessageHandlerFactory MessageHandlerFactory
        {
            get
            {
                return _httpMessageHandlerFactory;
            }
            set
            {
                _httpMessageHandlerFactory = value;
            }
        }

#if !FEATURE_NETNATIVE
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }

                if (value.PolicyEnforcement == PolicyEnforcement.Always &&
                    !System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy.OSSupportsExtendedProtection)
                {
                    ExceptionHelper.PlatformNotSupported(SRServiceModel.ExtendedProtectionNotSupported);
                }

                _extendedProtectionPolicy = value;
            }
        }
#endif // !FEATURE_NETNATIVE

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
        public bool KeepAliveEnabled
        {
            get
            {
                return _keepAliveEnabled;
            }
            set
            {
                _keepAliveEnabled = value;
            }
        }

        // client
        // server
        [DefaultValue(TransportDefaults.MaxBufferSize)]
        public int MaxBufferSize
        {
            get
            {
                if (_maxBufferSizeInitialized || TransferMode != TransferMode.Buffered)
                    return _maxBufferSize;

                long maxReceivedMessageSize = MaxReceivedMessageSize;
                if (maxReceivedMessageSize > int.MaxValue)
                    return int.MaxValue;
                else
                    return (int)maxReceivedMessageSize;
            }
            set
            {
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRServiceModel.ValueMustBePositive));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRServiceModel.ValueMustBeNonNegative));
                }

                if (value > HttpTransportDefaults.MaxPendingAcceptsUpperLimit)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        string.Format(SRServiceModel.HttpMaxPendingAcceptsTooLargeError, HttpTransportDefaults.MaxPendingAcceptsUpperLimit)));
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
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }

                _method = value;
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
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }

                _realm = value;
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRange0));
                }
                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRangeTooBig));
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
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                _webSocketSettings = value;
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
        public bool UnsafeConnectionNtlmAuthentication
        {
            get
            {
                return _unsafeConnectionNtlmAuthentication;
            }

            set
            {
                _unsafeConnectionNtlmAuthentication = value;
            }
        }

        public bool UseDefaultWebProxy
        {
            get
            {
                return _useDefaultWebProxy;
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                AuthenticationSchemes effectiveAuthenticationSchemes = this.AuthenticationScheme;
                // Desktop: HttpTransportBindingElement.GetEffectiveAuthenticationSchemes(this.AuthenticationScheme, context.BindingParameters);

                return (T)(object)new SecurityCapabilities(this.GetSupportsClientAuthenticationImpl(effectiveAuthenticationSchemes),
                    effectiveAuthenticationSchemes == AuthenticationSchemes.Negotiate,
                    this.GetSupportsClientWindowsIdentityImpl(effectiveAuthenticationSchemes),
                    ProtectionLevel.None,
                    ProtectionLevel.None);
            }
            else if (typeof(T) == typeof(IBindingDeliveryCapabilities))
            {
                return (T)(object)new BindingDeliveryCapabilitiesHelper();
            }
            else if (typeof(T) == typeof(TransferMode))
            {
                return (T)(object)this.TransferMode;
            }
#if !FEATURE_NETNATIVE
            else if (typeof(T) == typeof(ExtendedProtectionPolicy))
            {
                return (T)(object)this.ExtendedProtectionPolicy;
            }
#endif //!FEATURE_NETNATIVE
            else if (typeof(T) == typeof(ITransportCompressionSupport))
            {
                return (T)(object)new TransportCompressionSupportHelper();
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

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (typeof(TChannel) == typeof(IRequestChannel))
            {
                return this.WebSocketSettings.TransportUsage != WebSocketTransportUsage.Always;
            }
            else if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return this.WebSocketSettings.TransportUsage != WebSocketTransportUsage.Never;
            }
            return false;
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            if (this.MessageHandlerFactory != null)
            {
                throw FxTrace.Exception.AsError(new InvalidOperationException(string.Format(SRServiceModel.HttpPipelineNotSupportedOnClientSide, "MessageHandlerFactory")));
            }

            if (!this.CanBuildChannelFactory<TChannel>(context))
            {
                Contract.Assert(context.Binding != null);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("TChannel", string.Format(SRServiceModel.CouldnTCreateChannelForChannelType2, context.Binding.Name, typeof(TChannel)));
            }

            if (_authenticationScheme == AuthenticationSchemes.None)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", string.Format(SRServiceModel.HttpAuthSchemeCannotBeNone,
                    _authenticationScheme));
            }
            else if (!_authenticationScheme.IsSingleton())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", string.Format(SRServiceModel.HttpRequiresSingleAuthScheme,
                    _authenticationScheme));
            }

            return (IChannelFactory<TChannel>)(object)new HttpChannelFactory<TChannel>(this, context);
        }

        internal virtual void OnImportPolicy(MetadataImporter importer, PolicyConversionContext policyContext)
        {
        }

        void ITransportPolicyImport.ImportPolicy(MetadataImporter importer, PolicyConversionContext policyContext)
        {
            ICollection<XmlElement> bindingAssertions = policyContext.GetBindingAssertions();
            List<XmlElement> httpAuthAssertions = new List<XmlElement>();

            bool foundAssertion = false;
            foreach (XmlElement assertion in bindingAssertions)
            {
                if (assertion.NamespaceURI != TransportPolicyConstants.HttpTransportNamespace)
                {
                    continue;
                }

                switch (assertion.LocalName)
                {
                    case TransportPolicyConstants.BasicHttpAuthenticationName:
                        this.AuthenticationScheme = AuthenticationSchemes.Basic;
                        break;
                    case TransportPolicyConstants.DigestHttpAuthenticationName:
                        this.AuthenticationScheme = AuthenticationSchemes.Digest;
                        break;
                    case TransportPolicyConstants.NegotiateHttpAuthenticationName:
                        this.AuthenticationScheme = AuthenticationSchemes.Negotiate;
                        break;
                    case TransportPolicyConstants.NtlmHttpAuthenticationName:
                        this.AuthenticationScheme = AuthenticationSchemes.Ntlm;
                        break;
                    default:
                        continue;
                }

                if (foundAssertion)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(
                        string.Format(SRServiceModel.HttpTransportCannotHaveMultipleAuthenticationSchemes, policyContext.Contract.Namespace, policyContext.Contract.Name)));
                }

                foundAssertion = true;
                httpAuthAssertions.Add(assertion);
            }
            httpAuthAssertions.ForEach(delegate (XmlElement element) { bindingAssertions.Remove(element); });

            // This code is being used when we are generating the client configration. Before that, we should already set TransportUsage as Always if the server
            // is using WebSocket.
            if (this.WebSocketSettings.TransportUsage == WebSocketTransportUsage.Always)
            {
                foreach (XmlElement assertion in bindingAssertions)
                {
                    if (assertion.NamespaceURI != TransportPolicyConstants.WebSocketPolicyNamespace)
                    {
                        continue;
                    }

                    string transferMode = assertion.LocalName;
                    TransferMode result;
                    if (!Enum.TryParse<TransferMode>(transferMode, true, out result) || !TransferModeHelper.IsDefined(result) || result == TransferMode.Buffered)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(string.Format(
                                SRServiceModel.WebSocketTransportPolicyAssertionInvalid,
                                policyContext.Contract.Namespace,
                                policyContext.Contract.Name,
                                transferMode,
                                TransferMode.Streamed,
                                TransferMode.StreamedRequest,
                                TransferMode.StreamedResponse)));
                    }

                    this.TransferMode = result;
                    bindingAssertions.Remove(assertion);
                    break;
                }
            }

            OnImportPolicy(importer, policyContext);
        }

        internal override bool IsMatch(BindingElement b)
        {
            if (!base.IsMatch(b))
                return false;
            HttpTransportBindingElement http = b as HttpTransportBindingElement;
            if (http == null)
                return false;
            if (_allowCookies != http._allowCookies)
                return false;
            if (_authenticationScheme != http._authenticationScheme)
                return false;
            if (_decompressionEnabled != http._decompressionEnabled)
                return false;
            if (_hostNameComparisonMode != http._hostNameComparisonMode)
                return false;
            if (_inheritBaseAddressSettings != http._inheritBaseAddressSettings)
                return false;
            if (_keepAliveEnabled != http._keepAliveEnabled)
                return false;
            if (_maxBufferSize != http._maxBufferSize)
                return false;
            if (_method != http._method)
                return false;
            if (_realm != http._realm)
                return false;
            if (_transferMode != http._transferMode)
                return false;
            if (_unsafeConnectionNtlmAuthentication != http._unsafeConnectionNtlmAuthentication)
                return false;
            if (_useDefaultWebProxy != http._useDefaultWebProxy)
                return false;
            if (!this.WebSocketSettings.Equals(http.WebSocketSettings))
                return false;

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
