// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ServiceModel.Channels;

namespace System.ServiceModel.Security
{
    internal static class SecurityUtils
    {
        // Copied from TransportDefaults
        public const int MaxSecurityFaultSize = 16384;

        internal static void ThrowIfNegotiationFault(Message message, EndpointAddress target)
        {
            if (message.IsFault)
            {
                MessageFault fault = MessageFault.CreateFault(message, MaxSecurityFaultSize);
                Exception faultException = new FaultException(fault, message.Headers.Action);
                if (fault.Code != null && fault.Code.IsReceiverFault && fault.Code.SubCode != null)
                {
                    FaultCode subCode = fault.Code.SubCode;
                    if (subCode.Name == "ServerTooBusy" && subCode.Namespace == "http://schemas.microsoft.com/ws/2006/05/security")
                    {
                        throw new ServerTooBusyException(SR.Format(SR.SecurityServerTooBusy, target), faultException);
                    }
                    else if (subCode.Name == "EndpointUnavailable" && subCode.Namespace == message.Version.Addressing.Namespace())
                    {
                        throw new EndpointNotFoundException(SR.Format(SR.SecurityEndpointNotFound, target), faultException);
                    }
                }

                throw faultException;
            }
        }
    }

    internal static class AddressingVersionExtensions
    {
        public static string Namespace(this AddressingVersion addressingVersion)
        {
            // AddressingVersion.ToString() returns the string "{addressing name} ({addressing namespace})" so we can
            // extract the namespace out easily.
            var addressingVersionString = addressingVersion.ToString();
            int pos = addressingVersionString.IndexOf('(');
            return addressingVersionString.Substring(pos + 1, addressingVersionString.Length - pos - 2);
        }
    }
}
