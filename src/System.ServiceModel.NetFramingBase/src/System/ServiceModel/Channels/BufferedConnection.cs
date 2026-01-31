// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class BufferedConnection : DelegatingConnection
    {
        private Memory<byte> _writeBuffer;
        private int _writeBufferSize;
        private int _pendingWriteSize;
        private Exception _pendingWriteException;
        private Timer _flushTimer;
        private long _flushTimeout;
        private TimeSpan _pendingTimeout;
        private const int MaxFlushSkew = 100;

        public BufferedConnection(IConnection connection, TimeSpan flushTimeout, int writeBufferSize)
            : base(connection)
        {
            _flushTimeout = Ticks.FromTimeSpan(flushTimeout);
            _writeBufferSize = writeBufferSize;
        }

        private SemaphoreSlim ThisLock { get; } = new SemaphoreSlim(1);

        private void CancelFlushTimer()
        {
            if (_flushTimer != null)
            {
                _flushTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _pendingTimeout = TimeSpan.Zero;
            }
        }

        private async ValueTask FlushAsync(TimeSpan timeout)
        {
            ThrowPendingWriteException();

            await ThisLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await FlushCoreAsync(timeout).ConfigureAwait(false);
            }
            finally
            {
                ThisLock.Release();
            }
        }

        private async ValueTask FlushCoreAsync(TimeSpan timeout)
        {
            if (_pendingWriteSize > 0)
            {
                await Connection.WriteAsync(_writeBuffer.Slice(0, _pendingWriteSize), false, timeout).ConfigureAwait(false);
                _pendingWriteSize = 0;
            }
        }

        private static void OnFlushTimer(object state)
        {
            var bufferedConnection = (BufferedConnection)state;
            bufferedConnection.OnFlushTimerCore();
        }

        private async void OnFlushTimerCore()
        {
            await ThisLock.WaitAsync().ConfigureAwait(false);
            try
            {
                try
                {
                    await FlushCoreAsync(_pendingTimeout).ConfigureAwait(false);
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
            finally
            {
                ThisLock.Release();
            }
        }

        private void SetFlushTimer()
        {
            if (_flushTimer == null)
            {
                _flushTimer = new Timer(new TimerCallback(OnFlushTimer), this, _flushTimeout, Timeout.Infinite);
            }
            else
            {
                _flushTimer.Change(_flushTimeout, Timeout.Infinite);
            }
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, bool immediate, TimeSpan timeout)
        {
            if (buffer.IsEmpty)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(buffer.Length), buffer.Length, SR.Format(
                    SR.ValueMustBePositive)));
            }

            ThrowPendingWriteException();

            if (immediate || _flushTimeout == 0)
            {
                return WriteNowAsync(buffer, timeout);
            }
            else
            {
                return WriteLaterAsync(buffer, timeout);
            }

        }

        private async ValueTask WriteNowAsync(ReadOnlyMemory<byte> buffer, TimeSpan timeout)
        {
            await ThisLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_pendingWriteSize > 0)
                {
                    int remainingSize = _writeBufferSize - _pendingWriteSize;
                    CancelFlushTimer();
                    if (buffer.Length <= remainingSize)
                    {
                        buffer.CopyTo(_writeBuffer.Slice(_pendingWriteSize));
                        _pendingWriteSize += buffer.Length;
                        await FlushCoreAsync(timeout).ConfigureAwait(false);
                        return;
                    }
                    else
                    {
                        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                        await FlushCoreAsync(timeoutHelper.RemainingTime()).ConfigureAwait(false);
                        timeout = timeoutHelper.RemainingTime();
                    }
                }

                await Connection.WriteAsync(buffer, true, timeout).ConfigureAwait(false);
            }
            finally
            {
                ThisLock.Release();
            }
        }

        private async ValueTask WriteLaterAsync(ReadOnlyMemory<byte> buffer, TimeSpan timeout)
        {
            await ThisLock.WaitAsync().ConfigureAwait(false);
            try
            {
                bool setTimer = _pendingWriteSize == 0;
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

                while (buffer.Length > 0)
                {
                    if (buffer.Length >= _writeBufferSize && _pendingWriteSize == 0)
                    {
                        await Connection.WriteAsync(buffer, false, timeoutHelper.RemainingTime()).ConfigureAwait(false);
                        buffer = default;
                    }
                    else
                    {
                        if (_writeBuffer.IsEmpty)
                        {
                            _writeBuffer = Fx.AllocateByteArray(_writeBufferSize);
                        }

                        int remainingSize = _writeBufferSize - _pendingWriteSize;
                        int copySize = buffer.Length;
                        if (copySize > remainingSize)
                        {
                            copySize = remainingSize;
                        }

                        buffer.CopyTo(_writeBuffer.Slice(_pendingWriteSize, copySize));
                        _pendingWriteSize += copySize;
                        if (_pendingWriteSize == _writeBufferSize)
                        {
                            await FlushCoreAsync(timeoutHelper.RemainingTime()).ConfigureAwait(false);
                            setTimer = true;
                        }
                        buffer = buffer.Slice(copySize);
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
            finally
            {
                ThisLock.Release();
            }
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

        protected TimeSpan FlushTimeout => _flushTimeout;

        protected int WriteBufferSize => _writeBufferSize;

        public async ValueTask<IConnection> ConnectAsync(Uri uri, TimeSpan timeout)
        {
            return new BufferedConnection(await _connectionInitiator.ConnectAsync(uri, timeout).ConfigureAwait(false), _flushTimeout, _writeBufferSize);
        }
    }
}
