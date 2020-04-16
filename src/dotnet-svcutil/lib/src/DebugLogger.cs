// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    using System;
    using System.Threading.Tasks;

    internal class DebugLogger : ILogger, NuGet.Common.ILogger
    {
        public bool EnableTracing { get; set; }

        private void LogMessage(string message, LogTag logTag, bool logToUI = false)
        {
            if (this.EnableTracing || logToUI)
            {
                message = message?.Trim();

                if (!string.IsNullOrEmpty(message))
                {
                    if (logTag == LogTag.Error)
                    {
                        ToolConsole.WriteError(message, isTrace: !logToUI);
                    }
                    else if (logTag == LogTag.Warning)
                    {
                        ToolConsole.WriteWarning(message, isTrace: !logToUI);
                    }
                    else
                    {
                        ToolConsole.WriteLine(message, logToUI ? LogTag.Information : LogTag.LogMessage);
                    }
                }
            }
        }

        #region ILogger
        public Task WriteMessageAsync(string message, bool logToUI)
        {
            return Task.Run(() => LogMessage(message, LogTag.LogMessage, logToUI));
        }

        public Task WriteErrorAsync(string errorMessage, bool logToUI)
        {
            return Task.Run(() => LogMessage(errorMessage, LogTag.Error, logToUI));
        }

        public Task WriteWarningAsync(string warningMessage, bool logToUI)
        {
            return Task.Run(() => LogMessage(warningMessage, LogTag.Warning, logToUI));
        }

        public async Task<DateTime> WriteStartOperationAsync(string message, bool logToUI = false)
        {
            var startTime = DateTime.Now;
            await WriteMessageAsync(SafeLogger.GetStartOperationMessage(message, startTime), logToUI);
            return startTime;
        }

        public Task WriteEndOperationAsync(DateTime startTime, bool logToUI = false)
        {
            return WriteMessageAsync(SafeLogger.GetEndOperationMessage(startTime), logToUI);
        }
        #endregion

        #region NuGet.Common.ILogger

        void NuGet.Common.ILogger.LogDebug(string data)
        {
            LogMessage(data, LogTag.LogMessage);
        }

        void NuGet.Common.ILogger.LogVerbose(string data)
        {
            LogMessage(data, LogTag.LogMessage);
        }

        void NuGet.Common.ILogger.LogInformation(string data)
        {
            LogMessage(data, LogTag.LogMessage);
        }

        void NuGet.Common.ILogger.LogMinimal(string data)
        {
            LogMessage(data, LogTag.LogMessage);
        }

        void NuGet.Common.ILogger.LogWarning(string data)
        {
            LogMessage(data, LogTag.Warning);
        }

        void NuGet.Common.ILogger.LogError(string data)
        {
            LogMessage(data, LogTag.Error);
        }

        void NuGet.Common.ILogger.LogInformationSummary(string data)
        {
            LogMessage(data, LogTag.LogMessage);
        }

        public void LogErrorSummary(string data)
        {
            LogMessage(data, LogTag.Error);
        }
        #endregion
    }
}
