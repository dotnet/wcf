// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Runtime
{
    //Admin - End User/Admin/Support/Tools
    //Operational - Admin/Support/Tools
    //Analytic - Tools
    //Debug - Developers
    internal enum TraceChannel
    {
        Admin = 16,
        Operational = 17,
        Analytic = 18,
        Debug = 19,
        Perf = 20,
        Application = 9, //This is reserved for Windows Event Log
    }
}
