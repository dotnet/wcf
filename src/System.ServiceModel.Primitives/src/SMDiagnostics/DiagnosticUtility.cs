// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel
{
    /// <summary>
    /// This is the Management utility class.
    /// </summary>
    internal static class DiagnosticUtility
    {
        internal const string DefaultTraceListenerName = "Default";

        private const string TraceSourceName = "TraceSourceNameToReplace";
        internal const string EventSourceName = TraceSourceName + " [COR_BUILD_MAJOR].[COR_BUILD_MINOR].[CLR_OFFICIAL_ASSEMBLY_NUMBER].0";

        private static ExceptionUtility s_exceptionUtility = null;
        private static bool s_shouldUseActivity = false;
        private static object s_lockObject = new object();

        internal static bool ShouldUseActivity
        {
            get { return s_shouldUseActivity; }
        }

        public static ExceptionUtility ExceptionUtility
        {
            get
            {
                return s_exceptionUtility ?? GetExceptionUtility();
            }
        }

        private static ExceptionUtility GetExceptionUtility()
        {
            lock (s_lockObject)
            {
                if (s_exceptionUtility == null)
                {
                    s_exceptionUtility = new ExceptionUtility();
                }
            }

            return s_exceptionUtility;
        }

        internal static void TraceHandledException(Exception exception, TraceEventType traceEventType)
        {
            FxTrace.Exception.TraceHandledException(exception, traceEventType);
        }

        internal static bool ShouldTrace(TraceEventType type)
        {
            bool retval = false;
            if (TracingEnabled)
            {
                switch (type)
                {
                    case TraceEventType.Critical:
                        retval = ShouldTraceCritical;
                        break;
                    case TraceEventType.Error:
                        retval = ShouldTraceError;
                        break;
                    case TraceEventType.Warning:
                        retval = ShouldTraceWarning;
                        break;
                    case TraceEventType.Information:
                        retval = ShouldTraceInformation;
                        break;
                    case TraceEventType.Verbose:
                        retval = ShouldTraceVerbose;
                        break;
                }
            }
            return retval;
        }

        internal static bool ShouldTraceCritical => false;
        internal static bool ShouldTraceError => false;
        internal static bool ShouldTraceWarning => false;
        internal static bool ShouldTraceInformation => false;
        internal static bool ShouldTraceVerbose => false;
        internal static bool TracingEnabled => false;

        [Conditional("DEBUG")]
        internal static void DebugAssert(bool condition, string message)
        {
            if (!condition)
            {
                DebugAssert(message);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [Conditional("DEBUG")]
        internal static void DebugAssert(string message)
        {
            Fx.Assert(message);
        }
    }
}
