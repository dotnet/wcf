// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
using System.ServiceModel.Channels;
using System.Xml;

namespace System.ServiceModel
{
    public class NetMsmqBinding : MsmqBindingBase
    {
        // Default XmlDictionaryReaderQuotas (mirrors System.Xml's EncoderDefaults).
        private const int DefaultMaxArrayLength = 16384;
        private const int DefaultMaxBytesPerRead = 4096;
        private const int DefaultMaxDepth = 32;
        private const int DefaultMaxNameTableCharCount = 16384;
        private const int DefaultMaxStringContentLength = 8192;

        private BinaryMessageEncodingBindingElement _encoding;
        public NetMsmqBinding()
        {
            Initialize();
            Security = new NetMsmqSecurity();
        }

        public NetMsmqBinding(NetMsmqSecurityMode securityMode)
        {
            if (!NetMsmqSecurityModeHelper.IsDefined(securityMode))
            {
                throw new InvalidEnumArgumentException(nameof(securityMode), (int)securityMode, typeof(NetMsmqSecurityMode));
            }
            Initialize();
            Security = new NetMsmqSecurity(securityMode);
        }

        [DefaultValue(MsmqDefaults.QueueTransferProtocol)]
        public QueueTransferProtocol QueueTransferProtocol
        {
            get { return ((MsmqTransportBindingElement)_transport).QueueTransferProtocol; }
            set { ((MsmqTransportBindingElement)_transport).QueueTransferProtocol = value; }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get { return _encoding.ReaderQuotas; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                value.CopyTo(_encoding.ReaderQuotas);
            }
        }

        public NetMsmqSecurity Security
        {
            get;
            set;
        }

        public EnvelopeVersion EnvelopeVersion => EnvelopeVersion.Soap12;

        public long MaxBufferPoolSize
        {
            get { return _transport.MaxBufferPoolSize; }
            set { _transport.MaxBufferPoolSize = value; }
        }

        internal int MaxPoolSize
        {
            get { return ((MsmqTransportBindingElement)_transport).MaxPoolSize; }
            set { ((MsmqTransportBindingElement)_transport).MaxPoolSize = value; }
        }

        [DefaultValue(MsmqDefaults.UseActiveDirectory)]
        public bool UseActiveDirectory
        {
            get { return ((MsmqTransportBindingElement)_transport).UseActiveDirectory; }
            set { ((MsmqTransportBindingElement)_transport).UseActiveDirectory = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeReaderQuotas()
        {
            return ReaderQuotas.MaxArrayLength != DefaultMaxArrayLength
                || ReaderQuotas.MaxBytesPerRead != DefaultMaxBytesPerRead
                || ReaderQuotas.MaxDepth != DefaultMaxDepth
                || ReaderQuotas.MaxNameTableCharCount != DefaultMaxNameTableCharCount
                || ReaderQuotas.MaxStringContentLength != DefaultMaxStringContentLength;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeSecurity()
        {
            if (Security.Mode != NetMsmqSecurity.DefaultMode)
            {
                return true;
            }
            if (Security.Transport.MsmqAuthenticationMode != MsmqDefaults.MsmqAuthenticationMode
                || Security.Transport.MsmqEncryptionAlgorithm != MsmqDefaults.MsmqEncryptionAlgorithm
                || Security.Transport.MsmqSecureHashAlgorithm != MsmqDefaults.MsmqSecureHashAlgorithm
                || Security.Transport.MsmqProtectionLevel != MsmqDefaults.MsmqProtectionLevel)
            {
                return true;
            }
            if (Security.Message.AlgorithmSuite != MsmqDefaults.MessageSecurityAlgorithmSuite
                || Security.Message.ClientCredentialType != MsmqDefaults.DefaultClientCredentialType)
            {
                return true;
            }
            return false;
        }

        private void Initialize()
        {
            _transport = new MsmqTransportBindingElement();
            _encoding = new BinaryMessageEncodingBindingElement();
        }

        public override BindingElementCollection CreateBindingElements()
        {
            // Order matters: encoding -> _transport.
            //
            // Message-level security (NetMsmqSecurityMode.Message / Both) needs a
            // SecurityBindingElement and the WS-Security message-protection stack,
            // which this client-side port does not carry. The mode is rejected in
            // GetTransport rather than dropped: silently emitting an unprotected
            // binding for a caller who asked for message security would put
            // plaintext on the wire under a configuration that claims otherwise.
            BindingElementCollection bindingElements = new BindingElementCollection();
            bindingElements.Add(_encoding);
            bindingElements.Add(GetTransport());
            return bindingElements.Clone();
        }

        private MsmqBindingElementBase GetTransport()
        {
            if (Security.Mode == NetMsmqSecurityMode.Message || Security.Mode == NetMsmqSecurityMode.Both)
            {
                throw new NotSupportedException(SR.Format(SR.MsmqSecurityModeNotSupported, Security.Mode));
            }
            Security.ConfigureTransportSecurity(_transport);
            return _transport;
        }
    }
}
