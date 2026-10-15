// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Net;
using System.Runtime;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Web
{
    public class OutgoingWebRequestContext
    {
        private readonly OperationContext _operationContext;

        internal OutgoingWebRequestContext(OperationContext operationContext)
        {
            Fx.Assert(operationContext != null, "operationContext is null");
            _operationContext = operationContext;
        }

        public string Accept
        {
            get { return MessageProperty.Headers[HttpRequestHeader.Accept]; }
            set { MessageProperty.Headers[HttpRequestHeader.Accept] = value; }
        }

        public long ContentLength
        {
            get { return long.Parse(MessageProperty.Headers[HttpRequestHeader.ContentLength], CultureInfo.InvariantCulture); }
            set { MessageProperty.Headers[HttpRequestHeader.ContentLength] = value.ToString(CultureInfo.InvariantCulture); }
        }

        public string ContentType
        {
            get { return MessageProperty.Headers[HttpRequestHeader.ContentType]; }
            set { MessageProperty.Headers[HttpRequestHeader.ContentType] = value; }
        }

        public WebHeaderCollection Headers
        {
            get { return MessageProperty.Headers; }
        }

        public string IfMatch
        {
            get { return MessageProperty.Headers[HttpRequestHeader.IfMatch]; }
            set { MessageProperty.Headers[HttpRequestHeader.IfMatch] = value; }
        }

        public string IfModifiedSince
        {
            get { return MessageProperty.Headers[HttpRequestHeader.IfModifiedSince]; }
            set { MessageProperty.Headers[HttpRequestHeader.IfModifiedSince] = value; }
        }

        public string IfNoneMatch
        {
            get { return MessageProperty.Headers[HttpRequestHeader.IfNoneMatch]; }
            set { MessageProperty.Headers[HttpRequestHeader.IfNoneMatch] = value; }
        }

        public string IfUnmodifiedSince
        {
            get { return MessageProperty.Headers[HttpRequestHeader.IfUnmodifiedSince]; }
            set { MessageProperty.Headers[HttpRequestHeader.IfUnmodifiedSince] = value; }
        }

        public string Method
        {
            get { return MessageProperty.Method; }
            set { MessageProperty.Method = value; }
        }

        public bool SuppressEntityBody
        {
            get { return MessageProperty.SuppressEntityBody; }
            set { MessageProperty.SuppressEntityBody = value; }
        }

        public string UserAgent
        {
            get { return MessageProperty.Headers[HttpRequestHeader.UserAgent]; }
            set { MessageProperty.Headers[HttpRequestHeader.UserAgent] = value; }
        }

        private HttpRequestMessageProperty MessageProperty
        {
            get
            {
                if (!_operationContext.OutgoingMessageProperties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    _operationContext.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, new HttpRequestMessageProperty());
                }

                return _operationContext.OutgoingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            }
        }
    }
}
