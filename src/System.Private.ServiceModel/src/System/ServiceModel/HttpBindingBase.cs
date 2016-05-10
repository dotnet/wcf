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
    public abstract class HttpBindingBase : Binding, IBindingRuntimePreferences
    {
        // private BindingElements
        private HttpTransportBindingElement _httpTransport;
        private HttpsTransportBindingElement _httpsTransport;
        private TextMessageEncodingBindingElement _textEncoding;

        internal HttpBindingBase()
        {
            _httpTransport = new HttpTransportBindingElement();
            _httpsTransport = new HttpsTransportBindingElement();

            _textEncoding = new TextMessageEncodingBindingElement();
            _textEncoding.MessageVersion = MessageVersion.Soap11;
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

        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get
            {
                return _textEncoding.ReaderQuotas;
            }

            set
            {
                if (value == null)
                {
                    throw FxTrace.Exception.ArgumentNull("value");
                }

                value.CopyTo(_textEncoding.ReaderQuotas);
                this.SetReaderQuotas(value);
            }
        }

        public override string Scheme
        {
            get
            {
                return this.GetTransport().Scheme;
            }
        }

        public EnvelopeVersion EnvelopeVersion
        {
            get { return this.GetEnvelopeVersion(); }
        }

        public Encoding TextEncoding
        {
            get
            {
                return _textEncoding.WriteEncoding;
            }

            set
            {
                _textEncoding.WriteEncoding = value;
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

        public bool UseDefaultWebProxy
        {
            get
            {
                return _httpTransport.UseDefaultWebProxy;
            }
        }

        bool IBindingRuntimePreferences.ReceiveSynchronously
        {
            get { return false; }
        }

        internal TextMessageEncodingBindingElement TextMessageEncodingBindingElement
        {
            get
            {
                return _textEncoding;
            }
        }

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
            Fx.Assert(this.BasicHttpSecurity != null, "this.BasicHttpSecurity should not return null from a derived class.");

            BasicHttpSecurity basicHttpSecurity = this.BasicHttpSecurity;
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
            BasicHttpSecurity security = this.BasicHttpSecurity;
            if (security == null)
            {
                return;
            }

            BasicHttpSecurityMode mode = security.Mode;
            if (mode == BasicHttpSecurityMode.None)
            {
                return;
            }
            else if (mode == BasicHttpSecurityMode.Message || mode == BasicHttpSecurityMode.TransportWithMessageCredential)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.Format(SR.UnsupportedSecuritySetting, "Mode", mode)));
            }

            // Transport.ClientCredentialType = InheritedFromHost are not supported.
            Fx.Assert(
                (mode == BasicHttpSecurityMode.Transport) || (mode == BasicHttpSecurityMode.TransportCredentialOnly),
                "Unexpected BasicHttpSecurityMode value: " + mode);
            HttpTransportSecurity transport = security.Transport;
            if (transport != null && transport.ClientCredentialType == HttpClientCredentialType.InheritedFromHost)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.Format(SR.UnsupportedSecuritySetting, "Transport.ClientCredentialType", transport.ClientCredentialType)));
            }
        }
    }
}
