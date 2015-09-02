// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Net.Security;
using System.ServiceModel.Security;

namespace System.ServiceModel.Channels
{
    public class WindowsStreamSecurityBindingElement : StreamUpgradeBindingElement
    {
        private ProtectionLevel _protectionLevel;

        public WindowsStreamSecurityBindingElement()
            : base()
        {
            _protectionLevel = ConnectionOrientedTransportDefaults.ProtectionLevel;
        }

        protected WindowsStreamSecurityBindingElement(WindowsStreamSecurityBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _protectionLevel = elementToBeCloned._protectionLevel;
        }

        [DefaultValue(ConnectionOrientedTransportDefaults.ProtectionLevel)]
        public ProtectionLevel ProtectionLevel
        {
            get
            {
                return _protectionLevel;
            }
            set
            {
                ProtectionLevelHelper.Validate(value);
                _protectionLevel = value;
            }
        }

        public override BindingElement Clone()
        {
            return new WindowsStreamSecurityBindingElement(this);
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

        public override StreamUpgradeProvider BuildClientStreamUpgradeProvider(BindingContext context)
        {
            throw ExceptionHelper.PlatformNotSupported("WindowsStreamSecurity Client upgrade");
            // return new WindowsStreamSecurityUpgradeProvider(this, context, true);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T)(object)new SecurityCapabilities(true, true, true, _protectionLevel, _protectionLevel);
            }
            else if (typeof(T) == typeof(IdentityVerifier))
            {
                return (T)(object)IdentityVerifier.CreateDefault();
            }
            else
            {
                return context.GetInnerProperty<T>();
            }
        }

        internal override bool IsMatch(BindingElement b)
        {
            if (b == null)
            {
                return false;
            }
            WindowsStreamSecurityBindingElement security = b as WindowsStreamSecurityBindingElement;
            if (security == null)
            {
                return false;
            }
            if (_protectionLevel != security._protectionLevel)
            {
                return false;
            }

            return true;
        }
    }
}
