// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;

namespace System.ServiceModel.Channels
{
    public abstract class ConnectionOrientedTransportBindingElement
        : TransportBindingElement

    {
        private int _connectionBufferSize;
        private int _maxBufferSize;
        private bool _maxBufferSizeInitialized;
        private TransferMode _transferMode;

        internal ConnectionOrientedTransportBindingElement()
            : base()
        {
            _connectionBufferSize = ConnectionOrientedTransportDefaults.ConnectionBufferSize;
            MaxOutputDelay = ConnectionOrientedTransportDefaults.MaxOutputDelay;
            _maxBufferSize = TransportDefaults.MaxBufferSize;
            _transferMode = ConnectionOrientedTransportDefaults.TransferMode;
        }

        internal ConnectionOrientedTransportBindingElement(ConnectionOrientedTransportBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _connectionBufferSize = elementToBeCloned._connectionBufferSize;
            ExposeConnectionProperty = elementToBeCloned.ExposeConnectionProperty;
            _maxBufferSize = elementToBeCloned._maxBufferSize;
            _maxBufferSizeInitialized = elementToBeCloned._maxBufferSizeInitialized;
            _transferMode = elementToBeCloned._transferMode;
        }

        // client
        // server
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

        // client
        internal bool ExposeConnectionProperty { get; set; }

        // server
        [DefaultValue(TransportDefaults.MaxBufferSize)]
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

            if (typeof(T) == typeof(TransferMode))
            {
                return (T)(object)TransferMode;
            }
            else
            {
                return base.GetProperty<T>(context);
            }
        }

        internal override bool IsMatch(BindingElement b)
        {
            if (!base.IsMatch(b))
            {
                return false;
            }

            ConnectionOrientedTransportBindingElement connection = b as ConnectionOrientedTransportBindingElement;
            if (connection == null)
            {
                return false;
            }

            if (_connectionBufferSize != connection._connectionBufferSize)
            {
                return false;
            }

            if (_maxBufferSize != connection._maxBufferSize)
            {
                return false;
            }

            if (_transferMode != connection._transferMode)
            {
                return false;
            }

            return true;
        }

        private MessageEncodingBindingElement FindMessageEncodingBindingElement(BindingElementCollection bindingElements, out bool createdNew)
        {
            createdNew = false;
            MessageEncodingBindingElement encodingBindingElement = bindingElements.Find<MessageEncodingBindingElement>();
            if (encodingBindingElement == null)
            {
                createdNew = true;
                encodingBindingElement = new BinaryMessageEncodingBindingElement();
            }
            return encodingBindingElement;
        }
    }
}
