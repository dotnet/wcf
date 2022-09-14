// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Security;

namespace System.Runtime
{
    internal static class Ticks
    {
        public static long Now
        {
            [Fx.Tag.SecurityNote(Miscellaneous = "Why isn't the SuppressUnmanagedCodeSecurity attribute working in this case?")]
            [SecuritySafeCritical]
            get
            {
                return DateTime.UtcNow.Ticks;
            }
        }

        public static long FromMilliseconds(int milliseconds)
        {
            return checked((long)milliseconds * TimeSpan.TicksPerMillisecond);
        }

        public static int ToMilliseconds(long ticks)
        {
            return checked((int)(ticks / TimeSpan.TicksPerMillisecond));
        }

        public static long FromTimeSpan(TimeSpan duration)
        {
            return duration.Ticks;
        }

        public static TimeSpan ToTimeSpan(long ticks)
        {
            return new TimeSpan(ticks);
        }

        public static long Add(long firstTicks, long secondTicks)
        {
            if (firstTicks == long.MaxValue || firstTicks == long.MinValue)
            {
                return firstTicks;
            }
            if (secondTicks == long.MaxValue || secondTicks == long.MinValue)
            {
                return secondTicks;
            }
            if (firstTicks >= 0 && long.MaxValue - firstTicks <= secondTicks)
            {
                return long.MaxValue - 1;
            }
            if (firstTicks <= 0 && long.MinValue - firstTicks >= secondTicks)
            {
                return long.MinValue + 1;
            }
            return checked(firstTicks + secondTicks);
        }
    }
}
