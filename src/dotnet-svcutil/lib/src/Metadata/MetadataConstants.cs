// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
#if PRIVATE_RTLIB
using XmlNs = Microsoft.Xml;
#else
using XmlNs = System.Xml;
#endif
namespace Microsoft.Tools.ServiceModel.Svcutil.Metadata
{
    internal static class MetadataConstants
    {
        public static class WSDL
        {
            public const string Prefix = "wsdl";
            public const string NamespaceUri = System.Web.Services.Description.ServiceDescription.Namespace;
            public static class Elements
            {
                public const string Root = "definitions";
            }
        }

        public static class XmlSchema
        {
            public const string Prefix = "xsd";
            public const string NamespaceUri = XmlNs.Schema.XmlSchema.Namespace;
            public static class Elements
            {
                public const string Root = "schema";
            }
        }

        public static class Xml
        {
            public const string Prefix = "xml";
            public const string NamespaceUri = "http://www.w3.org/XML/1998/namespace";
            public static class Attributes
            {
                public const string Base = "base";
                public const string Id = "id";
            }
        }

        public static class WSAddressing
        {
            public const string Prefix = "wsa";
            public const string NamespaceUri = "http://schemas.xmlsoap.org/ws/2004/08/addressing";
            public static class Elements
            {
                public const string EndpointReference = "EndpointReference";
            }
        }

        public static class Wsu
        {
            public const string Prefix = "wsu";
            public const string NamespaceUri = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";
            public static class Attributes
            {
                public const string Id = "Id";
            }
        }

        public static class WSPolicy
        {
            public const string Prefix = "wsp";
            public const string NamespaceUri = "http://schemas.xmlsoap.org/ws/2004/09/policy";
            public const string NamespaceUri15 = "http://www.w3.org/ns/ws-policy";

            public static class Attributes
            {
                public const string PolicyURIs = "PolicyURIs";
            }
            public static class Elements
            {
                public const string PolicyReference = "PolicyReference";
                public const string All = "All";
                public const string ExactlyOne = "ExactlyOne";
                public const string Policy = "Policy";
            }
        }

        public static class Uri
        {
            public const string UriSchemeFile = "file";
            public const string UriSchemeHttp = "http";
            public const string UriSchemeHttps = "https";
            public const string UriSchemeNetTcp = "net.tcp";
            public const string UriSchemeNetPipe = "net.pipe";
        }
    }
}
