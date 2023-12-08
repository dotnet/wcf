// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    [Serializable]
    public class MessageHeaderException : ProtocolException
    {
        public MessageHeaderException(string message) : this(message, null, null) { }
        public MessageHeaderException(string message, bool isDuplicate) : this(message, null, null, isDuplicate, null) { }
        public MessageHeaderException(string message, Exception innerException) : this(message, null, null, innerException) { }
        public MessageHeaderException(string message, string headerName, string ns) : this(message, headerName, ns, null) { }
        public MessageHeaderException(string message, string headerName, string ns, bool isDuplicate) : this(message, headerName, ns, isDuplicate, null) { }
        public MessageHeaderException(string message, string headerName, string ns, Exception innerException) : this(message, headerName, ns, false, innerException) { }
        public MessageHeaderException(string message, string headerName, string ns, bool isDuplicate, Exception innerException) : base(message, innerException)
        {
            HeaderName = headerName;
            HeaderNamespace = ns;
            IsDuplicate = isDuplicate;
        }

        public string HeaderName { get; }

        public string HeaderNamespace { get; }

        // IsDuplicate==true means there was more than one; IsDuplicate==false means there were zero
        public bool IsDuplicate { get; }

        internal Message ProvideFault(MessageVersion messageVersion)
        {
            Contract.Assert(messageVersion.Addressing == AddressingVersion.WSAddressing10);
            WSAddressing10ProblemHeaderQNameFault phf = new WSAddressing10ProblemHeaderQNameFault(this);
            Message message = System.ServiceModel.Channels.Message.CreateMessage(messageVersion, phf, AddressingVersion.WSAddressing10.FaultAction);
            phf.AddHeaders(message.Headers);
            return message;
        }

        // for serialization
        public MessageHeaderException() { }
#pragma warning disable SYSLIB0051
        protected MessageHeaderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#pragma warning restore SYSLIB0051
    }
}
