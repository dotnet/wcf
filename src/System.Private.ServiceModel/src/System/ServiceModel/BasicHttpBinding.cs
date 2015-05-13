// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
            this.Initialize();
            _basicHttpSecurity.Mode = securityMode;
        }

        private BasicHttpBinding(BasicHttpSecurity security)
            : base()
        {
            this.Initialize();
            _basicHttpSecurity = security;
        }

        public WSMessageEncoding MessageEncoding
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

        // check that properties of the HttpTransportBindingElement and 
        // MessageEncodingBindingElement not exposed as properties on BasicHttpBinding 
        // match default values of the binding elements
        private bool IsBindingElementsMatch(HttpTransportBindingElement transport, MessageEncodingBindingElement encoding)
        {
            if (this.MessageEncoding == WSMessageEncoding.Text)
            {
                if (!this.TextMessageEncodingBindingElement.IsMatch(encoding))
                    return false;
            }
            if (!this.GetTransport().IsMatch(transport))
                return false;

            return true;
        }

        internal override EnvelopeVersion GetEnvelopeVersion()
        {
            return EnvelopeVersion.Soap11;
        }

        internal override void InitializeFrom(HttpTransportBindingElement transport, MessageEncodingBindingElement encoding)
        {
            base.InitializeFrom(transport, encoding);
            // BasicHttpBinding only supports Text and Mtom encoding
            if (encoding is TextMessageEncodingBindingElement)
            {
                this.MessageEncoding = WSMessageEncoding.Text;
            }
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingParameterCollection parameters)
        {
            if ((this.BasicHttpSecurity.Mode == BasicHttpSecurityMode.Transport ||
                this.BasicHttpSecurity.Mode == BasicHttpSecurityMode.TransportCredentialOnly) &&
                this.BasicHttpSecurity.Transport.ClientCredentialType == HttpClientCredentialType.InheritedFromHost)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.HttpClientCredentialTypeInvalid, this.BasicHttpSecurity.Transport.ClientCredentialType)));
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
            SecurityBindingElement wsSecurity = this.BasicHttpSecurity.CreateMessageSecurity();
            if (wsSecurity != null)
            {
                bindingElements.Add(wsSecurity);
            }
            // add encoding (text or mtom)
            if (this.MessageEncoding == WSMessageEncoding.Text)
                bindingElements.Add(this.TextMessageEncodingBindingElement);
            // add transport (http or https)
            bindingElements.Add(this.GetTransport());

            return bindingElements.Clone();
        }

        private void Initialize()
        {
            _basicHttpSecurity = new BasicHttpSecurity();
        }
    }
}
