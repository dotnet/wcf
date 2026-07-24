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
        private BinaryMessageEncodingBindingElement _encoding;
        private NetMsmqSecurity _security;

        public NetMsmqBinding()
        {
            Initialize();
            _security = new NetMsmqSecurity();
        }

        public NetMsmqBinding(NetMsmqSecurityMode securityMode)
        {
            if (!NetMsmqSecurityModeHelper.IsDefined(securityMode))
            {
                throw new InvalidEnumArgumentException(nameof(securityMode), (int)securityMode, typeof(NetMsmqSecurityMode));
            }
            Initialize();
            _security = new NetMsmqSecurity(securityMode);
        }

        [DefaultValue(MsmqDefaults.QueueTransferProtocol)]
        public QueueTransferProtocol QueueTransferProtocol
        {
            get { return ((MsmqTransportBindingElement)transport).QueueTransferProtocol; }
            set { ((MsmqTransportBindingElement)transport).QueueTransferProtocol = value; }
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
            get { return _security; }
            set { _security = value; }
        }

        public EnvelopeVersion EnvelopeVersion => EnvelopeVersion.Soap12;

        public long MaxBufferPoolSize
        {
            get { return transport.MaxBufferPoolSize; }
            set { transport.MaxBufferPoolSize = value; }
        }

        internal int MaxPoolSize
        {
            get { return ((MsmqTransportBindingElement)transport).MaxPoolSize; }
            set { ((MsmqTransportBindingElement)transport).MaxPoolSize = value; }
        }

        [DefaultValue(MsmqDefaults.UseActiveDirectory)]
        public bool UseActiveDirectory
        {
            get { return ((MsmqTransportBindingElement)transport).UseActiveDirectory; }
            set { ((MsmqTransportBindingElement)transport).UseActiveDirectory = value; }
        }

        // Default XmlDictionaryReaderQuotas (mirrors System.Xml's EncoderDefaults).
        private const int DefaultMaxArrayLength = 16384;
        private const int DefaultMaxBytesPerRead = 4096;
        private const int DefaultMaxDepth = 32;
        private const int DefaultMaxNameTableCharCount = 16384;
        private const int DefaultMaxStringContentLength = 8192;

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
            if (_security.Mode != NetMsmqSecurity.DefaultMode)
            {
                return true;
            }
            if (_security.Transport.MsmqAuthenticationMode != MsmqDefaults.MsmqAuthenticationMode
                || _security.Transport.MsmqEncryptionAlgorithm != MsmqDefaults.MsmqEncryptionAlgorithm
                || _security.Transport.MsmqSecureHashAlgorithm != MsmqDefaults.MsmqSecureHashAlgorithm
                || _security.Transport.MsmqProtectionLevel != MsmqDefaults.MsmqProtectionLevel)
            {
                return true;
            }
            if (_security.Message.AlgorithmSuite != MsmqDefaults.MessageSecurityAlgorithmSuite
                || _security.Message.ClientCredentialType != MsmqDefaults.DefaultClientCredentialType)
            {
                return true;
            }
            return false;
        }

        private void Initialize()
        {
            transport = new MsmqTransportBindingElement();
            _encoding = new BinaryMessageEncodingBindingElement();
        }

        public override BindingElementCollection CreateBindingElements()
        {
            var bindingElements = new BindingElementCollection();
            // Order matters: security (if any) -> encoding -> transport.
            // Message security is not yet supported on the client port; see
            // MessageSecurityOverMsmq for context.
            bindingElements.Add(_encoding);
            bindingElements.Add(GetTransport());
            return bindingElements.Clone();
        }

        private MsmqBindingElementBase GetTransport()
        {
            _security.ConfigureTransportSecurity(transport);
            return transport;
        }
    }
}
