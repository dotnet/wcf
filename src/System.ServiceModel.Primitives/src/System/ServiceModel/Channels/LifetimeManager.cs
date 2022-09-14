// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task CloseAsync(TimeSpan timeout)
        {
            lock (ThisLock)
            {
                ThrowIfNotOpened();
                _state = LifetimeState.Closing;
            }

            await OnCloseAsync(timeout);
            _state = LifetimeState.Closed;
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

        private async Task<CommunicationWaitResult> CloseCoreAsync(TimeSpan timeout, bool aborting)
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
                        busyWaiter = new AsyncCommunicationWaiter(ThisLock);
                        _busyWaiter = busyWaiter;
                    }
                    Interlocked.Increment(ref _busyWaiterCount);
                }
            }

            if (busyWaiter != null)
            {
                result = await busyWaiter.WaitAsync(timeout, aborting);
                if (Interlocked.Decrement(ref _busyWaiterCount) == 0)
                {
                    busyWaiter.Dispose();
                    _busyWaiter = null;
                }
            }

            return result;
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
                        busyWaiter = new AsyncCommunicationWaiter(ThisLock);
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
            return OnCloseAsync(timeout).ToApm(callback, state);
        }

        protected virtual void OnClose(TimeSpan timeout)
        {
            switch (CloseCore(timeout, false))
            {
                case CommunicationWaitResult.Expired:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(SRP.Format(SRP.SFxCloseTimedOut1, timeout)));
                case CommunicationWaitResult.Aborted:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(GetType().ToString()));
            }
        }

        protected virtual async Task OnCloseAsync(TimeSpan timeout)
        {
            switch (await CloseCoreAsync(timeout, false))
            {
                case CommunicationWaitResult.Expired:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(SRP.Format(SRP.SFxCloseTimedOut1, timeout)));
                case CommunicationWaitResult.Aborted:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(GetType().ToString()));
            }
        }

        protected virtual void OnEmpty()
        {
        }

        protected virtual void OnEndClose(IAsyncResult result)
        {
            result.ToApmEnd();
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
        Task<CommunicationWaitResult> WaitAsync(TimeSpan timeout, bool aborting);
        CommunicationWaitResult Wait(TimeSpan timeout, bool aborting);
    }

    internal class AsyncCommunicationWaiter : ICommunicationWaiter
    {
        private bool _closed;
        private CommunicationWaitResult _result;

        private TaskCompletionSource<bool> _tcs;

        internal AsyncCommunicationWaiter(object mutex)
        {
            ThisLock = mutex;
            _tcs = new TaskCompletionSource<bool>();
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
                _tcs?.TrySetResult(false);
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

                _tcs.TrySetResult(true);
            }
        }

        public async Task<CommunicationWaitResult> WaitAsync(TimeSpan timeout, bool aborting)
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

            bool expired = !await _tcs.Task.AwaitWithTimeout(timeout);

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
                    _tcs.TrySetResult(!expired);  // unblock other waiters if there are any
                }
            }

            return _result;
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

            bool expired = !_tcs.Task.WaitForCompletionNoSpin(timeout);

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
                    _tcs.TrySetResult(false);  // unblock other waiters if there are any
                }
            }

            return _result;
        }
    }
}
