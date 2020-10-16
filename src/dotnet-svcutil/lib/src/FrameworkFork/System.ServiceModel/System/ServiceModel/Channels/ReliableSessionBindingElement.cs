// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using System.ComponentModel;
    using System.Runtime;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Security;
    using Microsoft.Xml;

    public sealed class ReliableSessionBindingElement : BindingElement, IPolicyExportExtension
    {
        private TimeSpan _acknowledgementInterval = ReliableSessionDefaults.AcknowledgementInterval;
        private bool _flowControlEnabled = ReliableSessionDefaults.FlowControlEnabled;
        private TimeSpan _inactivityTimeout = ReliableSessionDefaults.InactivityTimeout;
        private int _maxPendingChannels = ReliableSessionDefaults.MaxPendingChannels;
        private int _maxRetryCount = ReliableSessionDefaults.MaxRetryCount;
        private int _maxTransferWindowSize = ReliableSessionDefaults.MaxTransferWindowSize;
        private bool _ordered = ReliableSessionDefaults.Ordered;
        private ReliableMessagingVersion _reliableMessagingVersion = ReliableMessagingVersion.Default;

        private static MessagePartSpecification s_bodyOnly;

        public ReliableSessionBindingElement()
        {
        }

        internal ReliableSessionBindingElement(ReliableSessionBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            this.AcknowledgementInterval = elementToBeCloned.AcknowledgementInterval;
            this.FlowControlEnabled = elementToBeCloned.FlowControlEnabled;
            this.InactivityTimeout = elementToBeCloned.InactivityTimeout;
            this.MaxPendingChannels = elementToBeCloned.MaxPendingChannels;
            this.MaxRetryCount = elementToBeCloned.MaxRetryCount;
            this.MaxTransferWindowSize = elementToBeCloned.MaxTransferWindowSize;
            this.Ordered = elementToBeCloned.Ordered;
            this.ReliableMessagingVersion = elementToBeCloned.ReliableMessagingVersion;
        }

        public ReliableSessionBindingElement(bool ordered)
        {
            _ordered = ordered;
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.TimeSpanMustbeGreaterThanTimeSpanZero));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRangeTooBig));
                }

                _acknowledgementInterval = value;
            }
        }

        [DefaultValue(ReliableSessionDefaults.FlowControlEnabled)]
        public bool FlowControlEnabled
        {
            get
            {
                return _flowControlEnabled;
            }
            set
            {
                _flowControlEnabled = value;
            }
        }

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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.TimeSpanMustbeGreaterThanTimeSpanZero));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRangeTooBig));
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
                                                    string.Format(SRServiceModel.ValueMustBeInRange, 0, 16384)));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.ValueMustBePositive));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                                                    string.Format(SRServiceModel.ValueMustBeInRange, 0, 4096)));
                _maxTransferWindowSize = value;
            }
        }

        [DefaultValue(ReliableSessionDefaults.Ordered)]
        public bool Ordered
        {
            get
            {
                return _ordered;
            }
            set
            {
                _ordered = value;
            }
        }

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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }

                if (!ReliableMessagingVersion.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
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
            throw new NotImplementedException();
        }


        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            throw new NotImplementedException();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            throw new NotImplementedException();
        }

        internal override bool IsMatch(BindingElement b)
        {
            if (b == null)
                return false;
            ReliableSessionBindingElement session = b as ReliableSessionBindingElement;
            if (session == null)
                return false;
            if (_acknowledgementInterval != session._acknowledgementInterval)
                return false;
            if (_flowControlEnabled != session._flowControlEnabled)
                return false;
            if (_inactivityTimeout != session._inactivityTimeout)
                return false;
            if (_maxPendingChannels != session._maxPendingChannels)
                return false;
            if (_maxRetryCount != session._maxRetryCount)
                return false;
            if (_maxTransferWindowSize != session._maxTransferWindowSize)
                return false;
            if (_ordered != session._ordered)
                return false;
            if (_reliableMessagingVersion != session._reliableMessagingVersion)
                return false;

            return true;
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

        private void SetSecuritySettings(BindingContext context)
        {
            SecurityBindingElement element = context.RemainingBindingElements.Find<SecurityBindingElement>();

            if (element != null)
            {
                element.LocalServiceSettings.ReconnectTransportOnFailure = true;
            }
        }

        private void VerifyTransportMode(BindingContext context)
        {
            TransportBindingElement transportElement = context.RemainingBindingElements.Find<TransportBindingElement>();

            // Verify ManualAdderssing is turned off.
            if ((transportElement != null) && (transportElement.ManualAddressing))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new InvalidOperationException(SRServiceModel.ManualAddressingNotSupported));
            }

            ConnectionOrientedTransportBindingElement connectionElement = transportElement as ConnectionOrientedTransportBindingElement;
            HttpTransportBindingElement httpElement = transportElement as HttpTransportBindingElement;

            // Verify TransportMode is Buffered.
            TransferMode transportTransferMode;

            if (connectionElement != null)
            {
                transportTransferMode = connectionElement.TransferMode;
            }
            else if (httpElement != null)
            {
                transportTransferMode = httpElement.TransferMode;
            }
            else
            {
                // Not one of the elements we can check, we have to assume TransferMode.Buffered.
                transportTransferMode = TransferMode.Buffered;
            }

            if (transportTransferMode != TransferMode.Buffered)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new InvalidOperationException(string.Format(SRServiceModel.TransferModeNotSupported,
                    transportTransferMode, this.GetType().Name)));
            }
        }

        void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext context)
        {
            if (exporter == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");

            if (context == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");

            if (context.BindingElements != null)
            {
                BindingElementCollection bindingElements = context.BindingElements;
                ReliableSessionBindingElement settings = bindingElements.Find<ReliableSessionBindingElement>();

                if (settings != null)
                {
                    // ReliableSession assertion
                    XmlElement assertion = settings.CreateReliabilityAssertion(exporter.PolicyVersion, bindingElements);
                    context.GetBindingAssertions().Add(assertion);
                }
            }
        }

        private static XmlElement CreatePolicyElement(PolicyVersion policyVersion, XmlDocument doc)
        {
            string policy = MetadataStrings.WSPolicy.Elements.Policy;
            string policyNs = policyVersion.Namespace;
            string policyPrefix = MetadataStrings.WSPolicy.Prefix;

            return doc.CreateElement(policyPrefix, policy, policyNs);
        }

        private XmlElement CreateReliabilityAssertion(PolicyVersion policyVersion, BindingElementCollection bindingElements)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement child = null;
            string reliableSessionPrefix;
            string reliableSessionNs;
            string assertionPrefix;
            string assertionNs;

            if (this.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                reliableSessionPrefix = ReliableSessionPolicyStrings.ReliableSessionFebruary2005Prefix;
                reliableSessionNs = ReliableSessionPolicyStrings.ReliableSessionFebruary2005Namespace;
                assertionPrefix = reliableSessionPrefix;
                assertionNs = reliableSessionNs;
            }
            else
            {
                reliableSessionPrefix = ReliableSessionPolicyStrings.ReliableSession11Prefix;
                reliableSessionNs = ReliableSessionPolicyStrings.ReliableSession11Namespace;
                assertionPrefix = ReliableSessionPolicyStrings.NET11Prefix;
                assertionNs = ReliableSessionPolicyStrings.NET11Namespace;
            }

            // ReliableSession assertion
            XmlElement assertion = doc.CreateElement(reliableSessionPrefix, ReliableSessionPolicyStrings.ReliableSessionName, reliableSessionNs);

            if (this.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                // Policy
                XmlElement policy = CreatePolicyElement(policyVersion, doc);

                // SequenceSTR
                if (IsSecureConversationEnabled(bindingElements))
                {
                    XmlElement sequenceSTR = doc.CreateElement(reliableSessionPrefix, ReliableSessionPolicyStrings.SequenceSTR, reliableSessionNs);
                    policy.AppendChild(sequenceSTR);
                }

                // DeliveryAssurance
                XmlElement deliveryAssurance = doc.CreateElement(reliableSessionPrefix, ReliableSessionPolicyStrings.DeliveryAssurance, reliableSessionNs);

                // Policy
                XmlElement nestedPolicy = CreatePolicyElement(policyVersion, doc);

                // ExactlyOnce
                XmlElement exactlyOnce = doc.CreateElement(reliableSessionPrefix, ReliableSessionPolicyStrings.ExactlyOnce, reliableSessionNs);
                nestedPolicy.AppendChild(exactlyOnce);

                if (_ordered)
                {
                    // InOrder
                    XmlElement inOrder = doc.CreateElement(reliableSessionPrefix, ReliableSessionPolicyStrings.InOrder, reliableSessionNs);
                    nestedPolicy.AppendChild(inOrder);
                }

                deliveryAssurance.AppendChild(nestedPolicy);
                policy.AppendChild(deliveryAssurance);
                assertion.AppendChild(policy);
            }

            // Nested InactivityTimeout assertion
            child = doc.CreateElement(assertionPrefix, ReliableSessionPolicyStrings.InactivityTimeout, assertionNs);
            WriteMillisecondsAttribute(child, this.InactivityTimeout);
            assertion.AppendChild(child);

            // Nested AcknowledgementInterval assertion
            child = doc.CreateElement(assertionPrefix, ReliableSessionPolicyStrings.AcknowledgementInterval, assertionNs);
            WriteMillisecondsAttribute(child, this.AcknowledgementInterval);
            assertion.AppendChild(child);

            return assertion;
        }

        private static bool IsSecureConversationEnabled(BindingElementCollection bindingElements)
        {
            bool foundRM = false;

            for (int i = 0; i < bindingElements.Count; i++)
            {
                if (!foundRM)
                {
                    ReliableSessionBindingElement bindingElement = bindingElements[i] as ReliableSessionBindingElement;
                    foundRM = (bindingElement != null);
                }
                else
                {
                    SecurityBindingElement securityBindingElement = bindingElements[i] as SecurityBindingElement;

                    if (securityBindingElement != null)
                    {
                        SecurityBindingElement bootstrapSecurity;

                        // The difference in bool (requireCancellation) does not affect whether the binding is valid,
                        // but the method does match on the value so we need to pass both true and false.
                        return SecurityBindingElement.IsSecureConversationBinding(securityBindingElement, true, out bootstrapSecurity)
                            || SecurityBindingElement.IsSecureConversationBinding(securityBindingElement, false, out bootstrapSecurity);
                    }

                    break;
                }
            }

            return false;
        }

        private static void WriteMillisecondsAttribute(XmlElement childElement, TimeSpan timeSpan)
        {
            UInt64 milliseconds = Convert.ToUInt64(timeSpan.TotalMilliseconds);
            childElement.SetAttribute(ReliableSessionPolicyStrings.Milliseconds, XmlConvert.ToString(milliseconds));
        }

        private class BindingDeliveryCapabilitiesHelper : IBindingDeliveryCapabilities
        {
            private ReliableSessionBindingElement _element;
            private IBindingDeliveryCapabilities _inner;

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
