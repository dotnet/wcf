// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.Contracts;
using System.Threading;

namespace System.Runtime
{
    internal struct TimeoutHelper
    {
        public static readonly TimeSpan MaxWait = TimeSpan.FromMilliseconds(int.MaxValue);
        private static readonly CancellationToken s_precancelledToken = new CancellationToken(true);

        private bool _deadlineSet;
        private DateTime _deadline;

        public TimeoutHelper(TimeSpan timeout)
        {
            Contract.Assert(timeout >= TimeSpan.Zero, "timeout must be non-negative");

            OriginalTimeout = timeout;
            _deadline = DateTime.MaxValue;
            _deadlineSet = (timeout == TimeSpan.MaxValue);
        }

        public TimeSpan OriginalTimeout { get; }

        public static bool IsTooLarge(TimeSpan timeout)
        {
            return (timeout > MaxWait) && (timeout != TimeSpan.MaxValue);
        }

        //public static TimeSpan FromMilliseconds(int milliseconds)
        //{
        //    if (milliseconds == Timeout.Infinite)
        //    {
        //        return TimeSpan.MaxValue;
        //    }
        //    else
        //    {
        //        return TimeSpan.FromMilliseconds(milliseconds);
        //    }
        //}

        //public static int ToMilliseconds(TimeSpan timeout)
        //{
        //    if (timeout == TimeSpan.MaxValue)
        //    {
        //        return Timeout.Infinite;
        //    }
        //    else
        //    {
        //        long ticks = Ticks.FromTimeSpan(timeout);
        //        if (ticks / TimeSpan.TicksPerMillisecond > int.MaxValue)
        //        {
        //            return int.MaxValue;
        //        }
        //        return Ticks.ToMilliseconds(ticks);
        //    }
        //}

        //public static TimeSpan Min(TimeSpan val1, TimeSpan val2)
        //{
        //    if (val1 > val2)
        //    {
        //        return val2;
        //    }
        //    else
        //    {
        //        return val1;
        //    }
        //}

        //public static TimeSpan Add(TimeSpan timeout1, TimeSpan timeout2)
        //{
        //    return Ticks.ToTimeSpan(Ticks.Add(Ticks.FromTimeSpan(timeout1), Ticks.FromTimeSpan(timeout2)));
        //}

        //public static DateTime Add(DateTime time, TimeSpan timeout)
        //{
        //    if (timeout >= TimeSpan.Zero && DateTime.MaxValue - time <= timeout)
        //    {
        //        return DateTime.MaxValue;
        //    }
        //    if (timeout <= TimeSpan.Zero && DateTime.MinValue - time >= timeout)
        //    {
        //        return DateTime.MinValue;
        //    }
        //    return time + timeout;
        //}

        //public static DateTime Subtract(DateTime time, TimeSpan timeout)
        //{
        //    return Add(time, TimeSpan.Zero - timeout);
        //}

        //public static TimeSpan Divide(TimeSpan timeout, int factor)
        //{
        //    if (timeout == TimeSpan.MaxValue)
        //    {
        //        return TimeSpan.MaxValue;
        //    }

        //    return Ticks.ToTimeSpan((Ticks.FromTimeSpan(timeout) / factor) + 1);
        //}

        public TimeSpan RemainingTime()
        {
            if (!_deadlineSet)
            {
                SetDeadline();
                return OriginalTimeout;
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

        //public TimeSpan ElapsedTime()
        //{
        //    return OriginalTimeout - RemainingTime();
        //}

        private void SetDeadline()
        {
            Contract.Assert(!_deadlineSet, "TimeoutHelper deadline set twice.");
            _deadline = DateTime.UtcNow + OriginalTimeout;
            _deadlineSet = true;
        }

        //public static void ThrowIfNegativeArgument(TimeSpan timeout)
        //{
        //    ThrowIfNegativeArgument(timeout, "timeout");
        //}

        //public static void ThrowIfNegativeArgument(TimeSpan timeout, string argumentName)
        //{
        //    if (timeout < TimeSpan.Zero)
        //    {
        //        throw Fx.Exception.ArgumentOutOfRange(argumentName, timeout, InternalSR.TimeoutMustBeNonNegative(argumentName, timeout));
        //    }
        //}

        //public static void ThrowIfNonPositiveArgument(TimeSpan timeout)
        //{
        //    ThrowIfNonPositiveArgument(timeout, "timeout");
        //}

        //public static void ThrowIfNonPositiveArgument(TimeSpan timeout, string argumentName)
        //{
        //    if (timeout <= TimeSpan.Zero)
        //    {
        //        throw Fx.Exception.ArgumentOutOfRange(argumentName, timeout, InternalSR.TimeoutMustBePositive(argumentName, timeout));
        //    }
        //}

        //public static bool WaitOne(WaitHandle waitHandle, TimeSpan timeout)
        //{
        //    ThrowIfNegativeArgument(timeout);
        //    if (timeout == TimeSpan.MaxValue)
        //    {
        //        waitHandle.WaitOne();
        //        return true;
        //    }
        //    else
        //    {
        //        // http://msdn.microsoft.com/en-us/library/85bbbxt9(v=vs.110).aspx
        //        // with exitContext was used in Desktop which is not supported in Net Native or CoreClr
        //        return waitHandle.WaitOne(timeout);
        //    }
        //}

        //public static bool Wait(ManualResetEventSlim mres, TimeSpan timeout)
        //{
        //    ThrowIfNegativeArgument(timeout);
        //    if (timeout == TimeSpan.MaxValue)
        //    {
        //        mres.Wait();
        //        return true;
        //    }
        //    else
        //    {
        //        return mres.Wait(timeout);
        //    }
        //}

        //internal static TimeoutException CreateEnterTimedOutException(TimeSpan timeout)
        //{
        //    return new TimeoutException(SRP.Format(SRP.LockTimeoutExceptionMessage, timeout));
        //}
    }

}
