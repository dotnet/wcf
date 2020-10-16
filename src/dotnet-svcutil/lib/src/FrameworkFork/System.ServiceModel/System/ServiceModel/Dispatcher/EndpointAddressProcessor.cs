// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.Runtime;
using System.ServiceModel.Channels;
using System.Text;
using Microsoft.Xml;
using Microsoft.Xml.Schema;

namespace System.ServiceModel.Dispatcher
{
    internal class EndpointAddressProcessor
    {
        // QName Attributes
        internal static readonly string XsiNs = XmlUtil.XmlSerializerSchemaInstanceNamespace;

        internal const string SerNs = "http://schemas.microsoft.com/2003/10/Serialization/";
        internal const string TypeLN = "type";
        internal const string ItemTypeLN = "ItemType";
        internal const string FactoryTypeLN = "FactoryType";

        internal static string GetComparableForm(StringBuilder builder, XmlReader reader)
        {
            List<Attr> attrSet = new List<Attr>();
            int valueLength = -1;
            while (!reader.EOF)
            {
                XmlNodeType type = reader.MoveToContent();
                switch (type)
                {
                    case XmlNodeType.Element:
                        CompleteValue(builder, valueLength);
                        valueLength = -1;

                        builder.Append("<");
                        AppendString(builder, reader.LocalName);
                        builder.Append(":");
                        AppendString(builder, reader.NamespaceURI);
                        builder.Append(" ");

                        // Scan attributes
                        attrSet.Clear();
                        if (reader.MoveToFirstAttribute())
                        {
                            do
                            {
                                // Ignore namespaces
                                if (reader.Prefix == "xmlns" || reader.Name == "xmlns")
                                {
                                    continue;
                                }
                                if (reader.LocalName == AddressingStrings.IsReferenceParameter && reader.NamespaceURI == Addressing10Strings.Namespace)
                                {
                                    continue;  // ignore IsReferenceParameter
                                }

                                string val = reader.Value;
                                if ((reader.LocalName == TypeLN && reader.NamespaceURI == XsiNs) ||
                                    (reader.NamespaceURI == SerNs && (reader.LocalName == ItemTypeLN || reader.LocalName == FactoryTypeLN)))
                                {
                                    string local, ns;
                                    XmlUtil.ParseQName(reader, val, out local, out ns);
                                    val = local + "^" + local.Length.ToString(CultureInfo.InvariantCulture) + ":" + ns + "^" + ns.Length.ToString(CultureInfo.InvariantCulture);
                                }
                                else if (reader.LocalName == XD.UtilityDictionary.IdAttribute.Value && reader.NamespaceURI == XD.UtilityDictionary.Namespace.Value)
                                {
                                    // ignore wsu:Id attributes added by security to sign the header
                                    continue;
                                }
                                attrSet.Add(new Attr(reader.LocalName, reader.NamespaceURI, val));
                            } while (reader.MoveToNextAttribute());
                        }
                        reader.MoveToElement();

                        if (attrSet.Count > 0)
                        {
                            attrSet.Sort();
                            for (int i = 0; i < attrSet.Count; ++i)
                            {
                                Attr a = attrSet[i];

                                AppendString(builder, a.local);
                                builder.Append(":");
                                AppendString(builder, a.ns);
                                builder.Append("=\"");
                                AppendString(builder, a.val);
                                builder.Append("\" ");
                            }
                        }

                        if (reader.IsEmptyElement)
                            builder.Append("></>");  // Should be the same as an empty tag.
                        else
                            builder.Append(">");
                        break;

                    case XmlNodeType.EndElement:
                        CompleteValue(builder, valueLength);
                        valueLength = -1;
                        builder.Append("</>");
                        break;

                    // Need to escape CDATA values
                    case XmlNodeType.CDATA:
                        CompleteValue(builder, valueLength);
                        valueLength = -1;

                        builder.Append("<![CDATA[");
                        AppendString(builder, reader.Value);
                        builder.Append("]]>");
                        break;

                    case XmlNodeType.SignificantWhitespace:
                    case XmlNodeType.Text:
                        if (valueLength < 0)
                            valueLength = builder.Length;

                        builder.Append(reader.Value);
                        break;

                    default:
                        // Do nothing
                        break;
                }
                reader.Read();
            }
            return builder.ToString();
        }

        private static void AppendString(StringBuilder builder, string s)
        {
            builder.Append(s);
            builder.Append("^");
            builder.Append(s.Length.ToString(CultureInfo.InvariantCulture));
        }

        private static void CompleteValue(StringBuilder builder, int startLength)
        {
            if (startLength < 0)
                return;

            int len = builder.Length - startLength;
            builder.Append("^");
            builder.Append(len.ToString(CultureInfo.InvariantCulture));
        }

        internal class Attr : IComparable<Attr>
        {
            internal string local;
            internal string ns;
            internal string val;
            private string _key;

            internal Attr(string l, string ns, string v)
            {
                this.local = l;
                this.ns = ns;
                this.val = v;
                _key = ns + ":" + l;
            }

            public int CompareTo(Attr a)
            {
                return string.Compare(_key, a._key, StringComparison.Ordinal);
            }
        }
    }
}
