// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Net.Security;
using System.ServiceModel.Security;

namespace System.ServiceModel.Channels
{
    public class WindowsStreamSecurityBindingElement : StreamUpgradeBindingElement
    {
        private ProtectionLevel _protectionLevel;

        public WindowsStreamSecurityBindingElement() : base()
        {
            _protectionLevel = ConnectionOrientedTransportDefaults.ProtectionLevel;
        }

        protected WindowsStreamSecurityBindingElement(WindowsStreamSecurityBindingElement elementToBeCloned) : base(elementToBeCloned)
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

        public override BindingElement Clone() => new WindowsStreamSecurityBindingElement(this);

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

        public override StreamUpgradeProvider BuildClientStreamUpgradeProvider(BindingContext context) => new WindowsStreamSecurityUpgradeProvider(this, context);

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
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
    }
}

