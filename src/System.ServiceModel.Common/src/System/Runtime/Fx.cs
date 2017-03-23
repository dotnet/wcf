// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Diagnostics;

namespace System.Runtime
{
    public static class Fx
    {
#if DEBUG
        //const string WinFXRegistryKey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP";
        //const string WcfRegistryKey = WinFXRegistryKey + @"\CDF\v4.0\Debug";
        //const string AssertsFailFastName = "AssertsFailFast";
        //const string BreakOnExceptionTypesName = "BreakOnExceptionTypes";
        //const string FastDebugName = "FastDebug";
        //const string StealthDebuggerName = "StealthDebugger";
        //static bool fastDebugRetrieved;
        //static bool fastDebugCache;
        //static bool stealthDebuggerRetrieved;
        //static bool stealthDebuggerCache;
#endif

        private const string defaultEventSource = "System.Runtime";

        private static ExceptionTrace s_exceptionTrace;
        private static EtwDiagnosticTrace s_diagnosticTrace;

        internal static ExceptionTrace Exception
        {
            get
            {
                if (s_exceptionTrace == null)
                {
                    // don't need a lock here since a true singleton is not required
                    s_exceptionTrace = new ExceptionTrace(defaultEventSource, Trace);
                }

                return s_exceptionTrace;
            }
        }

        internal static EtwDiagnosticTrace Trace
        {
            get
            {
                if (s_diagnosticTrace == null)
                {
                    s_diagnosticTrace = InitializeTracing();
                }

                return s_diagnosticTrace;
            }
        }

        private static EtwDiagnosticTrace InitializeTracing()
        {
            EtwDiagnosticTrace trace = new EtwDiagnosticTrace(defaultEventSource, EtwDiagnosticTrace.DefaultEtwProviderId);

            return trace;
        }

        [Conditional("DEBUG")]
        public static void Assert(bool condition, string description)
        {
            if (!condition)
            {
                Assert(description);
            }
        }

        [Conditional("DEBUG")]
        public static void Assert(string description)
        {
            AssertHelper.FireAssert(description);
        }

        public static void AssertAndThrowFatal(bool condition, string description)
        {
            if (!condition)
            {
                AssertAndThrowFatal(description);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Exception AssertAndThrowFatal(string description)
        {
            Fx.Assert(description);
            TraceCore.ShipAssertExceptionMessage(Trace, description);
            throw new FatalInternalException(description);
        }


        public static bool IsFatal(Exception exception)
        {
            while (exception != null)
            {
                if (exception is FatalException ||
                    exception is OutOfMemoryException ||
                    exception is FatalInternalException)
                {
                    return true;
                }

                // These exceptions aren't themselves fatal, but since the CLR uses them to wrap other exceptions,
                // we want to check to see whether they've been used to wrap a fatal exception.  If so, then they
                // count as fatal.
                if (exception is TypeInitializationException ||
                    exception is TargetInvocationException)
                {
                    exception = exception.InnerException;
                }
                else if (exception is AggregateException)
                {
                    // AggregateExceptions have a collection of inner exceptions, which may themselves be other
                    // wrapping exceptions (including nested AggregateExceptions).  Recursively walk this
                    // hierarchy.  The (singular) InnerException is included in the collection.
                    ReadOnlyCollection<Exception> innerExceptions = ((AggregateException)exception).InnerExceptions;
                    foreach (Exception innerException in innerExceptions)
                    {
                        if (IsFatal(innerException))
                        {
                            return true;
                        }
                    }

                    break;
                }
                else
                {
                    break;
                }
            }

            return false;
        }

        // This method should be only used for debug build.
        internal static bool AssertsFailFast
        {
            get
            {
                return false;
            }
        }

        internal class InternalException : Exception
        {
            public InternalException(string description)
                : base(SR.Format(SR.ShipAssertExceptionMessage, description))
            {
            }
        }

        internal class FatalInternalException : InternalException
        {
            public FatalInternalException(string description)
                : base(description)
            {
            }
        }
        public static byte[] AllocateByteArray(int size)
        {
            try
            {
                // Safe to catch OOM from this as long as the ONLY thing it does is a simple allocation of a primitive type (no method calls).
                return new byte[size];
            }
            catch (OutOfMemoryException exception)
            {
                // Desktop wraps the OOM inside a new InsufficientMemoryException, traces, and then throws it.
                // Project N and K trace and throw the original OOM.  InsufficientMemoryException does not exist in N and K.
                Exception.AsError(exception);
                throw;
            }
        }
    }
}