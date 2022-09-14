// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ServiceModel.Channels;
using System.Xml;

namespace System.ServiceModel
{
    public abstract class WSHttpBindingBase : Binding
    {
        private OptionalReliableSession _reliableSession;
        private TextMessageEncodingBindingElement _textEncoding;
        private MtomMessageEncodingBindingElement _mtomEncoding;
        private ReliableSessionBindingElement _session;

        protected WSHttpBindingBase()
            : base()
        {
            Initialize();
        }

        protected WSHttpBindingBase(bool reliableSessionEnabled) : this()
        {
            ReliableSession.Enabled = reliableSessionEnabled;
        }

        [DefaultValue(HttpTransportDefaults.BypassProxyOnLocal)]
        public bool BypassProxyOnLocal
        {
            get { return HttpTransport.BypassProxyOnLocal; }
            set
            {
                HttpTransport.BypassProxyOnLocal = value;
                HttpsTransport.BypassProxyOnLocal = value;
            }
        }

        [DefaultValue(false)]
        public bool TransactionFlow
        {
            get { return false; }
            set
            {
                if (value)
                {
                    throw ExceptionHelper.PlatformNotSupported();
                }
            }
        }

        [DefaultValue(TransportDefaults.MaxBufferPoolSize)]
        public long MaxBufferPoolSize
        {
            get { return HttpTransport.MaxBufferPoolSize; }
            set
            {
                HttpTransport.MaxBufferPoolSize = value;
                HttpsTransport.MaxBufferPoolSize = value;
            }
        }

        [DefaultValue(TransportDefaults.MaxReceivedMessageSize)]
        public long MaxReceivedMessageSize
        {
            get { return HttpTransport.MaxReceivedMessageSize; }
            set
            {
                if (value > int.MaxValue)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new ArgumentOutOfRangeException(nameof(MaxReceivedMessageSize),
                        SR.MaxReceivedMessageSizeMustBeInIntegerRange));
                }
                HttpTransport.MaxReceivedMessageSize = value;
                HttpsTransport.MaxReceivedMessageSize = value;
                _mtomEncoding.MaxBufferSize = (int)value;
            }
        }

        public WSMessageEncoding MessageEncoding { get; set; }

        [DefaultValue(HttpTransportDefaults.ProxyAddress)]
        public Uri ProxyAddress
        {
            get { return HttpTransport.ProxyAddress; }
            set
            {
                HttpTransport.ProxyAddress = value;
                HttpsTransport.ProxyAddress = value;
            }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get { return _textEncoding.ReaderQuotas; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }

                value.CopyTo(_textEncoding.ReaderQuotas);
                value.CopyTo(_mtomEncoding.ReaderQuotas);
            }
        }

        public OptionalReliableSession ReliableSession
        {
            get { return _reliableSession; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(value)));
                }

                _reliableSession.CopySettings(value);
            }
        }

        public override string Scheme { get { return GetTransport().Scheme; } }

        public EnvelopeVersion EnvelopeVersion
        {
            get { return EnvelopeVersion.Soap12; }
        }

        public Text.Encoding TextEncoding
        {
            get { return _textEncoding.WriteEncoding; }
            set
            {
                _textEncoding.WriteEncoding = value;
                _mtomEncoding.WriteEncoding = value;
            }
        }

        [DefaultValue(HttpTransportDefaults.UseDefaultWebProxy)]
        public bool UseDefaultWebProxy
        {
            get { return HttpTransport.UseDefaultWebProxy; }
            set
            {
                HttpTransport.UseDefaultWebProxy = value;
                HttpsTransport.UseDefaultWebProxy = value;
            }
        }

        internal HttpTransportBindingElement HttpTransport { get; private set; }

        internal HttpsTransportBindingElement HttpsTransport { get; private set; }

        private void Initialize()
        {
            HttpTransport = new HttpTransportBindingElement();
            HttpsTransport = new HttpsTransportBindingElement();
            _session = new ReliableSessionBindingElement(true);
            _textEncoding = new TextMessageEncodingBindingElement();
            _textEncoding.MessageVersion = MessageVersion.Soap12WSAddressing10;
            _mtomEncoding = new MtomMessageEncodingBindingElement();
            _mtomEncoding.MessageVersion = MessageVersion.Soap12WSAddressing10;
            _reliableSession = new OptionalReliableSession(_session);
        }

        public override BindingElementCollection CreateBindingElements()
        {   // return collection of BindingElements
            BindingElementCollection bindingElements = new BindingElementCollection();
            // order of BindingElements is important
            // context

            // reliable
            if (_reliableSession.Enabled)
            {
                bindingElements.Add(_session);
            }

            // add security (*optional)
            SecurityBindingElement wsSecurity = CreateMessageSecurity();
            if (wsSecurity != null)
            {
                bindingElements.Add(wsSecurity);
            }

            // add encoding (text or mtom)
            WSMessageEncodingHelper.SyncUpEncodingBindingElementProperties(_textEncoding, _mtomEncoding);
            if (this.MessageEncoding == WSMessageEncoding.Text)
            {
                bindingElements.Add(_textEncoding);
            }
            else if (this.MessageEncoding == WSMessageEncoding.Mtom)
            {
                bindingElements.Add(_mtomEncoding);
            }

            // add transport (http or https)
            bindingElements.Add(GetTransport());

            return bindingElements.Clone();
        }

        protected abstract TransportBindingElement GetTransport();
        protected abstract SecurityBindingElement CreateMessageSecurity();
    }
}
