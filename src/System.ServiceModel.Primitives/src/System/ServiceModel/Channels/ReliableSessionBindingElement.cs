// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Xml;

namespace System.ServiceModel.Channels
{
    public sealed class ReliableSessionBindingElement : BindingElement
    {
        private TimeSpan _acknowledgementInterval = ReliableSessionDefaults.AcknowledgementInterval;
        private TimeSpan _inactivityTimeout = ReliableSessionDefaults.InactivityTimeout;
        private int _maxPendingChannels = ReliableSessionDefaults.MaxPendingChannels;
        private int _maxRetryCount = ReliableSessionDefaults.MaxRetryCount;
        private int _maxTransferWindowSize = ReliableSessionDefaults.MaxTransferWindowSize;
        private ReliableMessagingVersion _reliableMessagingVersion = ReliableMessagingVersion.Default;
        private static MessagePartSpecification s_bodyOnly;

        public ReliableSessionBindingElement()
        {
        }

        internal ReliableSessionBindingElement(ReliableSessionBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            AcknowledgementInterval = elementToBeCloned.AcknowledgementInterval;
            FlowControlEnabled = elementToBeCloned.FlowControlEnabled;
            InactivityTimeout = elementToBeCloned.InactivityTimeout;
            MaxPendingChannels = elementToBeCloned.MaxPendingChannels;
            MaxRetryCount = elementToBeCloned.MaxRetryCount;
            MaxTransferWindowSize = elementToBeCloned.MaxTransferWindowSize;
            Ordered = elementToBeCloned.Ordered;
            ReliableMessagingVersion = elementToBeCloned.ReliableMessagingVersion;
        }

        public ReliableSessionBindingElement(bool ordered)
        {
            Ordered = ordered;
        }

        [DefaultValue(typeof(TimeSpan), ReliableSessionDefaults.AcknowledgementIntervalString)]
        public TimeSpan AcknowledgementInterval
        {
            get
            {
                return _acknowledgementInterval;
            }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRP.TimeSpanMustbeGreaterThanTimeSpanZero));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRP.SFxTimeoutOutOfRangeTooBig));
                }

                _acknowledgementInterval = value;
            }
        }

        [DefaultValue(ReliableSessionDefaults.FlowControlEnabled)]
        public bool FlowControlEnabled { get; set; } = ReliableSessionDefaults.FlowControlEnabled;

        [DefaultValue(typeof(TimeSpan), ReliableSessionDefaults.InactivityTimeoutString)]
        public TimeSpan InactivityTimeout
        {
            get
            {
                return _inactivityTimeout;
            }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRP.TimeSpanMustbeGreaterThanTimeSpanZero));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRP.SFxTimeoutOutOfRangeTooBig));
                }

                _inactivityTimeout = value;
            }
        }

        [DefaultValue(ReliableSessionDefaults.MaxPendingChannels)]
        public int MaxPendingChannels
        {
            get
            {
                return _maxPendingChannels;
            }
            set
            {
                if (value <= 0 || value > 16384)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                                                    SRP.Format(SRP.ValueMustBeInRange, 0, 16384)));
                _maxPendingChannels = value;
            }
        }

        [DefaultValue(ReliableSessionDefaults.MaxRetryCount)]
        public int MaxRetryCount
        {
            get
            {
                return _maxRetryCount;
            }
            set
            {
                if (value <= 0)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                                                    SRP.ValueMustBePositive));
                _maxRetryCount = value;
            }
        }

        [DefaultValue(ReliableSessionDefaults.MaxTransferWindowSize)]
        public int MaxTransferWindowSize
        {
            get
            {
                return _maxTransferWindowSize;
            }
            set
            {
                if (value <= 0 || value > 4096)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                                                    SRP.Format(SRP.ValueMustBeInRange, 0, 4096)));
                _maxTransferWindowSize = value;
            }
        }

        [DefaultValue(ReliableSessionDefaults.Ordered)]
        public bool Ordered { get; set; } = ReliableSessionDefaults.Ordered;

        [DefaultValue(typeof(ReliableMessagingVersion), ReliableSessionDefaults.ReliableMessagingVersionString)]
        public ReliableMessagingVersion ReliableMessagingVersion
        {
            get
            {
                return _reliableMessagingVersion;
            }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }

                if (!ReliableMessagingVersion.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }

                _reliableMessagingVersion = value;
            }
        }

        private static MessagePartSpecification BodyOnly
        {
            get
            {
                if (s_bodyOnly == null)
                {
                    MessagePartSpecification temp = new MessagePartSpecification(true);
                    temp.MakeReadOnly();
                    s_bodyOnly = temp;
                }

                return s_bodyOnly;
            }
        }

        public override BindingElement Clone()
        {
            return new ReliableSessionBindingElement(this);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }
            if (typeof(T) == typeof(ChannelProtectionRequirements))
            {
                ChannelProtectionRequirements myRequirements = GetProtectionRequirements();
                myRequirements.Add(context.GetInnerProperty<ChannelProtectionRequirements>() ?? new ChannelProtectionRequirements());
                return (T)(object)myRequirements;
            }
            else if (typeof(T) == typeof(IBindingDeliveryCapabilities))
            {
                return (T)(object)new BindingDeliveryCapabilitiesHelper(this, context.GetInnerProperty<IBindingDeliveryCapabilities>());
            }
            else
            {
                return context.GetInnerProperty<T>();
            }
        }

        private ChannelProtectionRequirements GetProtectionRequirements()
        {
            // Listing headers that must be signed.
            ChannelProtectionRequirements result = new ChannelProtectionRequirements();
            MessagePartSpecification signedReliabilityMessageParts = WsrmIndex.GetSignedReliabilityMessageParts(
                _reliableMessagingVersion);
            result.IncomingSignatureParts.AddParts(signedReliabilityMessageParts);
            result.OutgoingSignatureParts.AddParts(signedReliabilityMessageParts);

            if (_reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                // Adding RM protocol message actions so that each RM protocol message's body will be 
                // signed and encrypted.
                // From the Client to the Service
                ScopedMessagePartSpecification signaturePart = result.IncomingSignatureParts;
                ScopedMessagePartSpecification encryptionPart = result.IncomingEncryptionParts;
                ProtectProtocolMessage(signaturePart, encryptionPart, WsrmFeb2005Strings.AckRequestedAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, WsrmFeb2005Strings.CreateSequenceAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, WsrmFeb2005Strings.SequenceAcknowledgementAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, WsrmFeb2005Strings.LastMessageAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, WsrmFeb2005Strings.TerminateSequenceAction);

                // From the Service to the Client
                signaturePart = result.OutgoingSignatureParts;
                encryptionPart = result.OutgoingEncryptionParts;
                ProtectProtocolMessage(signaturePart, encryptionPart, WsrmFeb2005Strings.CreateSequenceResponseAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, WsrmFeb2005Strings.SequenceAcknowledgementAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, WsrmFeb2005Strings.LastMessageAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, WsrmFeb2005Strings.TerminateSequenceAction);
            }
            else if (_reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                // Adding RM protocol message actions so that each RM protocol message's body will be 
                // signed and encrypted.
                // From the Client to the Service
                ScopedMessagePartSpecification signaturePart = result.IncomingSignatureParts;
                ScopedMessagePartSpecification encryptionPart = result.IncomingEncryptionParts;
                ProtectProtocolMessage(signaturePart, encryptionPart, Wsrm11Strings.AckRequestedAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, Wsrm11Strings.CloseSequenceAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, Wsrm11Strings.CloseSequenceResponseAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, Wsrm11Strings.CreateSequenceAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, Wsrm11Strings.FaultAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, Wsrm11Strings.SequenceAcknowledgementAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, Wsrm11Strings.TerminateSequenceAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, Wsrm11Strings.TerminateSequenceResponseAction);

                // From the Service to the Client
                signaturePart = result.OutgoingSignatureParts;
                encryptionPart = result.OutgoingEncryptionParts;
                ProtectProtocolMessage(signaturePart, encryptionPart, Wsrm11Strings.AckRequestedAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, Wsrm11Strings.CloseSequenceAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, Wsrm11Strings.CloseSequenceResponseAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, Wsrm11Strings.CreateSequenceResponseAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, Wsrm11Strings.FaultAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, Wsrm11Strings.SequenceAcknowledgementAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, Wsrm11Strings.TerminateSequenceAction);
                ProtectProtocolMessage(signaturePart, encryptionPart, Wsrm11Strings.TerminateSequenceResponseAction);
            }
            else
            {
                throw Fx.AssertAndThrow("Reliable messaging version not supported.");
            }

            return result;
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));

            VerifyTransportMode(context);

            if (typeof(TChannel) == typeof(IOutputSessionChannel))
            {
                if (context.CanBuildInnerChannelFactory<IRequestSessionChannel>())
                {
                    return (IChannelFactory<TChannel>)(object)
                        new ReliableChannelFactory<TChannel, IRequestSessionChannel>(
                        this, context.BuildInnerChannelFactory<IRequestSessionChannel>(), context.Binding);
                }
                else if (context.CanBuildInnerChannelFactory<IRequestChannel>())
                {
                    return (IChannelFactory<TChannel>)(object)
                        new ReliableChannelFactory<TChannel, IRequestChannel>(
                        this, context.BuildInnerChannelFactory<IRequestChannel>(), context.Binding);
                }
                else if (context.CanBuildInnerChannelFactory<IDuplexSessionChannel>())
                {
                    return (IChannelFactory<TChannel>)(object)
                        new ReliableChannelFactory<TChannel, IDuplexSessionChannel>(
                        this, context.BuildInnerChannelFactory<IDuplexSessionChannel>(), context.Binding);
                }
                else if (context.CanBuildInnerChannelFactory<IDuplexChannel>())
                {
                    return (IChannelFactory<TChannel>)(object)
                        new ReliableChannelFactory<TChannel, IDuplexChannel>(
                        this, context.BuildInnerChannelFactory<IDuplexChannel>(), context.Binding);
                }
            }
            else if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                if (context.CanBuildInnerChannelFactory<IDuplexSessionChannel>())
                {
                    return (IChannelFactory<TChannel>)(object)
                        new ReliableChannelFactory<TChannel, IDuplexSessionChannel>(
                        this, context.BuildInnerChannelFactory<IDuplexSessionChannel>(), context.Binding);
                }
                else if (context.CanBuildInnerChannelFactory<IDuplexChannel>())
                {
                    return (IChannelFactory<TChannel>)(object)
                        new ReliableChannelFactory<TChannel, IDuplexChannel>(
                        this, context.BuildInnerChannelFactory<IDuplexChannel>(), context.Binding);
                }
            }
            else if (typeof(TChannel) == typeof(IRequestSessionChannel))
            {
                if (context.CanBuildInnerChannelFactory<IRequestSessionChannel>())
                {
                    return (IChannelFactory<TChannel>)(object)
                        new ReliableChannelFactory<TChannel, IRequestSessionChannel>(
                        this, context.BuildInnerChannelFactory<IRequestSessionChannel>(), context.Binding);
                }
                else if (context.CanBuildInnerChannelFactory<IRequestChannel>())
                {
                    return (IChannelFactory<TChannel>)(object)
                        new ReliableChannelFactory<TChannel, IRequestChannel>(
                        this, context.BuildInnerChannelFactory<IRequestChannel>(), context.Binding);
                }
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("TChannel", SRP.Format(SRP.ChannelTypeNotSupported, typeof(TChannel)));
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));

            if (typeof(TChannel) == typeof(IOutputSessionChannel))
            {
                return context.CanBuildInnerChannelFactory<IRequestSessionChannel>()
                    || context.CanBuildInnerChannelFactory<IRequestChannel>()
                    || context.CanBuildInnerChannelFactory<IDuplexSessionChannel>()
                    || context.CanBuildInnerChannelFactory<IDuplexChannel>();
            }
            else if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return context.CanBuildInnerChannelFactory<IDuplexSessionChannel>()
                    || context.CanBuildInnerChannelFactory<IDuplexChannel>();
            }
            else if (typeof(TChannel) == typeof(IRequestSessionChannel))
            {
                return context.CanBuildInnerChannelFactory<IRequestSessionChannel>()
                    || context.CanBuildInnerChannelFactory<IRequestChannel>();
            }

            return false;
        }

        private static void ProtectProtocolMessage(
            ScopedMessagePartSpecification signaturePart,
            ScopedMessagePartSpecification encryptionPart,
            string action)
        {
            signaturePart.AddParts(BodyOnly, action);
            encryptionPart.AddParts(MessagePartSpecification.NoParts, action);
            //encryptionPart.AddParts(BodyOnly, action);
        }

        private void VerifyTransportMode(BindingContext context)
        {
            TransportBindingElement transportElement = context.RemainingBindingElements.Find<TransportBindingElement>();

            // Verify ManualAdderssing is turned off.
            if ((transportElement != null) && (transportElement.ManualAddressing))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new InvalidOperationException(SRP.ManualAddressingNotSupported));
            }

            // Verify TransportMode is Buffered.
            TransferMode transportTransferMode;
            Tuple<TransferMode> transportTransferModeHolder = transportElement.GetProperty<Tuple<TransferMode>>(context);

            if (transportTransferModeHolder != null)
            {
                transportTransferMode = transportTransferModeHolder.Item1;
            }
            else
            {
                // Not one of the elements we can check, we have to assume TransferMode.Buffered.
                transportTransferMode = TransferMode.Buffered;
            }

            if (transportTransferMode != TransferMode.Buffered)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new InvalidOperationException(SRP.Format(SRP.TransferModeNotSupported,
                    transportTransferMode, GetType().Name)));
            }
        }

        private static XmlElement CreatePolicyElement(PolicyVersion policyVersion, XmlDocument doc)
        {
            string policy = MetadataStrings.WSPolicy.Elements.Policy;
            string policyNs = policyVersion.Namespace;
            string policyPrefix = MetadataStrings.WSPolicy.Prefix;

            return doc.CreateElement(policyPrefix, policy, policyNs);
        }

        private class BindingDeliveryCapabilitiesHelper : IBindingDeliveryCapabilities
        {
            private readonly ReliableSessionBindingElement _element;
            private readonly IBindingDeliveryCapabilities _inner;

            internal BindingDeliveryCapabilitiesHelper(ReliableSessionBindingElement element, IBindingDeliveryCapabilities inner)
            {
                _element = element;
                _inner = inner;
            }
            bool IBindingDeliveryCapabilities.AssuresOrderedDelivery
            {
                get { return _element.Ordered; }
            }

            bool IBindingDeliveryCapabilities.QueuedDelivery
            {
                get { return _inner != null ? _inner.QueuedDelivery : false; }
            }
        }
    }
}
