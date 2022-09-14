// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    // Base Stream that delegates all its methods to another Stream.
    public abstract class DelegatingStream : Stream
    {
        private bool _disposed;

        protected DelegatingStream(Stream stream)
        {
            BaseStream = stream ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(stream));
        }

        protected Stream BaseStream { get; }
        public override bool CanRead => BaseStream.CanRead;
        public override bool CanSeek => BaseStream.CanSeek;
        public override bool CanTimeout => BaseStream.CanTimeout;
        public override bool CanWrite => BaseStream.CanWrite;
        public override long Length => BaseStream.Length;

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    BaseStream.Dispose();
                }

                _disposed = true;
            }

            base.Dispose(disposing);
        }

        public override async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                await BaseStream.DisposeAsync();
                GC.SuppressFinalize(this);
                _disposed = true;
            }

            await base.DisposeAsync();
        }

        public override long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }

        public override int ReadTimeout
        {
            get => BaseStream.ReadTimeout;
            set => BaseStream.ReadTimeout = value;
        }

        public override int WriteTimeout
        {
            get => BaseStream.WriteTimeout;
            set => BaseStream.WriteTimeout = value;
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) => BaseStream.CopyToAsync(destination, bufferSize, cancellationToken);
        public override void Flush() => BaseStream.Flush();
        public override Task FlushAsync(CancellationToken cancellationToken) => BaseStream.FlushAsync(cancellationToken);
        public override long Seek(long offset, SeekOrigin origin) => BaseStream.Seek(offset, origin);
        public override void SetLength(long value) => BaseStream.SetLength(value);

        public override int Read(byte[] buffer, int offset, int count) => BaseStream.Read(buffer, offset, count);
        public override int Read(Span<byte> buffer) => BaseStream.Read(buffer);
        public override int ReadByte() => BaseStream.ReadByte();
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => BaseStream.BeginRead(buffer, offset, count, callback, state);
        public override int EndRead(IAsyncResult asyncResult) => BaseStream.EndRead(asyncResult);
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => BaseStream.ReadAsync(buffer, offset, count, cancellationToken);
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => BaseStream.ReadAsync(buffer, cancellationToken);

        public override void Write(byte[] buffer, int offset, int count) => BaseStream.Write(buffer, offset, count);
        public override void Write(ReadOnlySpan<byte> buffer) => BaseStream.Write(buffer);
        public override void WriteByte(byte value) => BaseStream.WriteByte(value);
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => BaseStream.BeginWrite(buffer, offset, count, callback, state);
        public override void EndWrite(IAsyncResult asyncResult) => BaseStream.EndWrite(asyncResult);
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => BaseStream.WriteAsync(buffer, offset, count, cancellationToken);
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) => BaseStream.WriteAsync(buffer, cancellationToken);
    }
}
