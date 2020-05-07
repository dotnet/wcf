// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    /// <summary>
    /// This class is based on BufferedStream from the Desktop version of .Net. Only the write functionality
    /// is needed by WCF so the read capability has been removed. This allowed some extra logic to be removed
    /// from the write code path. Also some validation code has been removed as this class is no longer
    /// general purpose and is only used in pre-known scenarios and only called by WCF code. Some validation
    /// checks have been converted to only run on a debug build to allow catching code bugs in other WCF code, 
    /// but not causing release build overhead.
    /// 
    /// One of the design goals here is to prevent the buffer from getting in the way and slowing
    /// down underlying stream accesses when it is not needed.
    /// See a large comment in Write for the details of the write buffer heuristic.
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
    internal sealed class BufferedWriteStream : Stream
    {
        public const int DefaultBufferSize = 4096;
        private Stream _stream; // Underlying stream.  Close sets _stream to null.
        private byte[] _buffer; // Wwrite buffer.
        private readonly int _bufferSize; // Length of internal buffer (not counting the shadow buffer).
        private int _writePos; // Write pointer within buffer.
        private readonly SemaphoreSlim _sem = new SemaphoreSlim(1, 1);

        public BufferedWriteStream(Stream stream) : this(stream, DefaultBufferSize) { }

        public BufferedWriteStream(Stream stream, int bufferSize)
        {
            Contract.Assert(stream != Null, "stream!=Null");
            Contract.Assert(bufferSize > 0, "bufferSize>0");
            Contract.Assert(stream.CanWrite);
            _stream = stream;
            _bufferSize = bufferSize;

            EnsureBufferAllocated();
        }

        private void EnsureNotClosed()
        {
            if (_stream == null)
                throw new ObjectDisposedException("BufferedWriteStream");
        }

        private void EnsureCanWrite()
        {
            Contract.Requires(_stream != null);
            if (!_stream.CanWrite)
                throw new NotSupportedException("write");
        }

        /// <summary><code>MaxShadowBufferSize</code> is chosen such that shadow buffers are not allocated on the Large Object Heap.
        /// Currently, an object is allocated on the LOH if it is larger than 85000 bytes.
        /// We will go with exactly 80 KBytes, although this is somewhat arbitrary.</summary>
        private const int MaxShadowBufferSize = 81920; // Make sure not to get to the Large Object Heap.

        private void EnsureShadowBufferAllocated()
        {
            Contract.Assert(_buffer != null);
            Contract.Assert(_bufferSize > 0);

            // Already have shadow buffer?
            if (_buffer.Length != _bufferSize || _bufferSize >= MaxShadowBufferSize)
            {
                return;
            }

            byte[] shadowBuffer = new byte[Math.Min(_bufferSize + _bufferSize, MaxShadowBufferSize)];
            Array.Copy(_buffer, 0, shadowBuffer, 0, _writePos);
            _buffer = shadowBuffer;
        }

        private void EnsureBufferAllocated()
        {
            if (_buffer == null)
                _buffer = new byte[_bufferSize];
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return _stream != null && _stream.CanWrite; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override long Length
        {
            get { throw new NotSupportedException("Position"); }
        }

        public override long Position
        {
            get { throw new NotSupportedException("Position"); }
            set { throw new NotSupportedException("Position"); }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && _stream != null)
                {
                    try
                    {
                        Flush();
                    }
                    finally
                    {
                        _stream.Dispose();
                    }
                }
            }
            finally
            {
                _stream = null;
                _buffer = null;

                // Call base.Dispose(bool) to cleanup async IO resources
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            EnsureNotClosed();

            // Has WRITE data in the buffer:
            if (_writePos > 0)
            {
                FlushWrite();
                Contract.Assert(_writePos == 0);
                return;
            }

            // We had no data in the buffer, but we still need to tell the underlying stream to flush.
            _stream.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<int>(cancellationToken);
            }

            EnsureNotClosed();

            return FlushAsyncInternal(cancellationToken);
        }

        private async Task FlushAsyncInternal(CancellationToken cancellationToken)
        {
            await _sem.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_writePos > 0)
                {
                    await FlushWriteAsync(cancellationToken).ConfigureAwait(false);
                    Contract.Assert(_writePos == 0);
                    return;
                }

                // We had no data in the buffer, but we still need to tell the underlying stream to flush.
                await _stream.FlushAsync(cancellationToken).ConfigureAwait(false);

                // There was nothing in the buffer:
                Contract.Assert(_writePos == 0);
            }
            finally
            {
                _sem.Release();
            }
        }

        private void FlushWrite()
        {
            Contract.Assert(_buffer != null && _bufferSize >= _writePos,
                "BufferedWriteStream: Write buffer must be allocated and write position must be in the bounds of the buffer in FlushWrite!");

            _stream.Write(_buffer, 0, _writePos);
            _writePos = 0;
            _stream.Flush();
        }

        private async Task FlushWriteAsync(CancellationToken cancellationToken)
        {
            Contract.Assert(_buffer != null && _bufferSize >= _writePos,
                "BufferedWriteStream: Write buffer must be allocated and write position must be in the bounds of the buffer in FlushWrite!");

            await _stream.WriteAsync(_buffer, 0, _writePos, cancellationToken).ConfigureAwait(false);
            _writePos = 0;
            await _stream.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        public override int Read([In, Out] byte[] array, int offset, int count)
        {
            throw new NotSupportedException("Read");
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotSupportedException("ReadAsync");
        }

        public override int ReadByte()
        {
            throw new NotSupportedException("ReadByte");
        }

        private void WriteToBuffer(byte[] array, ref int offset, ref int count)
        {
            int bytesToWrite = Math.Min(_bufferSize - _writePos, count);

            if (bytesToWrite <= 0)
            {
                return;
            }

            Array.Copy(array, offset, _buffer, _writePos, bytesToWrite);

            _writePos += bytesToWrite;
            count -= bytesToWrite;
            offset += bytesToWrite;
        }

        private void WriteToBuffer(byte[] array, ref int offset, ref int count, out Exception error)
        {
            try
            {
                error = null;
                WriteToBuffer(array, ref offset, ref count);
            }
            catch (Exception ex)
            {
                error = ex;
            }
        }

        public override void Write(byte[] array, int offset, int count)
        {
            Contract.Assert(array != null);
            Contract.Assert(offset >= 0);
            Contract.Assert(count >= 0);
            Contract.Assert(count <= array.Length - offset);

            EnsureNotClosed();
            EnsureCanWrite();

            #region Write algorithm comment
            // We need to use the buffer, while avoiding unnecessary buffer usage / memory copies.
            // We ASSUME that memory copies are much cheaper than writes to the underlying stream, so if an extra copy is
            // guaranteed to reduce the number of writes, we prefer it.
            // We pick a simple strategy that makes degenerate cases rare if our assumptions are right.
            //
            // For every write, we use a simple heuristic (below) to decide whether to use the buffer.
            // The heuristic has the desirable property (*) that if the specified user data can fit into the currently available
            // buffer space without filling it up completely, the heuristic will always tell us to use the buffer. It will also
            // tell us to use the buffer in cases where the current write would fill the buffer, but the remaining data is small
            // enough such that subsequent operations can use the buffer again.
            // 
            // Algorithm:
            // Determine whether or not to buffer according to the heuristic (below).
            // If we decided to use the buffer:
            //     Copy as much user data as we can into the buffer.
            //     If we consumed all data: We are finished.
            //     Otherwise, write the buffer out.
            //     Copy the rest of user data into the now cleared buffer (no need to write out the buffer again as the heuristic
            //     will prevent it from being filled twice).
            // If we decided not to use the buffer:
            //     Can the data already in the buffer and current user data be combines to a single write
            //     by allocating a "shadow" buffer of up to twice the size of _bufferSize (up to a limit to avoid LOH)?
            //     Yes, it can:
            //         Allocate a larger "shadow" buffer and ensure the buffered  data is moved there.
            //         Copy user data to the shadow buffer.
            //         Write shadow buffer to the underlying stream in a single operation.
            //     No, it cannot (amount of data is still too large):
            //         Write out any data possibly in the buffer.
            //         Write out user data directly.
            //
            // Heuristic:
            // If the subsequent write operation that follows the current write operation will result in a write to the
            // underlying stream in case that we use the buffer in the current write, while it would not have if we avoided
            // using the buffer in the current write (by writing current user data to the underlying stream directly), then we
            // prefer to avoid using the buffer since the corresponding memory copy is wasted (it will not reduce the number
            // of writes to the underlying stream, which is what we are optimising for).
            // ASSUME that the next write will be for the same amount of bytes as the current write (most common case) and
            // determine if it will cause a write to the underlying stream. If the next write is actually larger, our heuristic
            // still yields the right behaviour, if the next write is actually smaller, we may making an unnecessary write to
            // the underlying stream. However, this can only occur if the current write is larger than half the buffer size and
            // we will recover after one iteration.
            // We have:
            //     useBuffer = (_writePos + count + count < _bufferSize + _bufferSize)
            //
            // Example with _bufferSize = 20, _writePos = 6, count = 10:
            //
            //     +---------------------------------------+---------------------------------------+
            //     |             current buffer            | next iteration's "future" buffer      |
            //     +---------------------------------------+---------------------------------------+ 
            //     |0| | | | | | | | | |1| | | | | | | | | |2| | | | | | | | | |3| | | | | | | | | |
            //     |0|1|2|3|4|5|6|7|8|9|0|1|2|3|4|5|6|7|8|9|0|1|2|3|4|5|6|7|8|9|0|1|2|3|4|5|6|7|8|9|
            //     +-----------+-------------------+-------------------+---------------------------+
            //     | _writePos |  current count    | assumed next count|avail buff after next write|
            //     +-----------+-------------------+-------------------+---------------------------+
            //
            // A nice property (*) of this heuristic is that it will always succeed if the user data completely fits into the
            // available buffer, i.e. if count < (_bufferSize - _writePos).
            #endregion Write algorithm comment

            Contract.Assert(_writePos < _bufferSize);

            int totalUserBytes;
            bool useBuffer;
            checked
            {
                // We do not expect buffer sizes big enough for an overflow, but if it happens, lets fail early:
                totalUserBytes = _writePos + count;
                useBuffer = (totalUserBytes + count < (_bufferSize + _bufferSize));
            }

            if (useBuffer)
            {
                WriteToBuffer(array, ref offset, ref count);

                if (_writePos < _bufferSize)
                {
                    Contract.Assert(count == 0);
                    return;
                }

                Contract.Assert(count >= 0);
                Contract.Assert(_writePos == _bufferSize);
                Contract.Assert(_buffer != null);

                _stream.Write(_buffer, 0, _writePos);
                _writePos = 0;

                WriteToBuffer(array, ref offset, ref count);

                Contract.Assert(count == 0);
                Contract.Assert(_writePos < _bufferSize);
            }
            else
            {
                // if (!useBuffer)
                // Write out the buffer if necessary.
                if (_writePos > 0)
                {
                    Contract.Assert(_buffer != null);
                    Contract.Assert(totalUserBytes >= _bufferSize);

                    // Try avoiding extra write to underlying stream by combining previously buffered data with current user data:
                    if (totalUserBytes <= (_bufferSize + _bufferSize) && totalUserBytes <= MaxShadowBufferSize)
                    {
                        EnsureShadowBufferAllocated();
                        Array.Copy(array, offset, _buffer, _writePos, count);
                        _stream.Write(_buffer, 0, totalUserBytes);
                        _writePos = 0;
                        return;
                    }

                    _stream.Write(_buffer, 0, _writePos);
                    _writePos = 0;
                }

                // Write out user data.
                _stream.Write(array, offset, count);
            }
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Contract.Assert(buffer != null);
            Contract.Assert(offset >= 0);
            Contract.Assert(count >= 0);
            Contract.Assert(count <= buffer.Length - offset);
            // Fast path check for cancellation already requested
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<int>(cancellationToken);

            EnsureNotClosed();
            EnsureCanWrite();

            // Try to satisfy the request from the buffer synchronously. But still need a sem-lock in case that another
            // Async IO Task accesses the buffer concurrently. If we fail to acquire the lock without waiting, make this 
            // an Async operation.
            Task semaphoreLockTask = _sem.WaitAsync();
            if (semaphoreLockTask.Status == TaskStatus.RanToCompletion)
            {
                bool completeSynchronously = true;
                try
                {
                    Contract.Assert(_writePos < _bufferSize);

                    // If the write completely fits into the buffer, we can complete synchronously:
                    completeSynchronously = (count < _bufferSize - _writePos);

                    if (completeSynchronously)
                    {
                        Exception error;
                        WriteToBuffer(buffer, ref offset, ref count, out error);
                        Contract.Assert(count == 0);

                        return (error == null)
                                   ? Task.CompletedTask
                                   : Task.FromException(error);
                    }
                }
                finally
                {
                    if (completeSynchronously)
                    {
                        // if this is FALSE, we will be entering WriteToUnderlyingStreamAsync and releasing there.
                        _sem.Release();
                    }
                }
            }

            // Delegate to the async implementation.
            return WriteToUnderlyingStreamAsync(buffer, offset, count, cancellationToken, semaphoreLockTask);
        }

        private async Task WriteToUnderlyingStreamAsync(byte[] array, int offset, int count,
                                                        CancellationToken cancellationToken,
                                                        Task semaphoreLockTask)
        {
            // (These should be Contract.Requires(..) but that method had some issues in async methods; using Assert(..) for now.)
            EnsureNotClosed();
            EnsureCanWrite();

            // See the LARGE COMMENT in Write(..) for the explanation of the write buffer algorithm.

            await semaphoreLockTask.ConfigureAwait(false);
            try
            {
                // The buffer might have been changed by another async task while we were waiting on the semaphore.
                // However, note that if we recalculate the sync completion condition to TRUE, then useBuffer will also be TRUE.
                int totalUserBytes;
                bool useBuffer;
                checked
                {
                    // We do not expect buffer sizes big enough for an overflow, but if it happens, lets fail early:
                    totalUserBytes = _writePos + count;
                    useBuffer = (totalUserBytes + count < (_bufferSize + _bufferSize));
                }

                if (useBuffer)
                {
                    WriteToBuffer(array, ref offset, ref count);

                    if (_writePos < _bufferSize)
                    {
                        Contract.Assert(count == 0);
                        return;
                    }

                    Contract.Assert(count >= 0);
                    Contract.Assert(_writePos == _bufferSize);
                    Contract.Assert(_buffer != null);

                    await _stream.WriteAsync(_buffer, 0, _writePos, cancellationToken).ConfigureAwait(false);
                    _writePos = 0;

                    WriteToBuffer(array, ref offset, ref count);

                    Contract.Assert(count == 0);
                    Contract.Assert(_writePos < _bufferSize);
                }
                else
                {
                    // if (!useBuffer)

                    // Write out the buffer if necessary.
                    if (_writePos > 0)
                    {
                        Contract.Assert(_buffer != null);
                        Contract.Assert(totalUserBytes >= _bufferSize);

                        // Try avoiding extra write to underlying stream by combining previously buffered data with current user data:
                        if (totalUserBytes <= (_bufferSize + _bufferSize) && totalUserBytes <= MaxShadowBufferSize)
                        {
                            EnsureShadowBufferAllocated();
                            Buffer.BlockCopy(array, offset, _buffer, _writePos, count);
                            await _stream.WriteAsync(_buffer, 0, totalUserBytes, cancellationToken).ConfigureAwait(false);
                            _writePos = 0;
                            return;
                        }

                        await _stream.WriteAsync(_buffer, 0, _writePos, cancellationToken).ConfigureAwait(false);
                        _writePos = 0;
                    }

                    // Write out user data.
                    await _stream.WriteAsync(array, offset, count, cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                _sem.Release();
            }
        }

        public override void WriteByte(byte value)
        {
            EnsureNotClosed();

            // We should not be flushing here, but only writing to the underlying stream, but previous version flushed, so we keep this.
            if (_writePos >= _bufferSize - 1)
            {
                FlushWrite();
            }

            _buffer[_writePos++] = value;

            Contract.Assert(_writePos < _bufferSize);
        }


        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("seek");
        }


        public override void SetLength(long value)
        {
            throw new NotSupportedException("SetLength");
        }
    }
}
