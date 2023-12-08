// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    [Serializable]
    public class ProtocolException : CommunicationException
    {
        public ProtocolException() { }
        public ProtocolException(string message) : base(message) { }
        public ProtocolException(string message, Exception innerException) : base(message, innerException) { }
#pragma warning disable SYSLIB0051
        protected ProtocolException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#pragma warning restore SYSLIB0051

        internal static ProtocolException ReceiveShutdownReturnedNonNull(Message message)
        {
            if (message.IsFault)
            {
                try
                {
                    MessageFault fault = MessageFault.CreateFault(message, 64 * 1024);
                    FaultReasonText reason = fault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture);
                    string text = SRP.Format(SRP.ReceiveShutdownReturnedFault, reason.Text);
                    return new ProtocolException(text);
                }
                catch (QuotaExceededException)
                {
                    string text = SRP.Format(SRP.ReceiveShutdownReturnedLargeFault, message.Headers.Action);
                    return new ProtocolException(text);
                }
            }
            else
            {
                string text = SRP.Format(SRP.ReceiveShutdownReturnedMessage, message.Headers.Action);
                return new ProtocolException(text);
            }
        }

        internal static ProtocolException OneWayOperationReturnedNonNull(Message message)
        {
            if (message.IsFault)
            {
                try
                {
                    MessageFault fault = MessageFault.CreateFault(message, 64 * 1024);
                    FaultReasonText reason = fault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture);
                    string text = SRP.Format(SRP.OneWayOperationReturnedFault, reason.Text);
                    return new ProtocolException(text);
                }
                catch (QuotaExceededException)
                {
                    string text = SRP.Format(SRP.OneWayOperationReturnedLargeFault, message.Headers.Action);
                    return new ProtocolException(text);
                }
            }
            else
            {
                string text = SRP.Format(SRP.OneWayOperationReturnedMessage, message.Headers.Action);
                return new ProtocolException(text);
            }
        }
    }
}
