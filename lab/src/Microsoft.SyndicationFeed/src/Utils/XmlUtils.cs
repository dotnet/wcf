// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    static class XmlUtils
    {
        public const string XmlNs = "http://www.w3.org/XML/1998/namespace";

        public static string GetValue(string xmlNode)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlNode)))
            {
                reader.MoveToContent();
                return reader.ReadElementContentAsString();
            }
        }

        public static IEnumerable<ISyndicationAttribute> ReadAttributes(string content)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(content)))
            {
                reader.MoveToContent();
                return ReadAttributes(reader);
            }
        }

        public static IEnumerable<ISyndicationAttribute> ReadAttributes(XmlReader reader)
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

                    attributes.Add(new SyndicationAttribute(name, ns, reader.Value));
                }
            }

            return attributes;
        }

        public static Task<string> ReadOuterXmlAsync(XmlReader reader)
        {
            if (reader.Settings.Async)
            {
                return reader.ReadOuterXmlAsync();
            }

            return Task.FromResult(reader.ReadOuterXml());
        }

        public static Task SkipAsync(XmlReader reader)
        {
            if (reader.Settings.Async)
            {
                return reader.SkipAsync();
            }

            reader.Skip();

            return Task.CompletedTask;
        }

        public static Task<bool> ReadAsync(XmlReader reader)
        {
            if (reader.Settings.Async)
            {
                return reader.ReadAsync();
            }

            return Task.FromResult(reader.Read());
        }

        public static XmlReader CreateXmlReader(string value)
        {
            return XmlReader.Create(new StringReader(value),
                                    new XmlReaderSettings()
                                    {
                                        ConformanceLevel = ConformanceLevel.Fragment,
                                        DtdProcessing = DtdProcessing.Ignore,
                                        IgnoreComments = true,
                                        IgnoreWhitespace = true
                                    });
        }

        private static bool IsXmlns(string name, string ns)
        {
            return name == "xmlns" || ns == "http://www.w3.org/2000/xmlns/";
        }

        private static bool IsXmlSchemaType(string name, string ns)
        {
            return name == "type" && ns == "http://www.w3.org/2001/XMLSchema-instance";
        }
    }
}
