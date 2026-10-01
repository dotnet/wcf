// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Channels
{
    public abstract class MsmqBindingElementBase : TransportBindingElement
    {
        private DeadLetterQueue _deadLetterQueue;
        private int _maxRetryCycles;
        private ReceiveErrorHandling _receiveErrorHandling;
        private int _receiveRetryCount;
        private TimeSpan _retryCycleDelay;
        private TimeSpan _timeToLive;
        private TimeSpan _validityDuration;

        internal MsmqBindingElementBase()
        {
            CustomDeadLetterQueue = null;
            _deadLetterQueue = MsmqDefaults.DeadLetterQueue;
            Durable = MsmqDefaults.Durable;
            ExactlyOnce = MsmqDefaults.ExactlyOnce;
            _maxRetryCycles = MsmqDefaults.MaxRetryCycles;
            ReceiveContextEnabled = MsmqDefaults.ReceiveContextEnabled;
            _receiveErrorHandling = MsmqDefaults.ReceiveErrorHandling;
            _receiveRetryCount = MsmqDefaults.ReceiveRetryCount;
            _retryCycleDelay = MsmqDefaults.RetryCycleDelay;
            _timeToLive = MsmqDefaults.TimeToLive;
            _validityDuration = MsmqDefaults.ValidityDuration;
            MsmqTransportSecurity = new MsmqTransportSecurity();
            UseMsmqTracing = MsmqDefaults.UseMsmqTracing;
            UseSourceJournal = MsmqDefaults.UseSourceJournal;
        }

        internal MsmqBindingElementBase(MsmqBindingElementBase elementToBeCloned) : base(elementToBeCloned)
        {
            CustomDeadLetterQueue = elementToBeCloned.CustomDeadLetterQueue;
            _deadLetterQueue = elementToBeCloned._deadLetterQueue;
            Durable = elementToBeCloned.Durable;
            ExactlyOnce = elementToBeCloned.ExactlyOnce;
            _maxRetryCycles = elementToBeCloned._maxRetryCycles;
            MsmqTransportSecurity = new MsmqTransportSecurity(elementToBeCloned.MsmqTransportSecurity);
            ReceiveContextEnabled = elementToBeCloned.ReceiveContextEnabled;
            _receiveErrorHandling = elementToBeCloned._receiveErrorHandling;
            _receiveRetryCount = elementToBeCloned._receiveRetryCount;
            _retryCycleDelay = elementToBeCloned._retryCycleDelay;
            _timeToLive = elementToBeCloned._timeToLive;
            _validityDuration = elementToBeCloned._validityDuration;
            UseMsmqTracing = elementToBeCloned.UseMsmqTracing;
            UseSourceJournal = elementToBeCloned.UseSourceJournal;
        }

        internal abstract MsmqUri.IAddressTranslator AddressTranslator { get; }

        public Uri CustomDeadLetterQueue { get; set; }

        public DeadLetterQueue DeadLetterQueue
        {
            get { return _deadLetterQueue; }
            set
            {
                if (!DeadLetterQueueHelper.IsDefined(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _deadLetterQueue = value;
            }
        }

        public bool Durable { get; set; }

        public bool TransactedReceiveEnabled => ExactlyOnce;

        public bool ExactlyOnce { get; set; }

        public int ReceiveRetryCount
        {
            get { return _receiveRetryCount; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, SR.MsmqNonNegativeArgumentExpected);
                }
                _receiveRetryCount = value;
            }
        }

        public int MaxRetryCycles
        {
            get { return _maxRetryCycles; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, SR.MsmqNonNegativeArgumentExpected);
                }
                _maxRetryCycles = value;
            }
        }

        public MsmqTransportSecurity MsmqTransportSecurity { get; internal set; }

        public bool ReceiveContextEnabled { get; set; }

        public ReceiveErrorHandling ReceiveErrorHandling
        {
            get { return _receiveErrorHandling; }
            set
            {
                if (!IsReceiveErrorHandlingDefined(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _receiveErrorHandling = value;
            }
        }

        public TimeSpan RetryCycleDelay
        {
            get { return _retryCycleDelay; }
            set
            {
                ValidateTimeoutValue(value);
                _retryCycleDelay = value;
            }
        }

        public TimeSpan TimeToLive
        {
            get { return _timeToLive; }
            set
            {
                ValidateTimeoutValue(value);
                _timeToLive = value;
            }
        }

        // Receive-side setting, carried for .NET Framework surface parity: it
        // bounds how long a poison-message record stays valid. The client-side
        // send path does not consume it.
        public TimeSpan ValidityDuration
        {
            get { return _validityDuration; }
            set
            {
                ValidateTimeoutValue(value);
                _validityDuration = value;
            }
        }

        public bool UseMsmqTracing { get; set; }

        public bool UseSourceJournal { get; set; }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return null;
            }
            if (typeof(T) == typeof(IBindingDeliveryCapabilities))
            {
                return (T)(object)new BindingDeliveryCapabilitiesHelper();
            }
            return base.GetProperty<T>(context);
        }

        private static bool IsReceiveErrorHandlingDefined(ReceiveErrorHandling value)
        {
            return value == ReceiveErrorHandling.Fault
                || value == ReceiveErrorHandling.Drop
                || value == ReceiveErrorHandling.Reject
                || value == ReceiveErrorHandling.Move;
        }

        private static void ValidateTimeoutValue(TimeSpan value)
        {
            if (value < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, SR.SFxTimeoutOutOfRange0);
            }
            if (value.TotalMilliseconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, SR.SFxTimeoutOutOfRangeTooBig);
            }
        }

        private sealed class BindingDeliveryCapabilitiesHelper : IBindingDeliveryCapabilities
        {
            bool IBindingDeliveryCapabilities.AssuresOrderedDelivery => false;
            bool IBindingDeliveryCapabilities.QueuedDelivery => true;
        }
    }
}
