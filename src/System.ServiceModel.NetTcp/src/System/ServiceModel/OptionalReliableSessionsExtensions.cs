// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    internal static class OptionalReliableSessionsExtensions
    {
        internal static void CopySettings(this OptionalReliableSession thisPtr, OptionalReliableSession copyFrom)
        {
            thisPtr.Ordered = copyFrom.Ordered;
            thisPtr.InactivityTimeout = copyFrom.InactivityTimeout;
            thisPtr.Enabled = copyFrom.Enabled;
        }
    }
}
