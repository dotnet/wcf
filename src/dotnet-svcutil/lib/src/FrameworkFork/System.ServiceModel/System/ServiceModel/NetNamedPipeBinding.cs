// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ServiceModel.Channels;
using Microsoft.Xml;

namespace System.ServiceModel
{
    public class NetNamedPipeBinding : Binding, IBindingRuntimePreferences
    {
        // private BindingElements
        TransactionFlowBindingElement _context;
        BinaryMessageEncodingBindingElement _encoding;
        NamedPipeTransportBindingElement _namedPipe;
        NetNamedPipeSecurity _security = new NetNamedPipeSecurity();

        public NetNamedPipeBinding()
            : base()
        {
            Initialize();
        }

        public NetNamedPipeBinding(NetNamedPipeSecurityMode securityMode)
            : this()
        {
            this._security.Mode = securityMode;
        }
        
        NetNamedPipeBinding(NetNamedPipeSecurity security)
            : this()
        {
            this._security = security;
        }

        [DefaultValue(TransactionFlowDefaults.Transactions)]
        public bool TransactionFlow
        {
            get { return _context.Transactions; }
            set { _context.Transactions = value; }
        }

        public TransactionProtocol TransactionProtocol
        {
            get { return this._context.TransactionProtocol; }
            set { this._context.TransactionProtocol = value; }
        }

        [DefaultValue(ConnectionOrientedTransportDefaults.TransferMode)]
        public TransferMode TransferMode
        {
            get { return _namedPipe.TransferMode; }
            set { _namedPipe.TransferMode = value; }
        }

        [DefaultValue(ConnectionOrientedTransportDefaults.HostNameComparisonMode)]
        public HostNameComparisonMode HostNameComparisonMode
        {
            get { return _namedPipe.HostNameComparisonMode; }
            set { _namedPipe.HostNameComparisonMode = value; }
        }

        [DefaultValue(TransportDefaults.MaxBufferPoolSize)]
        public long MaxBufferPoolSize
        {
            get { return _namedPipe.MaxBufferPoolSize; }
            set
            {
                _namedPipe.MaxBufferPoolSize = value;
            }
        }

        [DefaultValue(TransportDefaults.MaxBufferSize)]
        public int MaxBufferSize
        {
            get { return _namedPipe.MaxBufferSize; }
            set { _namedPipe.MaxBufferSize = value; }
        }

        public int MaxConnections
        {
            get { return _namedPipe.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint; }
            set { _namedPipe.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint = value; }
        }

        [DefaultValue(TransportDefaults.MaxReceivedMessageSize)]
        public long MaxReceivedMessageSize
        {
            get { return _namedPipe.MaxReceivedMessageSize; }
            set { _namedPipe.MaxReceivedMessageSize = value; }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get { return _encoding.ReaderQuotas; }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                value.CopyTo(_encoding.ReaderQuotas);
            }
        }

        bool IBindingRuntimePreferences.ReceiveSynchronously
        {
            get { return false; }
        }

        public override string Scheme { get { return _namedPipe.Scheme; } }

        public EnvelopeVersion EnvelopeVersion
        {
            get { return EnvelopeVersion.Soap12; }
        }

        public NetNamedPipeSecurity Security
        {
            get { return this._security; }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                this._security = value;
            }
        }

        static TransactionFlowBindingElement GetDefaultTransactionFlowBindingElement()
        {
            return new TransactionFlowBindingElement(false);
        }

        void Initialize()
        {
            _namedPipe = new NamedPipeTransportBindingElement();
            _encoding = new BinaryMessageEncodingBindingElement();
            _context = GetDefaultTransactionFlowBindingElement();
        }

        void InitializeFrom(NamedPipeTransportBindingElement namedPipe, BinaryMessageEncodingBindingElement encoding, TransactionFlowBindingElement context)
        {
            Initialize();
            this.HostNameComparisonMode = namedPipe.HostNameComparisonMode;
            this.MaxBufferPoolSize = namedPipe.MaxBufferPoolSize;
            this.MaxBufferSize = namedPipe.MaxBufferSize;
            
            this.MaxReceivedMessageSize = namedPipe.MaxReceivedMessageSize;
            this.TransferMode = namedPipe.TransferMode;

            this.ReaderQuotas = encoding.ReaderQuotas;

            this.TransactionFlow = context.Transactions;
            this.TransactionProtocol = context.TransactionProtocol;
        }

        // check that properties of the HttpTransportBindingElement and 
        // MessageEncodingBindingElement not exposed as properties on BasicHttpBinding 
        // match default values of the binding elements
        bool IsBindingElementsMatch(NamedPipeTransportBindingElement namedPipe, BinaryMessageEncodingBindingElement encoding, TransactionFlowBindingElement context)
        {
            if (!this._namedPipe.IsMatch(namedPipe))
                return false;
            if (!this._encoding.IsMatch(encoding))
                return false;
            if (!this._context.IsMatch(context))
                return false;
            return true;
        }

        public override BindingElementCollection CreateBindingElements()
        {   // return collection of BindingElements
            BindingElementCollection bindingElements = new BindingElementCollection();
            // order of BindingElements is important
            // add context
            bindingElements.Add(_context);
            // add encoding
            bindingElements.Add(_encoding);
            // add transport security
            WindowsStreamSecurityBindingElement transportSecurity = CreateTransportSecurity();
            if (transportSecurity != null)
            {
                bindingElements.Add(transportSecurity);
            }
            // add transport (named pipes)
            bindingElements.Add(this._namedPipe);

            return bindingElements.Clone();
        }

        internal static bool TryCreate(BindingElementCollection elements, out Binding binding)
        {
            binding = null;
            if (elements.Count > 4)
                return false;

            TransactionFlowBindingElement context = null;
            BinaryMessageEncodingBindingElement encoding = null;
            WindowsStreamSecurityBindingElement security = null;
            NamedPipeTransportBindingElement namedPipe = null;

            foreach (BindingElement element in elements)
            {
                if (element is TransactionFlowBindingElement)
                    context = element as TransactionFlowBindingElement;
                else if (element is BinaryMessageEncodingBindingElement)
                    encoding = element as BinaryMessageEncodingBindingElement;
                else if (element is WindowsStreamSecurityBindingElement)
                    security = element as WindowsStreamSecurityBindingElement;
                else if (element is NamedPipeTransportBindingElement)
                    namedPipe = element as NamedPipeTransportBindingElement;
                else
                    return false;
            }

            if (namedPipe == null)
                return false;

            if (encoding == null)
                return false;

            if (context == null)
                context = GetDefaultTransactionFlowBindingElement();

            NetNamedPipeSecurity pipeSecurity;
            if (!TryCreateSecurity(security, out pipeSecurity))
                return false;

            NetNamedPipeBinding netNamedPipeBinding = new NetNamedPipeBinding(pipeSecurity);
            netNamedPipeBinding.InitializeFrom(namedPipe, encoding, context);

            if (!netNamedPipeBinding.IsBindingElementsMatch(namedPipe, encoding, context))
                return false;

            binding = netNamedPipeBinding;
            return true;
        }

        WindowsStreamSecurityBindingElement CreateTransportSecurity()
        {
            if (this._security.Mode == NetNamedPipeSecurityMode.Transport)
            {
                return this._security.CreateTransportSecurity();
            }
            else
            {
                return null;
            }
        }

        static bool TryCreateSecurity(WindowsStreamSecurityBindingElement wssbe, out NetNamedPipeSecurity security)
        {
            NetNamedPipeSecurityMode mode = wssbe == null ? NetNamedPipeSecurityMode.None : NetNamedPipeSecurityMode.Transport;
            return NetNamedPipeSecurity.TryCreate(wssbe, mode, out security);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeReaderQuotas()
        {
            return (!EncoderDefaults.IsDefaultReaderQuotas(this.ReaderQuotas));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeSecurity()
        {
            if (this._security.Mode != NetNamedPipeSecurity.DefaultMode)
            {
                return true;
            }
            if (this._security.Transport.ProtectionLevel != NamedPipeTransportSecurity.DefaultProtectionLevel)
            {
                return true;
            }
            return false;
        }
    }
}
