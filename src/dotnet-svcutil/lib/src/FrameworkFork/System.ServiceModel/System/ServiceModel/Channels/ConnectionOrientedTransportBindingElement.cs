// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ServiceModel.Description;

namespace System.ServiceModel.Channels
{
    public abstract class ConnectionOrientedTransportBindingElement
        : TransportBindingElement,
        ITransportPolicyImport
    {
        private int _connectionBufferSize;
        private bool _exposeConnectionProperty;
        private HostNameComparisonMode _hostNameComparisonMode;
        private TimeSpan _maxOutputDelay;
        private int _maxBufferSize;
        private bool _maxBufferSizeInitialized;
        private TransferMode _transferMode;

        internal ConnectionOrientedTransportBindingElement()
            : base()
        {
            _connectionBufferSize = ConnectionOrientedTransportDefaults.ConnectionBufferSize;
            _hostNameComparisonMode = ConnectionOrientedTransportDefaults.HostNameComparisonMode;
            _maxOutputDelay = ConnectionOrientedTransportDefaults.MaxOutputDelay;
            _maxBufferSize = TransportDefaults.MaxBufferSize;
            _transferMode = ConnectionOrientedTransportDefaults.TransferMode;
        }

        internal ConnectionOrientedTransportBindingElement(ConnectionOrientedTransportBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _connectionBufferSize = elementToBeCloned._connectionBufferSize;
            _exposeConnectionProperty = elementToBeCloned._exposeConnectionProperty;
            _hostNameComparisonMode = elementToBeCloned._hostNameComparisonMode;
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRServiceModel.ValueMustBeNonNegative));
                }

                _connectionBufferSize = value;
            }
        }

        // client
        internal bool ExposeConnectionProperty
        {
            get
            {
                return _exposeConnectionProperty;
            }
            set
            {
                _exposeConnectionProperty = value;
            }
        }

        [DefaultValue(ConnectionOrientedTransportDefaults.HostNameComparisonMode)]
        public HostNameComparisonMode HostNameComparisonMode
        {
            get
            {
                return _hostNameComparisonMode;
            }

            set
            {
                HostNameComparisonModeHelper.Validate(value);
                _hostNameComparisonMode = value;
            }
        }

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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        SRServiceModel.ValueMustBePositive));
                }

                _maxBufferSizeInitialized = true;
                _maxBufferSize = value;
            }
        }

        internal TimeSpan MaxOutputDelay
        {
            get
            {
                return _maxOutputDelay;
            }
        }

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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
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

        void ITransportPolicyImport.ImportPolicy(MetadataImporter importer, PolicyConversionContext policyContext)
        {
            if (PolicyConversionContext.FindAssertion(policyContext.GetBindingAssertions(), TransportPolicyConstants.StreamedName, TransportPolicyConstants.DotNetFramingNamespace, true) != null)
            {
                this.TransferMode = TransferMode.Streamed;
            }

            WindowsStreamSecurityBindingElement.ImportPolicy(importer, policyContext);
            SslStreamSecurityBindingElement.ImportPolicy(importer, policyContext);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            if (typeof(T) == typeof(TransferMode))
            {
                return (T)(object)this.TransferMode;
            }
            else
            {
                return base.GetProperty<T>(context);
            }
        }

        internal override bool IsMatch(BindingElement b)
        {
            if (!base.IsMatch(b))
                return false;

            ConnectionOrientedTransportBindingElement connection = b as ConnectionOrientedTransportBindingElement;
            if (connection == null)
                return false;

            if (_connectionBufferSize != connection._connectionBufferSize)
                return false;

            if (_maxBufferSize != connection._maxBufferSize)
                return false;

            if (_transferMode != connection._transferMode)
                return false;

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
