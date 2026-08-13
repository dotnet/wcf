// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IO;
using System.Runtime;
using System.Xml;

namespace System.ServiceModel.Channels
{
    internal sealed class XmlStreamedByteStreamReader : XmlByteStreamReader
    {
        private Stream _stream;

        private XmlStreamedByteStreamReader(Stream stream, XmlDictionaryReaderQuotas quotas)
            : base(quotas)
        {
            Fx.Assert(stream != null, "The 'stream' parameter should not be null.");

            _stream = stream;
        }

        public static XmlStreamedByteStreamReader Create(Stream stream, XmlDictionaryReaderQuotas quotas) => new XmlStreamedByteStreamReader(stream, quotas);

        protected override void OnClose()
        {
            _stream = null;
            base.OnClose();
        }

        public override int ReadContentAsBase64(byte[] buffer, int index, int count)
        {
            EnsureInContent();
            ByteStreamMessageUtility.EnsureByteBoundaries(buffer, index, count, true);

            if (count == 0)
            {
                return 0;
            }

            int numBytesRead = _stream.Read(buffer, index, count);
            if (numBytesRead == 0)
            {
                _position = ReaderPosition.EndElement;
            }

            return numBytesRead;
        }

        protected override byte[] OnToByteArray()
        {
            throw Fx.Exception.AsError(
                  new InvalidOperationException(SRP.GetByteArrayFromStreamContentNotAllowed));
        }

        protected override Stream OnToStream()
        {
            Stream result = _stream;

            Fx.Assert(result != null, "The inner stream is null. Please check if the reader is closed or the ToStream method was already called before.");

            _stream = null;
            return result;
        }

        public override bool TryGetBase64ContentLength(out int length)
        {
            // in ByteStream encoder, we're not concerned about individual xml nodes
            // therefore we can just return the entire length of the stream
            if (!IsClosed && _stream != null && _stream.CanSeek)
            {
                long streamLength = _stream.Length;
                if (streamLength <= int.MaxValue)
                {
                    length = (int)streamLength;
                    return true;
                }
            }

            length = -1;
            return false;
        }
    }
}
