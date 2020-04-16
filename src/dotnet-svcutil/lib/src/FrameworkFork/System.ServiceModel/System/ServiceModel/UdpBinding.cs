// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime;
    using System.ServiceModel.Channels;
    using System.Text;
    using Microsoft.Xml;

    public class UdpBinding : Binding, IBindingRuntimePreferences
    {
        private TextMessageEncodingBindingElement _textEncoding;
        private UdpTransportBindingElement _udpTransport;

        public UdpBinding()
            : base()
        {
            _textEncoding = new TextMessageEncodingBindingElement();
            _udpTransport = new UdpTransportBindingElement();
        }

        private UdpBinding(UdpTransportBindingElement transport, TextMessageEncodingBindingElement encoding)
            : this()
        {
            this.DuplicateMessageHistoryLength = transport.DuplicateMessageHistoryLength;
            this.MaxBufferPoolSize = transport.MaxBufferPoolSize;
            this.MaxPendingMessagesTotalSize = transport.MaxPendingMessagesTotalSize;
            this.MaxReceivedMessageSize = transport.MaxReceivedMessageSize;
            this.MaxRetransmitCount = Math.Max(transport.RetransmissionSettings.MaxUnicastRetransmitCount, transport.RetransmissionSettings.MaxMulticastRetransmitCount);
            this.MulticastInterfaceId = transport.MulticastInterfaceId;
            this.TimeToLive = transport.TimeToLive;

            this.ReaderQuotas = encoding.ReaderQuotas;
            this.TextEncoding = encoding.WriteEncoding;
        }

        [DefaultValue(UdpConstants.Defaults.DuplicateMessageHistoryLength)]
        public int DuplicateMessageHistoryLength
        {
            get
            {
                return _udpTransport.DuplicateMessageHistoryLength;
            }
            set
            {
                _udpTransport.DuplicateMessageHistoryLength = value;
            }
        }

        [DefaultValue(TransportDefaults.MaxBufferPoolSize)]
        public long MaxBufferPoolSize
        {
            get
            {
                return _udpTransport.MaxBufferPoolSize;
            }
            set
            {
                _udpTransport.MaxBufferPoolSize = value;
            }
        }

        [DefaultValue(UdpConstants.Defaults.MaxRetransmitCount)]
        public int MaxRetransmitCount
        {
            get
            {
                return Math.Max(_udpTransport.RetransmissionSettings.MaxUnicastRetransmitCount, _udpTransport.RetransmissionSettings.MaxMulticastRetransmitCount);
            }
            set
            {
                _udpTransport.RetransmissionSettings.MaxUnicastRetransmitCount = value;
                _udpTransport.RetransmissionSettings.MaxMulticastRetransmitCount = value;
            }
        }

        [DefaultValue(UdpConstants.Defaults.DefaultMaxPendingMessagesTotalSize)]
        public long MaxPendingMessagesTotalSize
        {
            get
            {
                return _udpTransport.MaxPendingMessagesTotalSize;
            }
            set
            {
                _udpTransport.MaxPendingMessagesTotalSize = value;
            }
        }

        [DefaultValue(UdpConstants.Defaults.MaxReceivedMessageSize)]
        public long MaxReceivedMessageSize
        {
            get
            {
                return _udpTransport.MaxReceivedMessageSize;
            }
            set
            {
                _udpTransport.MaxReceivedMessageSize = value;
            }
        }

        [DefaultValue(UdpConstants.Defaults.MulticastInterfaceId)]
        public string MulticastInterfaceId
        {
            get { return _udpTransport.MulticastInterfaceId; }
            set { _udpTransport.MulticastInterfaceId = value; }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get { return _textEncoding.ReaderQuotas; }
            set
            {
                if (value == null)
                {
                    throw FxTrace.Exception.ArgumentNull("value");
                }
                value.CopyTo(_textEncoding.ReaderQuotas);
            }
        }

        //TODO: [TypeConverter(typeof(EncodingConverter))]
        public Encoding TextEncoding
        {
            get { return _textEncoding.WriteEncoding; }
            set { _textEncoding.WriteEncoding = value; }
        }

        [DefaultValue(UdpConstants.Defaults.TimeToLive)]
        public int TimeToLive
        {
            get { return _udpTransport.TimeToLive; }
            set { _udpTransport.TimeToLive = value; }
        }

        public override string Scheme
        {
            get { return _udpTransport.Scheme; }
        }

        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection bindingElements = new BindingElementCollection();
            bindingElements.Add(_textEncoding);
            bindingElements.Add(_udpTransport);

            return bindingElements.Clone();
        }

        private bool BindingElementsPropertiesMatch(UdpTransportBindingElement transport, MessageEncodingBindingElement encoding)
        {
            if (!_udpTransport.IsMatch(transport))
            {
                return false;
            }

            if (!_textEncoding.IsMatch(encoding))
            {
                return false;
            }

            return true;
        }

        [SuppressMessage(FxCop.Category.Design, FxCop.Rule.InterfaceMethodsShouldBeCallableByChildTypes, Justification = "no need to call this from derrived classes")]
        bool IBindingRuntimePreferences.ReceiveSynchronously
        {
            get { return false; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeReaderQuotas()
        {
            return (!EncoderDefaults.IsDefaultReaderQuotas(this.ReaderQuotas));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeTextEncoding()
        {
            return (!this.TextEncoding.Equals(TextEncoderDefaults.Encoding));
        }

        internal static bool TryCreate(BindingElementCollection bindingElements, out Binding binding)
        {
            binding = null;

            if (bindingElements.Count > 2)
            {
                return false;
            }

            UdpTransportBindingElement transport = null;
            TextMessageEncodingBindingElement encoding = null;

            foreach (BindingElement bindingElement in bindingElements)
            {
                if (bindingElement is UdpTransportBindingElement)
                {
                    transport = bindingElement as UdpTransportBindingElement;
                }
                else if (bindingElement is TextMessageEncodingBindingElement)
                {
                    encoding = bindingElement as TextMessageEncodingBindingElement;
                }
                else
                {
                    return false;
                }
            }

            if (transport == null || encoding == null)
            {
                return false;
            }

            UdpBinding udpBinding = new UdpBinding(transport, encoding);

            if (!udpBinding.BindingElementsPropertiesMatch(transport, encoding))
            {
                return false;
            }

            binding = udpBinding;
            return true;
        }
    }
}
