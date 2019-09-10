//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal interface ILogger
    {
        Task WriteErrorAsync(string errorMessage, bool logToUI);
        Task WriteWarningAsync(string warningMessage, bool logToUI);
        Task WriteMessageAsync(string message, bool logToUI);

        Task<DateTime> WriteStartOperationAsync(string message, bool logToUI = false);
        Task WriteEndOperationAsync(DateTime startTime, bool logToUI = false);
    }
}
