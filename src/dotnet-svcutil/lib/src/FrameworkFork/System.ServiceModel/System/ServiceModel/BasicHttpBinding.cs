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
            if (securityMode == BasicHttpSecurityMode.Message)
            {
                throw ExceptionHelper.PlatformNotSupported(string.Format(SRServiceModel.UnsupportedSecuritySetting, "securityMode", securityMode));
            }

            Initialize();
            _basicHttpSecurity.Mode = securityMode;
        }

        private BasicHttpBinding(BasicHttpSecurity security)
            : base()
        {
            this.Initialize();
            _basicHttpSecurity = security;
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
            else if (this.MessageEncoding == WSMessageEncoding.Mtom)
            {
                if (!this.MtomMessageEncodingBindingElement.IsMatch(encoding))
                    return false;
            }
            if (!this.GetTransport().IsMatch(transport))
                return false;

            return true;
        }

        internal override void InitializeFrom(HttpTransportBindingElement transport, MessageEncodingBindingElement encoding)
        {
            base.InitializeFrom(transport, encoding);
            // BasicHttpBinding only supports Text and Mtom encoding
            if (encoding is TextMessageEncodingBindingElement)
            {
                this.MessageEncoding = WSMessageEncoding.Text;
            }
            else if (encoding is MtomMessageEncodingBindingElement)
            {
                _messageEncoding = WSMessageEncoding.Mtom;
            }
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingParameterCollection parameters)
        {
            if ((BasicHttpSecurity.Mode == BasicHttpSecurityMode.Transport ||
                BasicHttpSecurity.Mode == BasicHttpSecurityMode.TransportCredentialOnly) &&
                BasicHttpSecurity.Transport.ClientCredentialType == HttpClientCredentialType.InheritedFromHost)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.HttpClientCredentialTypeInvalid, BasicHttpSecurity.Transport.ClientCredentialType)));
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
            WSMessageEncodingHelper.SyncUpEncodingBindingElementProperties(this.TextMessageEncodingBindingElement, this.MtomMessageEncodingBindingElement);
            if (this.MessageEncoding == WSMessageEncoding.Text)
                bindingElements.Add(this.TextMessageEncodingBindingElement);
            else if (this.MessageEncoding == WSMessageEncoding.Mtom)
                bindingElements.Add(this.MtomMessageEncodingBindingElement);

            // add transport (http or https)
            bindingElements.Add(GetTransport());

            return bindingElements.Clone();
        }

        private void Initialize()
        {
            _basicHttpSecurity = new BasicHttpSecurity();
        }

        internal static bool TryCreate(BindingElementCollection elements, out Binding binding)
        {
            binding = null;
            if (elements.Count > 3)
                return false;

            SecurityBindingElement securityElement = null;
            MessageEncodingBindingElement encoding = null;
            HttpTransportBindingElement transport = null;

            foreach (BindingElement element in elements)
            {
                if (element is SecurityBindingElement)
                    securityElement = element as SecurityBindingElement;
                else if (element is TransportBindingElement)
                    transport = element as HttpTransportBindingElement;
                else if (element is MessageEncodingBindingElement)
                    encoding = element as MessageEncodingBindingElement;
                else
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
                return false;
            if (encoding == null)
                return false;
            // BasicHttpBinding only supports Soap11
            if (!encoding.CheckEncodingVersion(EnvelopeVersion.Soap11))
                return false;

            BasicHttpSecurity security;
            if (!HttpBindingBase.TryCreateSecurity(securityElement, mode, transportSecurity, out security))
                return false;

            BasicHttpBinding basicHttpBinding = new BasicHttpBinding(security);
            basicHttpBinding.InitializeFrom(transport, encoding);

            // make sure all our defaults match
            if (!basicHttpBinding.IsBindingElementsMatch(transport, encoding))
                return false;

            binding = basicHttpBinding;
            return true;
        }
    }
}
