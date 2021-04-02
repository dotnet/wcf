// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;

namespace ScenarioTests.Common
{
    public class ThreadHoppingSynchronizationContext : SynchronizationContext
    {
        public static ThreadHoppingSynchronizationContext Instance { get; } = new ThreadHoppingSynchronizationContext();

        public override void Post(SendOrPostCallback d, object state)
        {
            var t = new Thread(CallbackRunner);
            t.Start((d, state));
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            var t = new Thread(CallbackRunner);
            t.Start((d, state));
        }

        public static void CallbackRunner(object param)
        {
            (SendOrPostCallback d, object state) = ((SendOrPostCallback, object))param;
            d(state);
        }
    }
}
