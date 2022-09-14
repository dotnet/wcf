// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;

namespace System.ServiceModel
{
    internal static class XmlReaderExtensions
    {
        internal static string ReadElementString(this XmlReader reader)
        {
            if (reader.MoveToContent() != XmlNodeType.Element)
            {
                var lineInfo = reader as IXmlLineInfo;
                throw new XmlException(SRP.Format(SRP.Xml_InvalidNodeType, reader.NodeType.ToString()), null, lineInfo?.LineNumber ?? 0, lineInfo?.LinePosition ?? 0);
            }

            return reader.ReadElementContentAsString();
        }

        internal static string ReadElementString(this XmlReader reader, string localname, string ns)
        {
            if (reader.MoveToContent() != XmlNodeType.Element)
            {
                var lineInfo = reader as IXmlLineInfo;
                throw new XmlException(SRP.Format(SRP.Xml_InvalidNodeType, reader.NodeType.ToString()), null, lineInfo?.LineNumber ?? 0, lineInfo?.LinePosition ?? 0);
            }

            return reader.ReadElementContentAsString(localname, ns);
        }
    }
}
