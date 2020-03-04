// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.Runtime.Diagnostics
{
    // Order is important here. The order must match the order of strings in EventLog.mc
    internal enum EventLogCategory : ushort
    {
        ServiceAuthorization = 1,  // reserved
        MessageAuthentication,     // reserved
        ObjectAccess,              // reserved
        Tracing,
        WebHost,
        FailFast,
        MessageLogging,
        PerformanceCounter,
        Wmi,
        ComPlus,
        StateMachine,
        Wsat,
        SharingService,
        ListenerAdapter
    }
}
