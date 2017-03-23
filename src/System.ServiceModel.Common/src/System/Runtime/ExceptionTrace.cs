// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Diagnostics;

namespace System.Runtime
{
    internal class ExceptionTrace
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

        public ArgumentException Argument(string paramName, string message)
        {
            return TraceException<ArgumentException>(new ArgumentException(message, paramName));
        }

        public ArgumentNullException ArgumentNull(string paramName)
        {
            return TraceException<ArgumentNullException>(new ArgumentNullException(paramName));
        }

        public ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, object actualValue, string message)
        {
            return TraceException<ArgumentOutOfRangeException>(new ArgumentOutOfRangeException(paramName, actualValue, message));
        }

        public Exception AsError(Exception exception)
        {
            // AggregateExceptions are automatically unwrapped.
            AggregateException aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                return AsError<Exception>(aggregateException);
            }

            // TargetInvocationExceptions are automatically unwrapped.
            TargetInvocationException targetInvocationException = exception as TargetInvocationException;
            if (targetInvocationException != null && targetInvocationException.InnerException != null)
            {
                return AsError(targetInvocationException.InnerException);
            }

            return TraceException<Exception>(exception);
        }

        public Exception AsError<TPreferredException>(AggregateException aggregateException)
        {
            return AsError<TPreferredException>(aggregateException, _eventSourceName);
        }

        /// <summary>
        /// Extracts the first inner exception of type <typeparamref name="TPreferredException"/>
        /// from the <see cref="AggregateException"/> if one is present.
        /// </summary>
        /// <remarks>
        /// If no <typeparamref name="TPreferredException"/> inner exception is present, this
        /// method returns the first inner exception.   All inner exceptions will be traced,
        /// including the one returned.   The containing <paramref name="aggregateException"/>
        /// will not be traced unless there are no inner exceptions.
        /// </remarks>
        /// <typeparam name="TPreferredException">The preferred type of inner exception to extract.   
        /// Use <c>typeof(Exception)</c> to extract the first exception regardless of type.</typeparam>
        /// <param name="aggregateException">The <see cref="AggregateException"/> to examine.</param>
        /// <param name="eventSource">The event source to trace.</param>
        /// <returns>The extracted exception.  It will not be <c>null</c> 
        /// but it may not be of type <typeparamref name="TPreferredException"/>.</returns>
        public Exception AsError<TPreferredException>(AggregateException aggregateException, string eventSource)
        {
            Fx.Assert(aggregateException != null, "aggregateException cannot be null.");

            // If aggregateException contains any fatal exceptions, return it directly
            // without tracing it or any inner exceptions.
            if (Fx.IsFatal(aggregateException))
            {
                return aggregateException;
            }

            // Collapse possibly nested graph into a flat list.
            // Empty inner exception list is unlikely but possible via public api.
            ReadOnlyCollection<Exception> innerExceptions = aggregateException.Flatten().InnerExceptions;
            if (innerExceptions.Count == 0)
            {
                return TraceException(aggregateException, eventSource);
            }

            // Find the first inner exception, giving precedence to TPreferredException
            Exception favoredException = null;
            foreach (Exception nextInnerException in innerExceptions)
            {
                // AggregateException may wrap TargetInvocationException, so unwrap those as well
                TargetInvocationException targetInvocationException = nextInnerException as TargetInvocationException;

                Exception innerException = (targetInvocationException != null && targetInvocationException.InnerException != null)
                                                ? targetInvocationException.InnerException
                                                : nextInnerException;

                if (innerException is TPreferredException && favoredException == null)
                {
                    favoredException = innerException;
                }

                // All inner exceptions are traced
                TraceException<Exception>(innerException, eventSource);
            }

            if (favoredException == null)
            {
                Fx.Assert(innerExceptions.Count > 0, "InnerException.Count is known to be > 0 here.");
                favoredException = innerExceptions[0];
            }

            return favoredException;
        }

        private TException TraceException<TException>(TException exception)
            where TException : Exception
        {
            return TraceException<TException>(exception, _eventSourceName);
        }

        private TException TraceException<TException>(TException exception, string eventSource)
            where TException : Exception
        {
            if (TraceCore.ThrowingExceptionIsEnabled(_diagnosticTrace))
            {
                TraceCore.ThrowingException(_diagnosticTrace, eventSource, exception != null ? exception.ToString() : string.Empty, exception);
            }

            BreakOnException(exception);

            return exception;
        }

        [Conditional("DEBUG")]
        private void BreakOnException(Exception exception)
        {
            // This method should check an array of exception types in 
            // Fx.BreakOnExceptionTypes and cause a native debug break
            // if one of those exception types is being thrown. We need
            // to replace this with a cross platform mechanism.
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void TraceFailFast(string message)
        {
        }
    }
}