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

    public sealed class OneWayBindingElement : BindingElement,
        IPolicyExportExtension
    {
        private bool _packetRoutable;
        private int _maxAcceptedChannels;

        public OneWayBindingElement()
        {
            _packetRoutable = OneWayDefaults.PacketRoutable;
            _maxAcceptedChannels = OneWayDefaults.MaxAcceptedChannels;
        }

        private OneWayBindingElement(OneWayBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _packetRoutable = elementToBeCloned.PacketRoutable;
            _maxAcceptedChannels = elementToBeCloned._maxAcceptedChannels;
        }

        // server
        [DefaultValue(OneWayDefaults.MaxAcceptedChannels)]
        public int MaxAcceptedChannels
        {
            get
            {
                return _maxAcceptedChannels;
            }
            set
            {
                if (value <= 0)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.ValueMustBePositive));

                _maxAcceptedChannels = value;
            }
        }

        [DefaultValue(OneWayDefaults.PacketRoutable)]
        public bool PacketRoutable
        {
            get
            {
                return _packetRoutable;
            }

            set
            {
                _packetRoutable = value;
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

        private static MessagePartSpecification s_oneWaySignedMessageParts;

        private static MessagePartSpecification OneWaySignedMessageParts
        {
            get
            {
                if (s_oneWaySignedMessageParts == null)
                {
                    MessagePartSpecification tempSignedMessageParts = new MessagePartSpecification(
                        new XmlQualifiedName(DotNetOneWayStrings.HeaderName, DotNetOneWayStrings.Namespace)
                        );
                    tempSignedMessageParts.MakeReadOnly();
                    s_oneWaySignedMessageParts = tempSignedMessageParts;
                }

                return s_oneWaySignedMessageParts;
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
