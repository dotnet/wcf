// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.Xml;

namespace System.ServiceModel.Channels
{
    public sealed class ByteStreamMessageEncodingBindingElement : MessageEncodingBindingElement
    {
        private readonly XmlDictionaryReaderQuotas _readerQuotas;
        private bool _moveBodyReaderToContent;

        public ByteStreamMessageEncodingBindingElement() : this((XmlDictionaryReaderQuotas)null)
        {
        }

        public ByteStreamMessageEncodingBindingElement(XmlDictionaryReaderQuotas quota)
        {
            _readerQuotas = new XmlDictionaryReaderQuotas();
            if (quota != null)
            {
                quota.CopyTo(_readerQuotas);
            }
        }

        private ByteStreamMessageEncodingBindingElement(ByteStreamMessageEncodingBindingElement byteStreamEncoderBindingElement)
            : this(byteStreamEncoderBindingElement._readerQuotas)
        {
            _moveBodyReaderToContent = byteStreamEncoderBindingElement._moveBodyReaderToContent;
        }

        public override MessageVersion MessageVersion
        {
            get
            {
                return MessageVersion.None;
            }
            set
            {
                if (value != MessageVersion.None)
                {
                    throw Fx.Exception.Argument(nameof(MessageVersion), SRP.Format(SRP.ByteStreamMessageEncoderMessageVersionNotSupported, value));
                }
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
                {
                    throw Fx.Exception.ArgumentNull(nameof(ReaderQuotas));
                }

                value.CopyTo(_readerQuotas);
            }
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            return InternalCanBuildChannelFactory<TChannel>(context);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            return InternalBuildChannelFactory<TChannel>(context);
        }

        internal void EnableBodyReaderMoveToContent() => _moveBodyReaderToContent = true;

        public override MessageEncoderFactory CreateMessageEncoderFactory() => new ByteStreamMessageEncoderFactory(_readerQuotas, _moveBodyReaderToContent);

        public override BindingElement Clone() => new ByteStreamMessageEncodingBindingElement(this);
    }
}
