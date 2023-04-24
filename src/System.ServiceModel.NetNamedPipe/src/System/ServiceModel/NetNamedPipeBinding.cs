// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.ServiceModel.Channels;
using System.Xml;

namespace System.ServiceModel
{
    [SupportedOSPlatform("windows")]
    public class NetNamedPipeBinding : Binding
    {
        // private BindingElements
        private BinaryMessageEncodingBindingElement _encoding;
        private NamedPipeTransportBindingElement _namedPipe;
        private NetNamedPipeSecurity _security = new NetNamedPipeSecurity();

        public NetNamedPipeBinding() : base()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw ExceptionHelper.PlatformNotSupported(SR.Format(SR.PlatformNotSupported_NetNamedPipe));
            }

            Initialize();
        }

        public NetNamedPipeBinding(NetNamedPipeSecurityMode securityMode) : this()
        {
            _security.Mode = securityMode;
        }

        [DefaultValue(ConnectionOrientedTransportDefaults.TransferMode)]
        public TransferMode TransferMode
        {
            get { return _namedPipe.TransferMode; }
            set { _namedPipe.TransferMode = value; }
        }

        [DefaultValue(NPTransportDefaults.MaxBufferPoolSize)]
        public long MaxBufferPoolSize
        {
            get { return _namedPipe.MaxBufferPoolSize; }
            set { _namedPipe.MaxBufferPoolSize = value; }
        }

        [DefaultValue(NPTransportDefaults.MaxBufferSize)]
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

        [DefaultValue(NPTransportDefaults.MaxReceivedMessageSize)]
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                value.CopyTo(_encoding.ReaderQuotas);
            }
        }

        public override string Scheme => _namedPipe.Scheme;

        public EnvelopeVersion EnvelopeVersion => EnvelopeVersion.Soap12;

        public NetNamedPipeSecurity Security
        {
            get { return _security; }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                _security = value;
            }
        }

        private void Initialize()
        {
            _namedPipe = new NamedPipeTransportBindingElement();
            _encoding = new BinaryMessageEncodingBindingElement();
        }

        public override BindingElementCollection CreateBindingElements()
        {   // return collection of BindingElements
            BindingElementCollection bindingElements = new BindingElementCollection();
            // order of BindingElements is important
            // add encoding
            bindingElements.Add(_encoding);
            // add transport security
            WindowsStreamSecurityBindingElement transportSecurity = CreateTransportSecurity();
            if (transportSecurity != null)
            {
                bindingElements.Add(transportSecurity);
            }
            // add transport (named pipes)
            bindingElements.Add(_namedPipe);

            return bindingElements.Clone();
        }

        private WindowsStreamSecurityBindingElement CreateTransportSecurity()
        {
            if (_security.Mode == NetNamedPipeSecurityMode.Transport)
            {
                return _security.CreateTransportSecurity();
            }
            else
            {
                return null;
            }
        }
    }
}
