// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    [Serializable]
    internal class ActionMismatchAddressingException : ProtocolException
    {
        public ActionMismatchAddressingException(string message, string soapActionHeader, string httpActionHeader)
            : base(message)
        {
            HttpActionHeader = httpActionHeader;
            SoapActionHeader = soapActionHeader;
        }

#pragma warning disable SYSLIB0051
        protected ActionMismatchAddressingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#pragma warning restore SYSLIB0051

        public string HttpActionHeader { get; }

        public string SoapActionHeader { get; }

        internal Message ProvideFault(MessageVersion messageVersion)
        {
            Fx.Assert(messageVersion.Addressing == AddressingVersion.WSAddressing10, "");
            WSAddressing10ProblemHeaderQNameFault phf = new WSAddressing10ProblemHeaderQNameFault(this);
            Message message = Channels.Message.CreateMessage(messageVersion, phf, messageVersion.Addressing.FaultAction);
            phf.AddHeaders(message.Headers);
            return message;
        }
    }
}
