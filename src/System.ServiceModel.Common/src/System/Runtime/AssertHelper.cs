// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Diagnostics;

namespace System.Runtime
{
    internal static class AssertHelper
    {
        
        internal static void FireAssert(string message)
        {
            try
            {
                InternalFireAssert(ref message);
            }
            finally
            {
                Debug.Assert(false, message);
            }
        }

        private static void InternalFireAssert(ref string message)
        {
            try
            {
                if (Fx.AssertsFailFast)
                {
                    try
                    {
                        Fx.Exception.TraceFailFast(message);
                    }
                    finally
                    {
                        Environment.FailFast(message);
                    }
                }
            }
            catch (Exception exception)
            {
                if (Fx.IsFatal(exception))
                {
                    throw;
                }

                string newMessage = "Exception during FireAssert!";
                try
                {
                    newMessage = string.Concat(newMessage, " [", exception.GetType().Name, ": ", exception.Message, "] --> ", message);
                }
                finally
                {
                    message = newMessage;
                }
                throw;
            }
        }
    }
}