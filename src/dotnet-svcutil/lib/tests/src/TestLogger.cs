// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Threading.Tasks;

namespace SvcutilTest
{
    internal class TestLogger : Microsoft.Tools.ServiceModel.Svcutil.ILogger
    {
        static readonly string programFilesx64 = Environment.GetEnvironmentVariable("ProgramW6432")?.Replace('\\', '/');

        private StringBuilder logBuilder;

        public string Log { get { return this.logBuilder.ToString(); } }

        public bool Verbose { get; set; }

        public TestLogger()
        {
            this.logBuilder = new StringBuilder();
        }

        #region ILogger
        public Task WriteEndOperationAsync(DateTime startTime, bool logToUI = false)
        {
            return Task.CompletedTask;
        }

        public Task WriteErrorAsync(string errorMessage, bool logToUI)
        {
            WriteMessageAsync($"Error: {errorMessage}", logToUI);
            return Task.CompletedTask;
        }

        public Task WriteMessageAsync(string message, bool logToUI)
        {
            if (logToUI || Verbose)
            {
                this.logBuilder.AppendLine(message);
            }
            return Task.CompletedTask;
        }

        public Task<DateTime> WriteStartOperationAsync(string message, bool logToUI = false)
        {
            WriteMessageAsync(message, logToUI);
            return Task.FromResult(DateTime.Now);
        }

        public Task WriteWarningAsync(string warningMessage, bool logToUI)
        {
            WriteMessageAsync($"Warning: {warningMessage}", logToUI);
            return Task.CompletedTask;
        }
        #endregion

        public override string ToString()
        {
            return this.logBuilder.ToString();
        }
    }
}
