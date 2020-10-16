// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    internal class ActionMismatchAddressingException : ProtocolException
    {
        private string _httpActionHeader;
        private string _soapActionHeader;

        public ActionMismatchAddressingException(string message, string soapActionHeader, string httpActionHeader)
            : base(message)
        {
            _httpActionHeader = httpActionHeader;
            _soapActionHeader = soapActionHeader;
        }

        public string HttpActionHeader
        {
            get
            {
                return _httpActionHeader;
            }
        }

        public string SoapActionHeader
        {
            get
            {
                return _soapActionHeader;
            }
        }

        internal Message ProvideFault(MessageVersion messageVersion)
        {
            Fx.Assert(messageVersion.Addressing == AddressingVersion.WSAddressing10, "");
            WSAddressing10ProblemHeaderQNameFault phf = new WSAddressing10ProblemHeaderQNameFault(this);
            Message message = System.ServiceModel.Channels.Message.CreateMessage(messageVersion, phf, messageVersion.Addressing.FaultAction);
            phf.AddHeaders(message.Headers);
            return message;
        }
    }
}
