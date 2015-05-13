// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Dispatcher
{
    internal sealed class MessageOperationFormatter : IClientMessageFormatter
    {
        public object DeserializeReply(Message message, object[] parameters)
        {
            if (message == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("message"));
            if (parameters != null && parameters.Length > 0)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.SFxParametersMustBeEmpty));

            return message;
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            if (parameters == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("parameters"));
            if (parameters.Length != 1 || !(parameters[0] is Message))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.SFxParameterMustBeMessage));

            return (Message)parameters[0];
        }
    }
}
