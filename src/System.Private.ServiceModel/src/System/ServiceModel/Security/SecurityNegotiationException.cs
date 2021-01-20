// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace System.ServiceModel.Security
{
    [Serializable]
    public class SecurityNegotiationException : CommunicationException
    {
        public SecurityNegotiationException() : base() { }
        public SecurityNegotiationException(string message) : base(message) { }
        public SecurityNegotiationException(String message, Exception innerException) : base(message, innerException) { }
        protected SecurityNegotiationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
