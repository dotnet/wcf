// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.IO;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal abstract class DetectEofStream : DelegatingStream
    {
        private bool _isAtEof;

        protected DetectEofStream(Stream stream)
            : base(stream)
        {
            _isAtEof = false;
        }

        protected bool IsAtEof
        {
            get { return _isAtEof; }
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, Threading.CancellationToken cancellationToken)
        {
            if (_isAtEof)
            {
                return 0;
            }
            int returnValue = await base.ReadAsync(buffer, offset, count, cancellationToken);
            if (returnValue == 0)
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
            if (_isAtEof)
            {
                return 0;
            }
            int returnValue = base.Read(buffer, offset, count);
            if (returnValue == 0)
            {
                ReceivedEof();
            }
            return returnValue;
        }

        private void ReceivedEof()
        {
            if (!_isAtEof)
            {
                _isAtEof = true;
                this.OnReceivedEof();
            }
        }

        protected virtual void OnReceivedEof()
        {
        }
    }
}
