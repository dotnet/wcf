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
        private const ushort FailFastEventLogCategory = 6;
        internal const int MaxExceptionStringLength = 28 * 1024;
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
                        string serializedException = EtwDiagnosticTrace.ExceptionToTraceString(exception, MaxExceptionStringLength);
                        WcfEventSource.Instance.ThrowingEtwException(_eventSourceName, exception != null ? exception.ToString() : string.Empty, serializedException);
                    }
                    break;
                case EventLevel.Critical:
                    if (WcfEventSource.Instance.EtwUnhandledExceptionIsEnabled())
                    {
                        string serializedException = EtwDiagnosticTrace.ExceptionToTraceString(exception, MaxExceptionStringLength);
                        WcfEventSource.Instance.EtwUnhandledException(exception != null ? exception.ToString() : string.Empty, serializedException);
                    }
                    break;
                default:
                    if (WcfEventSource.Instance.ThrowingEtwExceptionVerboseIsEnabled())
                    {
                        string serializedException = EtwDiagnosticTrace.ExceptionToTraceString(exception, MaxExceptionStringLength);
                        WcfEventSource.Instance.ThrowingEtwExceptionVerbose(_eventSourceName, exception != null ? exception.ToString() : string.Empty, serializedException);
                    }

                    break;
            }
        }

        public void TraceEtwException(Exception exception, TraceEventType eventLevel)
        {
            switch (eventLevel)
            {
                case TraceEventType.Error:
                case TraceEventType.Warning:
                    if (WcfEventSource.Instance.ThrowingEtwExceptionIsEnabled())
                    {
                        string serializedException = EtwDiagnosticTrace.ExceptionToTraceString(exception, MaxExceptionStringLength);
                        WcfEventSource.Instance.ThrowingEtwException(_eventSourceName, exception != null ? exception.ToString() : string.Empty, serializedException);
                    }
                    break;
                case TraceEventType.Critical:
                    if (WcfEventSource.Instance.EtwUnhandledExceptionIsEnabled())
                    {
                        string serializedException = EtwDiagnosticTrace.ExceptionToTraceString(exception, MaxExceptionStringLength);
                        WcfEventSource.Instance.EtwUnhandledException(exception != null ? exception.ToString() : string.Empty, serializedException);
                    }
                    break;
                default:
                    if (WcfEventSource.Instance.ThrowingEtwExceptionVerboseIsEnabled())
                    {
                        string serializedException = EtwDiagnosticTrace.ExceptionToTraceString(exception, MaxExceptionStringLength);
                        WcfEventSource.Instance.ThrowingEtwExceptionVerbose(_eventSourceName, exception != null ? exception.ToString() : string.Empty, serializedException);
                    }

                    break;
            }
        }

        public void AsInformation(Exception exception)
        {
            //Traces an informational trace message
        }

        public void AsWarning(Exception exception)
        {
            //Traces a warning trace message
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

            return TraceException(exception);
        }

        public Exception AsError(Exception exception, string eventSource)
        {
            // AggregateExceptions are automatically unwrapped.
            AggregateException aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                return AsError<Exception>(aggregateException, eventSource);
            }

            // TargetInvocationExceptions are automatically unwrapped.
            TargetInvocationException targetInvocationException = exception as TargetInvocationException;
            if (targetInvocationException != null && targetInvocationException.InnerException != null)
            {
                return AsError(targetInvocationException.InnerException, eventSource);
            }

            return TraceException(exception, eventSource);
        }

        public Exception AsError(TargetInvocationException targetInvocationException, string eventSource)
        {
            Fx.Assert(targetInvocationException != null, "targetInvocationException cannot be null.");

            // If targetInvocationException contains any fatal exceptions, return it directly
            // without tracing it or any inner exceptions.
            if (Fx.IsFatal(targetInvocationException))
            {
                return targetInvocationException;
            }

            // A non-null inner exception could require further unwrapping in AsError.
            Exception innerException = targetInvocationException.InnerException;
            if (innerException != null)
            {
                return AsError(innerException, eventSource);
            }

            // A null inner exception is unlikely but possible.
            // In this case, trace and return the targetInvocationException itself.
            return TraceException<Exception>(targetInvocationException, eventSource);
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
                TraceException(innerException, eventSource);
            }

            if (favoredException == null)
            {
                Fx.Assert(innerExceptions.Count > 0, "InnerException.Count is known to be > 0 here.");
                favoredException = innerExceptions[0];
            }

            return favoredException;
        }

        public ArgumentException Argument(string paramName, string message)
        {
            return TraceException(new ArgumentException(message, paramName));
        }

        public ArgumentNullException ArgumentNull(string paramName)
        {
            return TraceException(new ArgumentNullException(paramName));
        }

        public ArgumentNullException ArgumentNull(string paramName, string message)
        {
            return TraceException(new ArgumentNullException(paramName, message));
        }

        public ArgumentException ArgumentNullOrEmpty(string paramName)
        {
            return Argument(paramName, InternalSR.ArgumentNullOrEmpty(paramName));
        }

        public ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, object actualValue, string message)
        {
            return TraceException(new ArgumentOutOfRangeException(paramName, actualValue, message));
        }

        // When throwing ObjectDisposedException, it is highly recommended that you use this ctor
        // [C#]
        // public ObjectDisposedException(string objectName, string message);
        // And provide null for objectName but meaningful and relevant message for message. 
        // It is recommended because end user really does not care or can do anything on the disposed object, commonly an internal or private object.
        public ObjectDisposedException ObjectDisposed(string message)
        {
            // pass in null, not disposedObject.GetType().FullName as per the above guideline
            return TraceException(new ObjectDisposedException(null, message));
        }

        public void TraceHandledException(Exception exception, TraceEventType traceEventType)
        {
            switch (traceEventType)
            {
                case TraceEventType.Error:
                    if (!TraceCore.HandledExceptionErrorIsEnabled(_diagnosticTrace))
                        break;
                    TraceCore.HandledExceptionError(_diagnosticTrace, exception != null ? exception.ToString() : string.Empty, exception);
                    break;
                case TraceEventType.Warning:
                    if (!TraceCore.HandledExceptionWarningIsEnabled(_diagnosticTrace))
                        break;
                    TraceCore.HandledExceptionWarning(_diagnosticTrace, exception != null ? exception.ToString() : string.Empty, exception);
                    break;
                case TraceEventType.Verbose:
                    if (!TraceCore.HandledExceptionVerboseIsEnabled(_diagnosticTrace))
                        break;
                    TraceCore.HandledExceptionVerbose(_diagnosticTrace, exception != null ? exception.ToString() : string.Empty, exception);
                    break;
                default:
                    if (!TraceCore.HandledExceptionIsEnabled(_diagnosticTrace))
                        break;
                    TraceCore.HandledException(_diagnosticTrace, exception != null ? exception.ToString() : string.Empty, exception);
                    break;
            }
        }

        public void TraceUnhandledException(Exception exception)
        {
            TraceCore.UnhandledException(_diagnosticTrace, exception != null ? exception.ToString() : string.Empty, exception);
        }

        private TException TraceException<TException>(TException exception)
            where TException : Exception
        {
            return TraceException(exception, _eventSourceName);
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

        private void BreakOnException(Exception exception)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void TraceFailFast(string message)
        {
        }
    }
}
