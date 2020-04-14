// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

#if !SILVERLIGHT
using System.Net;
#endif

namespace Microsoft.Xml
{
    using System;

    internal class XmlNullResolver : XmlResolver
    {
        public static readonly XmlNullResolver Singleton = new XmlNullResolver();

        // Private constructor ensures existing only one instance of XmlNullResolver
        private XmlNullResolver() { }

        public override Object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            throw new XmlException(ResXml.Xml_NullResolver, string.Empty);
        }

#if !SILVERLIGHT
        public override ICredentials Credentials
        {
            set { /* Do nothing */ }
        }
#endif
    }
}
