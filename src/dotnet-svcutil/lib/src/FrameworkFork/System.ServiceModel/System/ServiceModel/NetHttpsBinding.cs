// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using Microsoft.Xml;

namespace System.ServiceModel
{
    public class NetHttpsBinding : HttpBindingBase
    {
        private BinaryMessageEncodingBindingElement _binaryMessageEncodingBindingElement;
        private NetHttpMessageEncoding _messageEncoding;
        private BasicHttpsSecurity _basicHttpsSecurity;

        public NetHttpsBinding() : this(BasicHttpsSecurity.DefaultMode) { }

        public NetHttpsBinding(BasicHttpsSecurityMode securityMode)
        {
            if (securityMode == BasicHttpsSecurityMode.TransportWithMessageCredential)
            {
                throw ExceptionHelper.PlatformNotSupported(string.Format(SRServiceModel.UnsupportedSecuritySetting, "securityMode", securityMode));
            }

            Initialize();
            _basicHttpsSecurity.Mode = securityMode;
        }

        public NetHttpMessageEncoding MessageEncoding
        {
            get { return _messageEncoding; }
            set { _messageEncoding = value; }
        }

        public BasicHttpsSecurity Security
        {
            get
            {
                return _basicHttpsSecurity;
            }

            set
            {
                if (value == null)
                {
                    throw FxTrace.Exception.ArgumentNull("value");
                }

                _basicHttpsSecurity = value;
            }
        }

        internal override BasicHttpSecurity BasicHttpSecurity
        {
            get
            {
                return _basicHttpsSecurity.BasicHttpSecurity;
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
                throw FxTrace.Exception.AsError(new InvalidOperationException(string.Format(SRServiceModel.HttpClientCredentialTypeInvalid, BasicHttpSecurity.Transport.ClientCredentialType)));
            }

            return base.BuildChannelFactory<TChannel>(parameters);
        }


        public override BindingElementCollection CreateBindingElements()
        {
            CheckSettings();

            // return collection of BindingElements
            BindingElementCollection bindingElements = new BindingElementCollection();

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
                    throw ExceptionHelper.PlatformNotSupported(string.Format(SRServiceModel.UnsupportedBindingProperty, "MessageEncoding", MessageEncoding));
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

        internal override void CheckSettings()
        {
            base.CheckSettings();

            // Mtom is not supported.
            if (MessageEncoding == NetHttpMessageEncoding.Mtom)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(string.Format(SRServiceModel.UnsupportedBindingProperty, "MessageEncoding", MessageEncoding)));
            }
        }

        private void Initialize()
        {
            _messageEncoding = NetHttpBindingDefaults.MessageEncoding;
            _binaryMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement() { MessageVersion = MessageVersion.Soap12WSAddressing10 };
            TextMessageEncodingBindingElement.MessageVersion = MessageVersion.Soap12WSAddressing10;
            InternalWebSocketSettings.TransportUsage = NetHttpBindingDefaults.TransportUsage;
            InternalWebSocketSettings.SubProtocol = WebSocketTransportSettings.SoapSubProtocol;
            _basicHttpsSecurity = new BasicHttpsSecurity();
        }
    }
}
