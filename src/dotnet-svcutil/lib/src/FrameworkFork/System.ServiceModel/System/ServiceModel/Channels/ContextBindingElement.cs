// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Channels
{
    using System;
    using System.ComponentModel;
    using System.Net.Security;
    using System.Runtime.CompilerServices;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;

    public class ContextBindingElement : BindingElement, IPolicyExportExtension, IContextSessionProvider, /*TODO: IWmiInstanceProvider,*/ IContextBindingElement
    {
        internal const ContextExchangeMechanism DefaultContextExchangeMechanism = ContextExchangeMechanism.ContextSoapHeader;
        internal const bool DefaultContextManagementEnabled = true;
        internal const ProtectionLevel DefaultProtectionLevel = ProtectionLevel.Sign;
        ContextExchangeMechanism contextExchangeMechanism;

        bool contextManagementEnabled;
        ProtectionLevel protectionLevel;

        public ContextBindingElement()
            : this(DefaultProtectionLevel, DefaultContextExchangeMechanism, null, DefaultContextManagementEnabled)
        {
            // empty
        }

        public ContextBindingElement(ProtectionLevel protectionLevel)
            : this(protectionLevel, DefaultContextExchangeMechanism, null, DefaultContextManagementEnabled)
        {
            // empty
        }

        public ContextBindingElement(ProtectionLevel protectionLevel, ContextExchangeMechanism contextExchangeMechanism)
            : this(protectionLevel, contextExchangeMechanism, null, DefaultContextManagementEnabled)
        {
            // empty
        }


        public ContextBindingElement(ProtectionLevel protectionLevel, ContextExchangeMechanism contextExchangeMechanism, Uri clientCallbackAddress)
            : this(protectionLevel, contextExchangeMechanism, clientCallbackAddress, DefaultContextManagementEnabled)
        {
            // empty
        }

        public ContextBindingElement(ProtectionLevel protectionLevel, ContextExchangeMechanism contextExchangeMechanism, Uri clientCallbackAddress, bool contextManagementEnabled)
        {
            this.ProtectionLevel = protectionLevel;
            this.ContextExchangeMechanism = contextExchangeMechanism;
            this.ClientCallbackAddress = clientCallbackAddress;
            this.ContextManagementEnabled = contextManagementEnabled;
        }

        ContextBindingElement(ContextBindingElement other)
            : base(other)
        {
            this.ProtectionLevel = other.ProtectionLevel;
            this.ContextExchangeMechanism = other.ContextExchangeMechanism;
            this.ClientCallbackAddress = other.ClientCallbackAddress;
            this.ContextManagementEnabled = other.ContextManagementEnabled;
        }

        [DefaultValue(null)]
        public Uri ClientCallbackAddress
        {
            get;
            set;
        }

        [DefaultValue(DefaultContextExchangeMechanism)]
        public ContextExchangeMechanism ContextExchangeMechanism
        {
            get
            {
                return this.contextExchangeMechanism;
            }
            set
            {
                if (!ContextExchangeMechanismHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.contextExchangeMechanism = value;
            }
        }

        [DefaultValue(DefaultContextManagementEnabled)]
        public bool ContextManagementEnabled
        {
            get
            {
                return this.contextManagementEnabled;
            }
            set
            {
                this.contextManagementEnabled = value;
            }
        }

        [DefaultValue(DefaultProtectionLevel)]
        public ProtectionLevel ProtectionLevel
        {
            get
            {
                return this.protectionLevel;
            }
            set
            {
                if (!ProtectionLevelHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.protectionLevel = value;
            }
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            throw new NotImplementedException();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            return (typeof(TChannel) == typeof(IOutputChannel)
                || typeof(TChannel) == typeof(IOutputSessionChannel)
                || typeof(TChannel) == typeof(IRequestChannel)
                || typeof(TChannel) == typeof(IRequestSessionChannel)
                || (typeof(TChannel) == typeof(IDuplexSessionChannel) && this.ContextExchangeMechanism != ContextExchangeMechanism.HttpCookie))
                && context.CanBuildInnerChannelFactory<TChannel>();
        }

        public override BindingElement Clone()
        {
            return new ContextBindingElement(this);
        }

        public virtual void ExportPolicy(MetadataExporter exporter, PolicyConversionContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            throw new NotImplementedException();
        }

        public override T GetProperty<T>(BindingContext context)
        {
            throw new NotImplementedException();
        }

        internal override bool IsMatch(BindingElement b)
        {
            if (b == null)
            {
                return false;
            }

            ContextBindingElement other = b as ContextBindingElement;
            if (other == null)
            {
                return false;
            }

            if (this.ClientCallbackAddress != other.ClientCallbackAddress)
            {
                return false;
            }

            if (this.ContextExchangeMechanism != other.ContextExchangeMechanism)
            {
                return false;
            }

            if (this.ContextManagementEnabled != other.ContextManagementEnabled)
            {
                return false;
            }

            if (this.ProtectionLevel != other.protectionLevel)
            {
                return false;
            }

            return true;
        }
    }
}
