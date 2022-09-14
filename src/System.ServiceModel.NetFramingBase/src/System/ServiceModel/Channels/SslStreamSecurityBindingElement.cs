// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.ServiceModel.Security;

namespace System.ServiceModel.Channels
{
    public class SslStreamSecurityBindingElement : StreamUpgradeBindingElement
    {
        private IdentityVerifier _identityVerifier;
        private SslProtocols _sslProtocols;

        public SslStreamSecurityBindingElement()
        {
            RequireClientCertificate = NFTransportDefaults.RequireClientCertificate;
            _sslProtocols = NFTransportDefaults.SslProtocols;
        }

        protected SslStreamSecurityBindingElement(SslStreamSecurityBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _identityVerifier = elementToBeCloned._identityVerifier;
            RequireClientCertificate = elementToBeCloned.RequireClientCertificate;
            _sslProtocols = elementToBeCloned._sslProtocols;
        }

        public IdentityVerifier IdentityVerifier
        {
            get
            {
                if (_identityVerifier == null)
                {
                    _identityVerifier = IdentityVerifier.CreateDefault();
                }

                return _identityVerifier;
            }
            set
            {
                _identityVerifier = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
            }
        }

        [DefaultValue(NFTransportDefaults.RequireClientCertificate)]
        public bool RequireClientCertificate { get; set; }

        [DefaultValue(NFTransportDefaults.SslProtocols)]
        public SslProtocols SslProtocols
        {
            get
            {
                return _sslProtocols;
            }
            set
            {
                SslProtocolsHelper.Validate(value);
                _sslProtocols = value;
            }
        }


        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }

            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        public override BindingElement Clone()
        {
            return new SslStreamSecurityBindingElement(this);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T)(object)new SecurityCapabilities(RequireClientCertificate, true, RequireClientCertificate,
                    ProtectionLevel.EncryptAndSign, ProtectionLevel.EncryptAndSign);
            }
            else if (typeof(T) == typeof(IdentityVerifier))
            {
                return (T)(object)IdentityVerifier;
            }

            else
            {
                return context.GetInnerProperty<T>();
            }
        }

        public override StreamUpgradeProvider BuildClientStreamUpgradeProvider(BindingContext context)
        {
            return SslStreamSecurityUpgradeProvider.CreateClientProvider(this, context);
        }
    }
}
