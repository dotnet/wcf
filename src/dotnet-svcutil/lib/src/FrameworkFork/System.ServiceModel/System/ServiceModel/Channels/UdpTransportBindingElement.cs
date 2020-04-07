// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime;
    using System.ServiceModel.Description;
    using Microsoft.Xml;

    // was UdpTransportBindingElement
    public class UdpTransportBindingElement
        : TransportBindingElement,
        IPolicyExportExtension,
        ITransportPolicyImport,
        IWsdlExportExtension
    {
        int duplicateMessageHistoryLength;
        long maxPendingMessagesTotalSize;
        UdpRetransmissionSettings retransmissionSettings;
        int socketReceiveBufferSize;
        int timeToLive;

        [SuppressMessage(FxCop.Category.Usage, FxCop.Rule.DoNotCallOverridableMethodsInConstructors, Justification = "this call is intended")]
        public UdpTransportBindingElement()
            : base()
        {
            this.duplicateMessageHistoryLength = UdpConstants.Defaults.DuplicateMessageHistoryLength;
            this.maxPendingMessagesTotalSize = UdpConstants.Defaults.DefaultMaxPendingMessagesTotalSize;

            this.retransmissionSettings = new UdpRetransmissionSettings();
            this.socketReceiveBufferSize = UdpConstants.Defaults.SocketReceiveBufferSize;
            this.timeToLive = UdpConstants.Defaults.TimeToLive;
        }

        internal UdpTransportBindingElement(UdpTransportBindingElement other)
            : base(other)
        {
            this.duplicateMessageHistoryLength = other.duplicateMessageHistoryLength;
            this.maxPendingMessagesTotalSize = other.maxPendingMessagesTotalSize;
            this.retransmissionSettings = other.retransmissionSettings.Clone();
            this.socketReceiveBufferSize = other.socketReceiveBufferSize;
            this.timeToLive = other.timeToLive;
            this.MulticastInterfaceId = other.MulticastInterfaceId;
        }

        [DefaultValue(UdpConstants.Defaults.DuplicateMessageHistoryLength)]
        public int DuplicateMessageHistoryLength
        {
            get { return this.duplicateMessageHistoryLength; }
            set
            {
                const int min = 0;
                if (value < min)
                {
                    throw FxTrace.Exception.ArgumentOutOfRange("value", value,
                        SRServiceModel.Format(SRServiceModel.ArgumentOutOfMinRange, min));
                }
                this.duplicateMessageHistoryLength = value;
            }
        }

        [DefaultValue(UdpConstants.Defaults.DefaultMaxPendingMessagesTotalSize)]
        public long MaxPendingMessagesTotalSize
        {
            get
            {
                return this.maxPendingMessagesTotalSize;
            }

            set
            {
                const long min = UdpConstants.MinPendingMessagesTotalSize;
                if (value < min)
                {
                    throw FxTrace.Exception.ArgumentOutOfRange("value", value,
                        SRServiceModel.Format(SRServiceModel.ArgumentOutOfMinRange, min));
                }

                this.maxPendingMessagesTotalSize = value;
            }
        }

        [DefaultValue(UdpConstants.Defaults.MulticastInterfaceId)]
        public string MulticastInterfaceId { get; set; }

        public UdpRetransmissionSettings RetransmissionSettings
        {
            get
            {
                return this.retransmissionSettings;
            }
            set
            {
                if (value == null)
                {
                    throw FxTrace.Exception.ArgumentNull("value");
                }

                this.retransmissionSettings = value;
            }
        }

        public override string Scheme
        {
            get { return UdpConstants.Scheme; }
        }

        [DefaultValue(UdpConstants.Defaults.SocketReceiveBufferSize)]
        public int SocketReceiveBufferSize
        {
            get { return this.socketReceiveBufferSize; }
            set
            {

                if (value < UdpConstants.MinReceiveBufferSize)
                {
                    throw FxTrace.Exception.ArgumentOutOfRange("value", value,
                        SRServiceModel.Format(SRServiceModel.ArgumentOutOfMinRange, UdpConstants.MinReceiveBufferSize));
                }

                this.socketReceiveBufferSize = value;
            }
        }

        [DefaultValue(UdpConstants.Defaults.TimeToLive)]
        public int TimeToLive
        {
            get { return this.timeToLive; }
            set
            {

                if (value < UdpConstants.MinTimeToLive || value > UdpConstants.MaxTimeToLive)
                {
                    throw FxTrace.Exception.ArgumentOutOfRange("value", value, "TODO: SR");
                        // SR.ArgumentOutOfMinMaxRange(UdpConstants.MinTimeToLive, UdpConstants.MaxTimeToLive));
                }
                this.timeToLive = value;
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
                throw FxTrace.Exception.ArgumentNull("context");
            }

            return (typeof(TChannel) == typeof(IOutputChannel) || typeof(TChannel) == typeof(IDuplexChannel));
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw FxTrace.Exception.ArgumentNull("context");
            }

            return base.GetProperty<T>(context);
        }

        public override BindingElement Clone()
        {
            return new UdpTransportBindingElement(this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeRetransmissionSettings()
        {
            return !this.RetransmissionSettings.IsMatch(new UdpRetransmissionSettings()); // only serialize non-default settings
        }

        [SuppressMessage(FxCop.Category.Design, FxCop.Rule.InterfaceMethodsShouldBeCallableByChildTypes, Justification = "no need to call this from derrived classes")]
        void IWsdlExportExtension.ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {
        }

        [SuppressMessage(FxCop.Category.Design, FxCop.Rule.InterfaceMethodsShouldBeCallableByChildTypes, Justification = "no need to call this from derrived classes")]
        void IWsdlExportExtension.ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
            throw new NotImplementedException();
        }

        [SuppressMessage(FxCop.Category.Design, FxCop.Rule.InterfaceMethodsShouldBeCallableByChildTypes, Justification = "no need to call this from derrived classes")]
        void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext context)
        {
            throw new NotImplementedException();
        }

        [SuppressMessage(FxCop.Category.Design, FxCop.Rule.InterfaceMethodsShouldBeCallableByChildTypes, Justification = "no need to call this from derrived classes")]
        void ITransportPolicyImport.ImportPolicy(MetadataImporter importer, PolicyConversionContext policyContext)
        {
            ICollection<XmlElement> bindingAssertions = policyContext.GetBindingAssertions();

            XmlElement retransmitAssertion = null;
            foreach (XmlElement assertion in bindingAssertions)
            {
                if (assertion.LocalName.Equals(UdpConstants.RetransmissionEnabled, StringComparison.Ordinal))
                {
                    this.DuplicateMessageHistoryLength = UdpConstants.Defaults.DuplicateMessageHistoryLengthWithRetransmission;
                    retransmitAssertion = assertion;
                }
            }

            if (retransmitAssertion != null)
            {
                bindingAssertions.Remove(retransmitAssertion);
            }
        }

        internal override bool IsMatch(BindingElement b)
        {
            if (!base.IsMatch(b))
            {
                return false;
            }

            UdpTransportBindingElement udpTransport = b as UdpTransportBindingElement;
            if (udpTransport == null)
            {
                return false;
            }

            if (this.DuplicateMessageHistoryLength != udpTransport.DuplicateMessageHistoryLength)
            {
                return false;
            }

            if (this.MaxPendingMessagesTotalSize != udpTransport.MaxPendingMessagesTotalSize)
            {
                return false;
            }

            if (!String.Equals(this.MulticastInterfaceId, udpTransport.MulticastInterfaceId, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!this.RetransmissionSettings.IsMatch(udpTransport.RetransmissionSettings))
            {
                return false;
            }

            if (this.TimeToLive != udpTransport.TimeToLive)
            {
                return false;
            }

            return true;
        }
    }
}
