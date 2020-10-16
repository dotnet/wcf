// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal static class XmlStrings
    {
        public static class UriScheme
        {
            public static string Http { get { return "http"; } }
            public static string Https { get { return "https"; } }
            public static string NetPipe { get; internal set; }
            public static string NetTcp { get { return "net.tcp"; } }
        }

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
            public const string NamespaceUri = Microsoft.Xml.Schema.XmlSchema.Namespace;
            public static class Elements
            {
                public const string Root = "schema";
            }
        }

        public static class MetadataExchange
        {
            public const string Prefix = "wsx";
            public const string Name = "WS-MetadataExchange";
            public const string NamespaceUri = "http://schemas.xmlsoap.org/ws/2004/09/mex";
            public static class Elements
            {
                public const string Metadata = "Metadata";
            }
        }

        public static class WsdlContractInheritance
        {
            public const string Prefix = "wsdl-ex";
            public const string NamespaceUri = "http://schemas.microsoft.com/ws/2005/01/WSDL/Extensions/ContractInheritance";
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
    }
}
