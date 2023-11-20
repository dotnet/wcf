// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ServiceModel.Channels;
using System.Xml;

namespace System.ServiceModel
{
    public class NetTcpBinding : Binding
    {
        private OptionalReliableSession _reliableSession;
        // private BindingElements
        private TcpTransportBindingElement _transport;
        private BinaryMessageEncodingBindingElement _encoding;
        private ReliableSessionBindingElement _session;
        private NetTcpSecurity _security = new NetTcpSecurity();

        public NetTcpBinding()
        {
            Initialize();
        }

        public NetTcpBinding(SecurityMode securityMode)
            : this()
        {
            _security.Mode = securityMode;
        }

        public NetTcpBinding(SecurityMode securityMode, bool reliableSessionEnabled) : this(securityMode)
        {
            ReliableSession.Enabled = reliableSessionEnabled;
        }

        [DefaultValue(ConnectionOrientedTransportDefaults.TransferMode)]
        public TransferMode TransferMode
        {
            get { return _transport.TransferMode; }
            set { _transport.TransferMode = value; }
        }

        [DefaultValue(TcpTransportDefaults.MaxBufferPoolSize)]
        public long MaxBufferPoolSize
        {
            get { return _transport.MaxBufferPoolSize; }
            set { _transport.MaxBufferPoolSize = value; }
        }

        [DefaultValue(TcpTransportDefaults.MaxBufferSize)]
        public int MaxBufferSize
        {
            get { return _transport.MaxBufferSize; }
            set { _transport.MaxBufferSize = value; }
        }

        public int MaxConnections
        {
            get { return _transport.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint; }
            set { _transport.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint = value; }
        }

        [DefaultValue(TcpTransportDefaults.MaxReceivedMessageSize)]
        public long MaxReceivedMessageSize
        {
            get { return _transport.MaxReceivedMessageSize; }
            set { _transport.MaxReceivedMessageSize = value; }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get { return _encoding.ReaderQuotas; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }

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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(value)));
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
                _security = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
            }
        }

        private void Initialize()
        {
            _transport = new TcpTransportBindingElement();
            _encoding = new BinaryMessageEncodingBindingElement();
            _session = new ReliableSessionBindingElement();
            _reliableSession = new OptionalReliableSession(_session);
        }

        public override BindingElementCollection CreateBindingElements()
        {
            // return collection of BindingElements
            BindingElementCollection bindingElements = new BindingElementCollection();
            // order of BindingElements is important
            // add session
            if (_reliableSession.Enabled)
            {
                // This check was originall in ReliableSessionBindingElement.VerifyTransportMode but
                // the transport base type isn't visible to primitives any more so the check has
                // been move here.
                if (_transport.TransferMode != TransferMode.Buffered)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new InvalidOperationException(SR.Format(SR.TransferModeNotSupported,
                        _transport.TransferMode, typeof(ReliableSessionBindingElement).Name)));
                }
                bindingElements.Add(_session);
            }

            // add security (*optional)
            SecurityBindingElement wsSecurity = CreateMessageSecurity();
            if (wsSecurity != null)
            {
                bindingElements.Add(wsSecurity);
            }
            // add encoding
            bindingElements.Add(_encoding);
            // add transport security
            BindingElement transportSecurity = CreateTransportSecurity();
            if (transportSecurity != null)
            {
                bindingElements.Add(transportSecurity);
            }
            _transport.ExtendedProtectionPolicy = _security.Transport.ExtendedProtectionPolicy;
            // add transport (tcp)
            bindingElements.Add(_transport);

            return bindingElements.Clone();
        }

        private BindingElement CreateTransportSecurity()
        {
            return _security.CreateTransportSecurity();
        }

        private SecurityBindingElement CreateMessageSecurity()
        {
            if (_security.Mode == SecurityMode.Message)
            {
                throw new PlatformNotSupportedException(nameof(SecurityMode.Message));
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
    }
}
