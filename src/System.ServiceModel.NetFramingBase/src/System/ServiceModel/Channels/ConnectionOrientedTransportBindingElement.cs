// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.ServiceModel.Channels
{
    public abstract class ConnectionOrientedTransportBindingElement : TransportBindingElement
    {
        private int _connectionBufferSize;
        private int _maxBufferSize;
        private bool _maxBufferSizeInitialized;
        private TransferMode _transferMode;

        public ConnectionOrientedTransportBindingElement() : base()
        {
            _connectionBufferSize = ConnectionOrientedTransportDefaults.ConnectionBufferSize;
            MaxOutputDelay = ConnectionOrientedTransportDefaults.MaxOutputDelay;
            _maxBufferSize = NFTransportDefaults.MaxBufferSize;
            _transferMode = ConnectionOrientedTransportDefaults.TransferMode;
        }

        public ConnectionOrientedTransportBindingElement(ConnectionOrientedTransportBindingElement elementToBeCloned) : base(elementToBeCloned)
        {
            _connectionBufferSize = elementToBeCloned._connectionBufferSize;
            _maxBufferSize = elementToBeCloned._maxBufferSize;
            _maxBufferSizeInitialized = elementToBeCloned._maxBufferSizeInitialized;
            _transferMode = elementToBeCloned._transferMode;
        }

        [DefaultValue(ConnectionOrientedTransportDefaults.ConnectionBufferSize)]
        public int ConnectionBufferSize
        {
            get
            {
                return _connectionBufferSize;
            }
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SR.ValueMustBeNonNegative));
                }

                _connectionBufferSize = value;
            }
        }

        [DefaultValue(NFTransportDefaults.MaxBufferSize)]
        public int MaxBufferSize
        {
            get
            {
                if (_maxBufferSizeInitialized || TransferMode != TransferMode.Buffered)
                {
                    return _maxBufferSize;
                }

                long maxReceivedMessageSize = MaxReceivedMessageSize;
                if (maxReceivedMessageSize > int.MaxValue)
                {
                    return int.MaxValue;
                }
                else
                {
                    return (int)maxReceivedMessageSize;
                }
            }
            set
            {
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), value,
                        SR.ValueMustBePositive));
                }

                _maxBufferSizeInitialized = true;
                _maxBufferSize = value;
            }
        }

        internal TimeSpan MaxOutputDelay { get; }

        [DefaultValue(ConnectionOrientedTransportDefaults.TransferMode)]
        public TransferMode TransferMode
        {
            get
            {
                return _transferMode;
            }
            set
            {
                TransferModeHelper.Validate(value);
                _transferMode = value;
            }
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }

            if (TransferMode == TransferMode.Buffered)
            {
                return (typeof(TChannel) == typeof(IDuplexSessionChannel));
            }
            else
            {
                return (typeof(TChannel) == typeof(IRequestChannel));
            }
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }
            else if (typeof(T) == typeof(Tuple<TransferMode>))
            {
                // Work around for ReliableSessionBindingElement.VerifyTransportMode not being able to
                // reference HttpTransportBindingElement to fetch the TransferMode.
                return (T)(object)Tuple.Create(TransferMode);
            }
            else
            {
                return base.GetProperty<T>(context);
            }
        }
    }
}
