// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;
    using Microsoft.Xml;


    public sealed class CompositeDuplexBindingElement : BindingElement //, IPolicyExportExtension
    {
        private Uri _clientBaseAddress;

        public CompositeDuplexBindingElement()
        {
        }

        private CompositeDuplexBindingElement(CompositeDuplexBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _clientBaseAddress = elementToBeCloned.ClientBaseAddress;
        }

        [DefaultValue(null)]
        public Uri ClientBaseAddress
        {
            get
            {
                return _clientBaseAddress;
            }

            set
            {
                _clientBaseAddress = value;
            }
        }

        public override BindingElement Clone()
        {
            return new CompositeDuplexBindingElement(this);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            if (typeof(TChannel) != typeof(IOutputChannel))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("TChannel",
                    string.Format(SRServiceModel.ChannelTypeNotSupported, typeof(TChannel)));
            }

            return context.BuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            return (typeof(TChannel) == typeof(IOutputChannel))
                && context.CanBuildInnerChannelFactory<IOutputChannel>();
        }

        private ChannelProtectionRequirements GetProtectionRequirements()
        {
            ChannelProtectionRequirements result = new ChannelProtectionRequirements();
            XmlQualifiedName refPropHeaderName = new XmlQualifiedName(XD.UtilityDictionary.UniqueEndpointHeaderName.Value,
                    XD.UtilityDictionary.UniqueEndpointHeaderNamespace.Value);
            MessagePartSpecification headerParts = new MessagePartSpecification(refPropHeaderName);
            headerParts.MakeReadOnly();
            result.IncomingSignatureParts.AddParts(headerParts);
            result.OutgoingSignatureParts.AddParts(headerParts);
            return result;
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                ISecurityCapabilities lowerCapabilities = context.GetInnerProperty<ISecurityCapabilities>();
                if (lowerCapabilities != null)
                {
                    // composite duplex cannot ensure that messages it receives are from the part it sends
                    // messages to. So it cannot offer server auth
                    return (T)(object)(new SecurityCapabilities(lowerCapabilities.SupportsClientAuthentication,
                        false, lowerCapabilities.SupportsClientWindowsIdentity, lowerCapabilities.SupportedRequestProtectionLevel,
                        System.Net.Security.ProtectionLevel.None));
                }
                else
                {
                    return null;
                }
            }
            else if (typeof(T) == typeof(ChannelProtectionRequirements))
            {
                ChannelProtectionRequirements myRequirements = this.GetProtectionRequirements();
                myRequirements.Add(context.GetInnerProperty<ChannelProtectionRequirements>() ?? new ChannelProtectionRequirements());
                return (T)(object)myRequirements;
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

            CompositeDuplexBindingElement duplex = b as CompositeDuplexBindingElement;
            if (duplex == null)
            {
                return false;
            }

            return (_clientBaseAddress == duplex._clientBaseAddress);
        }
    }
}
