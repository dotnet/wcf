// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public class BasicHttpBinding : HttpBindingBase
    {
        private WSMessageEncoding _messageEncoding = BasicHttpBindingDefaults.MessageEncoding;
        private BasicHttpSecurity _basicHttpSecurity;

        public BasicHttpBinding() : this(BasicHttpSecurityMode.None) { }

        public BasicHttpBinding(BasicHttpSecurityMode securityMode)
            : base()
        {
            if (securityMode == BasicHttpSecurityMode.Message ||
                securityMode == BasicHttpSecurityMode.TransportWithMessageCredential)
            {
                throw ExceptionHelper.PlatformNotSupported(SR.Format(SR.UnsupportedSecuritySetting, "securityMode", securityMode));
            }

            Initialize();
            _basicHttpSecurity.Mode = securityMode;
        }

        internal WSMessageEncoding MessageEncoding
        {
            get { return _messageEncoding; }
            set { _messageEncoding = value; }
        }

        public BasicHttpSecurity Security
        {
            get { return _basicHttpSecurity; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }

                _basicHttpSecurity = value;
            }
        }

        internal override BasicHttpSecurity BasicHttpSecurity
        {
            get
            {
                return _basicHttpSecurity;
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.HttpClientCredentialTypeInvalid, BasicHttpSecurity.Transport.ClientCredentialType)));
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

        private void Initialize()
        {
            _basicHttpSecurity = new BasicHttpSecurity();
        }
    }
}
