// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.Threading;

namespace System.ServiceModel.Channels
{
    internal enum LifetimeState
    {
        Opened,
        Closing,
        Closed
    }

    internal class LifetimeManager
    {
        private bool _aborted;
        private ICommunicationWaiter _busyWaiter;
        private int _busyWaiterCount;
        private LifetimeState _state;

        public LifetimeManager(object mutex)
        {
            ThisLock = mutex;
            _state = LifetimeState.Opened;
        }

        public int BusyCount { get; private set; }

        protected LifetimeState State
        {
            get { return _state; }
        }

        protected object ThisLock { get; }

        public void Abort()
        {
            lock (ThisLock)
            {
                if (State == LifetimeState.Closed || _aborted)
                {
                    return;
                }

                _aborted = true;
                _state = LifetimeState.Closing;
            }

            OnAbort();
            _state = LifetimeState.Closed;
        }

        private void ThrowIfNotOpened()
        {
            if (!_aborted && _state != LifetimeState.Opened)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(GetType().ToString()));
            }
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            lock (ThisLock)
            {
                ThrowIfNotOpened();
                _state = LifetimeState.Closing;
            }

            return OnBeginClose(timeout, callback, state);
        }

        public void Close(TimeSpan timeout)
        {
            lock (ThisLock)
            {
                ThrowIfNotOpened();
                _state = LifetimeState.Closing;
            }

            OnClose(timeout);
            _state = LifetimeState.Closed;
        }

        private CommunicationWaitResult CloseCore(TimeSpan timeout, bool aborting)
        {
            ICommunicationWaiter busyWaiter = null;
            CommunicationWaitResult result = CommunicationWaitResult.Succeeded;

            lock (ThisLock)
            {
                if (BusyCount > 0)
                {
                    if (_busyWaiter != null)
                    {
                        if (!aborting && _aborted)
                        {
                            return CommunicationWaitResult.Aborted;
                        }

                        busyWaiter = _busyWaiter;
                    }
                    else
                    {
                        busyWaiter = new SyncCommunicationWaiter(ThisLock);
                        _busyWaiter = busyWaiter;
                    }
                    Interlocked.Increment(ref _busyWaiterCount);
                }
            }

            if (busyWaiter != null)
            {
                result = busyWaiter.Wait(timeout, aborting);
                if (Interlocked.Decrement(ref _busyWaiterCount) == 0)
                {
                    busyWaiter.Dispose();
                    _busyWaiter = null;
                }
            }

            return result;
        }

        protected void DecrementBusyCount()
        {
            ICommunicationWaiter busyWaiter = null;
            bool empty = false;

            lock (ThisLock)
            {
                if (BusyCount <= 0)
                {
                    throw Fx.AssertAndThrow("LifetimeManager.DecrementBusyCount: (this.busyCount > 0)");
                }
                if (--BusyCount == 0)
                {
                    if (_busyWaiter != null)
                    {
                        busyWaiter = _busyWaiter;
                        Interlocked.Increment(ref _busyWaiterCount);
                    }
                    empty = true;
                }
            }

            if (busyWaiter != null)
            {
                busyWaiter.Signal();
                if (Interlocked.Decrement(ref _busyWaiterCount) == 0)
                {
                    busyWaiter.Dispose();
                    _busyWaiter = null;
                }
            }

            if (empty && State == LifetimeState.Opened)
            {
                OnEmpty();
            }
        }

        public void EndClose(IAsyncResult result)
        {
            OnEndClose(result);
            _state = LifetimeState.Closed;
        }

        protected virtual void IncrementBusyCount()
        {
            lock (ThisLock)
            {
                Fx.Assert(State == LifetimeState.Opened, "LifetimeManager.IncrementBusyCount: (this.State == LifetimeState.Opened)");
                BusyCount++;
            }
        }

        protected virtual void IncrementBusyCountWithoutLock()
        {
            Fx.Assert(State == LifetimeState.Opened, "LifetimeManager.IncrementBusyCountWithoutLock: (this.State == LifetimeState.Opened)");
            BusyCount++;
        }

        protected virtual void OnAbort()
        {
            // We have decided not to make this configurable
            CloseCore(TimeSpan.FromSeconds(1), true);
        }

        protected virtual IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            CloseCommunicationAsyncResult closeResult = null;

            lock (ThisLock)
            {
                if (BusyCount > 0)
                {
                    if (_busyWaiter != null)
                    {
                        Fx.Assert(_aborted, "LifetimeManager.OnBeginClose: (this.aborted == true)");
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(GetType().ToString()));
                    }
                    else
                    {
                        closeResult = new CloseCommunicationAsyncResult(timeout, callback, state, ThisLock);
                        Fx.Assert(_busyWaiter == null, "LifetimeManager.OnBeginClose: (this.busyWaiter == null)");
                        _busyWaiter = closeResult;
                        Interlocked.Increment(ref _busyWaiterCount);
                    }
                }
            }

            if (closeResult != null)
            {
                return closeResult;
            }
            else
            {
                return new CompletedAsyncResult(callback, state);
            }
        }

        protected virtual void OnClose(TimeSpan timeout)
        {
            switch (CloseCore(timeout, false))
            {
                case CommunicationWaitResult.Expired:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(SR.Format(SR.SFxCloseTimedOut1, timeout)));
                case CommunicationWaitResult.Aborted:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(GetType().ToString()));
            }
        }

        protected virtual void OnEmpty()
        {
        }

        protected virtual void OnEndClose(IAsyncResult result)
        {
            if (result is CloseCommunicationAsyncResult)
            {
                CloseCommunicationAsyncResult.End(result);
                if (Interlocked.Decrement(ref _busyWaiterCount) == 0)
                {
                    _busyWaiter.Dispose();
                    _busyWaiter = null;
                }
            }
            else
            {
                CompletedAsyncResult.End(result);
            }
        }
    }

    internal enum CommunicationWaitResult
    {
        Waiting,
        Succeeded,
        Expired,
        Aborted
    }

    internal interface ICommunicationWaiter : IDisposable
    {
        void Signal();
        CommunicationWaitResult Wait(TimeSpan timeout, bool aborting);
    }

    internal class CloseCommunicationAsyncResult : AsyncResult, ICommunicationWaiter
    {
        private CommunicationWaitResult _result;
        private Timer _timer;
        private TimeoutHelper _timeoutHelper;
        private TimeSpan _timeout;

        public CloseCommunicationAsyncResult(TimeSpan timeout, AsyncCallback callback, object state, object mutex)
            : base(callback, state)
        {
            _timeout = timeout;
            _timeoutHelper = new TimeoutHelper(timeout);
            ThisLock = mutex;

            if (timeout < TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(SR.Format(SR.SFxCloseTimedOut1, timeout)));
            }

            _timer = new Timer(new TimerCallback(new Action<object>(TimeoutCallback)), this, timeout, TimeSpan.FromMilliseconds(-1));
        }

        private object ThisLock { get; }

        public void Dispose()
        {
        }

        public static void End(IAsyncResult result)
        {
            AsyncResult.End<CloseCommunicationAsyncResult>(result);
        }

        public void Signal()
        {
            lock (ThisLock)
            {
                if (_result != CommunicationWaitResult.Waiting)
                {
                    return;
                }

                _result = CommunicationWaitResult.Succeeded;
            }
            _timer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
            Complete(false);
        }

        private void Timeout()
        {
            lock (ThisLock)
            {
                if (_result != CommunicationWaitResult.Waiting)
                {
                    return;
                }

                _result = CommunicationWaitResult.Expired;
            }
            Complete(false, new TimeoutException(SR.Format(SR.SFxCloseTimedOut1, _timeout)));
        }

        private static void TimeoutCallback(object state)
        {
            CloseCommunicationAsyncResult closeResult = (CloseCommunicationAsyncResult)state;
            closeResult.Timeout();
        }

        public CommunicationWaitResult Wait(TimeSpan timeout, bool aborting)
        {
            if (timeout < TimeSpan.Zero)
            {
                return CommunicationWaitResult.Expired;
            }

            // Synchronous Wait on AsyncResult should only be called in Abort code-path
            Fx.Assert(aborting, "CloseCommunicationAsyncResult.Wait: (aborting == true)");

            lock (ThisLock)
            {
                if (_result != CommunicationWaitResult.Waiting)
                {
                    return _result;
                }
                _result = CommunicationWaitResult.Aborted;
            }
            _timer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));

            TimeoutHelper.WaitOne(AsyncWaitHandle, timeout);

            Complete(false, new ObjectDisposedException(GetType().ToString()));
            return _result;
        }
    }

    internal class SyncCommunicationWaiter : ICommunicationWaiter
    {
        private bool _closed;
        private CommunicationWaitResult _result;
        private ManualResetEvent _waitHandle;

        public SyncCommunicationWaiter(object mutex)
        {
            ThisLock = mutex;
            _waitHandle = new ManualResetEvent(false);
        }

        private object ThisLock { get; }

        public void Dispose()
        {
            lock (ThisLock)
            {
                if (_closed)
                {
                    return;
                }

                _closed = true;
                _waitHandle.Dispose();
            }
        }

        public void Signal()
        {
            lock (ThisLock)
            {
                if (_closed)
                {
                    return;
                }

                _waitHandle.Set();
            }
        }

        public CommunicationWaitResult Wait(TimeSpan timeout, bool aborting)
        {
            if (_closed)
            {
                return CommunicationWaitResult.Aborted;
            }
            if (timeout < TimeSpan.Zero)
            {
                return CommunicationWaitResult.Expired;
            }

            if (aborting)
            {
                _result = CommunicationWaitResult.Aborted;
            }

            bool expired = !TimeoutHelper.WaitOne(_waitHandle, timeout);

            lock (ThisLock)
            {
                if (_result == CommunicationWaitResult.Waiting)
                {
                    _result = (expired ? CommunicationWaitResult.Expired : CommunicationWaitResult.Succeeded);
                }
            }

            lock (ThisLock)
            {
                if (!_closed)
                {
                    _waitHandle.Set();  // unblock other waiters if there are any
                }
            }

            return _result;
        }
    }
}
