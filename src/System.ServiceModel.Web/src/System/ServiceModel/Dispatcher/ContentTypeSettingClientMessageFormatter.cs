// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Dispatcher
{
    using System.Net;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Web;

    internal class ContentTypeSettingClientMessageFormatter : IClientMessageFormatter
    {
        IClientMessageFormatter innerFormatter;
        string outgoingContentType;

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
            this.outgoingContentType = outgoingContentType;
            this.innerFormatter = innerFormatter;
        }

        public object DeserializeReply(Message message, object[] parameters)
        {
            return this.innerFormatter.DeserializeReply(message, parameters);
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            Message message = this.innerFormatter.SerializeRequest(messageVersion, parameters);
            if (message != null)
            {
                AddRequestContentTypeProperty(message, this.outgoingContentType);
            }
            return message;
        }

        static void AddRequestContentTypeProperty(Message message, string contentType)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(message));
            }
            if (contentType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(contentType));
            }
            // dotnet/wcf's WebOperationContext (client subset) does not expose OutgoingRequest
            // (server-side OutgoingResponse is what we have). Fall through to the message-property
            // path which is the canonical way to set request content-type on outgoing client messages.
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
