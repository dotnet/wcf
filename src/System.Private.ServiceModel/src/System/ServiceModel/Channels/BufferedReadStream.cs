// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    /// <summary>
    /// This class is based on BufferedStream from the Desktop version of .Net. Only the read functionality
    /// is needed in some circumstances so the write capability has been removed. This allowed some extra logic 
    /// to be removed from the write code path. Also some validation code has been removed as this class is no 
    /// longer general purpose and is only used in pre-known scenarios and only called by WCF code. Some validation
    /// checks have been converted to only run on a debug build to allow catching code bugs in other WCF code, 
    /// but not causing release build overhead.
    /// 
    /// One of the design goals here is to prevent the buffer from getting in the way and slowing
    /// down underlying stream accesses when it is not needed.
    /// 
    /// This class will never cache more bytes than the max specified buffer size.
    /// However, it may use a temporary buffer of up to twice the size in order to combine several IO operations on
    /// the underlying stream into a single operation. This is because we assume that memory copies are significantly
    /// faster than IO operations on the underlying stream (if this was not true, using buffering is never appropriate).
    /// The max size of this "shadow" buffer is limited as to not allocate it on the LOH.
    /// Shadowing is always transient. Even when using this technique, this class still guarantees that the number of
    /// bytes cached (not yet written to the target stream or not yet consumed by the user) is never larger than the 
    /// actual specified buffer size.
    /// </summary>
    public class BufferedReadStream : Stream
    {
        private const int DefaultBufferSize = 8192;

        private Stream _stream;                         // Underlying stream.  Close sets _stream to null.
        private byte[] _buffer;                         // Read buffer.
        private readonly int _bufferSize;               // Length of internal buffer (not counting the shadow buffer).
        private int _readPos;                           // Read pointer within shared buffer.
        private int _readLen;                           // Number of bytes read in buffer from _stream.
        private Task<int> _lastSyncCompletedReadTask;   // The last successful Task returned from ReadAsync
                                                        // (perf optimization for successive reads of the same size)
        private BufferManager _bufferManager;           // Caller can supply buffer manager to reduce allocation

        private readonly SemaphoreSlim _sem = new SemaphoreSlim(1, 1);

        public const string BufferedReadStreamPropertyName = "ServiceModelBufferedReadStreamProperty";

        public BufferedReadStream(Stream stream) : this(stream, null, DefaultBufferSize) { }

        public BufferedReadStream(Stream stream, BufferManager bufferManager) : this(stream, bufferManager, DefaultBufferSize) { }

        public BufferedReadStream(Stream stream, BufferManager bufferManager, int bufferSize)
        {
            Contract.Assert(stream != Null, "stream != Stream.Null");
            Contract.Assert(stream != null, "stream != null");
            Contract.Assert(bufferSize > 0, "bufferSize > 0");
            Contract.Assert(stream.CanRead);
            _stream = stream;
            _bufferManager = bufferManager;
            _bufferSize = bufferSize;

            EnsureBufferAllocated();
        }

        private void EnsureNotClosed()
        {
            if (_stream == null)
            {
                throw new ObjectDisposedException(nameof(BufferedReadStream));
            }
        }

        private void EnsureCanRead()
        {
            Contract.Assert(_stream != null);
            Contract.Assert(_stream.CanRead);
        }

        private void EnsureBufferAllocated()
        {
            if (_buffer == null)
            {
                if (_bufferManager != null)
                {
                    _buffer = _bufferManager.TakeBuffer(_bufferSize);
                }
                else
                {
                    _buffer = new byte[_bufferSize];
                }
            }
        }

        public override bool CanRead
        {
            get { return _stream != null && _stream.CanRead; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override long Length
        {
            get { throw new NotSupportedException(nameof(Length)); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(nameof(Position)); }
            set { throw new NotSupportedException(nameof(Position)); }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    _stream?.Dispose();
                }
                finally
                {
                    _stream = null;
                    var tempBuffer = _buffer;
                    _buffer = null;
                    _bufferManager?.ReturnBuffer(tempBuffer);
                    _bufferManager = null;
                }
            }

            // Call base.Dispose(bool) to cleanup async IO resources
            base.Dispose(disposing);
        }

        public override void Flush()
        {
            // Read streams do not need to flush.
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            // Read streams do not need to flush.
            return Task.CompletedTask;
        }


        private int ReadFromBuffer(byte[] array, int offset, int count)
        {
            int readBytes = _readLen - _readPos;
            Contract.Assert(readBytes >= 0);

            if (readBytes == 0)
            {
                return 0;
            }

            Contract.Assert(readBytes > 0);

            if (readBytes > count)
            {
                readBytes = count;
            }

            Array.Copy(_buffer, _readPos, array, offset, readBytes);
            _readPos += readBytes;

            return readBytes;
        }


        private int ReadFromBuffer(byte[] array, int offset, int count, out Exception error)
        {
            try
            {
                error = null;
                return ReadFromBuffer(array, offset, count);
            }
            catch (Exception ex)
            {
                error = ex;
                return 0;
            }
        }

        public override int Read(byte[] array, int offset, int count)
        {
            Contract.Assert(array != null);
            Contract.Assert(offset >= 0);
            Contract.Assert(count >= 0);
            Contract.Assert(count <= array.Length - offset);

            EnsureNotClosed();
            EnsureCanRead();

            int bytesFromBuffer = ReadFromBuffer(array, offset, count);

            // We may have read less than the number of bytes the user asked for, but that is part of the Stream contract.

            // Reading again for more data may cause us to block if we're using a device with no clear end of file,
            // such as a serial port or pipe. If we blocked here and this code was used with redirected pipes for a
            // process's standard output, this can lead to deadlocks involving two processes.              
            // BUT - this is a breaking change. 
            // So: If we could not read all bytes the user asked for from the buffer, we will try once from the underlying
            // stream thus ensuring the same blocking behaviour as if the underlying stream was not wrapped in this BufferedStream.
            if (bytesFromBuffer == count)
            {
                return bytesFromBuffer;
            }

            int alreadySatisfied = bytesFromBuffer;
            if (bytesFromBuffer > 0)
            {
                count -= bytesFromBuffer;
                offset += bytesFromBuffer;
            }

            // So the READ buffer is empty.
            Contract.Assert(_readLen == _readPos);
            _readPos = _readLen = 0;

            using (TaskHelpers.RunTaskContinuationsOnOurThreads())
            {
                // If the requested read is larger than buffer size, avoid the buffer and still use a single read:
                if (count >= _bufferSize)
                {
                    return _stream.Read(array, offset, count) + alreadySatisfied;
                }

                // Ok. We can fill the buffer:
                _readLen = _stream.Read(_buffer, 0, _bufferSize);
            }

            bytesFromBuffer = ReadFromBuffer(array, offset, count);

            // We may have read less than the number of bytes the user asked for, but that is part of the Stream contract.
            // Reading again for more data may cause us to block if we're using a device with no clear end of stream,
            // such as a serial port or pipe.  If we blocked here & this code was used with redirected pipes for a process's
            // standard output, this can lead to deadlocks involving two processes. Additionally, translating one read on the
            // BufferedStream to more than one read on the underlying Stream may defeat the whole purpose of buffering if the
            // underlying reads are significantly more expensive.

            return bytesFromBuffer + alreadySatisfied;
        }

        private Task<int> LastSyncCompletedReadTask(int val)
        {
            Task<int> t = _lastSyncCompletedReadTask;
            Contract.Assert(t == null || t.Status == TaskStatus.RanToCompletion);

            if (t != null && t.Result == val)
            {
                return t;
            }

            t = Task.FromResult(val);
            _lastSyncCompletedReadTask = t;
            return t;
        }

        public async Task PreReadBufferAsync(byte preBufferedByte, CancellationToken cancellationToken)
        {
            Contract.Assert(_readPos == _readLen, "Buffer must be empty");
            _buffer[0] = preBufferedByte;
            _readLen = 1 + await _stream.ReadAsync(_buffer, 1, _bufferSize - 1, cancellationToken);
            _readPos = 0;
        }

        public Task PreReadBufferAsync(CancellationToken cancellationToken)
        {
            if (IsBufferEmpty || _readLen < _bufferSize)
            {
                return PreReadBufferAsyncInternal(cancellationToken);
            }

            return Task.CompletedTask;
        }

        private async Task PreReadBufferAsyncInternal(CancellationToken cancellationToken)
        {
            Contract.Assert(IsBufferEmpty || _readLen < _bufferSize);
            if (IsBufferEmpty)
            {
                _readLen = await _stream.ReadAsync(_buffer, 0, _bufferSize, cancellationToken);
                _readPos = 0;
            }
            else
            {
                _readLen += await _stream.ReadAsync(_buffer, _readLen, _bufferSize - _readLen, cancellationToken);
            }
        }

        public bool IsBufferEmpty { get { return _readPos == _readLen; } }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Contract.Assert(buffer != null);
            Contract.Assert(offset >= 0);
            Contract.Assert(count >= 0);
            Contract.Assert(count <= buffer.Length - offset);
            // Fast path check for cancellation already requested
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<int>(cancellationToken);
            }

            EnsureNotClosed();
            EnsureCanRead();

            int bytesFromBuffer = 0;
            // Try to satisfy the request from the buffer synchronously. But still need a sem-lock in case that another
            // Async IO Task accesses the buffer concurrently. If we fail to acquire the lock without waiting, make this 
            // an Async operation.
            Task semaphoreLockTask = _sem.WaitAsync(cancellationToken);
            if (semaphoreLockTask.Status == TaskStatus.RanToCompletion)
            {
                bool completeSynchronously = true;
                try
                {
                    Exception error;
                    bytesFromBuffer = ReadFromBuffer(buffer, offset, count, out error);

                    // If we satistied enough data from the buffer, we can complete synchronously.
                    // Reading again for more data may cause us to block if we're using a device with no clear end of file,
                    // such as a serial port or pipe. If we blocked here and this code was used with redirected pipes for a
                    // process's standard output, this can lead to deadlocks involving two processes.              
                    // BUT - this is a breaking change. 
                    // So: If we could not read all bytes the user asked for from the buffer, we will try once from the underlying
                    // stream thus ensuring the same blocking behaviour as if the underlying stream was not wrapped in this BufferedStream.
                    completeSynchronously = (bytesFromBuffer == count || error != null);

                    if (completeSynchronously)
                    {
                        return (error == null)
                                    ? LastSyncCompletedReadTask(bytesFromBuffer)
                                    : Task.FromException<int>(error);
                    }
                }
                finally
                {
                    if (completeSynchronously)  // if this is FALSE, we will be entering ReadFromUnderlyingStreamAsync and releasing there.
                    {
                        _sem.Release();
                    }
                }
            }

            // Delegate to the async implementation.
            return ReadFromUnderlyingStreamAsync(buffer, offset + bytesFromBuffer, count - bytesFromBuffer, cancellationToken,
                                                 bytesFromBuffer, semaphoreLockTask);
        }

        /// <summary>BufferedStream should be as thin a wrapper as possible. We want that ReadAsync delegates to
        /// ReadAsync of the underlying _stream and that BeginRead delegates to BeginRead of the underlying stream,
        /// rather than calling the base Stream which implements the one in terms of the other. This allows BufferedStream
        /// to affect the semantics of the stream it wraps as little as possible. At the same time, we want to share as
        /// much code between the APM and the Async pattern implementations as possible. This method is called by both with
        /// a corresponding useApmPattern value. Recall that Task implements IAsyncResult.</summary>
        /// <returns>-2 if _bufferSize was set to 0 while waiting on the semaphore; otherwise num of bytes read.</returns>
        private async Task<int> ReadFromUnderlyingStreamAsync(Byte[] array, int offset, int count,
                                                                CancellationToken cancellationToken,
                                                                int bytesAlreadySatisfied,
                                                                Task semaphoreLockTask)
        {
            // Employ async waiting based on the same synchronization used in BeginRead of the abstract Stream.        
            await semaphoreLockTask.ConfigureAwait(false);
            try
            {
                // The buffer might have been changed by another async task while we were waiting on the semaphore.
                // Check it now again.            
                int bytesFromBuffer = ReadFromBuffer(array, offset, count);
                if (bytesFromBuffer == count)
                {
                    return bytesAlreadySatisfied + bytesFromBuffer;
                }

                if (bytesFromBuffer > 0)
                {
                    count -= bytesFromBuffer;
                    offset += bytesFromBuffer;
                    bytesAlreadySatisfied += bytesFromBuffer;
                }

                Contract.Assert(_readLen == _readPos);
                _readPos = _readLen = 0;

                // If the requested read is larger than buffer size, avoid the buffer and still use a single read:
                if (count >= _bufferSize)
                {
                    return bytesAlreadySatisfied + await _stream.ReadAsync(array, offset, count, cancellationToken).ConfigureAwait(false);
                }

                // Ok. We can fill the buffer:
                _readLen = await _stream.ReadAsync(_buffer, 0, _bufferSize, cancellationToken).ConfigureAwait(false);

                bytesFromBuffer = ReadFromBuffer(array, offset, count);
                return bytesAlreadySatisfied + bytesFromBuffer;
            }
            finally
            {
                _sem.Release();
            }
        }

        public override int ReadByte()
        {
            EnsureNotClosed();
            EnsureCanRead();

            if (_readPos == _readLen)
            {
                using (TaskHelpers.RunTaskContinuationsOnOurThreads())
                {
                    _readLen = _stream.Read(_buffer, 0, _bufferSize);
                }
                _readPos = 0;
            }

            if (_readPos == _readLen)
            {
                return -1;
            }

            int b = _buffer[_readPos++];
            return b;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException(nameof(Write));
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotSupportedException(nameof(WriteAsync));
        }

        public override void WriteByte(byte value)
        {
            throw new NotSupportedException(nameof(WriteByte));
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(nameof(Seek));
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException(nameof(SetLength));
        }
    }
}