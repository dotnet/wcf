// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Http;

namespace System.ServiceModel.Channels
{
    public sealed class HttpRequestMessageProperty : IMessageProperty, IMergeEnabledMessageProperty
    {
        private TraditionalHttpRequestMessageProperty _traditionalProperty;
        private HttpRequestMessageBackedProperty _httpBackedProperty;
        private bool _initialCopyPerformed;
        private bool _useHttpBackedProperty;

        public HttpRequestMessageProperty()
        {
            _traditionalProperty = new TraditionalHttpRequestMessageProperty();
            _useHttpBackedProperty = false;
        }

        internal HttpRequestMessageProperty(WebHeaderCollection originalHeaders)
        {
            _traditionalProperty = new TraditionalHttpRequestMessageProperty(originalHeaders);
            _useHttpBackedProperty = false;
        }

        internal HttpRequestMessageProperty(HttpRequestMessage httpRequestMessage)
        {
            _httpBackedProperty = new HttpRequestMessageBackedProperty(httpRequestMessage);
            _useHttpBackedProperty = true;
        }

        public static string Name
        {
            get { return "httpRequest"; }
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

        public string Method
        {
            get
            {
                return _useHttpBackedProperty ?
                    _httpBackedProperty.Method :
                    _traditionalProperty.Method;
            }

            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }

                if (_useHttpBackedProperty)
                {
                    _httpBackedProperty.Method = value;
                }
                else
                {
                    _traditionalProperty.Method = value;
                }
            }
        }

        public string QueryString
        {
            get
            {
                return _useHttpBackedProperty ?
                    _httpBackedProperty.QueryString :
                    _traditionalProperty.QueryString;
            }

            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }

                if (_useHttpBackedProperty)
                {
                    _httpBackedProperty.QueryString = value;
                }
                else
                {
                    _traditionalProperty.QueryString = value;
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

        public HttpRequestMessage HttpRequestMessage
        {
            get
            {
                if (_useHttpBackedProperty)
                {
                    return _httpBackedProperty.HttpRequestMessage;
                }

                return null;
            }
        }

        IMessageProperty IMessageProperty.CreateCopy()
        {
            if (!_useHttpBackedProperty ||
                !_initialCopyPerformed)
            {
                _initialCopyPerformed = true;
                return this;
            }

            return _httpBackedProperty.CreateTraditionalRequestMessageProperty();
        }

        bool IMergeEnabledMessageProperty.TryMergeWithProperty(object propertyToMerge)
        {
            // The ImmutableDispatchRuntime will merge MessageProperty instances from the
            //  OperationContext (that were created before the response message was created) with
            //  MessageProperty instances on the message itself.  The message's version of the 
            //  HttpRequestMessageProperty may hold a reference to an HttpRequestMessage, and this 
            //  cannot be discarded, so values from the OperationContext's property must be set on 
            //  the message's version without completely replacing the message's property.
            if (_useHttpBackedProperty)
            {
                HttpRequestMessageProperty requestProperty = propertyToMerge as HttpRequestMessageProperty;
                if (requestProperty != null)
                {
                    if (!requestProperty._useHttpBackedProperty)
                    {
                        _httpBackedProperty.MergeWithTraditionalProperty(requestProperty._traditionalProperty);
                        requestProperty._traditionalProperty = null;
                        requestProperty._httpBackedProperty = _httpBackedProperty;
                        requestProperty._useHttpBackedProperty = true;
                    }

                    return true;
                }
            }

            return false;
        }

        private class TraditionalHttpRequestMessageProperty
        {
            public const string DefaultMethod = "POST";
            public const string DefaultQueryString = "";

            private string _method;

            public TraditionalHttpRequestMessageProperty()
            {
                _method = DefaultMethod;
                QueryString = DefaultQueryString;
            }

            private WebHeaderCollection _headers;
            private WebHeaderCollection _originalHeaders;

            public TraditionalHttpRequestMessageProperty(WebHeaderCollection originalHeaders) : this()
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

            public string Method
            {
                get
                {
                    return _method;
                }

                set
                {
                    _method = value;
                    HasMethodBeenSet = true;
                }
            }

            public bool HasMethodBeenSet { get; private set; }

            public string QueryString { get; set; }

            public bool SuppressEntityBody { get; set; }
        }

        private class HttpRequestMessageBackedProperty
        {
            public HttpRequestMessageBackedProperty(HttpRequestMessage httpRequestMessage)
            {
                Contract.Assert(httpRequestMessage != null, "The 'httpRequestMessage' property should never be null.");

                HttpRequestMessage = httpRequestMessage;
            }

            public HttpRequestMessage HttpRequestMessage { get; private set; }

            private WebHeaderCollection _headers;

            public WebHeaderCollection Headers
            {
                get
                {
                    if (_headers == null)
                    {
                        _headers = HttpRequestMessage.ToWebHeaderCollection();
                    }

                    return _headers;
                }
            }

            public string Method
            {
                get
                {
                    return HttpRequestMessage.Method.Method;
                }

                set
                {
                    HttpRequestMessage.Method = new HttpMethod(value);
                }
            }

            public string QueryString
            {
                get
                {
                    string query = HttpRequestMessage.RequestUri.Query;
                    return query.Length > 0 ? query.Substring(1) : string.Empty;
                }

                set
                {
                    UriBuilder uriBuilder = new UriBuilder(HttpRequestMessage.RequestUri);
                    uriBuilder.Query = value;
                    HttpRequestMessage.RequestUri = uriBuilder.Uri;
                }
            }

            public bool SuppressEntityBody
            {
                get
                {
                    HttpContent content = HttpRequestMessage.Content;
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
                    HttpContent content = HttpRequestMessage.Content;
                    if (value && content != null &&
                        (!content.Headers.ContentLength.HasValue ||
                        content.Headers.ContentLength.Value > 0))
                    {
                        HttpContent newContent = new ByteArrayContent(Array.Empty<byte>());
                        foreach (KeyValuePair<string, IEnumerable<string>> header in content.Headers)
                        {
                            newContent.Headers.AddHeaderWithoutValidation(header);
                        }

                        HttpRequestMessage.Content = newContent;
                        content.Dispose();
                    }
                    else if (!value && content == null)
                    {
                        HttpRequestMessage.Content = new ByteArrayContent(Array.Empty<byte>());
                    }
                }
            }

            public HttpRequestMessageProperty CreateTraditionalRequestMessageProperty()
            {
                HttpRequestMessageProperty copiedProperty = new HttpRequestMessageProperty();

                foreach (var headerKey in Headers.AllKeys)
                {
                    copiedProperty.Headers[headerKey] = Headers[headerKey];
                }

                if (Method != TraditionalHttpRequestMessageProperty.DefaultMethod)
                {
                    copiedProperty.Method = Method;
                }

                copiedProperty.QueryString = QueryString;
                copiedProperty.SuppressEntityBody = SuppressEntityBody;

                return copiedProperty;
            }

            public void MergeWithTraditionalProperty(TraditionalHttpRequestMessageProperty propertyToMerge)
            {
                if (propertyToMerge.HasMethodBeenSet)
                {
                    Method = propertyToMerge.Method;
                }

                if (propertyToMerge.QueryString != TraditionalHttpRequestMessageProperty.DefaultQueryString)
                {
                    QueryString = propertyToMerge.QueryString;
                }

                SuppressEntityBody = propertyToMerge.SuppressEntityBody;
                HttpRequestMessage.MergeWebHeaderCollection(propertyToMerge.Headers);
            }
        }
    }
}
