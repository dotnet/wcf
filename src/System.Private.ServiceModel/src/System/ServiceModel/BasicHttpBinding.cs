// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public class BasicHttpBinding : HttpBindingBase
    {
        private BasicHttpSecurity _basicHttpSecurity;

        public BasicHttpBinding() : this(BasicHttpSecurityMode.None) { }

        public BasicHttpBinding(BasicHttpSecurityMode securityMode)
            : base()
        {
            if (securityMode == BasicHttpSecurityMode.Message)
            {
                throw ExceptionHelper.PlatformNotSupported(SR.Format(SR.UnsupportedSecuritySetting, nameof(securityMode), securityMode));
            }

            Initialize();
            _basicHttpSecurity.Mode = securityMode;
        }

        public WSMessageEncoding MessageEncoding { get; set; } = BasicHttpBindingDefaults.MessageEncoding;

        public BasicHttpSecurity Security
        {
            get { return _basicHttpSecurity; }
            set
            {
                _basicHttpSecurity = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
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
            // add encoding (text or mtom)
            WSMessageEncodingHelper.SyncUpEncodingBindingElementProperties(TextMessageEncodingBindingElement, MtomMessageEncodingBindingElement);
            if (MessageEncoding == WSMessageEncoding.Text)
            {
                bindingElements.Add(TextMessageEncodingBindingElement);
            }
            else if (MessageEncoding == WSMessageEncoding.Mtom)
            {
                bindingElements.Add(MtomMessageEncodingBindingElement);
            }

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
