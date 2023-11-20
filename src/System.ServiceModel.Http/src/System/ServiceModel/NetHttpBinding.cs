// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ServiceModel.Channels;
using System.Xml;

namespace System.ServiceModel
{
    public class NetHttpBinding : HttpBindingBase
    {
        private BinaryMessageEncodingBindingElement _binaryMessageEncodingBindingElement;
        private ReliableSessionBindingElement _session;
        private OptionalReliableSession _reliableSession;
        private BasicHttpSecurity _basicHttpSecurity;

        public NetHttpBinding()
            : this(BasicHttpSecurityMode.None)
        {
        }

        public NetHttpBinding(BasicHttpSecurityMode securityMode)
            : base()
        {
            Initialize();
            _basicHttpSecurity.Mode = securityMode;
        }

        public NetHttpBinding(BasicHttpSecurityMode securityMode, bool reliableSessionEnabled) : this(securityMode)
        {
            ReliableSession.Enabled = reliableSessionEnabled;
        }

        private NetHttpBinding(BasicHttpSecurity security)
            : base()
        {
            Initialize();
            _basicHttpSecurity = security;
        }

        [DefaultValue(NetHttpMessageEncoding.Binary)]
        public NetHttpMessageEncoding MessageEncoding { get; set; }

        public BasicHttpSecurity Security
        {
            get
            {
                return _basicHttpSecurity;
            }

            set
            {
                _basicHttpSecurity = value ?? throw FxTrace.Exception.ArgumentNull(nameof(value));
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
                    throw FxTrace.Exception.ArgumentNull(nameof(value));
                }

                _reliableSession.CopySettings(value);
            }
        }

        public WebSocketTransportSettings WebSocketSettings
        {
            get
            {
                return InternalWebSocketSettings;
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
            if ((BasicHttpSecurity.Mode == BasicHttpSecurityMode.Transport ||
                BasicHttpSecurity.Mode == BasicHttpSecurityMode.TransportCredentialOnly) &&
                BasicHttpSecurity.Transport.ClientCredentialType == HttpClientCredentialType.InheritedFromHost)
            {
                throw FxTrace.Exception.AsError(new InvalidOperationException(SR.Format(SR.HttpClientCredentialTypeInvalid, BasicHttpSecurity.Transport.ClientCredentialType)));
            }

            return base.BuildChannelFactory<TChannel>(parameters);
        }

        public override BindingElementCollection CreateBindingElements()
        {
            CheckSettings();

            // return collection of BindingElements
            BindingElementCollection bindingElements = new BindingElementCollection();

            // order of BindingElements is important
            // add session
            if (_reliableSession.Enabled)
            {
                bindingElements.Add(_session);
            }

            // add security (*optional)
            SecurityBindingElement messageSecurity = BasicHttpSecurity.CreateMessageSecurity();
            if (messageSecurity != null)
            {
                bindingElements.Add(messageSecurity);
            }

            // add encoding
            switch (MessageEncoding)
            {
                case NetHttpMessageEncoding.Text:
                    bindingElements.Add(TextMessageEncodingBindingElement);
                    break;
                case NetHttpMessageEncoding.Mtom:
                    bindingElements.Add(MtomMessageEncodingBindingElement);
                    break;
                default:
                    bindingElements.Add(_binaryMessageEncodingBindingElement);
                    break;
            }

            // add transport (http or https)
            bindingElements.Add(GetTransport());

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

        private void Initialize()
        {
            MessageEncoding = NetHttpBindingDefaults.MessageEncoding;
            _binaryMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement() { MessageVersion = MessageVersion.Soap12WSAddressing10 };
            TextMessageEncodingBindingElement.MessageVersion = MessageVersion.Soap12WSAddressing10;
            MtomMessageEncodingBindingElement.MessageVersion = MessageVersion.Soap12WSAddressing10;
            _session = new ReliableSessionBindingElement();
            _reliableSession = new OptionalReliableSession(_session);
            WebSocketSettings.TransportUsage = NetHttpBindingDefaults.TransportUsage;
            WebSocketSettings.SubProtocol = WebSocketTransportSettings.SoapSubProtocol;
            _basicHttpSecurity = new BasicHttpSecurity();
        }
    }
}

