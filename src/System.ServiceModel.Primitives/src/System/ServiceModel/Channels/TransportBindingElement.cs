// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Security;
using System.Xml;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace System.ServiceModel.Channels
{
    public abstract class TransportBindingElement
        : BindingElement
    {
        private bool _manualAddressing;
        private long _maxBufferPoolSize;
        private long _maxReceivedMessageSize;

        protected TransportBindingElement()
        {
            _manualAddressing = TransportDefaults.ManualAddressing;
            _maxBufferPoolSize = TransportDefaults.MaxBufferPoolSize;
            _maxReceivedMessageSize = TransportDefaults.MaxReceivedMessageSize;
        }

        protected TransportBindingElement(TransportBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _manualAddressing = elementToBeCloned._manualAddressing;
            _maxBufferPoolSize = elementToBeCloned._maxBufferPoolSize;
            _maxReceivedMessageSize = elementToBeCloned._maxReceivedMessageSize;
        }

        [DefaultValue(TransportDefaults.ManualAddressing)]
        public virtual bool ManualAddressing
        {
            get
            {
                return _manualAddressing;
            }

            set
            {
                _manualAddressing = value;
            }
        }

        [DefaultValue(TransportDefaults.MaxBufferPoolSize)]
        public virtual long MaxBufferPoolSize
        {
            get
            {
                return _maxBufferPoolSize;
            }
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SRP.ValueMustBeNonNegative));
                }
                _maxBufferPoolSize = value;
            }
        }

        [DefaultValue(TransportDefaults.MaxReceivedMessageSize)]
        public virtual long MaxReceivedMessageSize
        {
            get
            {
                return _maxReceivedMessageSize;
            }
            set
            {
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SRP.ValueMustBePositive));
                }
                _maxReceivedMessageSize = value;
            }
        }

        public abstract string Scheme { get; }

        internal static IChannelFactory<TChannel> CreateChannelFactory<TChannel>(TransportBindingElement transport)
        {
            Binding binding = new CustomBinding(transport);
            return binding.BuildChannelFactory<TChannel>();
        }


        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }

            if (typeof(T) == typeof(ChannelProtectionRequirements))
            {
                ChannelProtectionRequirements myRequirements = GetProtectionRequirements(context);
                myRequirements.Add(context.GetInnerProperty<ChannelProtectionRequirements>() ?? new ChannelProtectionRequirements());
                return (T)(object)myRequirements;
            }

            // to cover all our bases, let's iterate through the BindingParameters to make sure
            // we haven't missed a query (since we're the Transport and we're at the bottom)
            Collection<BindingElement> bindingElements = context.BindingParameters.FindAll<BindingElement>();

            T result = default(T);
            for (int i = 0; i < bindingElements.Count; i++)
            {
                result = bindingElements[i].GetIndividualProperty<T>();
                if (result != default(T))
                {
                    return result;
                }
            }

            if (typeof(T) == typeof(MessageVersion))
            {
                return (T)(object)TransportDefaults.GetDefaultMessageEncoderFactory().MessageVersion;
            }

            if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
            {
                return (T)(object)new XmlDictionaryReaderQuotas();
            }

            return null;
        }

        private ChannelProtectionRequirements GetProtectionRequirements(AddressingVersion addressingVersion)
        {
            ChannelProtectionRequirements result = new ChannelProtectionRequirements();
            result.IncomingSignatureParts.AddParts(addressingVersion.SignedMessageParts);
            result.OutgoingSignatureParts.AddParts(addressingVersion.SignedMessageParts);
            return result;
        }

        internal ChannelProtectionRequirements GetProtectionRequirements(BindingContext context)
        {
            AddressingVersion addressingVersion = AddressingVersion.WSAddressing10;
            MessageEncodingBindingElement messageEncoderBindingElement = context.Binding.Elements.Find<MessageEncodingBindingElement>();
            if (messageEncoderBindingElement != null)
            {
                addressingVersion = messageEncoderBindingElement.MessageVersion.Addressing;
            }
            return GetProtectionRequirements(addressingVersion);
        }

        internal override bool IsMatch(BindingElement b)
        {
            if (b == null)
            {
                return false;
            }
            TransportBindingElement transport = b as TransportBindingElement;
            if (transport == null)
            {
                return false;
            }
            if (_maxBufferPoolSize != transport.MaxBufferPoolSize)
            {
                return false;
            }
            if (_maxReceivedMessageSize != transport.MaxReceivedMessageSize)
            {
                return false;
            }
            return true;
        }
    }
}
