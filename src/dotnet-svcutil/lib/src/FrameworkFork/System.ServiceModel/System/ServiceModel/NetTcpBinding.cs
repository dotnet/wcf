// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.ServiceModel.Channels;
using Microsoft.Xml;

namespace System.ServiceModel
{
    public class NetTcpBinding : Binding
    {
        private OptionalReliableSession _reliableSession;
        // private BindingElements
        private TcpTransportBindingElement _transport;
        private BinaryMessageEncodingBindingElement _encoding;
        private TransactionFlowBindingElement _context;
        private ReliableSessionBindingElement _session;
        private long _maxBufferPoolSize;
        private NetTcpSecurity _security = new NetTcpSecurity();

        public NetTcpBinding() { Initialize(); }
        public NetTcpBinding(SecurityMode securityMode)
            : this()
        {
            _security.Mode = securityMode;
        }

        public NetTcpBinding(SecurityMode securityMode, bool reliableSessionEnabled)
            : this(securityMode)
        {
            this.ReliableSession.Enabled = reliableSessionEnabled;
        }


        private NetTcpBinding(TcpTransportBindingElement transport, BinaryMessageEncodingBindingElement encoding, TransactionFlowBindingElement context, ReliableSessionBindingElement session, NetTcpSecurity security)
            : this()
        {
            _security = security;
            this.ReliableSession.Enabled = session != null;
            InitializeFrom(transport, encoding, context, session);
        }


        private NetTcpBinding(TcpTransportBindingElement transport,
                      BinaryMessageEncodingBindingElement encoding,
                      NetTcpSecurity security)
            : this()
        {
            _security = security;
        }

        [DefaultValue(ConnectionOrientedTransportDefaults.TransferMode)]
        public TransferMode TransferMode
        {
            get { return _transport.TransferMode; }
            set { _transport.TransferMode = value; }
        }

        [DefaultValue(ConnectionOrientedTransportDefaults.HostNameComparisonMode)]
        public HostNameComparisonMode HostNameComparisonMode
        {
            get { return _transport.HostNameComparisonMode; }
            set { _transport.HostNameComparisonMode = value; }
        }

        [DefaultValue(TransportDefaults.MaxBufferPoolSize)]
        public long MaxBufferPoolSize
        {
            get { return _maxBufferPoolSize; }
            set
            {
                _maxBufferPoolSize = value;
            }
        }

        [DefaultValue(TransportDefaults.MaxBufferSize)]
        public int MaxBufferSize
        {
            get { return _transport.MaxBufferSize; }
            set { _transport.MaxBufferSize = value; }
        }

        [DefaultValue(TransportDefaults.MaxReceivedMessageSize)]
        public long MaxReceivedMessageSize
        {
            get { return _transport.MaxReceivedMessageSize; }
            set { _transport.MaxReceivedMessageSize = value; }
        }

        [DefaultValue(TcpTransportDefaults.PortSharingEnabled)]
        public bool PortSharingEnabled
        {
            get { return _transport.PortSharingEnabled; }
            set { _transport.PortSharingEnabled = value; }
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

        public OptionalReliableSession ReliableSession
        {
            get
            {
                return _reliableSession;
            }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
                }
                _reliableSession.CopySettings(value);
            }
        }

        public override string Scheme { get { return _transport.Scheme; } }

        public EnvelopeVersion EnvelopeVersion
        {
            get { return EnvelopeVersion.Soap12; }
        }

        public NetTcpSecurity Security
        {
            get { return _security; }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                _security = value;
            }
        }

        public bool TransactionFlow
        {
            get { return _context.Transactions; }
            set { _context.Transactions = value; }
        }

        public TransactionProtocol TransactionProtocol
        {
            get { return _context.TransactionProtocol; }
            set { _context.TransactionProtocol = value; }
        }

        private static TransactionFlowBindingElement GetDefaultTransactionFlowBindingElement()
        {
            return new TransactionFlowBindingElement(NetTcpDefaults.TransactionsEnabled);
        }

        private void Initialize()
        {
            _transport = new TcpTransportBindingElement();
            _encoding = new BinaryMessageEncodingBindingElement();
            _context = new TransactionFlowBindingElement(NetTcpDefaults.TransactionsEnabled);
            _session = new ReliableSessionBindingElement();
            _reliableSession = new OptionalReliableSession(_session);

            // NetNative and CoreCLR initialize to what TransportBindingElement does in the desktop
            // This property is not available in shipped contracts
            _maxBufferPoolSize = TransportDefaults.MaxBufferPoolSize;
        }

        private void InitializeFrom(TcpTransportBindingElement transport, BinaryMessageEncodingBindingElement encoding, TransactionFlowBindingElement context, ReliableSessionBindingElement session)
        {
            // TODO: Fx.Assert(transport != null, "Invalid (null) transport value.");
            // TODO: Fx.Assert(encoding != null, "Invalid (null) encoding value.");
            // TODO: Fx.Assert(context != null, "Invalid (null) context value.");
            // TODO: Fx.Assert(security != null, "Invalid (null) security value.");

            // transport
            this.HostNameComparisonMode = transport.HostNameComparisonMode;
            this.MaxBufferPoolSize = transport.MaxBufferPoolSize;
            this.MaxBufferSize = transport.MaxBufferSize;

            this.MaxReceivedMessageSize = transport.MaxReceivedMessageSize;
            this.PortSharingEnabled = transport.PortSharingEnabled;
            this.TransferMode = transport.TransferMode;

            // encoding
            this.ReaderQuotas = encoding.ReaderQuotas;

            // context
            this.TransactionFlow = context.Transactions;
            this.TransactionProtocol = context.TransactionProtocol;

            //session
            if (session != null)
            {
                // only set properties that have standard binding manifestations
                _session.InactivityTimeout = session.InactivityTimeout;
                _session.Ordered = session.Ordered;
            }
        }

        // check that properties of the HttpTransportBindingElement and 
        // MessageEncodingBindingElement not exposed as properties on BasicHttpBinding 
        // match default values of the binding elements
        private bool IsBindingElementsMatch(TcpTransportBindingElement transport, BinaryMessageEncodingBindingElement encoding)
        {
            if (!_transport.IsMatch(transport))
                return false;

            if (!_encoding.IsMatch(encoding))
                return false;

            if (!_context.IsMatch(_context))
                return false;

            if (_reliableSession.Enabled)
            {
                if (!_session.IsMatch(_session))
                    return false;
            }

            return true;
        }

        // check that properties of the HttpTransportBindingElement and 
        // MessageEncodingBindingElement not exposed as properties on BasicHttpBinding 
        // match default values of the binding elements
        private bool IsBindingElementsMatch(TcpTransportBindingElement transport, BinaryMessageEncodingBindingElement encoding, TransactionFlowBindingElement context, ReliableSessionBindingElement session)
        {
            if (!_transport.IsMatch(transport))
                return false;
            if (!_encoding.IsMatch(encoding))
                return false;
            if (!_context.IsMatch(context))
                return false;
            if (_reliableSession.Enabled)
            {
                if (!_session.IsMatch(session))
                    return false;
            }
            else if (session != null)
                return false;

            return true;
        }

        private void CheckSettings()
        {
#if FEATURE_NETNATIVE // In .NET Native, some settings for the binding security are not supported; this check is not necessary for CoreCLR
                      
            NetTcpSecurity security = this.Security;
            if (security == null)
            {
                return;
            }

            SecurityMode mode = security.Mode;
            if (mode == SecurityMode.None)
            {
                return;
            }
            else if (mode == SecurityMode.Message)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.Format(SR.UnsupportedSecuritySetting, "Mode", mode)));
            }

            // Message.ClientCredentialType = Certificate, IssuedToken or Windows are not supported.
            if (mode == SecurityMode.TransportWithMessageCredential)
            {
                MessageSecurityOverTcp message = security.Message;
                if (message != null)
                {
                    MessageCredentialType mct = message.ClientCredentialType;
                    if ((mct == MessageCredentialType.Certificate) || (mct == MessageCredentialType.IssuedToken) || (mct == MessageCredentialType.Windows))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.Format(SR.UnsupportedSecuritySetting, "Message.ClientCredentialType", mct)));
                    }
                }
            }

            // Transport.ClientCredentialType = Certificate is not supported.
            Contract.Assert((mode == SecurityMode.Transport) || (mode == SecurityMode.TransportWithMessageCredential), "Unexpected SecurityMode value: " + mode);
            TcpTransportSecurity transport = security.Transport;
            if ((transport != null) && (transport.ClientCredentialType == TcpClientCredentialType.Certificate))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.Format(SR.UnsupportedSecuritySetting, "Transport.ClientCredentialType", transport.ClientCredentialType)));
            }
#endif // FEATURE_NETNATIVE
        }

        public override BindingElementCollection CreateBindingElements()
        {
            this.CheckSettings();

            // return collection of BindingElements
            BindingElementCollection bindingElements = new BindingElementCollection();
            // order of BindingElements is important
            bindingElements.Add(_context);
            // add session
            if (_reliableSession.Enabled)
                bindingElements.Add(_session);
            // add security (*optional)
            SecurityBindingElement wsSecurity = CreateMessageSecurity();
            if (wsSecurity != null)
                bindingElements.Add(wsSecurity);
            // add encoding
            bindingElements.Add(_encoding);
            // add transport security
            BindingElement transportSecurity = CreateTransportSecurity();
            if (transportSecurity != null)
            {
                bindingElements.Add(transportSecurity);
            }

            // add transport (tcp)
            bindingElements.Add(_transport);

            return bindingElements.Clone();
        }

        internal static bool TryCreate(BindingElementCollection elements, out Binding binding)
        {
            binding = null;
            if (elements.Count > 6)
                return false;

            // collect all binding elements
            TcpTransportBindingElement transport = null;
            BinaryMessageEncodingBindingElement encoding = null;
            TransactionFlowBindingElement context = null;
            ReliableSessionBindingElement session = null;
            SecurityBindingElement wsSecurity = null;
            BindingElement transportSecurity = null;

            foreach (BindingElement element in elements)
            {
                if (element is SecurityBindingElement)
                    wsSecurity = element as SecurityBindingElement;
                else if (element is TransportBindingElement)
                    transport = element as TcpTransportBindingElement;
                else if (element is MessageEncodingBindingElement)
                    encoding = element as BinaryMessageEncodingBindingElement;
                else if (element is TransactionFlowBindingElement)
                    context = element as TransactionFlowBindingElement;
                else if (element is ReliableSessionBindingElement)
                    session = element as ReliableSessionBindingElement;
                else
                {
                    if (transportSecurity != null)
                        return false;
                    transportSecurity = element;
                }
            }

            if (transport == null)
                return false;
            if (encoding == null)
                return false;
            if (context == null)
                context = GetDefaultTransactionFlowBindingElement();

            TcpTransportSecurity tcpTransportSecurity = new TcpTransportSecurity();
            UnifiedSecurityMode mode = GetModeFromTransportSecurity(transportSecurity);

            NetTcpSecurity security;
            if (!TryCreateSecurity(wsSecurity, mode, session != null, transportSecurity, tcpTransportSecurity, out security))
                return false;

            if (!SetTransportSecurity(transportSecurity, security.Mode, tcpTransportSecurity))
                return false;

            NetTcpBinding netTcpBinding = new NetTcpBinding(transport, encoding, context, session, security);
            if (!netTcpBinding.IsBindingElementsMatch(transport, encoding, context, session))
                return false;

            binding = netTcpBinding;
            return true;
        }


        private BindingElement CreateTransportSecurity()
        {
            return _security.CreateTransportSecurity();
        }

        private static UnifiedSecurityMode GetModeFromTransportSecurity(BindingElement transport)
        {
            return NetTcpSecurity.GetModeFromTransportSecurity(transport);
        }

        private static bool SetTransportSecurity(BindingElement transport, SecurityMode mode, TcpTransportSecurity transportSecurity)
        {
            return NetTcpSecurity.SetTransportSecurity(transport, mode, transportSecurity);
        }

        private SecurityBindingElement CreateMessageSecurity()
        {
            if (_security.Mode == SecurityMode.Message)
            {
                throw ExceptionHelper.PlatformNotSupported(nameof(SecurityMode.Message));
            }
            if (_security.Mode == SecurityMode.TransportWithMessageCredential)
            {
                return _security.CreateMessageSecurity(ReliableSession.Enabled);
            }
            else
            {
                return null;
            }
        }

        private static bool TryCreateSecurity(SecurityBindingElement sbe, UnifiedSecurityMode mode, bool isReliableSession, BindingElement transportSecurity, TcpTransportSecurity tcpTransportSecurity, out NetTcpSecurity security)
        {
            if (sbe != null)
                mode &= UnifiedSecurityMode.Message | UnifiedSecurityMode.TransportWithMessageCredential;
            else
                mode &= ~(UnifiedSecurityMode.Message | UnifiedSecurityMode.TransportWithMessageCredential);

            SecurityMode securityMode = SecurityModeHelper.ToSecurityMode(mode);
            // TODO: Fx.Assert(SecurityModeHelper.IsDefined(securityMode), string.Format("Invalid SecurityMode value: {0}.", securityMode.ToString()));

            if (NetTcpSecurity.TryCreate(sbe, securityMode, isReliableSession, transportSecurity, tcpTransportSecurity, out security))
                return true;

            return false;
        }
    }
}
