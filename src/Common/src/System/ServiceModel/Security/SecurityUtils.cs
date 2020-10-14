// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Security
{
    internal static partial class SecurityUtils
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
                    if (subCode.Name == DotNetSecurityStrings.SecurityServerTooBusyFault && subCode.Namespace == DotNetSecurityStrings.Namespace)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ServerTooBusyException(SR.Format(SR.SecurityServerTooBusy, target), faultException));
                    }
                    else if (subCode.Name == AddressingStrings.EndpointUnavailable && subCode.Namespace == message.Version.Addressing.Namespace())
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new EndpointNotFoundException(SR.Format(SR.SecurityEndpointNotFound, target), faultException));
                    }
                }

                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(faultException);
            }
        }
    }

    internal static class AddressingVersionExtensions
    {
        public static string Namespace(this AddressingVersion addressingVersion)
        {
            // AddressingVersion.ToString() returns the string "{addressing name} ({addressing namespace})" so we can
            // extract the namespace out easily.
            var addressingVersionString = AddressingVersion.WSAddressingAugust2004.ToString();
            int pos = addressingVersionString.IndexOf('(');
            return addressingVersionString.Substring(pos + 1, addressingVersionString.Length - pos - 2);
        }
    }
}
