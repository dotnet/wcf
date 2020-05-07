// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public class ProtocolException : CommunicationException
    {
        public ProtocolException() { }
        public ProtocolException(string message) : base(message) { }
        public ProtocolException(string message, Exception innerException) : base(message, innerException) { }

        internal static ProtocolException ReceiveShutdownReturnedNonNull(Message message)
        {
            if (message.IsFault)
            {
                try
                {
                    MessageFault fault = MessageFault.CreateFault(message, 64 * 1024);
                    FaultReasonText reason = fault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture);
                    string text = string.Format(SRServiceModel.ReceiveShutdownReturnedFault, reason.Text);
                    return new ProtocolException(text);
                }
                catch (QuotaExceededException)
                {
                    string text = string.Format(SRServiceModel.ReceiveShutdownReturnedLargeFault, message.Headers.Action);
                    return new ProtocolException(text);
                }
            }
            else
            {
                string text = string.Format(SRServiceModel.ReceiveShutdownReturnedMessage, message.Headers.Action);
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
                    string text = string.Format(SRServiceModel.OneWayOperationReturnedFault, reason.Text);
                    return new ProtocolException(text);
                }
                catch (QuotaExceededException)
                {
                    string text = string.Format(SRServiceModel.OneWayOperationReturnedLargeFault, message.Headers.Action);
                    return new ProtocolException(text);
                }
            }
            else
            {
                string text = string.Format(SRServiceModel.OneWayOperationReturnedMessage, message.Headers.Action);
                return new ProtocolException(text);
            }
        }
    }
}
