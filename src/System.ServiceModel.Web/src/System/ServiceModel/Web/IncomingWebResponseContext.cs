// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Net;
using System.Runtime;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Web
{
    public class IncomingWebResponseContext
    {
        private readonly OperationContext _operationContext;

        internal IncomingWebResponseContext(OperationContext operationContext)
        {
            Fx.Assert(operationContext != null, "operationContext is null");
            _operationContext = operationContext;
        }

        public long ContentLength
        {
            get { return long.Parse(EnsureMessageProperty().Headers[HttpResponseHeader.ContentLength], CultureInfo.InvariantCulture); }
        }

        public string ContentType
        {
            get { return EnsureMessageProperty().Headers[HttpResponseHeader.ContentType]; }
        }

        public string ETag
        {
            get { return EnsureMessageProperty().Headers[HttpResponseHeader.ETag]; }
        }

        public WebHeaderCollection Headers
        {
            get { return EnsureMessageProperty().Headers; }
        }

        public string Location
        {
            get { return EnsureMessageProperty().Headers[HttpResponseHeader.Location]; }
        }

        public HttpStatusCode StatusCode
        {
            get { return EnsureMessageProperty().StatusCode; }
        }

        public string StatusDescription
        {
            get { return EnsureMessageProperty().StatusDescription; }
        }

        private HttpResponseMessageProperty MessageProperty
        {
            get
            {
                if (_operationContext.IncomingMessageProperties == null)
                {
                    return null;
                }

                if (!_operationContext.IncomingMessageProperties.ContainsKey(HttpResponseMessageProperty.Name))
                {
                    return null;
                }

                return _operationContext.IncomingMessageProperties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty;
            }
        }

        private HttpResponseMessageProperty EnsureMessageProperty()
        {
            if (MessageProperty == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SR.Format(SR.HttpContextNoIncomingMessageProperty, typeof(HttpResponseMessageProperty).Name)));
            }

            return MessageProperty;
        }
    }
}
