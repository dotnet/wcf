// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Serialization;

namespace System.ServiceModel
{
    // Thrown when an operation fails because the caller lacks the
    // required permissions on a transport endpoint — typically an
    // ACL on a named pipe or an MSMQ queue. Lives in
    // System.ServiceModel.Primitives so transports that need the
    // type don't have to take a coupling on each other.
    //
    // Lived in System.ServiceModel.NetNamedPipe through 8.x; promoted
    // here so the MSMQ transport (which surfaces the same semantic
    // for MQ_ERROR_ACCESS_DENIED and MQ_ERROR_SHARING_VIOLATION) can
    // map to it without re-defining the type or taking a back-reference
    // on NetNamedPipe. NetNamedPipe retains a [TypeForwardedTo] so
    // existing consumers see no API change.
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
