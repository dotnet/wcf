// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ScenarioTests.Common
{
    // This class provides a mock for a Stream that allows tests
    // to override its methods to inject different test behaviors.
    public class TestMockStream : Stream
    {
        public TestMockStream() : this(new MemoryStream())
        {
        }

        public TestMockStream(Stream innerStream)
        {
            InnerStream = innerStream;
        }

        private Stream InnerStream { get; set; }

        // Func to call for CopyToAsync (default is inner stream's CopyToAsync)
        public Func<Stream, int, CancellationToken, Task> CopyToAsyncFunc { get; set; }

        // Func to call instead for Read() (default is inner stream's Read)
        public Func<byte[], int, int, int> ReadFunc { get; set; }

        public override bool CanRead { get { return InnerStream.CanRead; } }

        public override bool CanSeek { get { return InnerStream.CanSeek; } }

        public override bool CanWrite { get { return InnerStream.CanWrite; } }

        public override long Length { get { return InnerStream.Length; } }

        public override long Position
        {
            get { return InnerStream.Position; }
            set { InnerStream.Position = value; }
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            Task t = null;

            // Call out to mock user's func.  If the returned Task is non-null,
            // return it, else fall into default logic
            if (CopyToAsyncFunc != null)
            {
                t = CopyToAsyncFunc(destination, bufferSize, cancellationToken);
            }

            return t ?? InnerStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override void Flush()
        {
            InnerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return ReadFunc != null
                ? ReadFunc(buffer, offset, count)
                : InnerStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return InnerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            InnerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            InnerStream.Write(buffer, offset, count);
        }
    }
}
