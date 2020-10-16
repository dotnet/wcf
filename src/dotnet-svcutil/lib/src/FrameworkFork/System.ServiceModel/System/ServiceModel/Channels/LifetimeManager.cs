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
        private int _busyCount;
        private ICommunicationWaiter _busyWaiter;
        private int _busyWaiterCount;
        private object _mutex;
        private LifetimeState _state;

        public LifetimeManager(object mutex)
        {
            _mutex = mutex;
            _state = LifetimeState.Opened;
        }

        public int BusyCount
        {
            get { return _busyCount; }
        }

        protected LifetimeState State
        {
            get { return _state; }
        }

        protected object ThisLock
        {
            get { return _mutex; }
        }

        public void Abort()
        {
            lock (this.ThisLock)
            {
                if (this.State == LifetimeState.Closed || _aborted)
                    return;
                _aborted = true;
                _state = LifetimeState.Closing;
            }

            this.OnAbort();
            _state = LifetimeState.Closed;
        }

        private void ThrowIfNotOpened()
        {
            if (!_aborted && _state != LifetimeState.Opened)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(this.GetType().ToString()));
            }
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            lock (this.ThisLock)
            {
                this.ThrowIfNotOpened();
                _state = LifetimeState.Closing;
            }

            return this.OnBeginClose(timeout, callback, state);
        }

        public void Close(TimeSpan timeout)
        {
            lock (this.ThisLock)
            {
                this.ThrowIfNotOpened();
                _state = LifetimeState.Closing;
            }

            this.OnClose(timeout);
            _state = LifetimeState.Closed;
        }

        private CommunicationWaitResult CloseCore(TimeSpan timeout, bool aborting)
        {
            ICommunicationWaiter busyWaiter = null;
            CommunicationWaitResult result = CommunicationWaitResult.Succeeded;

            lock (this.ThisLock)
            {
                if (_busyCount > 0)
                {
                    if (_busyWaiter != null)
                    {
                        if (!aborting && _aborted)
                            return CommunicationWaitResult.Aborted;
                        busyWaiter = _busyWaiter;
                    }
                    else
                    {
                        busyWaiter = new SyncCommunicationWaiter(this.ThisLock);
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

            lock (this.ThisLock)
            {
                if (_busyCount <= 0)
                {
                    throw Fx.AssertAndThrow("LifetimeManager.DecrementBusyCount: (this.busyCount > 0)");
                }
                if (--_busyCount == 0)
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

            if (empty && this.State == LifetimeState.Opened)
                OnEmpty();
        }

        public void EndClose(IAsyncResult result)
        {
            this.OnEndClose(result);
            _state = LifetimeState.Closed;
        }

        protected virtual void IncrementBusyCount()
        {
            lock (this.ThisLock)
            {
                Fx.Assert(this.State == LifetimeState.Opened, "LifetimeManager.IncrementBusyCount: (this.State == LifetimeState.Opened)");
                _busyCount++;
            }
        }

        protected virtual void IncrementBusyCountWithoutLock()
        {
            Fx.Assert(this.State == LifetimeState.Opened, "LifetimeManager.IncrementBusyCountWithoutLock: (this.State == LifetimeState.Opened)");
            _busyCount++;
        }

        protected virtual void OnAbort()
        {
            // We have decided not to make this configurable
            CloseCore(TimeSpan.FromSeconds(1), true);
        }

        protected virtual IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            CloseCommunicationAsyncResult closeResult = null;

            lock (this.ThisLock)
            {
                if (_busyCount > 0)
                {
                    if (_busyWaiter != null)
                    {
                        Fx.Assert(_aborted, "LifetimeManager.OnBeginClose: (this.aborted == true)");
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(this.GetType().ToString()));
                    }
                    else
                    {
                        closeResult = new CloseCommunicationAsyncResult(timeout, callback, state, this.ThisLock);
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(string.Format(SRServiceModel.SFxCloseTimedOut1, timeout)));
                case CommunicationWaitResult.Aborted:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(this.GetType().ToString()));
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
                CompletedAsyncResult.End(result);
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
        private object _mutex;
        private CommunicationWaitResult _result;
        private Timer _timer;
        private TimeoutHelper _timeoutHelper;
        private TimeSpan _timeout;

        public CloseCommunicationAsyncResult(TimeSpan timeout, AsyncCallback callback, object state, object mutex)
            : base(callback, state)
        {
            _timeout = timeout;
            _timeoutHelper = new TimeoutHelper(timeout);
            _mutex = mutex;

            if (timeout < TimeSpan.Zero)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(string.Format(SRServiceModel.SFxCloseTimedOut1, timeout)));

            _timer = new Timer(new TimerCallback(new Action<object>(TimeoutCallback)), this, timeout, TimeSpan.FromMilliseconds(-1));
        }

        private object ThisLock
        {
            get { return _mutex; }
        }

        public void Dispose()
        {
        }

        public static void End(IAsyncResult result)
        {
            AsyncResult.End<CloseCommunicationAsyncResult>(result);
        }

        public void Signal()
        {
            lock (this.ThisLock)
            {
                if (_result != CommunicationWaitResult.Waiting)
                    return;
                _result = CommunicationWaitResult.Succeeded;
            }
            _timer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
            this.Complete(false);
        }

        private void Timeout()
        {
            lock (this.ThisLock)
            {
                if (_result != CommunicationWaitResult.Waiting)
                    return;
                _result = CommunicationWaitResult.Expired;
            }
            this.Complete(false, new TimeoutException(string.Format(SRServiceModel.SFxCloseTimedOut1, _timeout)));
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

            lock (this.ThisLock)
            {
                if (_result != CommunicationWaitResult.Waiting)
                {
                    return _result;
                }
                _result = CommunicationWaitResult.Aborted;
            }
            _timer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));

            TimeoutHelper.WaitOne(this.AsyncWaitHandle, timeout);

            this.Complete(false, new ObjectDisposedException(this.GetType().ToString()));
            return _result;
        }
    }

    internal class SyncCommunicationWaiter : ICommunicationWaiter
    {
        private bool _closed;
        private object _mutex;
        private CommunicationWaitResult _result;
        private ManualResetEvent _waitHandle;

        public SyncCommunicationWaiter(object mutex)
        {
            _mutex = mutex;
            _waitHandle = new ManualResetEvent(false);
        }

        private object ThisLock
        {
            get { return _mutex; }
        }

        public void Dispose()
        {
            lock (this.ThisLock)
            {
                if (_closed)
                    return;
                _closed = true;
                _waitHandle.Dispose();
            }
        }

        public void Signal()
        {
            lock (this.ThisLock)
            {
                if (_closed)
                    return;
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

            lock (this.ThisLock)
            {
                if (_result == CommunicationWaitResult.Waiting)
                {
                    _result = (expired ? CommunicationWaitResult.Expired : CommunicationWaitResult.Succeeded);
                }
            }

            lock (this.ThisLock)
            {
                if (!_closed)
                    _waitHandle.Set();  // unblock other waiters if there are any
            }

            return _result;
        }
    }
}
