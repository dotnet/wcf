// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Diagnostics;


namespace System
{
    internal partial class DiagnosticUtility
    {
        [Conditional("DEBUG")]
        public static void DebugAssert(string message)
        {
            DebugAssert(false, message);
        }

        [Conditional("DEBUG")]
        public static void DebugAssert(bool condition, string message)
        {
            Debug.Assert(condition, message);
        }

        internal static bool IsFatal(Exception exception)
        {
            while (exception != null)
            {
                // These exceptions aren't themselves fatal, but since the CLR uses them to wrap other exceptions,
                // we want to check to see whether they've been used to wrap a fatal exception.  If so, then they
                // count as fatal.
                if (exception is TypeInitializationException)
                {
                    exception = exception.InnerException;
                }
                else
                {
                    break;
                }
            }

            return false;
        }
    }
}

