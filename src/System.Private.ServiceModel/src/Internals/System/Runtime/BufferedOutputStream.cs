// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IO;

namespace System.Runtime
{
    internal class BufferedOutputStream : Stream
    {
        [Fx.Tag.Cache(typeof(byte), Fx.Tag.CacheAttrition.None, Scope = Fx.Tag.Strings.ExternallyManaged,
                    SizeLimit = Fx.Tag.Strings.ExternallyManaged)]
        private InternalBufferManager _bufferManager;
        [Fx.Tag.Queue(typeof(byte), SizeLimit = "BufferedOutputStream(maxSize)",
                    StaleElementsRemovedImmediately = true, EnqueueThrowsIfFull = true)]

        private byte[][] _chunks;

        private int _chunkCount;
        private byte[] _currentChunk;
        private int _currentChunkSize;
        private int _maxSize;
        private int _maxSizeQuota;
        private int _totalSize;
        private bool _callerReturnsBuffer;
        private bool _bufferReturned;
        private bool _initialized;

        // requires an explicit call to Init() by the caller
        public BufferedOutputStream()
        {
            _chunks = new byte[4][];
        }

        public BufferedOutputStream(int initialSize, int maxSize, InternalBufferManager bufferManager)
            : this()
        {
            Reinitialize(initialSize, maxSize, bufferManager);
        }

        public BufferedOutputStream(int maxSize)
            : this(0, maxSize, InternalBufferManager.Create(0, int.MaxValue))
        {
        }

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Length
        {
            get
            {
                return _totalSize;
            }
        }

        public override long Position
        {
            get
            {
                throw Fx.Exception.AsError(new NotSupportedException(InternalSR.SeekNotSupported));
            }
            set
            {
                throw Fx.Exception.AsError(new NotSupportedException(InternalSR.SeekNotSupported));
            }
        }

        public void Reinitialize(int initialSize, int maxSizeQuota, InternalBufferManager bufferManager)
        {
            Reinitialize(initialSize, maxSizeQuota, maxSizeQuota, bufferManager);
        }

        public void Reinitialize(int initialSize, int maxSizeQuota, int effectiveMaxSize, InternalBufferManager bufferManager)
        {
            Fx.Assert(!_initialized, "Clear must be called before re-initializing stream");
            _maxSizeQuota = maxSizeQuota;
            _maxSize = effectiveMaxSize;
            _bufferManager = bufferManager;
            _currentChunk = bufferManager.TakeBuffer(initialSize);
            _currentChunkSize = 0;
            _totalSize = 0;
            _chunkCount = 1;
            _chunks[0] = _currentChunk;
            _initialized = true;
        }

        private void AllocNextChunk(int minimumChunkSize)
        {
            int newChunkSize;
            if (_currentChunk.Length > (int.MaxValue / 2))
            {
                newChunkSize = int.MaxValue;
            }
            else
            {
                newChunkSize = _currentChunk.Length * 2;
            }
            if (minimumChunkSize > newChunkSize)
            {
                newChunkSize = minimumChunkSize;
            }
            byte[] newChunk = _bufferManager.TakeBuffer(newChunkSize);
            if (_chunkCount == _chunks.Length)
            {
                byte[][] newChunks = new byte[_chunks.Length * 2][];
                Array.Copy(_chunks, newChunks, _chunks.Length);
                _chunks = newChunks;
            }
            _chunks[_chunkCount++] = newChunk;
            _currentChunk = newChunk;
            _currentChunkSize = 0;
        }

#if NETStandard13
        public virtual IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
#else
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
#endif
        {
            throw Fx.Exception.AsError(new NotSupportedException(InternalSR.ReadNotSupported));
        }

#if NETStandard13
        public virtual int EndRead(IAsyncResult result)
#else
        public override int EndRead(IAsyncResult result)
#endif
        {
            throw Fx.Exception.AsError(new NotSupportedException(InternalSR.ReadNotSupported));
        }

#if NETStandard13
        public virtual IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
#else
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
#endif
        {
            Write(buffer, offset, size);
            return new CompletedAsyncResult(callback, state);
        }

#if NETStandard13
        public virtual void EndWrite(IAsyncResult result)
#else
        public override void EndWrite(IAsyncResult result)
#endif
        {
            CompletedAsyncResult.End(result);
        }

        public void Clear()
        {
            if (!_callerReturnsBuffer)
            {
                for (int i = 0; i < _chunkCount; i++)
                {
                    _bufferManager.ReturnBuffer(_chunks[i]);
                    _chunks[i] = null;
                }
            }

            _callerReturnsBuffer = false;
            _initialized = false;
            _bufferReturned = false;
            _chunkCount = 0;
            _currentChunk = null;
        }

#if NETStandard13
        public virtual void Close()
#else
        public override void Close()
#endif
        {
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int size)
        {
            throw Fx.Exception.AsError(new NotSupportedException(InternalSR.ReadNotSupported));
        }

        public override int ReadByte()
        {
            throw Fx.Exception.AsError(new NotSupportedException(InternalSR.ReadNotSupported));
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw Fx.Exception.AsError(new NotSupportedException(InternalSR.SeekNotSupported));
        }

        public override void SetLength(long value)
        {
            throw Fx.Exception.AsError(new NotSupportedException(InternalSR.SeekNotSupported));
        }

        public MemoryStream ToMemoryStream()
        {
            int bufferSize;
            byte[] buffer = ToArray(out bufferSize);
            return new MemoryStream(buffer, 0, bufferSize);
        }

        public byte[] ToArray(out int bufferSize)
        {
            Fx.Assert(_initialized, "No data to return from uninitialized stream");
            Fx.Assert(!_bufferReturned, "ToArray cannot be called more than once");

            byte[] buffer;
            if (_chunkCount == 1)
            {
                buffer = _currentChunk;
                bufferSize = _currentChunkSize;
                _callerReturnsBuffer = true;
            }
            else
            {
                buffer = _bufferManager.TakeBuffer(_totalSize);
                int offset = 0;
                int count = _chunkCount - 1;
                for (int i = 0; i < count; i++)
                {
                    byte[] chunk = _chunks[i];
                    Buffer.BlockCopy(chunk, 0, buffer, offset, chunk.Length);
                    offset += chunk.Length;
                }
                Buffer.BlockCopy(_currentChunk, 0, buffer, offset, _currentChunkSize);
                bufferSize = _totalSize;
            }

            _bufferReturned = true;
            return buffer;
        }

        public void Skip(int size)
        {
            WriteCore(null, 0, size);
        }

        public override void Write(byte[] buffer, int offset, int size)
        {
            WriteCore(buffer, offset, size);
        }

        protected virtual Exception CreateQuotaExceededException(int maxSizeQuota)
        {
            return new InvalidOperationException(InternalSR.BufferedOutputStreamQuotaExceeded(maxSizeQuota));
        }

        private void WriteCore(byte[] buffer, int offset, int size)
        {
            Fx.Assert(_initialized, "Cannot write to uninitialized stream");
            Fx.Assert(!_bufferReturned, "Cannot write to stream once ToArray has been called.");

            if (size < 0)
            {
                throw Fx.Exception.ArgumentOutOfRange("size", size, InternalSR.ValueMustBeNonNegative);
            }

            if ((int.MaxValue - size) < _totalSize)
            {
                throw Fx.Exception.AsError(CreateQuotaExceededException(_maxSizeQuota));
            }

            int newTotalSize = _totalSize + size;
            if (newTotalSize > _maxSize)
            {
                throw Fx.Exception.AsError(CreateQuotaExceededException(_maxSizeQuota));
            }

            int remainingSizeInChunk = _currentChunk.Length - _currentChunkSize;
            if (size > remainingSizeInChunk)
            {
                if (remainingSizeInChunk > 0)
                {
                    if (buffer != null)
                    {
                        Buffer.BlockCopy(buffer, offset, _currentChunk, _currentChunkSize, remainingSizeInChunk);
                    }
                    _currentChunkSize = _currentChunk.Length;
                    offset += remainingSizeInChunk;
                    size -= remainingSizeInChunk;
                }
                AllocNextChunk(size);
            }

            if (buffer != null)
            {
                Buffer.BlockCopy(buffer, offset, _currentChunk, _currentChunkSize, size);
            }
            _totalSize = newTotalSize;
            _currentChunkSize += size;
        }

        public override void WriteByte(byte value)
        {
            Fx.Assert(_initialized, "Cannot write to uninitialized stream");
            Fx.Assert(!_bufferReturned, "Cannot write to stream once ToArray has been called.");

            if (_totalSize == _maxSize)
            {
                throw Fx.Exception.AsError(CreateQuotaExceededException(_maxSize));
            }
            if (_currentChunkSize == _currentChunk.Length)
            {
                AllocNextChunk(1);
            }
            _currentChunk[_currentChunkSize++] = value;
        }
    }
}
