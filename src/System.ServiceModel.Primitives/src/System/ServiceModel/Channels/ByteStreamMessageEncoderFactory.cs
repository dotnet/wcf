// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Xml;

namespace System.ServiceModel.Channels
{
    internal class ByteStreamMessageEncoderFactory : MessageEncoderFactory
    {
        private readonly ByteStreamMessageEncoder _encoder;

        public ByteStreamMessageEncoderFactory(XmlDictionaryReaderQuotas quotas, bool moveBodyReaderToContent)
        {
            _encoder = new ByteStreamMessageEncoder(quotas, moveBodyReaderToContent);
        }

        public override MessageEncoder Encoder => _encoder;

        public override MessageVersion MessageVersion => _encoder.MessageVersion;
    }
}
