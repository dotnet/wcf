// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public class BasicHttpsBinding : HttpBindingBase
    {
        private WSMessageEncoding _messageEncoding = BasicHttpBindingDefaults.MessageEncoding;
        private BasicHttpsSecurity _basicHttpsSecurity;

        public BasicHttpsBinding() : this(BasicHttpsSecurity.DefaultMode) { }

        public BasicHttpsBinding(BasicHttpsSecurityMode securityMode)
        {
            if (securityMode == BasicHttpsSecurityMode.TransportWithMessageCredential)
            {
                throw ExceptionHelper.PlatformNotSupported(SRServiceModel.Format(SRServiceModel.UnsupportedSecuritySetting, "securityMode", securityMode));
            }

            _basicHttpsSecurity = new BasicHttpsSecurity();
            _basicHttpsSecurity.Mode = securityMode;
        }

        internal WSMessageEncoding MessageEncoding
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

        internal override EnvelopeVersion GetEnvelopeVersion()
        {
            return EnvelopeVersion.Soap11;
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingParameterCollection parameters)
        {
            if ((BasicHttpSecurity.Mode == BasicHttpSecurityMode.Transport ||
                BasicHttpSecurity.Mode == BasicHttpSecurityMode.TransportCredentialOnly) &&
                BasicHttpSecurity.Transport.ClientCredentialType == HttpClientCredentialType.InheritedFromHost)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.Format(SRServiceModel.HttpClientCredentialTypeInvalid, BasicHttpSecurity.Transport.ClientCredentialType)));
            }

            return base.BuildChannelFactory<TChannel>(parameters);
        }

        public override BindingElementCollection CreateBindingElements()
        {
            CheckSettings();

            // return collection of BindingElements
            BindingElementCollection bindingElements = new BindingElementCollection();
            // order of BindingElements is important
            // add security (*optional)
            SecurityBindingElement wsSecurity = BasicHttpSecurity.CreateMessageSecurity();
            if (wsSecurity != null)
            {
                bindingElements.Add(wsSecurity);
            }
            // add encoding
            if (MessageEncoding == WSMessageEncoding.Text)
                bindingElements.Add(TextMessageEncodingBindingElement);
            // add transport (http or https)
            bindingElements.Add(GetTransport());

            return bindingElements.Clone();
        }
    }
}
