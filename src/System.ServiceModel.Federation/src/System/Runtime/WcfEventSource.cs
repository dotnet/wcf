// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Tracing;
using System.Runtime.Diagnostics;

namespace System.Runtime
{
    internal sealed partial class WcfEventSource : EventSource
    {
        public bool SecurityTokenProviderOpenedIsEnabled()
        {
            return base.IsEnabled(EventLevel.Verbose, Keywords.Security, EventChannel.Debug);
        }

        [Event(EventIds.SecurityTokenProviderOpened, Level = EventLevel.Verbose, Channel = EventChannel.Debug, Opcode = EventOpcode.Start, Task = Tasks.SecureMessage,
            Keywords = Keywords.Security, Message = "SecurityTokenProvider opening completed.")]
        public void SecurityTokenProviderOpened(string AppDomain)
        {
            WriteEvent(EventIds.SecurityTokenProviderOpened, AppDomain);
        }

        [NonEvent]
        public void SecurityTokenProviderOpened(EventTraceActivity eventTraceActivity)
        {
            SetActivityId(eventTraceActivity);
            SecurityTokenProviderOpened("");
        }
    }
}
