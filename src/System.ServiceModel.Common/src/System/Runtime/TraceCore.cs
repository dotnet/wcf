// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Diagnostics;

namespace System.Runtime
{
    internal class TraceCore
    {
        /// <summary>
        /// Check if trace definition is enabled
        /// Event description ID=131, Level=verbose, Channel=Debug
        /// </summary>
        /// <param name="trace">The trace provider</param>
        internal static bool BufferPoolAllocationIsEnabled(EtwDiagnosticTrace trace)
        {
            return WcfEventSource.Instance.BufferPoolAllocationIsEnabled();
        }

        /// <summary>
        /// Gets trace definition like: Pool allocating {0} Bytes.
        /// Event description ID=131, Level=verbose, Channel=Debug
        /// </summary>
        /// <param name="trace">The trace provider</param>
        /// <param name="Size">Parameter 0 for event: Pool allocating {0} Bytes.</param>
        internal static void BufferPoolAllocation(EtwDiagnosticTrace trace, int Size)
        {
            WcfEventSource.Instance.BufferPoolAllocation(Size);
        }

        /// <summary>
        /// Check if trace definition is enabled
        /// Event description ID=132, Level=verbose, Channel=Debug
        /// </summary>
        /// <param name="trace">The trace provider</param>
        internal static bool BufferPoolChangeQuotaIsEnabled(EtwDiagnosticTrace trace)
        {
            return WcfEventSource.Instance.BufferPoolChangeQuotaIsEnabled();
        }

        /// <summary>
        /// Gets trace definition like: BufferPool of size {0}, changing quota by {1}.
        /// Event description ID=132, Level=verbose, Channel=Debug
        /// </summary>
        /// <param name="trace">The trace provider</param>
        /// <param name="PoolSize">Parameter 0 for event: BufferPool of size {0}, changing quota by {1}.</param>
        /// <param name="Delta">Parameter 1 for event: BufferPool of size {0}, changing quota by {1}.</param>
        internal static void BufferPoolChangeQuota(EtwDiagnosticTrace trace, int PoolSize, int Delta)
        {
            WcfEventSource.Instance.BufferPoolChangeQuota(PoolSize, Delta);
        }

        /// <summary>
        /// Gets trace definition like: An unexpected failure occurred. Applications should not attempt to handle this error. For diagnostic purposes, this English message is associated with the failure: {0}.
        /// Event description ID=57395, Level=error, Channel=Analytic
        /// </summary>
        /// <param name="trace">The trace provider</param>
        /// <param name="param0">Parameter 0 for event: An unexpected failure occurred. Applications should not attempt to handle this error. For diagnostic purposes, this English message is associated with the failure: {0}.</param>
        internal static void ShipAssertExceptionMessage(EtwDiagnosticTrace trace, string param0)
        {
            WcfEventSource.Instance.ShipAssertExceptionMessage(param0);
        }

        /// <summary>
        /// Check if trace definition is enabled
        /// Event description ID=57396, Level=warning, Channel=Analytic
        /// </summary>
        /// <param name="trace">The trace provider</param>
        internal static bool ThrowingExceptionIsEnabled(EtwDiagnosticTrace trace)
        {
            return WcfEventSource.Instance.ThrowingExceptionIsEnabled();
        }

        /// <summary>
        /// Gets trace definition like: Throwing an exception. Source: {0}. Exception details: {1}
        /// Event description ID=57396, Level=warning, Channel=Analytic
        /// </summary>
        /// <param name="trace">The trace provider</param>
        /// <param name="param0">Parameter 0 for event: Throwing an exception. Source: {0}. Exception details: {1}</param>
        /// <param name="param1">Parameter 1 for event: Throwing an exception. Source: {0}. Exception details: {1}</param>
        /// <param name="exception">Exception associated with the event</param>
        internal static void ThrowingException(EtwDiagnosticTrace trace, string param0, string param1, System.Exception exception)
        {
            string serializedException = EtwDiagnosticTrace.ExceptionToTraceString(exception, int.MaxValue);
            WcfEventSource.Instance.ThrowingException(param0, param1, serializedException);
        }
    }
}
