// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel;
using Microsoft.Xml;
using System.ComponentModel;

namespace System.ServiceModel.Channels
{
    public sealed class BinaryMessageEncodingBindingElement : MessageEncodingBindingElement
    {
        private int _maxReadPoolSize;
        private int _maxWritePoolSize;
        private XmlDictionaryReaderQuotas _readerQuotas;
        private int _maxSessionSize;
        private BinaryVersion _binaryVersion;
        private MessageVersion _messageVersion;
        private CompressionFormat _compressionFormat;
        private long _maxReceivedMessageSize;

        public BinaryMessageEncodingBindingElement()
        {
            _maxReadPoolSize = EncoderDefaults.MaxReadPoolSize;
            _maxWritePoolSize = EncoderDefaults.MaxWritePoolSize;
            _readerQuotas = new XmlDictionaryReaderQuotas();
            EncoderDefaults.ReaderQuotas.CopyTo(_readerQuotas);
            _maxSessionSize = BinaryEncoderDefaults.MaxSessionSize;
            _binaryVersion = BinaryEncoderDefaults.BinaryVersion;
            _messageVersion = MessageVersion.CreateVersion(BinaryEncoderDefaults.EnvelopeVersion);
            _compressionFormat = EncoderDefaults.DefaultCompressionFormat;
        }

        private BinaryMessageEncodingBindingElement(BinaryMessageEncodingBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _maxReadPoolSize = elementToBeCloned._maxReadPoolSize;
            _maxWritePoolSize = elementToBeCloned._maxWritePoolSize;
            _readerQuotas = new XmlDictionaryReaderQuotas();
            elementToBeCloned._readerQuotas.CopyTo(_readerQuotas);
            this.MaxSessionSize = elementToBeCloned.MaxSessionSize;
            this.BinaryVersion = elementToBeCloned.BinaryVersion;
            _messageVersion = elementToBeCloned._messageVersion;
            this.CompressionFormat = elementToBeCloned.CompressionFormat;
            _maxReceivedMessageSize = elementToBeCloned._maxReceivedMessageSize;
        }

        [DefaultValue(EncoderDefaults.DefaultCompressionFormat)]
        public CompressionFormat CompressionFormat
        {
            get
            {
                return _compressionFormat;
            }
            set
            {
                _compressionFormat = value;
            }
        }

        /* public */
        private BinaryVersion BinaryVersion
        {
            get
            {
                return _binaryVersion;
            }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
                }
                _binaryVersion = value;
            }
        }

        public override MessageVersion MessageVersion
        {
            get { return _messageVersion; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                if (value.Envelope != BinaryEncoderDefaults.EnvelopeVersion)
                {
                    string errorMsg = string.Format(SRServiceModel.UnsupportedEnvelopeVersion, this.GetType().FullName, BinaryEncoderDefaults.EnvelopeVersion, value.Envelope);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(errorMsg));
                }

                _messageVersion = MessageVersion.CreateVersion(BinaryEncoderDefaults.EnvelopeVersion, value.Addressing);
            }
        }

        [DefaultValue(EncoderDefaults.MaxReadPoolSize)]
        public int MaxReadPoolSize
        {
            get
            {
                return _maxReadPoolSize;
            }
            set
            {
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                                                    SRServiceModel.ValueMustBePositive));
                }
                _maxReadPoolSize = value;
            }
        }

        [DefaultValue(EncoderDefaults.MaxWritePoolSize)]
        public int MaxWritePoolSize
        {
            get
            {
                return _maxWritePoolSize;
            }
            set
            {
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                                                    SRServiceModel.ValueMustBePositive));
                }
                _maxWritePoolSize = value;
            }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get
            {
                return _readerQuotas;
            }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                value.CopyTo(_readerQuotas);
            }
        }

        [DefaultValue(BinaryEncoderDefaults.MaxSessionSize)]
        public int MaxSessionSize
        {
            get
            {
                return _maxSessionSize;
            }
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                                                    SRServiceModel.ValueMustBeNonNegative));
                }

                _maxSessionSize = value;
            }
        }

        private void VerifyCompression(BindingContext context)
        {
            if (_compressionFormat != CompressionFormat.None)
            {
                ITransportCompressionSupport compressionSupport = context.GetInnerProperty<ITransportCompressionSupport>();
                if (compressionSupport == null || !compressionSupport.IsCompressionFormatSupported(_compressionFormat))
                {
                    throw FxTrace.Exception.AsError(new NotSupportedException(string.Format(
                        SRServiceModel.TransportDoesNotSupportCompression,
                        _compressionFormat.ToString(),
                        this.GetType().Name,
                        CompressionFormat.None.ToString())));
                }
            }
        }

        private void SetMaxReceivedMessageSizeFromTransport(BindingContext context)
        {
            TransportBindingElement transport = context.Binding.Elements.Find<TransportBindingElement>();
            if (transport != null)
            {
                // We are guaranteed that a transport exists when building a binding;  
                // Allow the regular flow/checks to happen rather than throw here 
                // (InternalBuildChannelListener will call into the BindingContext. Validation happens there and it will throw) 
                _maxReceivedMessageSize = transport.MaxReceivedMessageSize;
            }
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            VerifyCompression(context);
            SetMaxReceivedMessageSizeFromTransport(context);
            return InternalBuildChannelFactory<TChannel>(context);
        }

        public override BindingElement Clone()
        {
            return new BinaryMessageEncodingBindingElement(this);
        }

        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new BinaryMessageEncoderFactory(
                this.MessageVersion,
                this.MaxReadPoolSize,
                this.MaxWritePoolSize,
                this.MaxSessionSize,
                this.ReaderQuotas,
                _maxReceivedMessageSize,
                this.BinaryVersion,
                this.CompressionFormat);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
            {
                return (T)(object)_readerQuotas;
            }
            else
            {
                return base.GetProperty<T>(context);
            }
        }

        internal override bool IsMatch(BindingElement b)
        {
            if (!base.IsMatch(b))
                return false;

            BinaryMessageEncodingBindingElement binary = b as BinaryMessageEncodingBindingElement;
            if (binary == null)
                return false;
            if (_maxReadPoolSize != binary.MaxReadPoolSize)
                return false;
            if (_maxWritePoolSize != binary.MaxWritePoolSize)
                return false;

            // compare XmlDictionaryReaderQuotas
            if (_readerQuotas.MaxStringContentLength != binary.ReaderQuotas.MaxStringContentLength)
                return false;
            if (_readerQuotas.MaxArrayLength != binary.ReaderQuotas.MaxArrayLength)
                return false;
            if (_readerQuotas.MaxBytesPerRead != binary.ReaderQuotas.MaxBytesPerRead)
                return false;
            if (_readerQuotas.MaxDepth != binary.ReaderQuotas.MaxDepth)
                return false;
            if (_readerQuotas.MaxNameTableCharCount != binary.ReaderQuotas.MaxNameTableCharCount)
                return false;

            if (this.MaxSessionSize != binary.MaxSessionSize)
                return false;
            if (this.CompressionFormat != binary.CompressionFormat)
                return false;
            return true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeReaderQuotas()
        {
            return (!EncoderDefaults.IsDefaultReaderQuotas(this.ReaderQuotas));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeMessageVersion()
        {
            return (!_messageVersion.IsMatch(MessageVersion.Default));
        }
    }
}
