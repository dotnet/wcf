// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
