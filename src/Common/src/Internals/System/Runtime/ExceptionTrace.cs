// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Tracing;
using System.Runtime.Diagnostics;

namespace System.Runtime
{
    internal partial class ExceptionTrace
    {
        private string _eventSourceName;
        private readonly EtwDiagnosticTrace _diagnosticTrace;

        public ExceptionTrace(string eventSourceName, EtwDiagnosticTrace diagnosticTrace)
        {
            Fx.Assert(diagnosticTrace != null, "'diagnosticTrace' MUST NOT be NULL.");

            _eventSourceName = eventSourceName;
            _diagnosticTrace = diagnosticTrace;
        }


        public void TraceEtwException(Exception exception, EventLevel eventLevel)
        {
            switch (eventLevel)
            {
                case EventLevel.Error:
                case EventLevel.Warning:
                    if (WcfEventSource.Instance.ThrowingEtwExceptionIsEnabled())
                    {
                        string serializedException = EtwDiagnosticTrace.ExceptionToTraceString(exception, int.MaxValue);
                        WcfEventSource.Instance.ThrowingEtwException(_eventSourceName, exception != null ? exception.ToString() : string.Empty, serializedException);
                    }
                    break;
                case EventLevel.Critical:
                    if (WcfEventSource.Instance.EtwUnhandledExceptionIsEnabled())
                    {
                        string serializedException = EtwDiagnosticTrace.ExceptionToTraceString(exception, int.MaxValue);
                        WcfEventSource.Instance.EtwUnhandledException(exception != null ? exception.ToString() : string.Empty, serializedException);
                    }
                    break;
                default:
                    if (WcfEventSource.Instance.ThrowingEtwExceptionVerboseIsEnabled())
                    {
                        string serializedException = EtwDiagnosticTrace.ExceptionToTraceString(exception, int.MaxValue);
                        WcfEventSource.Instance.ThrowingEtwExceptionVerbose(_eventSourceName, exception != null ? exception.ToString() : string.Empty, serializedException);
                    }

                    break;
            }
        }
    }
}
