// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime;
using System.Threading.Tasks;
using System.Threading;

namespace System.ServiceModel.Channels
{
    internal class MaxMessageSizeStream : DelegatingStream
    {
        private long _maxMessageSize;
        private long _totalBytesRead;
        private long _bytesWritten;

        public MaxMessageSizeStream(Stream stream, long maxMessageSize)
            : base(stream)
        {
            _maxMessageSize = maxMessageSize;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            count = PrepareRead(count);
            int bytesRead = await base.ReadAsync(buffer, offset, count, cancellationToken);
            return FinishRead(bytesRead);
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            int readCount = PrepareRead(buffer.Length);
            if (readCount < buffer.Length)
            {
                buffer = buffer.Slice(0, readCount);
            }
            int bytesRead = await base.ReadAsync(buffer, cancellationToken);
            return FinishRead(bytesRead);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            count = PrepareRead(count);
            return FinishRead(base.Read(buffer, offset, count));
        }

        public override int ReadByte()
        {
            PrepareRead(1);
            int i = base.ReadByte();
            if (i != -1)
            {
                FinishRead(1);
            }

            return i;
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            PrepareWrite(count);
            return base.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            PrepareWrite(buffer.Length);
            return base.WriteAsync(buffer, cancellationToken);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            PrepareWrite(count);
            base.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            PrepareWrite(1);
            base.WriteByte(value);
        }

        public static Exception CreateMaxReceivedMessageSizeExceededException(long maxMessageSize)
        {
            string message = SRP.Format(SRP.MaxReceivedMessageSizeExceeded, maxMessageSize);
            Exception inner = new QuotaExceededException(message);

            if (WcfEventSource.Instance.MaxReceivedMessageSizeExceededIsEnabled())
            {
                WcfEventSource.Instance.MaxReceivedMessageSizeExceeded(message);
            }

            return new CommunicationException(message, inner);
        }

        internal static Exception CreateMaxSentMessageSizeExceededException(long maxMessageSize)
        {
            string message = SRP.Format(SRP.MaxSentMessageSizeExceeded, maxMessageSize);
            Exception inner = new QuotaExceededException(message);

            if (WcfEventSource.Instance.MaxSentMessageSizeExceededIsEnabled())
            {
                WcfEventSource.Instance.MaxSentMessageSizeExceeded(message);
            }

            return new CommunicationException(message, inner);
        }

        private int PrepareRead(int bytesToRead)
        {
            if (_totalBytesRead >= _maxMessageSize)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateMaxReceivedMessageSizeExceededException(_maxMessageSize));
            }

            long bytesRemaining = _maxMessageSize - _totalBytesRead;

            if (bytesRemaining > int.MaxValue)
            {
                return bytesToRead;
            }
            else
            {
                return Math.Min(bytesToRead, (int)bytesRemaining);
            }
        }

        private int FinishRead(int bytesRead)
        {
            _totalBytesRead += bytesRead;
            return bytesRead;
        }

        private void PrepareWrite(int bytesToWrite)
        {
            if (_bytesWritten + bytesToWrite > _maxMessageSize)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateMaxSentMessageSizeExceededException(_maxMessageSize));
            }

            _bytesWritten += bytesToWrite;
        }
    }
}
