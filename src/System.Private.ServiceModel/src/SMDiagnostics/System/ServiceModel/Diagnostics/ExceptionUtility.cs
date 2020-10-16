// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Tracing;
using System.Runtime;
using System.Xml;

namespace System.ServiceModel.Diagnostics
{
    internal partial class ExceptionUtility
    {
        private const string ExceptionStackAsStringKey = "System.ServiceModel.Diagnostics.ExceptionUtility.ExceptionStackAsString";

        // This field should be only used for debug build.
        internal static ExceptionUtility mainInstance;

        [ThreadStatic]
        private static Guid s_activityId;

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

        public ArgumentNullException ThrowHelperArgumentNull(string paramName, string message)
        {
            return (ArgumentNullException)ThrowHelperError(new ArgumentNullException(paramName, message));
        }

        public ArgumentException ThrowHelperArgumentNullOrEmptyString(string arg)
        {
            return (ArgumentException)ThrowHelperError(new ArgumentException(SR.StringNullOrEmpty, arg));
        }

        public Exception ThrowHelperFatal(string message, Exception innerException)
        {
            return ThrowHelperError(new FatalException(message, innerException));
        }

        public Exception ThrowHelperInternal(bool fatal)
        {
            return fatal ? Fx.AssertAndThrowFatal("Fatal InternalException should never be thrown.") : Fx.AssertAndThrow("InternalException should never be thrown.");
        }

        public Exception ThrowHelperInvalidOperation(string message)
        {
            return ThrowHelperError(new InvalidOperationException(message));
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

        public Exception ThrowHelperWarning(Exception exception)
        {
            return ThrowHelper(exception, EventLevel.Warning);
        }

        internal Exception ThrowHelperXml(XmlReader reader, string message)
        {
            return ThrowHelperXml(reader, message, null);
        }

        internal Exception ThrowHelperXml(XmlReader reader, string message, Exception inner)
        {
            IXmlLineInfo lineInfo = reader as IXmlLineInfo;
            return ThrowHelperError(new XmlException(
                message,
                inner,
                (null != lineInfo) ? lineInfo.LineNumber : 0,
                (null != lineInfo) ? lineInfo.LinePosition : 0));
        }

        // On a single thread, these functions will complete just fine
        // and don't need to worry about locking issues because the effected
        // variables are ThreadStatic.
        internal static void UseActivityId(Guid activityId)
        {
            ExceptionUtility.s_activityId = activityId;
        }

        internal static void ClearActivityId()
        {
            ExceptionUtility.s_activityId = Guid.Empty;
        }
    }
}
