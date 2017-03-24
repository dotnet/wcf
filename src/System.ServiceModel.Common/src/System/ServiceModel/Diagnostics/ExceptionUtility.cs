// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Tracing;
using System.Runtime;

namespace System.ServiceModel.Diagnostics
{
    public class ExceptionUtility
    {
        private ExceptionTrace _exceptionTrace;
        private string _name;
        private string _eventSourceName;
        
        internal ExceptionUtility(string name, string eventSourceName, object exceptionTrace)
        {
            _exceptionTrace = (ExceptionTrace)exceptionTrace;
            _name = name;
            _eventSourceName = eventSourceName;
        }

        public ArgumentException ThrowHelperArgument(string message)
        {
            return (ArgumentException)ThrowHelperError(new ArgumentException(message));
        }

        public ArgumentException ThrowHelperArgument(string paramName, string message)
        {
            return (ArgumentException)ThrowHelperError(new ArgumentException(message, paramName));
        }

        public ArgumentNullException ThrowHelperArgumentNull(string paramName)
        {
            return (ArgumentNullException)ThrowHelperError(new ArgumentNullException(paramName));
        }

        public Exception ThrowHelperError(Exception exception)
        {
            return ThrowHelper(exception, EventLevel.Error);
        }

        internal Exception ThrowHelper(Exception exception, EventLevel eventLevel)
        {
            FxTrace.Exception.TraceEtwException(exception, eventLevel);

            return exception;
        }

        public Exception ThrowHelperCallback(string message, Exception innerException)
        {
            return ThrowHelperCritical(new CallbackException(message, innerException));
        }

        public Exception ThrowHelperCallback(Exception innerException)
        {
            return ThrowHelperCallback(SR.GenericCallbackException, innerException);
        }

        public Exception ThrowHelperCritical(Exception exception)
        {
            return ThrowHelper(exception, EventLevel.Critical);
        }
    }
}