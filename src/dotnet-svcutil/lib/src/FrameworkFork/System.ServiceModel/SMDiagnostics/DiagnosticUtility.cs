// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable 1634, 1691

using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.ServiceModel.Diagnostics;

namespace System
{
    /// <summary>
    /// This is the Management utility class.
    /// </summary>
    internal partial class DiagnosticUtility
    {
        private const string TraceSourceName = "TraceSourceNameToReplace";
        internal const string EventSourceName = TraceSourceName + " [COR_BUILD_MAJOR].[COR_BUILD_MINOR].[CLR_OFFICIAL_ASSEMBLY_NUMBER].0";
        internal const string DefaultTraceListenerName = "Default";

        private static bool s_shouldUseActivity = false;

        private static object s_lockObject = new object();
        private static ExceptionUtility s_exceptionUtility = null;
        private static void UpdateLevel()
        {
#pragma warning disable 618

#pragma warning restore 618
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
#pragma warning disable 618
                    DiagnosticUtility.s_exceptionUtility = new ExceptionUtility(DiagnosticUtility.TraceSourceName, DiagnosticUtility.EventSourceName, FxTrace.Exception);
#pragma warning restore 618
                }
            }
            return DiagnosticUtility.s_exceptionUtility;
        }

        static internal bool ShouldUseActivity
        {
            get { return DiagnosticUtility.s_shouldUseActivity; }
        }

        internal static class Utility
        {
            public static byte[] AllocateByteArray(int size)
            {
                try
                {
                    // Safe to catch OOM from this as long as the ONLY thing it does is a simple allocation of a primitive type (no method calls).
                    return new byte[size];
                }
                catch (OutOfMemoryException)
                {
                    // Convert OOM into an exception that can be safely handled by higher layers.
                    throw Fx.Exception.AsError(new InvalidOperationException());
                    // TODO: new InsufficientMemoryException(InternalSR.BufferAllocationFailed(size), exception));
                }
            }
        }
    }
}
#pragma warning restore 1634, 1691
