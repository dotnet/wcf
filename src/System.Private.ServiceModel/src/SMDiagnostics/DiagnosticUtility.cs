// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


#pragma warning disable 1634, 1691

using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel
{
    /// <summary>
    /// This is the Management utility class.
    /// </summary>
    public static partial class DiagnosticUtility
    {
        private const string TraceSourceName = "TraceSourceNameToReplace";
        internal const string EventSourceName = TraceSourceName + " [COR_BUILD_MAJOR].[COR_BUILD_MINOR].[CLR_OFFICIAL_ASSEMBLY_NUMBER].0";
        internal const string DefaultTraceListenerName = "Default";

        private static bool s_shouldUseActivity = false;

        private static object s_lockObject = new object();

        private static ExceptionUtility s_exceptionUtility = null;

        private static void UpdateLevel()
        {
        }

        public static ExceptionUtility ExceptionUtility
        {
            get
            {
                return DiagnosticUtility.s_exceptionUtility ?? GetExceptionUtility();
            }
        }

        private static ExceptionUtility GetExceptionUtility()
        {
            lock (DiagnosticUtility.s_lockObject)
            {
                if (DiagnosticUtility.s_exceptionUtility == null)
                {
                    DiagnosticUtility.s_exceptionUtility = new ExceptionUtility(DiagnosticUtility.TraceSourceName, DiagnosticUtility.EventSourceName, FxTrace.Exception);
                }
            }
            return DiagnosticUtility.s_exceptionUtility;
        }

        internal static void TraceHandledException(Exception exception, TraceEventType traceEventType)
        {
            FxTrace.Exception.TraceHandledException(exception, traceEventType);
        }

        static internal bool ShouldUseActivity
        {
            get { return DiagnosticUtility.s_shouldUseActivity; }
        }


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

        internal static bool ShouldTraceCritical => false;
        internal static bool ShouldTraceError => false;
        internal static bool ShouldTraceWarning => false;
        internal static bool ShouldTraceInformation => false;
        internal static bool ShouldTraceVerbose => false;
        internal static bool TracingEnabled => false;

        internal static bool ShouldTrace(TraceEventType type)
        {
            bool retval = false;
            if (DiagnosticUtility.TracingEnabled)
            {
                switch (type)
                {
                    case TraceEventType.Critical:
                        retval = DiagnosticUtility.ShouldTraceCritical;
                        break;
                    case TraceEventType.Error:
                        retval = DiagnosticUtility.ShouldTraceError;
                        break;
                    case TraceEventType.Warning:
                        retval = DiagnosticUtility.ShouldTraceWarning;
                        break;
                    case TraceEventType.Information:
                        retval = DiagnosticUtility.ShouldTraceInformation;
                        break;
                    case TraceEventType.Verbose:
                        retval = DiagnosticUtility.ShouldTraceVerbose;
                        break;
                }
            }
            return retval;
        }
    }
}
