//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Channels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;
    using Microsoft.Xml;

    public sealed class OneWayBindingElement : BindingElement,
        IPolicyExportExtension
    {
        bool packetRoutable;
        int maxAcceptedChannels;

        public OneWayBindingElement()
        {
            this.packetRoutable = OneWayDefaults.PacketRoutable;
            this.maxAcceptedChannels = OneWayDefaults.MaxAcceptedChannels;
        }

        OneWayBindingElement(OneWayBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            this.packetRoutable = elementToBeCloned.PacketRoutable;
            this.maxAcceptedChannels = elementToBeCloned.maxAcceptedChannels;
        }

        // server
        [DefaultValue(OneWayDefaults.MaxAcceptedChannels)]
        public int MaxAcceptedChannels
        {
            get
            {
                return this.maxAcceptedChannels;
            }
            set
            {
                if (value <= 0)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.ValueMustBePositive));

                this.maxAcceptedChannels = value;
            }
        }

        [DefaultValue(OneWayDefaults.PacketRoutable)]
        public bool PacketRoutable
        {
            get
            {
                return this.packetRoutable;
            }

            set
            {
                this.packetRoutable = value;
            }
        }

        public override BindingElement Clone()
        {
            return new OneWayBindingElement(this);
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

            if (typeof(TChannel) != typeof(IOutputChannel))
            {
                return false;
            }

            // we can convert IDuplexChannel
            if (context.CanBuildInnerChannelFactory<IDuplexChannel>())
            {
                return true;
            }

            // we can convert IDuplexSessionChannel
            if (context.CanBuildInnerChannelFactory<IDuplexSessionChannel>())
            {
                return true;
            }

            // and also IRequestChannel
            if (context.CanBuildInnerChannelFactory<IRequestChannel>())
            {
                return true;
            }

            return false;
        }

        static MessagePartSpecification oneWaySignedMessageParts;

        static MessagePartSpecification OneWaySignedMessageParts
        {
            get
            {
                if (oneWaySignedMessageParts == null)
                {
                    MessagePartSpecification tempSignedMessageParts = new MessagePartSpecification(
                        new XmlQualifiedName(DotNetOneWayStrings.HeaderName, DotNetOneWayStrings.Namespace)
                        );
                    tempSignedMessageParts.MakeReadOnly();
                    oneWaySignedMessageParts = tempSignedMessageParts;
                }

                return oneWaySignedMessageParts;
            }
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            // make sure our Datagram header is signed
            if (typeof(T) == typeof(ChannelProtectionRequirements))
            {
                ChannelProtectionRequirements myRequirements = new ChannelProtectionRequirements();
                if (PacketRoutable)
                {
                    myRequirements.IncomingSignatureParts.AddParts(OneWaySignedMessageParts);
                    myRequirements.OutgoingSignatureParts.AddParts(OneWaySignedMessageParts);
                }
                ChannelProtectionRequirements innerRequirements = context.GetInnerProperty<ChannelProtectionRequirements>();
                if (innerRequirements != null)
                {
                    myRequirements.Add(innerRequirements);
                }
                return (T)(object)myRequirements;
            }
            else
            {
                return context.GetInnerProperty<T>();
            }
        }

        internal override bool IsMatch(BindingElement b)
        {
            throw new NotImplementedException();
        }

        void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            if (context.BindingElements != null)
            {
                OneWayBindingElement oneWay = context.BindingElements.Find<OneWayBindingElement>();

                if (oneWay != null)
                {
                    // base assertion
                    XmlDocument doc = new XmlDocument();
                    XmlElement assertion = doc.CreateElement(OneWayPolicyConstants.Prefix,
                        OneWayPolicyConstants.OneWay, OneWayPolicyConstants.Namespace);

                    if (oneWay.PacketRoutable)
                    {
                        // add nested packet routable assertion 
                        XmlElement child = doc.CreateElement(OneWayPolicyConstants.Prefix, OneWayPolicyConstants.PacketRoutable, OneWayPolicyConstants.Namespace);
                        assertion.AppendChild(child);
                    }

                    context.GetBindingAssertions().Add(assertion);
                }
            }
        }
    }
}
