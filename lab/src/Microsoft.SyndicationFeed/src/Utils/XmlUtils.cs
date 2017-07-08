// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    }
}
