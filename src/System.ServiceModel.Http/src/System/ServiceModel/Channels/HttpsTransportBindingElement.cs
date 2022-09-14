// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Net;
using System.Net.Security;
using System.ComponentModel;

namespace System.ServiceModel.Channels
{
    public class HttpsTransportBindingElement
        : HttpTransportBindingElement
    {
        private MessageSecurityVersion _messageSecurityVersion;

        public HttpsTransportBindingElement()
            : base()
        {
            RequireClientCertificate = TransportDefaults.RequireClientCertificate;
        }

        protected HttpsTransportBindingElement(HttpsTransportBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            RequireClientCertificate = elementToBeCloned.RequireClientCertificate;
            _messageSecurityVersion = elementToBeCloned._messageSecurityVersion;
        }

        private HttpsTransportBindingElement(HttpTransportBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
        }

        [DefaultValue(TransportDefaults.RequireClientCertificate)]
        public bool RequireClientCertificate { get; set; }

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
            return RequireClientCertificate || base.GetSupportsClientAuthenticationImpl(effectiveAuthenticationSchemes);
        }

        internal override bool GetSupportsClientWindowsIdentityImpl(AuthenticationSchemes effectiveAuthenticationSchemes)
        {
            return RequireClientCertificate || base.GetSupportsClientWindowsIdentityImpl(effectiveAuthenticationSchemes);
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
                _messageSecurityVersion = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(value)));
            }
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }

            if (MessageHandlerFactory != null)
            {
                throw FxTrace.Exception.AsError(new InvalidOperationException(SR.Format(SR.HttpPipelineNotSupportedOnClientSide, "MessageHandlerFactory")));
            }

            if (!CanBuildChannelFactory<TChannel>(context))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("TChannel", SR.Format(SR.ChannelTypeNotSupported, typeof(TChannel)));
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                AuthenticationSchemes effectiveAuthenticationSchemes = AuthenticationScheme;
                // Desktop: HttpTransportBindingElement.GetEffectiveAuthenticationSchemes(this.AuthenticationScheme, context.BindingParameters);

                return (T)(object)new SecurityCapabilities(GetSupportsClientAuthenticationImpl(effectiveAuthenticationSchemes),
                    true,
                    GetSupportsClientWindowsIdentityImpl(effectiveAuthenticationSchemes),
                    ProtectionLevel.EncryptAndSign,
                    ProtectionLevel.EncryptAndSign);
            }
            else
            {
                return base.GetProperty<T>(context);
            }
        }
    }
}
