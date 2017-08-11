// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;

namespace Microsoft.SyndicationFeed
{
    static class XmlExtentions
    {
        public static ISyndicationContent ReadSyndicationContent(this XmlReader reader)
        {
            var content = new SyndicationContent(reader.Name, reader.NamespaceURI, null);

            //
            // Attributes
            if (reader.HasAttributes)
            {
                while (reader.MoveToNextAttribute())
                {
                    string ns = reader.NamespaceURI;
                    string name = reader.Name;

                    if (XmlUtils.IsXmlns(name, ns) || XmlUtils.IsXmlSchemaType(name, ns))
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
                        content.AddField(reader.ReadSyndicationContent());
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

        public static void WriteSyndicationContent(this XmlWriter writer, ISyndicationContent content)
        {
            //
            // Write opening 
            if (!string.IsNullOrEmpty(content.Namespace))
            {
                XmlUtils.SplitName(content.Name, out string prefix, out string localName);

                if (prefix != null)
                {
                    writer.WriteStartElement(prefix, localName, content.Namespace);
                }
                else
                {
                    writer.WriteStartElement(localName, content.Namespace);
                }
            }
            else
            {
                writer.WriteStartElement(content.Name);
            }

            //
            // Write attributes
            foreach (var a in content.Attributes)
            {
                writer.WriteSyndicationAttribute(a);
            }

            //
            // Write value
            if (content.Value != null)
            {
                writer.WriteString(content.Value);
            }
            //
            // Write Fields
            else
            {
                foreach (var field in content.Fields)
                {
                    writer.WriteSyndicationContent(field);
                }
            }

            //
            // Write closing 
            writer.WriteEndElement();
        }

        public static void WriteSyndicationAttribute(this XmlWriter writer, ISyndicationAttribute attr)
        {
            if (attr.Namespace != null)
            {
                XmlUtils.SplitName(attr.Name, out string prefix, out string localName);

                if (prefix != null)
                {
                    writer.WriteAttributeString(prefix, localName, attr.Namespace, attr.Value);
                }
                else
                {
                    writer.WriteAttributeString(localName, attr.Namespace, attr.Value);
                }
            }
            else
            {
                writer.WriteAttributeString(attr.Name, attr.Value);
            }
        }
    }
}
