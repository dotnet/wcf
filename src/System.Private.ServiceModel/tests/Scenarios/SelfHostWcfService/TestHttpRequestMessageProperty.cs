// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WcfService
{
    [DataContract(Namespace = "http://www.contoso.com/wcfnamespace")]
    internal class TestHttpRequestMessageProperty
    {
        private bool _suppressEntityBody;
        private string _method;
        private string _queryString;
        private Dictionary<string, string> _headers;

        public TestHttpRequestMessageProperty()
        {
            _headers = new Dictionary<string, string>();
        }

        [DataMember]
        public bool SuppressEntityBody
        {
            get { return _suppressEntityBody; }
            set { _suppressEntityBody = value; }
        }

        [DataMember]
        public string Method
        {
            get { return _method; }
            set { _method = value; }
        }

        [DataMember]
        public string QueryString
        {
            get { return _queryString; }
            set { _queryString = value; }
        }

        [DataMember]
        public Dictionary<string, string> Headers
        {
            get { return _headers; }
            set { _headers = value; }
        }
    }
}
