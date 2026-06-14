// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.IO;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace System.ServiceModel.Dispatcher
{
    internal class HttpStreamFormatter : IDispatchMessageFormatter, IClientMessageFormatter
    {
        private readonly string _contractName;
        private readonly string _contractNs;
        private readonly string _operationName;

        public HttpStreamFormatter(OperationDescription operation)
        {
            if (operation == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(operation));
            }

            _operationName = operation.Name;
            _contractName = operation.DeclaringContract.Name;
            _contractNs = operation.DeclaringContract.Namespace;
        }

        public void DeserializeRequest(Message message, object[] parameters)
        {
            parameters[0] = GetStreamFromMessage(message, true);
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            Message message = CreateMessageFromStream(result);
            if (result == null)
            {
                SingleBodyParameterMessageFormatter.SuppressReplyEntityBody(message);
            }

            return message;
        }

        public object DeserializeReply(Message message, object[] parameters)
        {
            return GetStreamFromMessage(message, false);
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            Message message = CreateMessageFromStream(parameters[0]);
            if (parameters[0] == null)
            {
                SingleBodyParameterMessageFormatter.SuppressRequestEntityBody(message);
            }
            return message;
        }

        internal static bool IsEmptyMessage(Message message) => message.IsEmpty;

        private Message CreateMessageFromStream(object data)
        {
            Message result;
            if (data == null)
            {
                result = Message.CreateMessage(MessageVersion.None, (string)null);
            }
            else
            {
                if (!(data is Stream streamData))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.Format(SR.ParameterIsNotStreamType, data.GetType(), _operationName, _contractName, _contractNs)));
                }

                // ByteStreamMessage / raw octet-stream pass-through is not supported in this client port.
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new NotSupportedException(SR.WebHttpRawContentNotSupported));
            }

            return result;
        }

        private Stream GetStreamFromMessage(Message message, bool isRequest)
        {
            message.Properties.TryGetValue(WebBodyFormatMessageProperty.Name, out object prop);
            WebBodyFormatMessageProperty formatProperty = (prop as WebBodyFormatMessageProperty);
            if (formatProperty == null)
            {
                // GET and DELETE do not go through the encoder
                if (IsEmptyMessage(message))
                {
                    return new MemoryStream();
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(SR.Format(SR.MessageFormatPropertyNotFound, _operationName, _contractName, _contractNs)));
                }
            }

            if (formatProperty.Format != WebContentFormat.Raw)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(SR.Format(SR.InvalidHttpMessageFormat, _operationName, _contractName, _contractNs, formatProperty.Format, WebContentFormat.Raw)));
            }

            // MessageBodyStream is server-side only; not ported.
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                new NotSupportedException(SR.WebHttpRawContentNotSupported));
        }
    }
}
