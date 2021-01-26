// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Tracing;

namespace System.ServiceModel.Diagnostics
{
    internal partial class ExceptionUtility
    {
        public Exception ThrowHelperError(Exception exception)
        {
            return ThrowHelper(exception, EventLevel.Error);
        }

        internal Exception ThrowHelperError(Exception exception, Guid activityId, object source)
        {
            return exception;
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
    }
}
