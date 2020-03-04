// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Diagnostics;


namespace System
{
// Not needed in dotnet-svcutil scenario. 
//     internal static class Fx
//     {
//         [Conditional("DEBUG")]
//         public static void Assert(bool condition, string message)
//         {
//             System.Diagnostics.Debug.Assert(condition, message);
//         }
// 
//         [Conditional("DEBUG")]
//         public static void Assert(string message)
//         {
//             Assert(false, message);
//         }
//     }


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

// Not needed in dotnet-svcutil scenario. 
//         internal static class ExceptionUtility
//         {
//             public static Exception ThrowHelperArgumentNull(string message)
//             {
//                 return new ArgumentNullException(message);
//             }
// 
//             public static Exception ThrowHelperError(Exception e)
//             {
//                 return e;
//             }
// 
//             public static Exception ThrowHelperArgument(string message)
//             {
//                 return new ArgumentException(message);
//             }
// 
//             public static Exception ThrowHelperArgument(string paramName, string message)
//             {
//                 return new ArgumentException(message, paramName);
//             }
// 
//             internal static Exception ThrowHelperFatal(string message, Exception innerException)
//             {
//                 return ThrowHelperError(new Exception(message, innerException));
//             }
//             internal static Exception ThrowHelperCallback(Exception e)
//             {
//                 return ThrowHelperError(e);
//             }
//         }

    }
}

// Not needed in dotnet-svcutil scenario. 
// namespace System.ServiceModel
// {
//     internal class DiagnosticUtility : System.Runtime.Serialization.DiagnosticUtility
//     {
//     }
// }

