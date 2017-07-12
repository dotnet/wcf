// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    static class XmlUtils
    {
        public const string XmlNs = "http://www.w3.org/XML/1998/namespace";

        public static bool IsXmlns(string name, string ns)
        {
            return name == "xmlns" || ns == "http://www.w3.org/2000/xmlns/";
        }

        public static bool IsXmlSchemaType(string name, string ns)
        {
            return name == "type" && ns == "http://www.w3.org/2001/XMLSchema-instance";
        }

        public static string GetValue(string xmlNode)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlNode)))
            {
                reader.MoveToContent();
                return reader.ReadElementContentAsString();
            }
        }

        public static IEnumerable<SyndicationAttribute> ReadAttributes(string content)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(content)))
            {
                reader.MoveToContent();
                return ReadAttributes(reader);
            }
        }

        public static IEnumerable<SyndicationAttribute> ReadAttributes(XmlReader reader)
        {
            // Read attributes
            var attributes = new List<SyndicationAttribute>();

            if (reader.HasAttributes)
            {
                while (reader.MoveToNextAttribute())
                {
                    string ns = reader.NamespaceURI;
                    string name = reader.LocalName;

                    if (IsXmlns(name, ns) || IsXmlSchemaType(name, ns))
                    {
                        continue;
                    }

                    attributes.Add(new SyndicationAttribute(new XmlQualifiedName(name, ns), reader.Value));
                }
            }

            return attributes;
        }
    }
}
