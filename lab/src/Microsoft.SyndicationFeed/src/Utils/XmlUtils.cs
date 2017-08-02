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

        public static Task WriteRaw(XmlWriter writer, string content)
        {

            if (writer.Settings.Async)
            {
                return writer.WriteRawAsync(content);
            }

            writer.WriteRaw(content);
            return Task.CompletedTask;
        }

        public static ISyndicationContent ReadSyndicationContent(XmlReader reader)
        {
            var content = new SyndicationContent(reader.Name);

            content.Namespace = reader.NamespaceURI;

            //
            // Attributes
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

                    content.AddAttribute(new SyndicationAttribute(name, ns, reader.Value));
                }

                reader.MoveToContent();
            }

            //
            // Content
            if (!reader.IsEmptyElement)
            {
                reader.ReadStartElement();

                //
                // Value
                if (reader.HasValue)
                {
                    content.Value = reader.ReadContentAsString();
                }
                //
                // Children
                else
                {
                    while (reader.IsStartElement())
                    {
                        content.AddField(ReadSyndicationContent(reader));
                    }
                }

                reader.ReadEndElement(); // end
            }
            else
            {
                reader.Skip();
            }

            return content;
        }

        public static void Write(ISyndicationContent content, XmlWriter writer)
        {
            //
            // Write opening name 
            writer.WriteStartElement(content.Name);

            //
            // Write attributes
            foreach (var attribute in content.Attributes)
            {
                writer.WriteAttributeString(attribute.Name, attribute.Namespace, attribute.Value);
            }

            var fields = (List<ISyndicationContent>)content.Fields;

            if (fields.Count == 0)
            {
                //
                // This element has no children.
                writer.WriteString(content.Value);
            }

            else
            {
                //
                // Write Fields
                foreach (var field in fields)
                {
                    Write(field, writer);
                }
            }

            //
            // Write closing name 
            writer.WriteEndElement();
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
