//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.Runtime
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    sealed class BackoffTimeoutHelper
    {
        readonly static int s_maxSkewMilliseconds = (int)(IOThreadTimer.SystemTimeResolutionTicks / TimeSpan.TicksPerMillisecond);
        readonly static long s_maxDriftTicks = IOThreadTimer.SystemTimeResolutionTicks * 2;
        readonly static TimeSpan s_defaultInitialWaitTime = TimeSpan.FromMilliseconds(1);
        readonly static TimeSpan s_defaultMaxWaitTime = TimeSpan.FromMinutes(1);

        DateTime _deadline;
        TimeSpan _maxWaitTime;
        TimeSpan _waitTime;
        IOThreadTimer _backoffTimer;
        Action<object> _backoffCallback;
        object _backoffState;
        Random _random;
        TimeSpan _originalTimeout;

        internal BackoffTimeoutHelper(TimeSpan timeout)
            : this(timeout, BackoffTimeoutHelper.s_defaultMaxWaitTime)
        {
        }

        internal BackoffTimeoutHelper(TimeSpan timeout, TimeSpan maxWaitTime)
            : this(timeout, maxWaitTime, BackoffTimeoutHelper.s_defaultInitialWaitTime)
        {
        }

        internal BackoffTimeoutHelper(TimeSpan timeout, TimeSpan maxWaitTime, TimeSpan initialWaitTime)
        {
            this._random = new Random(GetHashCode());
            this._maxWaitTime = maxWaitTime;
            this._originalTimeout = timeout;
            Reset(timeout, initialWaitTime);
        }

        public TimeSpan OriginalTimeout
        {
            get
            {
                return this._originalTimeout;
            }
        }

        void Reset(TimeSpan timeout, TimeSpan initialWaitTime)
        {
            if (timeout == TimeSpan.MaxValue)
            {
                this._deadline = DateTime.MaxValue;
            }
            else
            {
                this._deadline = DateTime.UtcNow + timeout;
            }
            this._waitTime = initialWaitTime;
        }

        public bool IsExpired()
        {
            if (this._deadline == DateTime.MaxValue)
            {
                return false;
            }
            else
            {
                return (DateTime.UtcNow >= this._deadline);
            }
        }

        public void WaitAndBackoff(Action<object> callback, object state)
        {
            if (this._backoffCallback != callback || this._backoffState != state)
            {
                if (this._backoffTimer != null)
                {
                    this._backoffTimer.Cancel();
                }
                this._backoffCallback = callback;
                this._backoffState = state;
                this._backoffTimer = new IOThreadTimer(callback, state, false, BackoffTimeoutHelper.s_maxSkewMilliseconds);
            }

            TimeSpan backoffTime = WaitTimeWithDrift();
            Backoff();
            this._backoffTimer.Set(backoffTime);
        }

        public void WaitAndBackoff()
        {
            Thread.Sleep(WaitTimeWithDrift());
            Backoff();
        }

        public async Task WaitAndBackoffAsync()
        {
            await Task.Delay(WaitTimeWithDrift());
            Backoff();
        }

        TimeSpan WaitTimeWithDrift()
        {
            return Ticks.ToTimeSpan(Math.Max(
                Ticks.FromTimeSpan(BackoffTimeoutHelper.s_defaultInitialWaitTime),
                Ticks.Add(Ticks.FromTimeSpan(this._waitTime),
                    (long)(uint)this._random.Next() % (2 * BackoffTimeoutHelper.s_maxDriftTicks + 1) - BackoffTimeoutHelper.s_maxDriftTicks)));
        }

        void Backoff()
        {
            if (_waitTime.Ticks >= (_maxWaitTime.Ticks / 2))
            {
                _waitTime = _maxWaitTime;
            }
            else
            {
                _waitTime = TimeSpan.FromTicks(_waitTime.Ticks * 2);
            }

            if (this._deadline != DateTime.MaxValue)
            {
                TimeSpan remainingTime = this._deadline - DateTime.UtcNow;
                if (this._waitTime > remainingTime)
                {
                    this._waitTime = remainingTime;
                    if (this._waitTime < TimeSpan.Zero)
                    {
                        this._waitTime = TimeSpan.Zero;
                    }
                }
            }
        }
    }
}
