//------------------------------------------------------------------------------
// <copyright file="XmlNullResolver.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">sdub</owner>
//------------------------------------------------------------------------------

#if !SILVERLIGHT
using System.Net;
#endif

namespace Microsoft.Xml {
				using System;
				
    internal class XmlNullResolver : XmlResolver {
        public static readonly XmlNullResolver Singleton = new XmlNullResolver();

        // Private constructor ensures existing only one instance of XmlNullResolver
        private XmlNullResolver() { }

        public override Object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn) {
            throw new XmlException(ResXml.Xml_NullResolver, string.Empty);
        }

#if !SILVERLIGHT
        public override ICredentials Credentials {
            set { /* Do nothing */ }
        }
#endif
    }
}
