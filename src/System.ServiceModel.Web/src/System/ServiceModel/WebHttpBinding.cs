// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public class WebHttpBinding : Binding
    {
        private HttpsTransportBindingElement _httpsTransportBindingElement;
        // private BindingElements
        private HttpTransportBindingElement _httpTransportBindingElement;
        private WebHttpSecurity _security = new WebHttpSecurity();
        private WebMessageEncodingBindingElement _webMessageEncodingBindingElement;

        public WebHttpBinding() : base()
        {
            Initialize();
        }

        public WebHttpBinding(WebHttpSecurityMode securityMode) : base()
        {
            Initialize();
            _security.Mode = securityMode;
        }

        [DefaultValue(HttpTransportDefaults.AllowCookies)]
        public bool AllowCookies
        {
            get { return _httpTransportBindingElement.AllowCookies; }
            set
            {
                _httpTransportBindingElement.AllowCookies = value;
                _httpsTransportBindingElement.AllowCookies = value;
            }
        }

        [DefaultValue(HttpTransportDefaults.BypassProxyOnLocal)]
        public bool BypassProxyOnLocal
        {
            get { return _httpTransportBindingElement.BypassProxyOnLocal; }
            set
            {
                _httpTransportBindingElement.BypassProxyOnLocal = value;
                _httpsTransportBindingElement.BypassProxyOnLocal = value;
            }
        }

        public EnvelopeVersion EnvelopeVersion => EnvelopeVersion.None;

        // HostNameComparisonMode is not exposed on HttpTransportBindingElement in
        // dotnet/wcf, so it is intentionally omitted here (unlike .NET Framework).

        [DefaultValue(TransportDefaults.MaxBufferPoolSize)]
        public long MaxBufferPoolSize
        {
            get { return _httpTransportBindingElement.MaxBufferPoolSize; }
            set
            {
                _httpTransportBindingElement.MaxBufferPoolSize = value;
                _httpsTransportBindingElement.MaxBufferPoolSize = value;
            }
        }

        [DefaultValue(TransportDefaults.MaxBufferSize)]
        public int MaxBufferSize
        {
            get { return _httpTransportBindingElement.MaxBufferSize; }
            set
            {
                _httpTransportBindingElement.MaxBufferSize = value;
                _httpsTransportBindingElement.MaxBufferSize = value;
            }
        }

        [DefaultValue(TransportDefaults.MaxReceivedMessageSize)]
        public long MaxReceivedMessageSize
        {
            get { return _httpTransportBindingElement.MaxReceivedMessageSize; }
            set
            {
                _httpTransportBindingElement.MaxReceivedMessageSize = value;
                _httpsTransportBindingElement.MaxReceivedMessageSize = value;
            }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get { return _webMessageEncodingBindingElement.ReaderQuotas; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }

                value.CopyTo(_webMessageEncodingBindingElement.ReaderQuotas);
            }
        }

        public override string Scheme => GetTransport().Scheme;

        public WebHttpSecurity Security
        {
            get { return _security; }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));

                _security = value;
            }
        }

        [DefaultValue(HttpTransportDefaults.ProxyAddress)]
        [TypeConverter(typeof(UriTypeConverter))]
        public Uri ProxyAddress
        {
            get { return _httpTransportBindingElement.ProxyAddress; }
            set
            {
                _httpTransportBindingElement.ProxyAddress = value;
                _httpsTransportBindingElement.ProxyAddress = value;
            }
        }

        [DefaultValue(HttpTransportDefaults.UseDefaultWebProxy)]
        public bool UseDefaultWebProxy
        {
            get { return _httpTransportBindingElement.UseDefaultWebProxy; }
            set
            {
                _httpTransportBindingElement.UseDefaultWebProxy = value;
                _httpsTransportBindingElement.UseDefaultWebProxy = value;
            }
        }

        [DefaultValue(HttpTransportDefaults.TransferMode)]
        public TransferMode TransferMode
        {
            get { return _httpTransportBindingElement.TransferMode; }
            set
            {
                _httpTransportBindingElement.TransferMode = value;
                _httpsTransportBindingElement.TransferMode = value;
            }
        }

        [TypeConverter(typeof(System.ComponentModel.StringConverter))]
        public Encoding WriteEncoding
        {
            get { return _webMessageEncodingBindingElement.WriteEncoding; }
            set
            {
                _webMessageEncodingBindingElement.WriteEncoding = value;
            }
        }

        public WebContentTypeMapper ContentTypeMapper
        {
            get { return _webMessageEncodingBindingElement.ContentTypeMapper; }
            set
            {
                _webMessageEncodingBindingElement.ContentTypeMapper = value;
            }
        }

        public bool CrossDomainScriptAccessEnabled
        {
            get { return _webMessageEncodingBindingElement.CrossDomainScriptAccessEnabled; }
            set
            {
                _webMessageEncodingBindingElement.CrossDomainScriptAccessEnabled = value;
            }
        }

        public override BindingElementCollection CreateBindingElements()
        {
            // return collection of BindingElements
            BindingElementCollection bindingElements = new BindingElementCollection();
            // order of BindingElements is important
            // add encoding
            bindingElements.Add(_webMessageEncodingBindingElement);
            // add transport (http or https)
            bindingElements.Add(GetTransport());

            return bindingElements.Clone();
        }

        private TransportBindingElement GetTransport()
        {
            if (_security.Mode == WebHttpSecurityMode.Transport)
            {
                _security.EnableTransportSecurity(_httpsTransportBindingElement);
                _security.ApplyAuthorizationPolicySupport(_httpsTransportBindingElement);
                return _httpsTransportBindingElement;
            }
            else if (_security.Mode == WebHttpSecurityMode.TransportCredentialOnly)
            {
                _security.EnableTransportAuthentication(_httpTransportBindingElement);
                _security.ApplyAuthorizationPolicySupport(_httpTransportBindingElement);
                return _httpTransportBindingElement;
            }
            else
            {
                // ensure that there is no transport security
                _security.DisableTransportAuthentication(_httpTransportBindingElement);
                _security.ApplyAuthorizationPolicySupport(_httpTransportBindingElement);
                return _httpTransportBindingElement;
            }
        }

        private void Initialize()
        {
            _httpTransportBindingElement = new HttpTransportBindingElement
            {
                ManualAddressing = true
            };

            _httpsTransportBindingElement = new HttpsTransportBindingElement
            {
                ManualAddressing = true
            };

            _webMessageEncodingBindingElement = new WebMessageEncodingBindingElement
            {
                MessageVersion = MessageVersion.None
            };
        }
    }
}
