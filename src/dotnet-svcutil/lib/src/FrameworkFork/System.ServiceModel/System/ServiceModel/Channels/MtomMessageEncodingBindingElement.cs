// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using System.Collections.Generic;
    using System.ServiceModel.Description;
    using System.Runtime.Serialization;
    using System.ServiceModel.Channels;
    using System.ServiceModel;
    using System.Text;
    using Microsoft.Xml;
    using System.ComponentModel;

    public sealed class MtomMessageEncodingBindingElement : MessageEncodingBindingElement, IWsdlExportExtension, IPolicyExportExtension
    {
        private int _maxReadPoolSize;
        private int _maxWritePoolSize;
        private XmlDictionaryReaderQuotas _readerQuotas;
        private int _maxBufferSize;
        private Encoding _writeEncoding;
        private MessageVersion _messageVersion;

        public MtomMessageEncodingBindingElement()
            : this(MessageVersion.Default, TextEncoderDefaults.Encoding)
        {
        }

        public MtomMessageEncodingBindingElement(MessageVersion messageVersion, Encoding writeEncoding)
        {
            if (messageVersion == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("messageVersion");

            if (messageVersion == MessageVersion.None)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(string.Format(SRServiceModel.MtomEncoderBadMessageVersion, messageVersion.ToString()), "messageVersion"));

            if (writeEncoding == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writeEncoding");

            TextEncoderDefaults.ValidateEncoding(writeEncoding);
            _maxReadPoolSize = EncoderDefaults.MaxReadPoolSize;
            _maxWritePoolSize = EncoderDefaults.MaxWritePoolSize;
            _readerQuotas = new XmlDictionaryReaderQuotas();
            EncoderDefaults.ReaderQuotas.CopyTo(_readerQuotas);
            _maxBufferSize = MtomEncoderDefaults.MaxBufferSize;
            _messageVersion = messageVersion;
            _writeEncoding = writeEncoding;
        }

        private MtomMessageEncodingBindingElement(MtomMessageEncodingBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _maxReadPoolSize = elementToBeCloned._maxReadPoolSize;
            _maxWritePoolSize = elementToBeCloned._maxWritePoolSize;
            _readerQuotas = new XmlDictionaryReaderQuotas();
            elementToBeCloned._readerQuotas.CopyTo(_readerQuotas);
            _maxBufferSize = elementToBeCloned._maxBufferSize;
            _writeEncoding = elementToBeCloned._writeEncoding;
            _messageVersion = elementToBeCloned._messageVersion;
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.ValueMustBePositive));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.ValueMustBePositive));
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
        }

        [DefaultValue(MtomEncoderDefaults.MaxBufferSize)]
        public int MaxBufferSize
        {
            get
            {
                return _maxBufferSize;
            }
            set
            {
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.ValueMustBePositive));
                }
                _maxBufferSize = value;
            }
        }

        // TODO: [TypeConverter(typeof(System.ServiceModel.Configuration.EncodingConverter))]
        public Encoding WriteEncoding
        {
            get
            {
                return _writeEncoding;
            }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");

                TextEncoderDefaults.ValidateEncoding(value);
                _writeEncoding = value;
            }
        }

        public override MessageVersion MessageVersion
        {
            get
            {
                return _messageVersion;
            }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                if (value == MessageVersion.None)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(string.Format(SRServiceModel.MtomEncoderBadMessageVersion, value.ToString()), "value"));
                }

                _messageVersion = value;
            }
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            return InternalBuildChannelFactory<TChannel>(context);
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            return InternalCanBuildChannelFactory<TChannel>(context);
        }

        public override BindingElement Clone()
        {
            return new MtomMessageEncodingBindingElement(this);
        }

        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            throw new NotImplementedException();
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

        void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext policyContext)
        {
            if (policyContext == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("policyContext");
            }
            XmlDocument document = new XmlDocument();

            policyContext.GetBindingAssertions().Add(document.CreateElement(
                MessageEncodingPolicyConstants.OptimizedMimeSerializationPrefix,
                MessageEncodingPolicyConstants.MtomEncodingName,
                MessageEncodingPolicyConstants.OptimizedMimeSerializationNamespace));
        }

        void IWsdlExportExtension.ExportContract(WsdlExporter exporter, WsdlContractConversionContext context) { }
        void IWsdlExportExtension.ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            SoapHelper.SetSoapVersion(context, exporter, _messageVersion.Envelope);
        }

        internal override bool CheckEncodingVersion(EnvelopeVersion version)
        {
            return _messageVersion.Envelope == version;
        }

        internal override bool IsMatch(BindingElement b)
        {
            if (!base.IsMatch(b))
                return false;
            MtomMessageEncodingBindingElement mtom = b as MtomMessageEncodingBindingElement;
            if (mtom == null)
                return false;
            if (_maxReadPoolSize != mtom.MaxReadPoolSize)
                return false;
            if (_maxWritePoolSize != mtom.MaxWritePoolSize)
                return false;

            // compare XmlDictionaryReaderQuotas
            if (_readerQuotas.MaxStringContentLength != mtom.ReaderQuotas.MaxStringContentLength)
                return false;
            if (_readerQuotas.MaxArrayLength != mtom.ReaderQuotas.MaxArrayLength)
                return false;
            if (_readerQuotas.MaxBytesPerRead != mtom.ReaderQuotas.MaxBytesPerRead)
                return false;
            if (_readerQuotas.MaxDepth != mtom.ReaderQuotas.MaxDepth)
                return false;
            if (_readerQuotas.MaxNameTableCharCount != mtom.ReaderQuotas.MaxNameTableCharCount)
                return false;

            if (_maxBufferSize != mtom.MaxBufferSize)
                return false;

            if (this.WriteEncoding.EncodingName != mtom.WriteEncoding.EncodingName)
                return false;
            if (!this.MessageVersion.IsMatch(mtom.MessageVersion))
                return false;

            return true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeMessageVersion()
        {
            return (!_messageVersion.IsMatch(MessageVersion.Default));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeReaderQuotas()
        {
            return (!EncoderDefaults.IsDefaultReaderQuotas(this.ReaderQuotas));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeWriteEncoding()
        {
            return (this.WriteEncoding != TextEncoderDefaults.Encoding);
        }
    }
}
