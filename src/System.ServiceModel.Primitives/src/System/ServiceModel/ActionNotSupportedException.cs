// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    [Serializable]
    public class ActionNotSupportedException : CommunicationException
    {
        public ActionNotSupportedException() { }
        public ActionNotSupportedException(string message) : base(message) { }
        public ActionNotSupportedException(string message, Exception innerException) : base(message, innerException) { }
        [Obsolete(DiagnosticId = "SYSLIB0051")]
        protected ActionNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        internal Message ProvideFault(MessageVersion messageVersion)
        {
            Contract.Assert(messageVersion.Addressing != AddressingVersion.None);
            FaultCode code = FaultCode.CreateSenderFaultCode(AddressingStrings.ActionNotSupported, messageVersion.Addressing.Namespace);
            string reason = Message;
            return Channels.Message.CreateMessage(messageVersion, code, reason, messageVersion.Addressing.FaultAction);
        }
    }
}
