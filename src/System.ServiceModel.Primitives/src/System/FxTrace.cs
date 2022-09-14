// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Runtime.Diagnostics;

namespace System
{
    internal static class FxTrace
    {
        private static ExceptionTrace s_exceptionTrace;
        private const string baseEventSourceName = "System.ServiceModel";
        private const string EventSourceVersion = "4.0.0.0";
        private static string s_eventSourceName;
        private static EtwDiagnosticTrace s_diagnosticTrace;
        private static readonly object s_lockObject = new object();

        public static ExceptionTrace Exception
        {
            get
            {
                if (s_exceptionTrace == null)
                {
                    // don't need a lock here since a true singleton is not required
                    s_exceptionTrace = new ExceptionTrace(EventSourceName, Trace);
                }

                return s_exceptionTrace;
            }
        }

        private static string EventSourceName
        {
            get
            {
                if (s_eventSourceName == null)
                {
                    s_eventSourceName = string.Concat(baseEventSourceName, " ", EventSourceVersion);
                }

                return s_eventSourceName;
            }
        }

        public static EtwDiagnosticTrace Trace
        {
            get
            {
                EnsureEtwProviderInitialized();
                return s_diagnosticTrace;
            }
        }

        private static void EnsureEtwProviderInitialized()
        {
            if (null == s_diagnosticTrace)
            {
                lock (s_lockObject)
                {
                    if (null == s_diagnosticTrace)
                    {
                        s_diagnosticTrace = InitializeTracing();
                    }
                }
            }
        }

        private static EtwDiagnosticTrace InitializeTracing()
        {
            EtwDiagnosticTrace trace = new EtwDiagnosticTrace();
            return trace;
        }
    }
}
