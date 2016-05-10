// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ComponentModel;
using System.Runtime;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace System.ServiceModel
{
    public class NetHttpBinding : HttpBindingBase
    {
        private BinaryMessageEncodingBindingElement _binaryMessageEncodingBindingElement;
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
                throw FxTrace.Exception.AsError(new InvalidOperationException(SR.Format(SR.HttpClientCredentialTypeInvalid, this.BasicHttpSecurity.Transport.ClientCredentialType)));
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
                    throw ExceptionHelper.PlatformNotSupported(SR.Format(SR.UnsupportedBindingProperty, "MessageEncoding", MessageEncoding));
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

        internal override void CheckSettings()
        {
            base.CheckSettings();

            // Mtom is not supported.
            if (this.MessageEncoding == NetHttpMessageEncoding.Mtom)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.Format(SR.UnsupportedBindingProperty, "MessageEncoding", this.MessageEncoding)));
            }
        }

        private void Initialize()
        {
            _messageEncoding = NetHttpBindingDefaults.MessageEncoding;
            _binaryMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement() { MessageVersion = MessageVersion.Soap12WSAddressing10 };
            this.TextMessageEncodingBindingElement.MessageVersion = MessageVersion.Soap12WSAddressing10;
            this.WebSocketSettings.TransportUsage = NetHttpBindingDefaults.TransportUsage;
            this.WebSocketSettings.SubProtocol = WebSocketTransportSettings.SoapSubProtocol;
            _basicHttpSecurity = new BasicHttpSecurity();
        }
    }
}

