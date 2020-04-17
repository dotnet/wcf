// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Net.Security;
using System.ComponentModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using Microsoft.Xml;

namespace System.ServiceModel.Channels
{
    public class HttpsTransportBindingElement
        : HttpTransportBindingElement, ITransportTokenAssertionProvider
    {
        private bool _requireClientCertificate;
        private MessageSecurityVersion _messageSecurityVersion;

        public HttpsTransportBindingElement()
            : base()
        {
            _requireClientCertificate = TransportDefaults.RequireClientCertificate;
        }

        protected HttpsTransportBindingElement(HttpsTransportBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _requireClientCertificate = elementToBeCloned._requireClientCertificate;
            _messageSecurityVersion = elementToBeCloned._messageSecurityVersion;
        }

        private HttpsTransportBindingElement(HttpTransportBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
        }

        [DefaultValue(TransportDefaults.RequireClientCertificate)]
        public bool RequireClientCertificate
        {
            get
            {
                return _requireClientCertificate;
            }
            set
            {
                _requireClientCertificate = value;
            }
        }

        public override string Scheme
        {
            get { return UriEx.UriSchemeHttps; }
        }

        public override BindingElement Clone()
        {
            return new HttpsTransportBindingElement(this);
        }

        internal override bool GetSupportsClientAuthenticationImpl(AuthenticationSchemes effectiveAuthenticationSchemes)
        {
            return _requireClientCertificate || base.GetSupportsClientAuthenticationImpl(effectiveAuthenticationSchemes);
        }

        internal override bool GetSupportsClientWindowsIdentityImpl(AuthenticationSchemes effectiveAuthenticationSchemes)
        {
            return _requireClientCertificate || base.GetSupportsClientWindowsIdentityImpl(effectiveAuthenticationSchemes);
        }

        // In order to generate sp:HttpsToken with the right policy.
        // See CSD 3105 for detail.
        internal MessageSecurityVersion MessageSecurityVersion
        {
            get
            {
                return _messageSecurityVersion;
            }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
                }
                _messageSecurityVersion = value;
            }
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            if (this.MessageHandlerFactory != null)
            {
                throw FxTrace.Exception.AsError(new InvalidOperationException(string.Format(SRServiceModel.HttpPipelineNotSupportedOnClientSide, "MessageHandlerFactory")));
            }

            if (!this.CanBuildChannelFactory<TChannel>(context))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("TChannel", string.Format(SRServiceModel.ChannelTypeNotSupported, typeof(TChannel)));
            }

            return (IChannelFactory<TChannel>)(object)new HttpsChannelFactory<TChannel>(this, context);
        }

        internal static HttpsTransportBindingElement CreateFromHttpBindingElement(HttpTransportBindingElement elementToBeCloned)
        {
            return new HttpsTransportBindingElement(elementToBeCloned);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                AuthenticationSchemes effectiveAuthenticationSchemes = this.AuthenticationScheme;
                // Desktop: HttpTransportBindingElement.GetEffectiveAuthenticationSchemes(this.AuthenticationScheme, context.BindingParameters);

                return (T)(object)new SecurityCapabilities(this.GetSupportsClientAuthenticationImpl(effectiveAuthenticationSchemes),
                    true,
                    this.GetSupportsClientWindowsIdentityImpl(effectiveAuthenticationSchemes),
                    ProtectionLevel.EncryptAndSign,
                    ProtectionLevel.EncryptAndSign);
            }
            else
            {
                return base.GetProperty<T>(context);
            }
        }

        internal override void OnImportPolicy(MetadataImporter importer, PolicyConversionContext policyContext)
        {
            base.OnImportPolicy(importer, policyContext);

            WSSecurityPolicy sp = null;
            if (WSSecurityPolicy.TryGetSecurityPolicyDriver(policyContext.GetBindingAssertions(), out sp))
                sp.TryImportWsspHttpsTokenAssertion(importer, policyContext.GetBindingAssertions(), this);
        }

        #region ITransportTokenAssertionProvider Members

        public XmlElement GetTransportTokenAssertion()
        {
            return null;
        }

        #endregion
    }
}
