// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public class MessageHeaderException : ProtocolException
    {
        private string _headerName;
        private string _headerNamespace;
        private bool _isDuplicate;

        public MessageHeaderException(string message)
            : this(message, null, null)
        {
        }
        public MessageHeaderException(string message, bool isDuplicate)
            : this(message, null, null)
        {
        }
        public MessageHeaderException(string message, Exception innerException)
            : this(message, null, null, innerException)
        {
        }
        public MessageHeaderException(string message, string headerName, string ns)
            : this(message, headerName, ns, null)
        {
        }
        public MessageHeaderException(string message, string headerName, string ns, bool isDuplicate)
            : this(message, headerName, ns, isDuplicate, null)
        {
        }
        public MessageHeaderException(string message, string headerName, string ns, Exception innerException)
            : this(message, headerName, ns, false, innerException)
        {
        }
        public MessageHeaderException(string message, string headerName, string ns, bool isDuplicate, Exception innerException)
            : base(message, innerException)
        {
            _headerName = headerName;
            _headerNamespace = ns;
            _isDuplicate = isDuplicate;
        }

        public string HeaderName { get { return _headerName; } }

        public string HeaderNamespace { get { return _headerNamespace; } }

        // IsDuplicate==true means there was more than one; IsDuplicate==false means there were zero
        public bool IsDuplicate { get { return _isDuplicate; } }

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
    }
}
