// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Serialization;

namespace System.ServiceModel
{
    [Serializable]
    public class ServerTooBusyException : CommunicationException
    {
        public ServerTooBusyException() { }
        public ServerTooBusyException(string message) : base(message) { }
        public ServerTooBusyException(string message, Exception innerException) : base(message, innerException) { }
#pragma warning disable SYSLIB0051
        protected ServerTooBusyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#pragma warning restore SYSLIB0051
    }
}
