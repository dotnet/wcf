// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal abstract class DetectEofStream : DelegatingStream
    {
        protected DetectEofStream(Stream stream)
            : base(stream)
        {
            IsAtEof = false;
        }

        protected bool IsAtEof { get; private set; }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, Threading.CancellationToken cancellationToken)
        {
            if (IsAtEof)
            {
                return 0;
            }
            int returnValue = await base.ReadAsync(buffer, offset, count, cancellationToken);
            if (count != 0 && returnValue == 0)
            {
                ReceivedEof();
            }
            return returnValue;
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (IsAtEof)
            {
                return 0;
            }
            int returnValue = await base.ReadAsync(buffer, cancellationToken);
            if (!buffer.IsEmpty && returnValue == 0)
            {
                ReceivedEof();
            }
            return returnValue;
        }

        public override int ReadByte()
        {
            int returnValue = base.ReadByte();
            if (returnValue == -1)
            {
                ReceivedEof();
            }
            return returnValue;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (IsAtEof)
            {
                return 0;
            }
            int returnValue = base.Read(buffer, offset, count);
            if (count != 0 && returnValue == 0)
            {
                ReceivedEof();
            }
            return returnValue;
        }

        private void ReceivedEof()
        {
            if (!IsAtEof)
            {
                IsAtEof = true;
                OnReceivedEof();
            }
        }

        protected virtual void OnReceivedEof()
        {
        }
    }
}
