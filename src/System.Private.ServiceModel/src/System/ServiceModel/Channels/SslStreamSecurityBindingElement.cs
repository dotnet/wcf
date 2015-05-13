// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Net.Security;
using System.ServiceModel.Security;

namespace System.ServiceModel.Channels
{
    public class SslStreamSecurityBindingElement : StreamUpgradeBindingElement
    {
        private IdentityVerifier _identityVerifier;
        private bool _requireClientCertificate;

        public SslStreamSecurityBindingElement()
        {
            _requireClientCertificate = TransportDefaults.RequireClientCertificate;
        }

        protected SslStreamSecurityBindingElement(SslStreamSecurityBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _identityVerifier = elementToBeCloned._identityVerifier;
            _requireClientCertificate = elementToBeCloned._requireClientCertificate;
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
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }

                _identityVerifier = value;
            }
        }

        [DefaultValue(TransportDefaults.RequireClientCertificate)]
        internal bool RequireClientCertificate
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

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T)(object)new SecurityCapabilities(this.RequireClientCertificate, true, this.RequireClientCertificate,
                    ProtectionLevel.EncryptAndSign, ProtectionLevel.EncryptAndSign);
            }
            else if (typeof(T) == typeof(IdentityVerifier))
            {
                return (T)(object)this.IdentityVerifier;
            }

            else
            {
                return context.GetInnerProperty<T>();
            }
        }

        public override StreamUpgradeProvider BuildClientStreamUpgradeProvider(BindingContext context)
        {
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityBindingElementn.BuildClientStreamUpgradeProvider is not supported.");
        }

        public override StreamUpgradeProvider BuildServerStreamUpgradeProvider(BindingContext context)
        {
            throw ExceptionHelper.PlatformNotSupported("SslStreamSecurityBindingElementn.BuildServerStreamUpgradeProvider is not supported.");
        }
    }
}
