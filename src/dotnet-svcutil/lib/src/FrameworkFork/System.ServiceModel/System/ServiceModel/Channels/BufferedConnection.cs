// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class BufferedConnection : DelegatingConnection
    {
        private byte[] _writeBuffer;
        private int _writeBufferSize;
        private int _pendingWriteSize;
        private Exception _pendingWriteException;
        private Timer _flushTimer;
        private TimeSpan _flushTimeout;
        private TimeSpan _pendingTimeout;
        private const int maxFlushSkew = 100;

        public BufferedConnection(IConnection connection, TimeSpan flushTimeout, int writeBufferSize)
            : base(connection)
        {
            _flushTimeout = flushTimeout;
            _writeBufferSize = writeBufferSize;
        }

        private object ThisLock
        {
            get { return this; }
        }

        public override void Close(TimeSpan timeout, bool asyncAndLinger)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            Flush(timeoutHelper.RemainingTime());
            base.Close(timeoutHelper.RemainingTime(), asyncAndLinger);
        }

        private void CancelFlushTimer()
        {
            if (_flushTimer != null)
            {
                _flushTimer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
                _pendingTimeout = TimeSpan.Zero;
            }
        }

        private void Flush(TimeSpan timeout)
        {
            ThrowPendingWriteException();

            lock (ThisLock)
            {
                FlushCore(timeout);
            }
        }

        private void FlushCore(TimeSpan timeout)
        {
            if (_pendingWriteSize > 0)
            {
                Connection.Write(_writeBuffer, 0, _pendingWriteSize, false, timeout);
                _pendingWriteSize = 0;
            }
        }

        private void OnFlushTimer(object state)
        {
            lock (ThisLock)
            {
                try
                {
                    FlushCore(_pendingTimeout);
                    _pendingTimeout = TimeSpan.Zero;
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    _pendingWriteException = e;
                    CancelFlushTimer();
                }
            }
        }

        private void SetFlushTimer()
        {
            if (_flushTimer == null)
            {
                _flushTimer = new Timer(new TimerCallback(new Action<object>(OnFlushTimer)), null, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
            }

            _flushTimer.Change(_flushTimeout, TimeSpan.FromMilliseconds(-1));
        }

        public override void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout, BufferManager bufferManager)
        {
            if (size <= 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("size", size, SRServiceModel.ValueMustBePositive));
            }

            ThrowPendingWriteException();

            if (immediate || _flushTimeout == TimeSpan.Zero)
            {
                WriteNow(buffer, offset, size, timeout, bufferManager);
            }
            else
            {
                WriteLater(buffer, offset, size, timeout);
                bufferManager.ReturnBuffer(buffer);
            }
        }

        public override void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout)
        {
            if (size <= 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("size", size, SRServiceModel.ValueMustBePositive));
            }

            ThrowPendingWriteException();

            if (immediate || _flushTimeout == TimeSpan.Zero)
            {
                WriteNow(buffer, offset, size, timeout);
            }
            else
            {
                WriteLater(buffer, offset, size, timeout);
            }
        }

        private void WriteNow(byte[] buffer, int offset, int size, TimeSpan timeout)
        {
            WriteNow(buffer, offset, size, timeout, null);
        }

        private void WriteNow(byte[] buffer, int offset, int size, TimeSpan timeout, BufferManager bufferManager)
        {
            lock (ThisLock)
            {
                if (_pendingWriteSize > 0)
                {
                    int remainingSize = _writeBufferSize - _pendingWriteSize;
                    CancelFlushTimer();
                    if (size <= remainingSize)
                    {
                        Buffer.BlockCopy(buffer, offset, _writeBuffer, _pendingWriteSize, size);
                        if (bufferManager != null)
                        {
                            bufferManager.ReturnBuffer(buffer);
                        }
                        _pendingWriteSize += size;
                        FlushCore(timeout);
                        return;
                    }
                    else
                    {
                        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                        FlushCore(timeoutHelper.RemainingTime());
                        timeout = timeoutHelper.RemainingTime();
                    }
                }

                if (bufferManager == null)
                {
                    Connection.Write(buffer, offset, size, true, timeout);
                }
                else
                {
                    Connection.Write(buffer, offset, size, true, timeout, bufferManager);
                }
            }
        }

        private void WriteLater(byte[] buffer, int offset, int size, TimeSpan timeout)
        {
            lock (ThisLock)
            {
                bool setTimer = (_pendingWriteSize == 0);
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

                while (size > 0)
                {
                    if (size >= _writeBufferSize && _pendingWriteSize == 0)
                    {
                        Connection.Write(buffer, offset, size, false, timeoutHelper.RemainingTime());
                        size = 0;
                    }
                    else
                    {
                        if (_writeBuffer == null)
                        {
                            _writeBuffer = Fx.AllocateByteArray(_writeBufferSize);
                        }

                        int remainingSize = _writeBufferSize - _pendingWriteSize;
                        int copySize = size;
                        if (copySize > remainingSize)
                        {
                            copySize = remainingSize;
                        }

                        Buffer.BlockCopy(buffer, offset, _writeBuffer, _pendingWriteSize, copySize);
                        _pendingWriteSize += copySize;
                        if (_pendingWriteSize == _writeBufferSize)
                        {
                            FlushCore(timeoutHelper.RemainingTime());
                            setTimer = true;
                        }
                        size -= copySize;
                        offset += copySize;
                    }
                }
                if (_pendingWriteSize > 0)
                {
                    if (setTimer)
                    {
                        SetFlushTimer();
                        _pendingTimeout = TimeoutHelper.Add(_pendingTimeout, timeoutHelper.RemainingTime());
                    }
                }
                else
                {
                    CancelFlushTimer();
                }
            }
        }

        public override AsyncCompletionResult BeginWrite(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout, Action<object> callback, object state)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            Flush(timeoutHelper.RemainingTime());
            return base.BeginWrite(buffer, offset, size, immediate, timeoutHelper.RemainingTime(), callback, state);
        }

        public override void EndWrite()
        {
            base.EndWrite();
        }

        private void ThrowPendingWriteException()
        {
            if (_pendingWriteException != null)
            {
                lock (ThisLock)
                {
                    if (_pendingWriteException != null)
                    {
                        Exception exceptionTothrow = _pendingWriteException;
                        _pendingWriteException = null;
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exceptionTothrow);
                    }
                }
            }
        }
    }

    internal class BufferedConnectionInitiator : IConnectionInitiator
    {
        private int _writeBufferSize;
        private TimeSpan _flushTimeout;
        private IConnectionInitiator _connectionInitiator;

        public BufferedConnectionInitiator(IConnectionInitiator connectionInitiator, TimeSpan flushTimeout, int writeBufferSize)
        {
            _connectionInitiator = connectionInitiator;
            _flushTimeout = flushTimeout;
            _writeBufferSize = writeBufferSize;
        }

        protected TimeSpan FlushTimeout
        {
            get
            {
                return _flushTimeout;
            }
        }

        protected int WriteBufferSize
        {
            get
            {
                return _writeBufferSize;
            }
        }

        public IConnection Connect(Uri uri, TimeSpan timeout)
        {
            return new BufferedConnection(_connectionInitiator.Connect(uri, timeout), _flushTimeout, _writeBufferSize);
        }

        public async Task<IConnection> ConnectAsync(Uri uri, TimeSpan timeout)
        {
            return new BufferedConnection(await _connectionInitiator.ConnectAsync(uri, timeout), _flushTimeout, _writeBufferSize);
        }
    }
}
