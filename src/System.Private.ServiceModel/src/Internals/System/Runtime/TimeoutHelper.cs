// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime
{
    public struct TimeoutHelper : IDisposable
    {
        private bool _cancellationTokenInitialized;
        private bool _deadlineSet;

        private CancellationToken _cancellationToken;
        private CancellationTokenSource _cts;
        private DateTime _deadline;
        private TimeSpan _originalTimeout;
        public static readonly TimeSpan MaxWait = TimeSpan.FromMilliseconds(Int32.MaxValue);
        private static Action<object> s_cancelOnTimeout = state => ((TimeoutHelper)state)._cts.Cancel();

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
            if (timeout == TimeSpan.MaxValue || timeout == Timeout.InfiniteTimeSpan)
            {
                _cancellationToken = CancellationToken.None;
            }
            else if (timeout > TimeSpan.Zero)
            {
                _cts = new CancellationTokenSource();
                _cancellationToken = _cts.Token;
                TimeoutTokenSource.FromTimeout((int)timeout.TotalMilliseconds).Register(s_cancelOnTimeout, this);
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

        public void Dispose()
        {
            if (_cancellationTokenInitialized && _cts !=null)
            {
                _cts.Dispose();
                _cancellationTokenInitialized = false;
                _cancellationToken = default(CancellationToken);
            }
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

    /// <summary>
    /// This class coalesces timeout tokens because cancelation tokens with timeouts are more expensive to expose.
    /// Disposing too many such tokens will cause thread contentions in high throughput scenario.
    ///
    /// Tokens with target cancelation time 15ms apart would resolve to the same instance.
    /// </summary>
    internal static class TimeoutTokenSource
    {
        private const int COALESCING_SPAN_MS = 15;
        private static readonly ConcurrentDictionary<long, Task<CancellationToken>> s_tokenCache =
            new ConcurrentDictionary<long, Task<CancellationToken>>();

        public static CancellationToken FromTimeout(int millisecondsTimeout)
        {
            return FromTimeoutAsync(millisecondsTimeout).Result;
        }

        public static Task<CancellationToken> FromTimeoutAsync(int millisecondsTimeout)
        {
            // Note that CancellationTokenSource constructor requires input to be >= -1,
            // restricting millisecondsTimeout to be >= -1 would enforce that
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException("Invalid millisecondsTimeout value " + millisecondsTimeout);
            }

            uint currentTime = (uint)Environment.TickCount;
            long targetTime = millisecondsTimeout + currentTime;
            // round the targetTime up to the next closest 15ms
            targetTime = ((targetTime + (COALESCING_SPAN_MS - 1)) / COALESCING_SPAN_MS) * COALESCING_SPAN_MS;

            Task<CancellationToken> tokenTask;

            if (!s_tokenCache.TryGetValue(targetTime, out tokenTask))
            {
                var tcs = new TaskCompletionSource<CancellationToken>();

                // only a single thread may succeed adding its task into the cache
                if (s_tokenCache.TryAdd(targetTime, tcs.Task))
                {
                    // Since this thread was successful reserving a spot in the cache, it would be the only thread
                    // that construct the CancellationTokenSource
                    var token = new CancellationTokenSource((int)(targetTime - currentTime)).Token;

                    // Clean up cache when Token is canceled
                    token.Register(t => {
                        Task<CancellationToken> ignored;
                        s_tokenCache.TryRemove((long)t, out ignored);
                    }, targetTime);

                    // set the result so other thread may observe the token, and return
                    tcs.TrySetResult(token);
                    tokenTask = tcs.Task;
                }
                else
                {
                    // for threads that failed when calling TryAdd, there should be one already in the cache
                    if (!s_tokenCache.TryGetValue(targetTime, out tokenTask))
                    {
                        // In unlikely scenario the token was already cancelled and timed out, we would not find it in cache.
                        // In this case we would simply create a non-coalsed token
                        tokenTask = Task.FromResult(new CancellationTokenSource(millisecondsTimeout).Token);
                    }
                }
            }
            return tokenTask;
        }
    }
}
