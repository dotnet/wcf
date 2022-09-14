// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.Diagnostics;
using System.Runtime;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Diagnostics
{
    internal partial class ExceptionUtility
    {
        public ArgumentException ThrowHelperArgument(string paramName, string message)
        {
            return (ArgumentException)ThrowHelperError(new ArgumentException(message, paramName));
        }

        internal Exception ThrowHelperError(Exception exception, Message message)
        {
            //// If the message is closed, we won't get an activity
            //Guid activityId = TraceUtility.ExtractActivityId(message);
            //if (DiagnosticUtility.ShouldTraceError)
            //{
            //    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Error, TraceCode.ThrowingException, GenerateMsdnTraceCode(TraceCode.ThrowingException),
            //        TraceSR.GetString(TraceSR.ThrowingException), null, exception, activityId, null);
            //}
            DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
            return exception;
        }

        public Exception ThrowHelperWarning(Exception exception)
        {
            return ThrowHelper(exception, EventLevel.Warning);
        }

        public Exception ThrowHelperError(Exception exception)
        {
            return ThrowHelper(exception, EventLevel.Error);
        }

        internal Exception ThrowHelperError(Exception exception, Guid activityId, object source)
        {
            return exception;
        }

        internal Exception ThrowHelper(Exception exception, TraceEventType eventType)
        {
            return ThrowHelper(exception, eventType, null);
        }

        internal Exception ThrowHelper(Exception exception, EventLevel eventLevel)
        {
            FxTrace.Exception.TraceEtwException(exception, eventLevel);
            return exception;
        }

        internal ArgumentNullException ThrowHelperArgumentNull(string paramName)
        {
            return (ArgumentNullException)ThrowHelperError(new ArgumentNullException(paramName));
        }

        internal Exception ThrowHelper(Exception exception, TraceEventType eventType, TraceRecord extendedData)
        {
            FxTrace.Exception.TraceEtwException(exception, eventType);

            return exception;
        }
    }
}
