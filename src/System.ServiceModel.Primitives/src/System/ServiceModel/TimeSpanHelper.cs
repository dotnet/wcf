// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Diagnostics.Contracts;
using System.Globalization;

namespace System.ServiceModel
{
    public static class TimeSpanHelper
    {
        static public TimeSpan FromMinutes(int minutes, string text)
        {
            TimeSpan value = TimeSpan.FromTicks(TimeSpan.TicksPerMinute * minutes);
            Contract.Assert(value == TimeSpan.Parse(text, CultureInfo.InvariantCulture), "");
            return value;
        }
        static public TimeSpan FromSeconds(int seconds, string text)
        {
            TimeSpan value = TimeSpan.FromTicks(TimeSpan.TicksPerSecond * seconds);
            Contract.Assert(value == TimeSpan.Parse(text, CultureInfo.InvariantCulture), "");
            return value;
        }
        static public TimeSpan FromMilliseconds(int ms, string text)
        {
            TimeSpan value = TimeSpan.FromTicks(TimeSpan.TicksPerMillisecond * ms);
            Contract.Assert(value == TimeSpan.Parse(text, CultureInfo.InvariantCulture), "");
            return value;
        }
    }
}
