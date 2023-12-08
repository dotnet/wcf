// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace System.ServiceModel
{
    [Serializable]
    public class InvalidMessageContractException : Exception
    {
        public InvalidMessageContractException() : base() { }
        public InvalidMessageContractException(string message) : base(message) { }
        public InvalidMessageContractException(String message, Exception innerException) : base(message, innerException) { }
#pragma warning disable SYSLIB0051
        protected InvalidMessageContractException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#pragma warning restore SYSLIB0051
    }
}

