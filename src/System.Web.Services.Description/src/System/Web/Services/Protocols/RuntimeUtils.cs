// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using System.Web.Services.Diagnostics;

namespace System.Web.Services.Protocols
{
    internal static class RuntimeUtils {
        internal static string ElementString(XmlElement element)
        {
            StringWriter xml = new StringWriter(CultureInfo.InvariantCulture);
            xml.Write("<");
            xml.Write(element.Name);
            if (element.NamespaceURI != null && element.NamespaceURI.Length > 0)
            {
                xml.Write(" xmlns");
                if (element.Prefix != null && element.Prefix.Length > 0)
                {
                    xml.Write(":");
                    xml.Write(element.Prefix);
                }
                xml.Write("='");
                xml.Write(element.NamespaceURI);
                xml.Write("'");
            }
            xml.Write(">..</");
            xml.Write(element.Name);
            xml.Write(">");

            return xml.ToString();
        }

        internal static void OnUnknownElement(object sender, XmlElementEventArgs e) {
            if (e.Element == null)
            {
                return;
            }

            string xml = ElementString(e.Element);
            Tracing.OnUnknownElement(sender, e);
            if (e.ExpectedElements == null)
            {
                throw new InvalidOperationException(SR.Format(SR.WebUnknownElement, xml));
            }
            else if (e.ExpectedElements.Length == 0)
            {
                throw new InvalidOperationException(SR.Format(SR.WebUnknownElement1, xml));
            }
            else
            {
                throw new InvalidOperationException(SR.Format(SR.WebUnknownElement2, xml, e.ExpectedElements));
            }
        }
    }
}
