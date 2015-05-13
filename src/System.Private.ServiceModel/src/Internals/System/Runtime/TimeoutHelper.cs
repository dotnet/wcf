// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Contracts;
using System;
using System.Threading;

namespace System.Runtime
{
    public struct TimeoutHelper
    {
        private bool _cancellationTokenInitialized;
        private bool _deadlineSet;

        private CancellationToken _cancellationToken;
        private CancellationTokenSource _cts;
        private DateTime _deadline;
        private TimeSpan _originalTimeout;
        public static readonly TimeSpan MaxWait = TimeSpan.FromMilliseconds(Int32.MaxValue);

        public TimeoutHelper(TimeSpan timeout)
        {
            Contract.Assert(timeout >= TimeSpan.Zero, "timeout must be non-negative");

            _cancellationTokenInitialized = false;
            _cts = null;
            _originalTimeout = timeout;
            _deadline = DateTime.MaxValue;
            _deadlineSet = (timeout == TimeSpan.MaxValue);
        }

        // No locks as we expect this class to be used linearly. 
        // If another CancellationTokenSource is created, we might have a CancellationToken outstanding
        // that isn't cancelled if _cts.Cancel() is called. This happens only on the Abort paths, so it's not an issue. 
        private void InitializeCancellationToken(TimeSpan timeout)
        {
            if (timeout > TimeSpan.Zero)
            {
                _cts = new CancellationTokenSource(timeout);
                _cancellationToken = _cts.Token;
            }
            else
            {
                _cancellationToken = new CancellationToken(true);
            }
            _cancellationTokenInitialized = true;
        }

        public CancellationToken CancellationToken
        {
            get
            {
                if (!_cancellationTokenInitialized)
                {
                    InitializeCancellationToken(this.RemainingTime());
                }
                return _cancellationToken;
            }
        }

        public void CancelCancellationToken(bool throwOnFirstException = false)
        {
            if (_cts != null)
            {
                _cts.Cancel(throwOnFirstException);
            }
        }

        public TimeSpan OriginalTimeout
        {
            get { return _originalTimeout; }
        }

        public static bool IsTooLarge(TimeSpan timeout)
        {
            return (timeout > TimeoutHelper.MaxWait) && (timeout != TimeSpan.MaxValue);
        }

        public static TimeSpan FromMilliseconds(int milliseconds)
        {
            if (milliseconds == Timeout.Infinite)
            {
                return TimeSpan.MaxValue;
            }
            else
            {
                return TimeSpan.FromMilliseconds(milliseconds);
            }
        }

        public static int ToMilliseconds(TimeSpan timeout)
        {
            if (timeout == TimeSpan.MaxValue)
            {
                return Timeout.Infinite;
            }
            else
            {
                long ticks = Ticks.FromTimeSpan(timeout);
                if (ticks / TimeSpan.TicksPerMillisecond > int.MaxValue)
                {
                    return int.MaxValue;
                }
                return Ticks.ToMilliseconds(ticks);
            }
        }

        public static TimeSpan Min(TimeSpan val1, TimeSpan val2)
        {
            if (val1 > val2)
            {
                return val2;
            }
            else
            {
                return val1;
            }
        }

        public static TimeSpan Add(TimeSpan timeout1, TimeSpan timeout2)
        {
            return Ticks.ToTimeSpan(Ticks.Add(Ticks.FromTimeSpan(timeout1), Ticks.FromTimeSpan(timeout2)));
        }

        public static DateTime Add(DateTime time, TimeSpan timeout)
        {
            if (timeout >= TimeSpan.Zero && DateTime.MaxValue - time <= timeout)
            {
                return DateTime.MaxValue;
            }
            if (timeout <= TimeSpan.Zero && DateTime.MinValue - time >= timeout)
            {
                return DateTime.MinValue;
            }
            return time + timeout;
        }

        public static DateTime Subtract(DateTime time, TimeSpan timeout)
        {
            return Add(time, TimeSpan.Zero - timeout);
        }

        public static TimeSpan Divide(TimeSpan timeout, int factor)
        {
            if (timeout == TimeSpan.MaxValue)
            {
                return TimeSpan.MaxValue;
            }

            return Ticks.ToTimeSpan((Ticks.FromTimeSpan(timeout) / factor) + 1);
        }

        public TimeSpan RemainingTime()
        {
            if (!_deadlineSet)
            {
                this.SetDeadline();
                return _originalTimeout;
            }
            else if (_deadline == DateTime.MaxValue)
            {
                return TimeSpan.MaxValue;
            }
            else
            {
                TimeSpan remaining = _deadline - DateTime.UtcNow;
                if (remaining <= TimeSpan.Zero)
                {
                    return TimeSpan.Zero;
                }
                else
                {
                    return remaining;
                }
            }
        }

        public TimeSpan ElapsedTime()
        {
            return _originalTimeout - this.RemainingTime();
        }

        private void SetDeadline()
        {
            Contract.Assert(!_deadlineSet, "TimeoutHelper deadline set twice.");
            _deadline = DateTime.UtcNow + _originalTimeout;
            _deadlineSet = true;
        }

        public static void ThrowIfNegativeArgument(TimeSpan timeout)
        {
            ThrowIfNegativeArgument(timeout, "timeout");
        }

        public static void ThrowIfNegativeArgument(TimeSpan timeout, string argumentName)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw Fx.Exception.ArgumentOutOfRange(argumentName, timeout, InternalSR.TimeoutMustBeNonNegative(argumentName, timeout));
            }
        }

        public static void ThrowIfNonPositiveArgument(TimeSpan timeout)
        {
            ThrowIfNonPositiveArgument(timeout, "timeout");
        }

        public static void ThrowIfNonPositiveArgument(TimeSpan timeout, string argumentName)
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw Fx.Exception.ArgumentOutOfRange(argumentName, timeout, InternalSR.TimeoutMustBePositive(argumentName, timeout));
            }
        }

        public static bool WaitOne(WaitHandle waitHandle, TimeSpan timeout)
        {
            ThrowIfNegativeArgument(timeout);
            if (timeout == TimeSpan.MaxValue)
            {
                waitHandle.WaitOne();
                return true;
            }
            else
            {
                // http://msdn.microsoft.com/en-us/library/85bbbxt9(v=vs.110).aspx 
                // with exitContext was used in Desktop which is not supported in Net Native or CoreClr
                return waitHandle.WaitOne(timeout);
            }
        }

        internal static TimeoutException CreateEnterTimedOutException(TimeSpan timeout)
        {
            return new TimeoutException(SR.Format(SR.LockTimeoutExceptionMessage, timeout));
        }
    }
}
