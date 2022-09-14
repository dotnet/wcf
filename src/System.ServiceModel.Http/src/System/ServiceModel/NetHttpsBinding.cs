// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;
using System.Xml;

namespace System.ServiceModel
{
    public class NetHttpsBinding : HttpBindingBase
    {
        private BinaryMessageEncodingBindingElement _binaryMessageEncodingBindingElement;
        private ReliableSessionBindingElement _session;
        private OptionalReliableSession _reliableSession;
        private BasicHttpsSecurity _basicHttpsSecurity;

        public NetHttpsBinding() : this(BasicHttpsSecurity.DefaultMode) { }

        public NetHttpsBinding(BasicHttpsSecurityMode securityMode)
        {
            if (securityMode == BasicHttpsSecurityMode.TransportWithMessageCredential)
            {
                throw ExceptionHelper.PlatformNotSupported(SR.Format(SR.UnsupportedSecuritySetting, "securityMode", securityMode));
            }

            Initialize();
            _basicHttpsSecurity.Mode = securityMode;
        }

        public NetHttpsBinding(BasicHttpsSecurityMode securityMode, bool reliableSessionEnabled) : this(securityMode)
        {
            ReliableSession.Enabled = reliableSessionEnabled;
        }

        public NetHttpMessageEncoding MessageEncoding { get; set; }

        public BasicHttpsSecurity Security
        {
            get
            {
                return _basicHttpsSecurity;
            }

            set
            {
                _basicHttpsSecurity = value ?? throw FxTrace.Exception.ArgumentNull(nameof(value));
            }
        }

        internal override BasicHttpSecurity BasicHttpSecurity
        {
            get
            {
                return _basicHttpsSecurity.BasicHttpSecurity;
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
            InternalWebSocketSettings.TransportUsage = NetHttpBindingDefaults.TransportUsage;
            InternalWebSocketSettings.SubProtocol = WebSocketTransportSettings.SoapSubProtocol;
            _basicHttpsSecurity = new BasicHttpsSecurity();
        }
    }
}
