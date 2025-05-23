// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !NET
using System.Runtime.Serialization;
#endif

namespace WcfService
{
    [DataContract(Name = "KnownTypeA", Namespace = "http://www.contoso.com/wcfnamespace")]
    public class KnownTypeA
    {
        private string _content;

        public KnownTypeA()
        {
        }

        public KnownTypeA(string content)
        {
            _content = content;
        }

        [DataMember]
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }
    }
}
