// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Channels
{
    public abstract class MsmqBindingElementBase : TransportBindingElement
    {
        private Uri _customDeadLetterQueue;
        private DeadLetterQueue _deadLetterQueue;
        private bool _durable;
        private bool _exactlyOnce;
        private int _maxRetryCycles;
        private ReceiveErrorHandling _receiveErrorHandling;
        private int _receiveRetryCount;
        private TimeSpan _retryCycleDelay;
        private TimeSpan _timeToLive;
        private MsmqTransportSecurity _msmqTransportSecurity;
        private bool _useMsmqTracing;
        private bool _useSourceJournal;
        private bool _receiveContextEnabled;

        internal MsmqBindingElementBase()
        {
            _customDeadLetterQueue = null;
            _deadLetterQueue = MsmqDefaults.DeadLetterQueue;
            _durable = MsmqDefaults.Durable;
            _exactlyOnce = MsmqDefaults.ExactlyOnce;
            _maxRetryCycles = MsmqDefaults.MaxRetryCycles;
            _receiveContextEnabled = MsmqDefaults.ReceiveContextEnabled;
            _receiveErrorHandling = MsmqDefaults.ReceiveErrorHandling;
            _receiveRetryCount = MsmqDefaults.ReceiveRetryCount;
            _retryCycleDelay = MsmqDefaults.RetryCycleDelay;
            _timeToLive = MsmqDefaults.TimeToLive;
            _msmqTransportSecurity = new MsmqTransportSecurity();
            _useMsmqTracing = MsmqDefaults.UseMsmqTracing;
            _useSourceJournal = MsmqDefaults.UseSourceJournal;
        }

        internal MsmqBindingElementBase(MsmqBindingElementBase elementToBeCloned) : base(elementToBeCloned)
        {
            _customDeadLetterQueue = elementToBeCloned._customDeadLetterQueue;
            _deadLetterQueue = elementToBeCloned._deadLetterQueue;
            _durable = elementToBeCloned._durable;
            _exactlyOnce = elementToBeCloned._exactlyOnce;
            _maxRetryCycles = elementToBeCloned._maxRetryCycles;
            _msmqTransportSecurity = new MsmqTransportSecurity(elementToBeCloned.MsmqTransportSecurity);
            _receiveContextEnabled = elementToBeCloned._receiveContextEnabled;
            _receiveErrorHandling = elementToBeCloned._receiveErrorHandling;
            _receiveRetryCount = elementToBeCloned._receiveRetryCount;
            _retryCycleDelay = elementToBeCloned._retryCycleDelay;
            _timeToLive = elementToBeCloned._timeToLive;
            _useMsmqTracing = elementToBeCloned._useMsmqTracing;
            _useSourceJournal = elementToBeCloned._useSourceJournal;
        }

        internal abstract MsmqUri.IAddressTranslator AddressTranslator { get; }

        public Uri CustomDeadLetterQueue
        {
            get { return _customDeadLetterQueue; }
            set { _customDeadLetterQueue = value; }
        }

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

        public bool Durable
        {
            get { return _durable; }
            set { _durable = value; }
        }

        public bool TransactedReceiveEnabled
        {
            get { return _exactlyOnce; }
        }

        public bool ExactlyOnce
        {
            get { return _exactlyOnce; }
            set { _exactlyOnce = value; }
        }

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

        public MsmqTransportSecurity MsmqTransportSecurity
        {
            get { return _msmqTransportSecurity; }
            internal set { _msmqTransportSecurity = value; }
        }

        public bool ReceiveContextEnabled
        {
            get { return _receiveContextEnabled; }
            set { _receiveContextEnabled = value; }
        }

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

        public bool UseMsmqTracing
        {
            get { return _useMsmqTracing; }
            set { _useMsmqTracing = value; }
        }

        public bool UseSourceJournal
        {
            get { return _useSourceJournal; }
            set { _useSourceJournal = value; }
        }

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
