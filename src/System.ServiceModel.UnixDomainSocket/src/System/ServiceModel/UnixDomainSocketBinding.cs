// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ServiceModel.Channels;
using System.Xml;

namespace System.ServiceModel
{
    public class UnixDomainSocketBinding : Binding
    {
        private UnixDomainSocketTransportBindingElement _transport;
        private BinaryMessageEncodingBindingElement _encoding;
        private UnixDomainSocketSecurity _security = new UnixDomainSocketSecurity();

        public UnixDomainSocketBinding()
        {
            Initialize();
        }

        public UnixDomainSocketBinding(UnixDomainSocketSecurityMode securityMode)
            : this()
        {
            _security.Mode = securityMode;
        }

        [DefaultValue(ConnectionOrientedTransportDefaults.TransferMode)]
        public TransferMode TransferMode
        {
            get { return _transport.TransferMode; }
            set { _transport.TransferMode = value; }
        }

        [DefaultValue(UnixDomainSocketTransportDefaults.MaxBufferPoolSize)]
        public long MaxBufferPoolSize
        {
            get { return _transport.MaxBufferPoolSize; }
            set { _transport.MaxBufferPoolSize = value; }
        }

        [DefaultValue(UnixDomainSocketTransportDefaults.MaxBufferSize)]
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

        [DefaultValue(UnixDomainSocketTransportDefaults.MaxReceivedMessageSize)]
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

        public override string Scheme { get { return _transport.Scheme; } }

        public EnvelopeVersion EnvelopeVersion
        {
            get { return EnvelopeVersion.Soap12; }
        }

        public UnixDomainSocketSecurity Security
        {
            get { return _security; }
            set
            {
                _security = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
            }
        }

        private void Initialize()
        {
            _transport = new UnixDomainSocketTransportBindingElement();
            _encoding = new BinaryMessageEncodingBindingElement();
        }

        public override BindingElementCollection CreateBindingElements()
        {
            // return collection of BindingElements
            BindingElementCollection bindingElements = new BindingElementCollection();
            BindingElement transportSecurity = CreateTransportSecurity();
            if (transportSecurity != null)
            {
                bindingElements.Add(transportSecurity);
            }
            _transport.ExtendedProtectionPolicy = _security.Transport.ExtendedProtectionPolicy;
            bindingElements.Add(_transport);

            return bindingElements.Clone();
        }

        private BindingElement CreateTransportSecurity()
        {
            return _security.CreateTransportSecurity();
        }
        
    }
}
