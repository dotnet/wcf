// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace System.Runtime
{
    [Serializable]
    internal class FatalException : Exception
    {
        public FatalException() { }
        public FatalException(string message) : base(message) { }

        public FatalException(string message, Exception innerException) : base(message, innerException)
        {
            // This can't throw something like ArgumentException because that would be worse than
            // throwing the fatal exception that was requested.
            Fx.Assert(innerException == null || !Fx.IsFatal(innerException), "FatalException can't be used to wrap fatal exceptions.");
        }

#pragma warning disable SYSLIB0051
        protected FatalException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#pragma warning restore SYSLIB0051
    }
}
