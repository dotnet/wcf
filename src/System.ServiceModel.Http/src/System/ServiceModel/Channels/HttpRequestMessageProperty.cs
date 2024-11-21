// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;

namespace System.ServiceModel.Channels
{
    public sealed class HttpRequestMessageProperty : IMessageProperty, IMergeEnabledMessageProperty
    {
        private TraditionalHttpRequestMessageProperty _traditionalProperty;

        public HttpRequestMessageProperty()
        {
            _traditionalProperty = new TraditionalHttpRequestMessageProperty();
        }

        internal HttpRequestMessageProperty(WebHeaderCollection originalHeaders)
        {
            _traditionalProperty = new TraditionalHttpRequestMessageProperty(originalHeaders);
        }

        public static string Name
        {
            get { return "httpRequest"; }
        }

        public WebHeaderCollection Headers
        {
            get
            {
                return _traditionalProperty.Headers;
            }
        }

        public string Method
        {
            get
            {
                return _traditionalProperty.Method;
            }

            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }

                _traditionalProperty.Method = value;
            }
        }

        public string QueryString
        {
            get => _traditionalProperty.QueryString;

            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }

                _traditionalProperty.QueryString = value;
            }
        }

        public bool SuppressEntityBody
        {
            get => _traditionalProperty.SuppressEntityBody;

            set
            {
                _traditionalProperty.SuppressEntityBody = value;
            }
        }

        IMessageProperty IMessageProperty.CreateCopy()
        {
            return this;
        }

        bool IMergeEnabledMessageProperty.TryMergeWithProperty(object propertyToMerge)
        {
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
    }
}
