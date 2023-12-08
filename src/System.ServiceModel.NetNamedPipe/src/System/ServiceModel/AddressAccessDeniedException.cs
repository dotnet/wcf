// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System.ServiceModel
{
    [Serializable]
    public class AddressAccessDeniedException : CommunicationException
    {
        public AddressAccessDeniedException() { }
        public AddressAccessDeniedException(string message) : base(message) { }
        public AddressAccessDeniedException(string message, Exception innerException) : base(message, innerException) { }
#pragma warning disable SYSLIB0051
        protected AddressAccessDeniedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#pragma warning restore SYSLIB0051
    }
}
