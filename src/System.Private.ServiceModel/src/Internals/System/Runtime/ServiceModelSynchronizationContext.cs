﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;

namespace System.Runtime
{
    public class ServiceModelSynchronizationContext : SynchronizationContext
    {
        public static ServiceModelSynchronizationContext Instance = new ServiceModelSynchronizationContext();

        public override void Post(SendOrPostCallback d, object state)
        {
            IOThreadScheduler.ScheduleCallbackNoFlow((s) => d(s), state);
        }
    }
}
