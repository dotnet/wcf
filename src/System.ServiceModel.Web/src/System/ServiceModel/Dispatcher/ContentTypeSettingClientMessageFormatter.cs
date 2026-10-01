// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;

namespace System.ServiceModel.Dispatcher
{
    internal class ContentTypeSettingClientMessageFormatter : IClientMessageFormatter
    {
        private readonly IClientMessageFormatter _innerFormatter;
        private readonly string _outgoingContentType;

        public ContentTypeSettingClientMessageFormatter(string outgoingContentType, IClientMessageFormatter innerFormatter)
        {
            if (outgoingContentType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(outgoingContentType));
            }
            if (innerFormatter == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(innerFormatter));
            }
            _outgoingContentType = outgoingContentType;
            _innerFormatter = innerFormatter;
        }

        public object DeserializeReply(Message message, object[] parameters)
        {
            return _innerFormatter.DeserializeReply(message, parameters);
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            Message message = _innerFormatter.SerializeRequest(messageVersion, parameters);
            if (message != null)
            {
                AddRequestContentTypeProperty(message, _outgoingContentType);
            }
            return message;
        }

        private static void AddRequestContentTypeProperty(Message message, string contentType)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(message));
            }
            if (contentType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(contentType));
            }

            OperationContext operationContext = OperationContext.Current;
            if (operationContext != null && operationContext.HasOutgoingMessageProperties)
            {
                OutgoingWebRequestContext requestContext = WebOperationContext.Current.OutgoingRequest;
                if (string.IsNullOrEmpty(requestContext.ContentType))
                {
                    requestContext.ContentType = contentType;
                }
            }
            else
            {
                object prop;
                message.Properties.TryGetValue(HttpRequestMessageProperty.Name, out prop);
                HttpRequestMessageProperty httpProperty;
                if (prop != null)
                {
                    httpProperty = (HttpRequestMessageProperty)prop;
                }
                else
                {
                    httpProperty = new HttpRequestMessageProperty();
                    message.Properties.Add(HttpRequestMessageProperty.Name, httpProperty);
                }
                if (string.IsNullOrEmpty(httpProperty.Headers[HttpRequestHeader.ContentType]))
                {
                    httpProperty.Headers[HttpRequestHeader.ContentType] = contentType;
                }
            }
        }
    }
}
