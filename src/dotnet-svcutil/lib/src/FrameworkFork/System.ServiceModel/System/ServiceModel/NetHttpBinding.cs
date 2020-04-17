// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Runtime;
using System.ServiceModel.Channels;
using System.Text;
using Microsoft.Xml;

namespace System.ServiceModel
{
    public class NetHttpBinding : HttpBindingBase
    {
        private BinaryMessageEncodingBindingElement _binaryMessageEncodingBindingElement;
        private ReliableSessionBindingElement _session;
        private OptionalReliableSession _reliableSession;
        private NetHttpMessageEncoding _messageEncoding;
        private BasicHttpSecurity _basicHttpSecurity;

        public NetHttpBinding()
            : this(BasicHttpSecurityMode.None)
        {
        }

        public NetHttpBinding(BasicHttpSecurityMode securityMode)
            : base()
        {
            this.Initialize();
            _basicHttpSecurity.Mode = securityMode;
        }

        public NetHttpBinding(BasicHttpSecurityMode securityMode, bool reliableSessionEnabled)
            : this(securityMode)
        {
            this.ReliableSession.Enabled = reliableSessionEnabled;
        }

        public NetHttpBinding(string configurationName)
            : base()
        {
            this.Initialize();
        }

        private NetHttpBinding(BasicHttpSecurity security)
            : base()
        {
            this.Initialize();
            _basicHttpSecurity = security;
        }

        [DefaultValue(NetHttpMessageEncoding.Binary)]
        public NetHttpMessageEncoding MessageEncoding
        {
            get { return _messageEncoding; }
            set { _messageEncoding = value; }
        }

        public BasicHttpSecurity Security
        {
            get
            {
                return _basicHttpSecurity;
            }

            set
            {
                if (value == null)
                {
                    throw FxTrace.Exception.ArgumentNull("value");
                }

                _basicHttpSecurity = value;
            }
        }

        public OptionalReliableSession ReliableSession
        {
            get
            {
                return _reliableSession;
            }

            set
            {
                if (value == null)
                {
                    throw FxTrace.Exception.ArgumentNull("value");
                }

                _reliableSession.CopySettings(value);
            }
        }

        public WebSocketTransportSettings WebSocketSettings
        {
            get
            {
                return this.InternalWebSocketSettings;
            }
        }

        internal override BasicHttpSecurity BasicHttpSecurity
        {
            get
            {
                return _basicHttpSecurity;
            }
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingParameterCollection parameters)
        {
            if ((this.BasicHttpSecurity.Mode == BasicHttpSecurityMode.Transport ||
                this.BasicHttpSecurity.Mode == BasicHttpSecurityMode.TransportCredentialOnly) &&
                this.BasicHttpSecurity.Transport.ClientCredentialType == HttpClientCredentialType.InheritedFromHost)
            {
                throw FxTrace.Exception.AsError(new InvalidOperationException(string.Format(SRServiceModel.HttpClientCredentialTypeInvalid, this.BasicHttpSecurity.Transport.ClientCredentialType)));
            }

            return base.BuildChannelFactory<TChannel>(parameters);
        }

        public override BindingElementCollection CreateBindingElements()
        {
            this.CheckSettings();

            // return collection of BindingElements
            BindingElementCollection bindingElements = new BindingElementCollection();

            // order of BindingElements is important

            // add security (*optional)
            SecurityBindingElement messageSecurity = this.BasicHttpSecurity.CreateMessageSecurity();
            if (messageSecurity != null)
            {
                bindingElements.Add(messageSecurity);
            }

            // add encoding
            switch (this.MessageEncoding)
            {
                case NetHttpMessageEncoding.Text:
                    bindingElements.Add(this.TextMessageEncodingBindingElement);
                    break;
                case NetHttpMessageEncoding.Mtom:
                    throw ExceptionHelper.PlatformNotSupported(string.Format(SRServiceModel.UnsupportedBindingProperty, "MessageEncoding", MessageEncoding));
                default:
                    bindingElements.Add(_binaryMessageEncodingBindingElement);
                    break;
            }

            // add transport (http or https)
            bindingElements.Add(this.GetTransport());

            return bindingElements.Clone();
        }

        internal override void SetReaderQuotas(XmlDictionaryReaderQuotas readerQuotas)
        {
            readerQuotas.CopyTo(_binaryMessageEncodingBindingElement.ReaderQuotas);
        }

        internal override EnvelopeVersion GetEnvelopeVersion()
        {
            return EnvelopeVersion.Soap12;
        }

        internal static bool TryCreate(BindingElementCollection elements, out Binding binding)
        {
            binding = null;
            if (elements.Count > 4)
            {
                return false;
            }

            ReliableSessionBindingElement session = null;
            SecurityBindingElement securityElement = null;
            MessageEncodingBindingElement encoding = null;
            HttpTransportBindingElement transport = null;

            foreach (BindingElement element in elements)
            {
                if (element is ReliableSessionBindingElement)
                {
                    session = element as ReliableSessionBindingElement;
                }

                if (element is SecurityBindingElement)
                {
                    securityElement = element as SecurityBindingElement;
                }
                else if (element is TransportBindingElement)
                {
                    transport = element as HttpTransportBindingElement;
                }
                else if (element is MessageEncodingBindingElement)
                {
                    encoding = element as MessageEncodingBindingElement;
                }
                else
                {
                    return false;
                }
            }

            if (transport == null || transport.WebSocketSettings.TransportUsage != WebSocketTransportUsage.Always)
            {
                return false;
            }

            HttpsTransportBindingElement httpsTransport = transport as HttpsTransportBindingElement;
            if ((securityElement != null) && (httpsTransport != null) && (httpsTransport.RequireClientCertificate != TransportDefaults.RequireClientCertificate))
            {
                return false;
            }

            // process transport binding element
            UnifiedSecurityMode mode;
            HttpTransportSecurity transportSecurity = new HttpTransportSecurity();
            if (!GetSecurityModeFromTransport(transport, transportSecurity, out mode))
            {
                return false;
            }

            if (encoding == null)
            {
                return false;
            }

            if (!(encoding is TextMessageEncodingBindingElement || encoding is MtomMessageEncodingBindingElement || encoding is BinaryMessageEncodingBindingElement))
            {
                return false;
            }

            if (encoding.MessageVersion != MessageVersion.Soap12WSAddressing10)
            {
                return false;
            }

            BasicHttpSecurity security;
            if (!TryCreateSecurity(securityElement, mode, transportSecurity, out security))
            {
                return false;
            }

            NetHttpBinding netHttpBinding = new NetHttpBinding(security);
            netHttpBinding.InitializeFrom(transport, encoding, session);

            // make sure all our defaults match
            if (!netHttpBinding.IsBindingElementsMatch(transport, encoding, session))
            {
                return false;
            }

            binding = netHttpBinding;
            return true;
        }

        internal override void CheckSettings()
        {
            base.CheckSettings();

            // Mtom is not supported.
            if (this.MessageEncoding == NetHttpMessageEncoding.Mtom)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(string.Format(SRServiceModel.UnsupportedBindingProperty, "MessageEncoding", this.MessageEncoding)));
            }
        }

        private void Initialize()
        {
            _messageEncoding = NetHttpBindingDefaults.MessageEncoding;
            _binaryMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement() { MessageVersion = MessageVersion.Soap12WSAddressing10 };
            this.TextMessageEncodingBindingElement.MessageVersion = MessageVersion.Soap12WSAddressing10;
            _session = new ReliableSessionBindingElement();
            _reliableSession = new OptionalReliableSession(_session);
            this.WebSocketSettings.TransportUsage = NetHttpBindingDefaults.TransportUsage;
            this.WebSocketSettings.SubProtocol = WebSocketTransportSettings.SoapSubProtocol;
            _basicHttpSecurity = new BasicHttpSecurity();
        }

        private void InitializeFrom(HttpTransportBindingElement transport, MessageEncodingBindingElement encoding, ReliableSessionBindingElement session)
        {
            this.InitializeFrom(transport, encoding);
            if (encoding is BinaryMessageEncodingBindingElement)
            {
                _messageEncoding = NetHttpMessageEncoding.Binary;
                BinaryMessageEncodingBindingElement binary = (BinaryMessageEncodingBindingElement)encoding;
                this.ReaderQuotas = binary.ReaderQuotas;
            }

            if (encoding is TextMessageEncodingBindingElement)
            {
                _messageEncoding = NetHttpMessageEncoding.Text;
            }
            else if (encoding is MtomMessageEncodingBindingElement)
            {
                _messageEncoding = NetHttpMessageEncoding.Mtom;
            }

            if (session != null)
            {
                // only set properties that have standard binding manifestations
                _session.InactivityTimeout = session.InactivityTimeout;
                _session.Ordered = session.Ordered;
            }
        }

        private bool IsBindingElementsMatch(HttpTransportBindingElement transport, MessageEncodingBindingElement encoding, ReliableSessionBindingElement session)
        {
            if (_reliableSession.Enabled)
            {
                if (!_session.IsMatch(session))
                {
                    return false;
                }
            }
            else if (session != null)
            {
                return false;
            }

            switch (this.MessageEncoding)
            {
                case NetHttpMessageEncoding.Text:
                    if (!this.TextMessageEncodingBindingElement.IsMatch(encoding))
                    {
                        return false;
                    }

                    break;
                case NetHttpMessageEncoding.Mtom:
                    if (!this.MtomMessageEncodingBindingElement.IsMatch(encoding))
                    {
                        return false;
                    }

                    break;
                default:    // NetHttpMessageEncoding.Binary
                    if (!_binaryMessageEncodingBindingElement.IsMatch(encoding))
                    {
                        return false;
                    }

                    break;
            }

            if (!this.GetTransport().IsMatch(transport))
            {
                return false;
            }

            return true;
        }
    }
}

