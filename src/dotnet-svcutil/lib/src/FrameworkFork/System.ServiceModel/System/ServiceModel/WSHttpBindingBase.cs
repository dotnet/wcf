// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Net;
    using System.Net.Security;
    using System.Runtime.Serialization;
    using System.Security.Principal;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;
    using System.ComponentModel;
    using Microsoft.Xml;

    public abstract class WSHttpBindingBase : Binding, IBindingRuntimePreferences
    {
        private WSMessageEncoding _messageEncoding;
        private OptionalReliableSession _reliableSession;
        // private BindingElements
        private HttpTransportBindingElement _httpTransport;
        private HttpsTransportBindingElement _httpsTransport;
        private TextMessageEncodingBindingElement _textEncoding;
        private MtomMessageEncodingBindingElement _mtomEncoding;
        private TransactionFlowBindingElement _txFlow;
        private ReliableSessionBindingElement _session;

        protected WSHttpBindingBase()
            : base()
        {
            Initialize();
        }

        protected WSHttpBindingBase(bool reliableSessionEnabled)
            : this()
        {
            this.ReliableSession.Enabled = reliableSessionEnabled;
        }

        [DefaultValue(false)]
        public bool TransactionFlow
        {
            get { return _txFlow.Transactions; }
            set { _txFlow.Transactions = value; }
        }

        [DefaultValue(HttpTransportDefaults.HostNameComparisonMode)]
        public HostNameComparisonMode HostNameComparisonMode
        {
            get { return _httpTransport.HostNameComparisonMode; }
            set
            {
                _httpTransport.HostNameComparisonMode = value;
                _httpsTransport.HostNameComparisonMode = value;
            }
        }

        [DefaultValue(TransportDefaults.MaxBufferPoolSize)]
        public long MaxBufferPoolSize
        {
            get { return _httpTransport.MaxBufferPoolSize; }
            set
            {
                _httpTransport.MaxBufferPoolSize = value;
                _httpsTransport.MaxBufferPoolSize = value;
            }
        }

        [DefaultValue(TransportDefaults.MaxReceivedMessageSize)]
        public long MaxReceivedMessageSize
        {
            get { return _httpTransport.MaxReceivedMessageSize; }
            set
            {
                if (value > int.MaxValue)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new ArgumentOutOfRangeException("value.MaxReceivedMessageSize",
                        SRServiceModel.MaxReceivedMessageSizeMustBeInIntegerRange));
                }
                _httpTransport.MaxReceivedMessageSize = value;
                _httpsTransport.MaxReceivedMessageSize = value;
                _mtomEncoding.MaxBufferSize = (int)value;
            }
        }

        [DefaultValue(WSHttpBindingDefaults.MessageEncoding)]
        public WSMessageEncoding MessageEncoding
        {
            get { return _messageEncoding; }
            set { _messageEncoding = value; }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get { return _textEncoding.ReaderQuotas; }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                value.CopyTo(_textEncoding.ReaderQuotas);
                value.CopyTo(_mtomEncoding.ReaderQuotas);
            }
        }

        public OptionalReliableSession ReliableSession
        {
            get { return _reliableSession; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
                }
                _reliableSession.CopySettings(value);
            }
        }

        public override string Scheme { get { return GetTransport().Scheme; } }

        public EnvelopeVersion EnvelopeVersion
        {
            get { return EnvelopeVersion.Soap12; }
        }

        // TODO: [TypeConverter(typeof(EncodingConverter))]
        public System.Text.Encoding TextEncoding
        {
            get { return _textEncoding.WriteEncoding; }
            set
            {
                _textEncoding.WriteEncoding = value;
                _mtomEncoding.WriteEncoding = value;
            }
        }
        bool IBindingRuntimePreferences.ReceiveSynchronously
        {
            get { return false; }
        }

        internal HttpTransportBindingElement HttpTransport
        {
            get { return _httpTransport; }
        }

        internal HttpsTransportBindingElement HttpsTransport
        {
            get { return _httpsTransport; }
        }

        internal ReliableSessionBindingElement ReliableSessionBindingElement
        {
            get { return _session; }
        }

        internal TransactionFlowBindingElement TransactionFlowBindingElement
        {
            get { return _txFlow; }
        }

        private static TransactionFlowBindingElement GetDefaultTransactionFlowBindingElement()
        {
            TransactionFlowBindingElement tfbe = new TransactionFlowBindingElement(false);
            tfbe.TransactionProtocol = TransactionProtocol.WSAtomicTransactionOctober2004;
            return tfbe;
        }

        private void Initialize()
        {
            _httpTransport = new HttpTransportBindingElement();
            _httpsTransport = new HttpsTransportBindingElement();
            _messageEncoding = WSHttpBindingDefaults.MessageEncoding;
            _txFlow = GetDefaultTransactionFlowBindingElement();
            _session = new ReliableSessionBindingElement(true);
            _textEncoding = new TextMessageEncodingBindingElement();
            _textEncoding.MessageVersion = MessageVersion.Soap12WSAddressing10;
            _mtomEncoding = new MtomMessageEncodingBindingElement();
            _mtomEncoding.MessageVersion = MessageVersion.Soap12WSAddressing10;
            _reliableSession = new OptionalReliableSession(_session);
        }

        private void InitializeFrom(HttpTransportBindingElement transport, MessageEncodingBindingElement encoding, TransactionFlowBindingElement txFlow, ReliableSessionBindingElement session)
        {
            this.HostNameComparisonMode = transport.HostNameComparisonMode;
            this.MaxBufferPoolSize = transport.MaxBufferPoolSize;
            this.MaxReceivedMessageSize = transport.MaxReceivedMessageSize;

            // this binding only supports Text and Mtom encoding
            if (encoding is TextMessageEncodingBindingElement)
            {
                this.MessageEncoding = WSMessageEncoding.Text;
                TextMessageEncodingBindingElement text = (TextMessageEncodingBindingElement)encoding;
                this.TextEncoding = text.WriteEncoding;
                this.ReaderQuotas = text.ReaderQuotas;
            }
            else if (encoding is MtomMessageEncodingBindingElement)
            {
                _messageEncoding = WSMessageEncoding.Mtom;
                MtomMessageEncodingBindingElement mtom = (MtomMessageEncodingBindingElement)encoding;
                this.TextEncoding = mtom.WriteEncoding;
                this.ReaderQuotas = mtom.ReaderQuotas;
            }
            this.TransactionFlow = txFlow.Transactions;
            _reliableSession.Enabled = session != null;

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
        private bool IsBindingElementsMatch(HttpTransportBindingElement transport, MessageEncodingBindingElement encoding, TransactionFlowBindingElement txFlow, ReliableSessionBindingElement session)
        {
            if (!this.GetTransport().IsMatch(transport))
                return false;
            if (this.MessageEncoding == WSMessageEncoding.Text)
            {
                if (!_textEncoding.IsMatch(encoding))
                    return false;
            }
            else if (this.MessageEncoding == WSMessageEncoding.Mtom)
            {
                if (!_mtomEncoding.IsMatch(encoding))
                    return false;
            }
            if (!_txFlow.IsMatch(txFlow))
                return false;

            if (_reliableSession.Enabled)
            {
                if (!_session.IsMatch(session))
                    return false;
            }
            else if (session != null)
            {
                return false;
            }

            return true;
        }

        public override BindingElementCollection CreateBindingElements()
        {   // return collection of BindingElements
            BindingElementCollection bindingElements = new BindingElementCollection();
            // order of BindingElements is important
            // context

            bindingElements.Add(_txFlow);
            // reliable
            if (_reliableSession.Enabled)
            {
                bindingElements.Add(_session);
            }

            // add security (*optional)
            SecurityBindingElement wsSecurity = this.CreateMessageSecurity();
            if (wsSecurity != null)
            {
                bindingElements.Add(wsSecurity);
            }

            // add encoding (text or mtom)
            WSMessageEncodingHelper.SyncUpEncodingBindingElementProperties(_textEncoding, _mtomEncoding);
            if (this.MessageEncoding == WSMessageEncoding.Text)
                bindingElements.Add(_textEncoding);
            else if (this.MessageEncoding == WSMessageEncoding.Mtom)
                bindingElements.Add(_mtomEncoding);

            // add transport (http or https)
            bindingElements.Add(GetTransport());

            return bindingElements.Clone();
        }

        internal static bool TryCreate(BindingElementCollection elements, out Binding binding)
        {
            binding = null;
            if (elements.Count > 6)
                return false;

            // collect all binding elements
            PrivacyNoticeBindingElement privacy = null;
            TransactionFlowBindingElement txFlow = null;
            ReliableSessionBindingElement session = null;
            SecurityBindingElement security = null;
            MessageEncodingBindingElement encoding = null;
            HttpTransportBindingElement transport = null;

            foreach (BindingElement element in elements)
            {
                if (element is SecurityBindingElement)
                    security = element as SecurityBindingElement;
                else if (element is TransportBindingElement)
                    transport = element as HttpTransportBindingElement;
                else if (element is MessageEncodingBindingElement)
                    encoding = element as MessageEncodingBindingElement;
                else if (element is TransactionFlowBindingElement)
                    txFlow = element as TransactionFlowBindingElement;
                else if (element is ReliableSessionBindingElement)
                    session = element as ReliableSessionBindingElement;
                else if (element is PrivacyNoticeBindingElement)
                    privacy = element as PrivacyNoticeBindingElement;
                else
                    return false;
            }

            if (transport == null)
                return false;
            if (encoding == null)
                return false;

            if (!transport.AuthenticationScheme.IsSingleton())
            {
                //multiple authentication schemes selected -- not supported in StandardBindings
                return false;
            }

            HttpsTransportBindingElement httpsTransport = transport as HttpsTransportBindingElement;
            if ((security != null) && (httpsTransport != null) && (httpsTransport.RequireClientCertificate != TransportDefaults.RequireClientCertificate))
            {
                return false;
            }

            if (null != privacy || !WSHttpBinding.TryCreate(security, transport, session, txFlow, out binding))
                if (!WSFederationHttpBinding.TryCreate(security, transport, privacy, session, txFlow, out binding))
                    if (!WS2007HttpBinding.TryCreate(security, transport, session, txFlow, out binding))
                        if (!WS2007FederationHttpBinding.TryCreate(security, transport, privacy, session, txFlow, out binding))
                            return false;

            if (txFlow == null)
            {
                txFlow = GetDefaultTransactionFlowBindingElement();
                if ((binding is WS2007HttpBinding) || (binding is WS2007FederationHttpBinding))
                {
                    txFlow.TransactionProtocol = TransactionProtocol.WSAtomicTransaction11;
                }
            }

            WSHttpBindingBase wSHttpBindingBase = binding as WSHttpBindingBase;
            wSHttpBindingBase.InitializeFrom(transport, encoding, txFlow, session);
            if (!wSHttpBindingBase.IsBindingElementsMatch(transport, encoding, txFlow, session))
                return false;

            return true;
        }

        protected abstract TransportBindingElement GetTransport();
        protected abstract SecurityBindingElement CreateMessageSecurity();
    }
}
