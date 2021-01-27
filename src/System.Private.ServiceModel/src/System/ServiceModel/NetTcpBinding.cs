// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ServiceModel.Channels;
using System.Xml;

namespace System.ServiceModel
{
    public class NetTcpBinding : Binding
    {
        private OptionalReliableSession reliableSession;
        // private BindingElements
        private TcpTransportBindingElement _transport;
        private BinaryMessageEncodingBindingElement _encoding;
        private ReliableSessionBindingElement session;
        private NetTcpSecurity _security = new NetTcpSecurity();

        public NetTcpBinding() { Initialize(); }
        public NetTcpBinding(SecurityMode securityMode)
            : this()
        {
            _security.Mode = securityMode;
        }

        public NetTcpBinding(SecurityMode securityMode, bool reliableSessionEnabled) : this(securityMode)
        {
            ReliableSession.Enabled = reliableSessionEnabled;
        }

        public NetTcpBinding(string configurationName)
            : this()
        {
            if (!string.IsNullOrEmpty(configurationName))
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
        }

        [DefaultValue(ConnectionOrientedTransportDefaults.TransferMode)]
        public TransferMode TransferMode
        {
            get { return _transport.TransferMode; }
            set { _transport.TransferMode = value; }
        }

        [DefaultValue(TransportDefaults.MaxBufferPoolSize)]
        public long MaxBufferPoolSize { get; set; }

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
                return reliableSession;
            }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(value)));
                }
                reliableSession.CopySettings(value);
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

            // NetNative and CoreCLR initialize to what TransportBindingElement does in the desktop
            // This property is not available in shipped contracts
            MaxBufferPoolSize = TransportDefaults.MaxBufferPoolSize;
            session = new ReliableSessionBindingElement();
            this.reliableSession = new OptionalReliableSession(session);
        }

        private void CheckSettings()
        {
        }

        public override BindingElementCollection CreateBindingElements()
        {
            CheckSettings();

            // return collection of BindingElements
            BindingElementCollection bindingElements = new BindingElementCollection();
            // order of BindingElements is important
            // add session
            if (reliableSession.Enabled)
            {
                bindingElements.Add(session);
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
    }
}
