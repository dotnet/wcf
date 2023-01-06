// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Runtime
{
    internal class CallbackException : FatalException
    {
        public CallbackException()
        {
        }

        public CallbackException(string message, Exception innerException) : base(message, innerException)
        {
            // This can't throw something like ArgumentException because that would be worse than
            // throwing the callback exception that was requested.
            Fx.Assert(innerException != null, "CallbackException requires an inner exception.");
            Fx.Assert(!Fx.IsFatal(innerException), "CallbackException can't be used to wrap fatal exceptions.");
        }
    }
}
