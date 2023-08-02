// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
using System.Runtime;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace System.ServiceModel
{
    public abstract class HttpBindingBase : Binding
    {
        // private BindingElements
        private HttpTransportBindingElement _httpTransport;
        private HttpsTransportBindingElement _httpsTransport;

        internal HttpBindingBase()
        {
            _httpTransport = new HttpTransportBindingElement();
            _httpsTransport = new HttpsTransportBindingElement();

            TextMessageEncodingBindingElement = new TextMessageEncodingBindingElement();
            TextMessageEncodingBindingElement.MessageVersion = MessageVersion.Soap11;
            MtomMessageEncodingBindingElement = new MtomMessageEncodingBindingElement();
            MtomMessageEncodingBindingElement.MessageVersion = MessageVersion.Soap11;

            _httpsTransport.WebSocketSettings = _httpTransport.WebSocketSettings;
        }

        [DefaultValue(HttpTransportDefaults.AllowCookies)]
        public bool AllowCookies
        {
            get
            {
                return _httpTransport.AllowCookies;
            }

            set
            {
                _httpTransport.AllowCookies = value;
                _httpsTransport.AllowCookies = value;
            }
        }

        [DefaultValue(HttpTransportDefaults.BypassProxyOnLocal)]
        public bool BypassProxyOnLocal
        {
            get
            {
                return _httpTransport.BypassProxyOnLocal;
            }

            set
            {
                _httpTransport.BypassProxyOnLocal = value;
                _httpsTransport.BypassProxyOnLocal = value;
            }
        }

        [DefaultValue(HttpTransportDefaults.HostNameComparisonMode)]
        public HostNameComparisonMode HostNameComparisonMode
        {
            get
            {
                return _httpTransport.HostNameComparisonMode;
            }

            set
            {
                _httpTransport.HostNameComparisonMode = value;
                _httpsTransport.HostNameComparisonMode = value;
            }
        }

        [DefaultValue(TransportDefaults.MaxBufferSize)]
        public int MaxBufferSize
        {
            get
            {
                return _httpTransport.MaxBufferSize;
            }

            set
            {
                _httpTransport.MaxBufferSize = value;
                _httpsTransport.MaxBufferSize = value;
                MtomMessageEncodingBindingElement.MaxBufferSize = value;
            }
        }

        [DefaultValue(TransportDefaults.MaxBufferPoolSize)]
        public long MaxBufferPoolSize
        {
            get
            {
                return _httpTransport.MaxBufferPoolSize;
            }

            set
            {
                _httpTransport.MaxBufferPoolSize = value;
                _httpsTransport.MaxBufferPoolSize = value;
            }
        }

        [DefaultValue(TransportDefaults.MaxReceivedMessageSize)]
        public long MaxReceivedMessageSize
        {
            get
            {
                return _httpTransport.MaxReceivedMessageSize;
            }

            set
            {
                _httpTransport.MaxReceivedMessageSize = value;
                _httpsTransport.MaxReceivedMessageSize = value;
            }
        }

        [DefaultValue(HttpTransportDefaults.ProxyAddress)]
        [TypeConverter(typeof(UriTypeConverter))]
        public Uri ProxyAddress
        {
            get
            {
                return _httpTransport.ProxyAddress;
            }

            set
            {
                _httpTransport.ProxyAddress = value;
                _httpsTransport.ProxyAddress = value;
            }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get
            {
                return TextMessageEncodingBindingElement.ReaderQuotas;
            }

            set
            {
                if (value == null)
                {
                    throw FxTrace.Exception.ArgumentNull("value");
                }

                value.CopyTo(TextMessageEncodingBindingElement.ReaderQuotas);
                value.CopyTo(MtomMessageEncodingBindingElement.ReaderQuotas);

                SetReaderQuotas(value);
            }
        }

        public override string Scheme
        {
            get
            {
                return GetTransport().Scheme;
            }
        }

        public EnvelopeVersion EnvelopeVersion
        {
            get { return GetEnvelopeVersion(); }
        }

        public Encoding TextEncoding
        {
            get
            {
                return TextMessageEncodingBindingElement.WriteEncoding;
            }

            set
            {
                TextMessageEncodingBindingElement.WriteEncoding = value;
                MtomMessageEncodingBindingElement.WriteEncoding = value;
            }
        }

        [DefaultValue(HttpTransportDefaults.TransferMode)]
        public TransferMode TransferMode
        {
            get
            {
                return _httpTransport.TransferMode;
            }

            set
            {
                _httpTransport.TransferMode = value;
                _httpsTransport.TransferMode = value;
            }
        }

        [DefaultValue(HttpTransportDefaults.UseDefaultWebProxy)]
        public bool UseDefaultWebProxy
        {
            get
            {
                return _httpTransport.UseDefaultWebProxy;
            }

            set
            {
                _httpTransport.UseDefaultWebProxy = value;
                _httpsTransport.UseDefaultWebProxy = value;
            }
        }

        internal TextMessageEncodingBindingElement TextMessageEncodingBindingElement { get; }

        internal MtomMessageEncodingBindingElement MtomMessageEncodingBindingElement { get; }

        internal abstract BasicHttpSecurity BasicHttpSecurity
        {
            get;
        }

        internal WebSocketTransportSettings InternalWebSocketSettings
        {
            get
            {
                return _httpTransport.WebSocketSettings;
            }
        }

        internal static bool GetSecurityModeFromTransport(HttpTransportBindingElement http, HttpTransportSecurity transportSecurity, out UnifiedSecurityMode mode)
        {
            mode = UnifiedSecurityMode.None;
            if (http == null)
            {
                return false;
            }

            Fx.Assert(http.AuthenticationScheme.IsSingleton(), "authenticationScheme used in an Http(s)ChannelFactory must be a singleton value.");

            if (http is HttpsTransportBindingElement)
            {
                mode = UnifiedSecurityMode.Transport | UnifiedSecurityMode.TransportWithMessageCredential;
                BasicHttpSecurity.EnableTransportSecurity((HttpsTransportBindingElement)http, transportSecurity);
            }
            else if (HttpTransportSecurity.IsDisabledTransportAuthentication(http))
            {
                mode = UnifiedSecurityMode.Message | UnifiedSecurityMode.None;
            }
            else if (!BasicHttpSecurity.IsEnabledTransportAuthentication(http, transportSecurity))
            {
                return false;
            }
            else
            {
                mode = UnifiedSecurityMode.TransportCredentialOnly;
            }

            return true;
        }

        internal TransportBindingElement GetTransport()
        {
            Fx.Assert(BasicHttpSecurity != null, "this.BasicHttpSecurity should not return null from a derived class.");

            BasicHttpSecurity basicHttpSecurity = BasicHttpSecurity;
            if (basicHttpSecurity.Mode == BasicHttpSecurityMode.Transport || basicHttpSecurity.Mode == BasicHttpSecurityMode.TransportWithMessageCredential)
            {
                basicHttpSecurity.EnableTransportSecurity(_httpsTransport);
                return _httpsTransport;
            }
            else if (basicHttpSecurity.Mode == BasicHttpSecurityMode.TransportCredentialOnly)
            {
                basicHttpSecurity.EnableTransportAuthentication(_httpTransport);
                return _httpTransport;
            }
            else
            {
                // ensure that there is no transport security
                basicHttpSecurity.DisableTransportAuthentication(_httpTransport);
                return _httpTransport;
            }
        }

        internal abstract EnvelopeVersion GetEnvelopeVersion();

        internal virtual void SetReaderQuotas(XmlDictionaryReaderQuotas readerQuotas)
        {
        }

        // In the Win8 profile, some settings for the binding security are not supported.
        internal virtual void CheckSettings()
        {
            BasicHttpSecurity security = BasicHttpSecurity;
            if (security == null)
            {
                return;
            }

            BasicHttpSecurityMode mode = security.Mode;
            if (mode == BasicHttpSecurityMode.None)
            {
                return;
            }
            else if (mode == BasicHttpSecurityMode.Message)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.Format(SR.UnsupportedSecuritySetting, nameof(security.Mode), mode)));
            }

            // Transport.ClientCredentialType = InheritedFromHost are not supported.
            Fx.Assert(
                (mode == BasicHttpSecurityMode.Transport) || (mode == BasicHttpSecurityMode.TransportCredentialOnly) || (mode == BasicHttpSecurityMode.TransportWithMessageCredential),
                "Unexpected BasicHttpSecurityMode value: " + mode);
            HttpTransportSecurity transport = security.Transport;
            if (transport != null && transport.ClientCredentialType == HttpClientCredentialType.InheritedFromHost)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.Format(SR.UnsupportedSecuritySetting, "Transport.ClientCredentialType", transport.ClientCredentialType)));
            }
        }
    }
}
