// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;

namespace System.Runtime
{
    internal sealed class BackoffTimeoutHelper
    {
        private static readonly int s_maxSkewMilliseconds = 15;
        private static readonly long s_maxDriftTicks = s_maxSkewMilliseconds * TimeSpan.TicksPerMillisecond * 2;
        private static readonly TimeSpan s_defaultInitialWaitTime = TimeSpan.FromMilliseconds(1);
        private static readonly TimeSpan s_defaultMaxWaitTime = TimeSpan.FromMinutes(1);
        private DateTime _deadline;
        private readonly TimeSpan _maxWaitTime;
        private TimeSpan _waitTime;
        private readonly Random _random;

        internal BackoffTimeoutHelper(TimeSpan timeout)
            : this(timeout, s_defaultMaxWaitTime)
        {
        }

        internal BackoffTimeoutHelper(TimeSpan timeout, TimeSpan maxWaitTime)
            : this(timeout, maxWaitTime, s_defaultInitialWaitTime)
        {
        }

        internal BackoffTimeoutHelper(TimeSpan timeout, TimeSpan maxWaitTime, TimeSpan initialWaitTime)
        {
            _random = new Random(GetHashCode());
            _maxWaitTime = maxWaitTime;
            OriginalTimeout = timeout;
            Reset(timeout, initialWaitTime);
        }

        public TimeSpan OriginalTimeout { get; }

        private void Reset(TimeSpan timeout, TimeSpan initialWaitTime)
        {
            if (timeout == TimeSpan.MaxValue)
            {
                _deadline = DateTime.MaxValue;
            }
            else
            {
                _deadline = DateTime.UtcNow + timeout;
            }
            _waitTime = initialWaitTime;
        }

        public bool IsExpired()
        {
            if (_deadline == DateTime.MaxValue)
            {
                return false;
            }
            else
            {
                return (DateTime.UtcNow >= _deadline);
            }
        }

        public async Task WaitAndBackoffAsync()
        {
            await Task.Delay(WaitTimeWithDrift());
            Backoff();
        }

        private TimeSpan WaitTimeWithDrift()
        {
            return Ticks.ToTimeSpan(Math.Max(
                Ticks.FromTimeSpan(s_defaultInitialWaitTime),
                Ticks.Add(Ticks.FromTimeSpan(_waitTime),
                    (uint)_random.Next() % (2 * BackoffTimeoutHelper.s_maxDriftTicks + 1) - s_maxDriftTicks)));
        }

        private void Backoff()
        {
            if (_waitTime.Ticks >= (_maxWaitTime.Ticks / 2))
            {
                _waitTime = _maxWaitTime;
            }
            else
            {
                _waitTime = TimeSpan.FromTicks(_waitTime.Ticks * 2);
            }

            if (_deadline != DateTime.MaxValue)
            {
                TimeSpan remainingTime = _deadline - DateTime.UtcNow;
                if (_waitTime > remainingTime)
                {
                    _waitTime = remainingTime;
                    if (_waitTime < TimeSpan.Zero)
                    {
                        _waitTime = TimeSpan.Zero;
                    }
                }
            }
        }
    }
}
