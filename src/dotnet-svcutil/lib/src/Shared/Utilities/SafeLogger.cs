// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class SafeLogger : IDisposable, ILogger
    {
        private DateTime _startTime = DateTime.MinValue;

        public ILogger logger { get; private set; }

        public SafeLogger(ILogger logger)
        {
            this.logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)")]
        public static string GetStartOperationMessage(string message, DateTime startTime)
        {
            return $"[{Process.GetCurrentProcess().Id}.{startTime.Millisecond}] {message}";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)")]
        public static string GetEndOperationMessage(DateTime startTime)
        {
            return $"[{Process.GetCurrentProcess().Id}.{startTime.Millisecond}] Time elapsed: {DateTime.Now - startTime}";
        }

        public static async Task<SafeLogger> WriteStartOperationAsync(ILogger logger, string message, bool logToUI = false)
        {
            var safeLogger = new SafeLogger(logger);
            safeLogger._startTime = await safeLogger.WriteStartOperationAsync(message, logToUI).ConfigureAwait(false);
            return safeLogger;
        }

        #region ILogger implementation
        public Task WriteErrorAsync(string errorMessage, bool logToUI)
        {
            if (this.logger != null)
            {
                return this.logger.WriteErrorAsync(errorMessage, logToUI);
            }
            return Task.CompletedTask;
        }

        public Task WriteWarningAsync(string warningMessage, bool logToUI)
        {
            if (this.logger != null)
            {
                return this.logger.WriteWarningAsync(warningMessage, logToUI);
            }
            return Task.CompletedTask;
        }

        public Task WriteMessageAsync(string message, bool logToUI)
        {
            if (this.logger != null)
            {
                return this.logger.WriteMessageAsync(message, logToUI);
            }
            return Task.CompletedTask;
        }

        public Task<DateTime> WriteStartOperationAsync(string message, bool logToUI = false)
        {
            if (this.logger != null)
            {
                return this.logger.WriteStartOperationAsync(message, logToUI);
            }
            return Task.FromResult(DateTime.Now);
        }

        public Task WriteEndOperationAsync(DateTime startTime, bool logToUI = false)
        {
            if (this.logger != null)
            {
                return this.logger.WriteEndOperationAsync(startTime, logToUI);
            }
            return Task.CompletedTask;
        }
        #endregion

        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing && this.logger != null && _startTime != DateTime.MinValue)
                {
                    Task.WaitAny(this.logger.WriteEndOperationAsync(_startTime));
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
