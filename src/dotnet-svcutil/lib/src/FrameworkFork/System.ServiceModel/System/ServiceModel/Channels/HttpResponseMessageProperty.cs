// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Http;

namespace System.ServiceModel.Channels
{
    public sealed class HttpResponseMessageProperty : IMessageProperty, IMergeEnabledMessageProperty
    {
        private TraditionalHttpResponseMessageProperty _traditionalProperty;
        private HttpResponseMessageBackedProperty _httpBackedProperty;
        private bool _useHttpBackedProperty;
        private bool _initialCopyPerformed;

        public HttpResponseMessageProperty()
        {
            _traditionalProperty = new TraditionalHttpResponseMessageProperty();
            _useHttpBackedProperty = false;
        }

        internal HttpResponseMessageProperty(WebHeaderCollection originalHeaders)
        {
            _traditionalProperty = new TraditionalHttpResponseMessageProperty(originalHeaders);
            _useHttpBackedProperty = false;
        }

        internal HttpResponseMessageProperty(HttpResponseMessage httpResponseMessage)
        {
            _httpBackedProperty = new HttpResponseMessageBackedProperty(httpResponseMessage);
            _useHttpBackedProperty = true;
        }

        public static string Name
        {
            get { return "httpResponse"; }
        }

        public WebHeaderCollection Headers
        {
            get
            {
                return _useHttpBackedProperty ?
                    _httpBackedProperty.Headers :
                    _traditionalProperty.Headers;
            }
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                return _useHttpBackedProperty ?
                    _httpBackedProperty.StatusCode :
                    _traditionalProperty.StatusCode;
            }

            set
            {
                int valueInt = (int)value;
                if (valueInt < 100 || valueInt > 599)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                        string.Format(SRServiceModel.ValueMustBeInRange, 100, 599)));
                }

                if (_useHttpBackedProperty)
                {
                    _httpBackedProperty.StatusCode = value;
                }
                else
                {
                    _traditionalProperty.StatusCode = value;
                }
            }
        }

        internal bool HasStatusCodeBeenSet
        {
            get
            {
                return _useHttpBackedProperty ?
                    true :
                    _traditionalProperty.HasStatusCodeBeenSet;
            }
        }

        public string StatusDescription
        {
            get
            {
                return _useHttpBackedProperty ?
                    _httpBackedProperty.StatusDescription :
                    _traditionalProperty.StatusDescription;
            }

            set
            {
                if (_useHttpBackedProperty)
                {
                    _httpBackedProperty.StatusDescription = value;
                }
                else
                {
                    _traditionalProperty.StatusDescription = value;
                }
            }
        }

        public bool SuppressEntityBody
        {
            get
            {
                return _useHttpBackedProperty ?
                    _httpBackedProperty.SuppressEntityBody :
                    _traditionalProperty.SuppressEntityBody;
            }

            set
            {
                if (_useHttpBackedProperty)
                {
                    _httpBackedProperty.SuppressEntityBody = value;
                }
                else
                {
                    _traditionalProperty.SuppressEntityBody = value;
                }
            }
        }

        public bool SuppressPreamble
        {
            get
            {
                return _useHttpBackedProperty ?
                    false :
                    _traditionalProperty.SuppressPreamble;
            }

            set
            {
                if (!_useHttpBackedProperty)
                {
                    _traditionalProperty.SuppressPreamble = value;
                }
            }
        }

        public HttpResponseMessage HttpResponseMessage
        {
            get
            {
                if (_useHttpBackedProperty)
                {
                    return _httpBackedProperty.HttpResponseMessage;
                }

                return null;
            }
        }

        internal static HttpResponseMessage GetHttpResponseMessageFromMessage(Message message)
        {
            HttpResponseMessage httpResponseMessage = null;

            HttpResponseMessageProperty property = message.Properties.GetValue<HttpResponseMessageProperty>(HttpResponseMessageProperty.Name);
            if (property != null)
            {
                httpResponseMessage = property.HttpResponseMessage;
                if (httpResponseMessage != null)
                {
                    httpResponseMessage.CopyPropertiesFromMessage(message);
                    message.EnsureReadMessageState();
                }
            }

            return httpResponseMessage;
        }

        IMessageProperty IMessageProperty.CreateCopy()
        {
            if (!_useHttpBackedProperty ||
                !_initialCopyPerformed)
            {
                _initialCopyPerformed = true;
                return this;
            }

            return _httpBackedProperty.CreateTraditionalResponseMessageProperty();
        }

        bool IMergeEnabledMessageProperty.TryMergeWithProperty(object propertyToMerge)
        {
            // The ImmutableDispatchRuntime will merge MessageProperty instances from the
            //  OperationContext (that were created before the response message was created) with
            //  MessageProperty instances on the message itself.  The message's version of the 
            //  HttpResponseMessageProperty may hold a reference to an HttpResponseMessage, and this 
            //  cannot be discarded, so values from the OperationContext's property must be set on 
            //  the message's version without completely replacing the message's property.
            if (_useHttpBackedProperty)
            {
                HttpResponseMessageProperty responseProperty = propertyToMerge as HttpResponseMessageProperty;
                if (responseProperty != null)
                {
                    if (!responseProperty._useHttpBackedProperty)
                    {
                        _httpBackedProperty.MergeWithTraditionalProperty(responseProperty._traditionalProperty);
                        responseProperty._traditionalProperty = null;
                        responseProperty._httpBackedProperty = _httpBackedProperty;
                        responseProperty._useHttpBackedProperty = true;
                    }

                    return true;
                }
            }

            return false;
        }

        private class TraditionalHttpResponseMessageProperty
        {
            public const HttpStatusCode DefaultStatusCode = HttpStatusCode.OK;
            public const string DefaultStatusDescription = null; // null means use description from status code

            private HttpStatusCode _statusCode;

            public TraditionalHttpResponseMessageProperty()
            {
                _statusCode = DefaultStatusCode;
                this.StatusDescription = DefaultStatusDescription;
            }

            private WebHeaderCollection _headers;
            private WebHeaderCollection _originalHeaders;

            public TraditionalHttpResponseMessageProperty(WebHeaderCollection originalHeaders) : this()
            {
                _originalHeaders = originalHeaders;
            }

            public WebHeaderCollection Headers
            {
                get
                {
                    if (_headers == null)
                    {
                        _headers = new WebHeaderCollection();
                        if (_originalHeaders != null)
                        {
                            foreach (var headerKey in _originalHeaders.AllKeys)
                            {
                                _headers[headerKey] = _originalHeaders[headerKey];
                            }
                            _originalHeaders = null;
                        }
                    }

                    return _headers;
                }
            }

            public HttpStatusCode StatusCode
            {
                get
                {
                    return _statusCode;
                }

                set
                {
                    _statusCode = value;
                    this.HasStatusCodeBeenSet = true;
                }
            }

            public bool HasStatusCodeBeenSet { get; private set; }

            public string StatusDescription { get; set; }

            public bool SuppressEntityBody { get; set; }

            public bool SuppressPreamble { get; set; }
        }

        private class HttpResponseMessageBackedProperty
        {
            public HttpResponseMessageBackedProperty(HttpResponseMessage httpResponseMessage)
            {
                Contract.Assert(httpResponseMessage != null, "The 'httpResponseMessage' property should never be null.");

                this.HttpResponseMessage = httpResponseMessage;
            }

            public HttpResponseMessage HttpResponseMessage { get; private set; }

            private WebHeaderCollection _headers;

            public WebHeaderCollection Headers
            {
                get
                {
                    if (_headers == null)
                    {
                        _headers = this.HttpResponseMessage.ToWebHeaderCollection();
                    }

                    return _headers;
                }
            }

            public HttpStatusCode StatusCode
            {
                get
                {
                    return this.HttpResponseMessage.StatusCode;
                }

                set
                {
                    this.HttpResponseMessage.StatusCode = value;
                }
            }

            public string StatusDescription
            {
                get
                {
                    return this.HttpResponseMessage.ReasonPhrase;
                }

                set
                {
                    this.HttpResponseMessage.ReasonPhrase = value;
                }
            }

            public bool SuppressEntityBody
            {
                get
                {
                    HttpContent content = this.HttpResponseMessage.Content;
                    if (content != null)
                    {
                        long? contentLength = content.Headers.ContentLength;

                        if (!contentLength.HasValue ||
                            (contentLength.HasValue && contentLength.Value > 0))
                        {
                            return false;
                        }
                    }

                    return true;
                }

                set
                {
                    HttpContent content = this.HttpResponseMessage.Content;
                    if (value && content != null &&
                        (!content.Headers.ContentLength.HasValue ||
                        content.Headers.ContentLength.Value > 0))
                    {
                        HttpContent newContent = new ByteArrayContent(Array.Empty<byte>());
                        foreach (KeyValuePair<string, IEnumerable<string>> header in content.Headers)
                        {
                            newContent.Headers.AddHeaderWithoutValidation(header);
                        }

                        this.HttpResponseMessage.Content = newContent;
                        content.Dispose();
                    }
                    else if (!value && content == null)
                    {
                        this.HttpResponseMessage.Content = new ByteArrayContent(Array.Empty<byte>());
                    }
                }
            }

            public HttpResponseMessageProperty CreateTraditionalResponseMessageProperty()
            {
                HttpResponseMessageProperty copiedProperty = new HttpResponseMessageProperty();

                foreach (var headerKey in this.Headers.AllKeys)
                {
                    copiedProperty.Headers[headerKey] = this.Headers[headerKey];
                }

                if (this.StatusCode != TraditionalHttpResponseMessageProperty.DefaultStatusCode)
                {
                    copiedProperty.StatusCode = this.StatusCode;
                }

                copiedProperty.StatusDescription = this.StatusDescription;
                copiedProperty.SuppressEntityBody = this.SuppressEntityBody;

                return copiedProperty;
            }

            public void MergeWithTraditionalProperty(TraditionalHttpResponseMessageProperty propertyToMerge)
            {
                if (propertyToMerge.HasStatusCodeBeenSet)
                {
                    this.StatusCode = propertyToMerge.StatusCode;
                }

                if (propertyToMerge.StatusDescription != TraditionalHttpResponseMessageProperty.DefaultStatusDescription)
                {
                    this.StatusDescription = propertyToMerge.StatusDescription;
                }

                this.SuppressEntityBody = propertyToMerge.SuppressEntityBody;
                this.HttpResponseMessage.MergeWebHeaderCollection(propertyToMerge.Headers);
            }
        }
    }
}

