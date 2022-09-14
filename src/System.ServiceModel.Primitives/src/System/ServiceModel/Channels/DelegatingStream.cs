// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IO;
using System.Runtime;
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

        // Do NOT override Read(Span<byte buffer) unless also providing implementations for all types derived from DelegatingStream
        // as the parent class won't have Read(byte[] buffer, int offset, int count) called. This is because BaseStream.Read(Span<byte>)
        // will forward the call to BaseStream.Read(byte[],int,int) and won't go through the parent implementation.
        // public override int Read(Span<byte> buffer) => BaseStream.Read(buffer);

        // Do NOT override BeginRead/EndRead in types derived from DelegatingStream otherwise the parent BeginRead and ReadAsync will both get called
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => ReadAsync(buffer, offset, count).ToApm(callback, state);
        public override int EndRead(IAsyncResult asyncResult) => asyncResult.ToApmEnd<int>();
        public override int Read(byte[] buffer, int offset, int count) => BaseStream.Read(buffer, offset, count);
        public override int ReadByte() => BaseStream.ReadByte();
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => BaseStream.ReadAsync(buffer, offset, count, cancellationToken);
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => BaseStream.ReadAsync(buffer, cancellationToken);

        // Do NOT override Write(ReadOnlySpan<byte> buffer) unless also providing implementations for all types derived from DelegatingStream.
        // This is for similar reasons as commented for Read(Span<byte>)
        //public override void Write(ReadOnlySpan<byte> buffer) => BaseStream.Write(buffer);

        // Do NOT override BeginWrite/EndWrite in types derived from DelegatingStream otherwise the parent BeginWrite and WriteAsync will both get called
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => WriteAsync(buffer, offset, count).ToApm(callback, state);
        public override void EndWrite(IAsyncResult asyncResult) => asyncResult.ToApmEnd();

        public override void Write(byte[] buffer, int offset, int count) => BaseStream.Write(buffer, offset, count);
        public override void WriteByte(byte value) => BaseStream.WriteByte(value);
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => BaseStream.WriteAsync(buffer, offset, count, cancellationToken);
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) => BaseStream.WriteAsync(buffer, cancellationToken);
    }
}
