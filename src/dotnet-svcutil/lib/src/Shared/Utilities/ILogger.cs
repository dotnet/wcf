// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
