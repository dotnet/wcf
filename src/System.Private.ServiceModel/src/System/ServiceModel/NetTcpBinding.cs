// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.ServiceModel.Channels;
using System.Xml;

namespace System.ServiceModel
{
    public class NetTcpBinding : Binding
    {
        // private BindingElements
        private TcpTransportBindingElement _transport;
        private BinaryMessageEncodingBindingElement _encoding;
        private long _maxBufferPoolSize;
        private NetTcpSecurity _security = new NetTcpSecurity();

        public NetTcpBinding() { Initialize(); }
        public NetTcpBinding(SecurityMode securityMode)
            : this()
        {
            _security.Mode = securityMode;
        }


        public NetTcpBinding(string configurationName)
            : this()
        {
            if (!String.IsNullOrEmpty(configurationName))
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
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

        private void Initialize()
        {
            _transport = new TcpTransportBindingElement();
            _encoding = new BinaryMessageEncodingBindingElement();

            // NetNative and CoreCLR initialize to what TransportBindingElement does in the desktop
            // This property is not available in shipped contracts
            _maxBufferPoolSize = TransportDefaults.MaxBufferPoolSize;
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
#endif // FEATURE_NETNATIVE
        }

        public override BindingElementCollection CreateBindingElements()
        {
            this.CheckSettings();

            // return collection of BindingElements
            BindingElementCollection bindingElements = new BindingElementCollection();
            // order of BindingElements is important
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
            if (_security.Mode == SecurityMode.Message || _security.Mode == SecurityMode.TransportWithMessageCredential)
            {
                throw ExceptionHelper.PlatformNotSupported("NetTcpBinding.CreateMessageSecurity is not supported.");
            }
            else
            {
                return null;
            }
        }
    }
}
