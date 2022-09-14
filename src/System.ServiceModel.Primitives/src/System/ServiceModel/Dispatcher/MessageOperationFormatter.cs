// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Dispatcher
{
    internal sealed class MessageOperationFormatter : IClientMessageFormatter, IDispatchMessageFormatter
    {
        private static MessageOperationFormatter s_instance;

        internal static MessageOperationFormatter Instance
        {
            get
            {
                if (MessageOperationFormatter.s_instance == null)
                {
                    MessageOperationFormatter.s_instance = new MessageOperationFormatter();
                }

                return MessageOperationFormatter.s_instance;
            }
        }

        public object DeserializeReply(Message message, object[] parameters)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(message)));
            }

            if (parameters != null && parameters.Length > 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.SFxParametersMustBeEmpty));
            }

            return message;
        }

        public void DeserializeRequest(Message message, object[] parameters)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(message)));
            }

            if (parameters == null)
            {
                throw TraceUtility.ThrowHelperError(new ArgumentNullException(nameof(parameters)), message);
            }

            if (parameters.Length != 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.SFxParameterMustBeArrayOfOneElement));
            }

            parameters[0] = message;
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            if (!(result is Message))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.SFxResultMustBeMessage));
            }

            if (parameters != null && parameters.Length > 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.SFxParametersMustBeEmpty));
            }

            return (Message)result;
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            if (parameters == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(parameters)));
            }

            if (parameters.Length != 1 || !(parameters[0] is Message))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.SFxParameterMustBeMessage));
            }

            return (Message)parameters[0];
        }
    }
}
