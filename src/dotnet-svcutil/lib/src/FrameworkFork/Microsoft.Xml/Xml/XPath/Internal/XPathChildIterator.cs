// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace MS.Internal.Xml.XPath
{
    using System;
    using Microsoft.Xml;
    using Microsoft.Xml.XPath;
    using System.Diagnostics;
    using System.Globalization;

    internal class XPathChildIterator : XPathAxisIterator
    {
        public XPathChildIterator(XPathNavigator nav, XPathNodeType type) : base(nav, type, /*matchSelf:*/false) { }
        public XPathChildIterator(XPathNavigator nav, string name, string namespaceURI) : base(nav, name, namespaceURI, /*matchSelf:*/false) { }
        public XPathChildIterator(XPathChildIterator it) : base(it) { }

        public override XPathNodeIterator Clone()
        {
            return new XPathChildIterator(this);
        }

        public override bool MoveNext()
        {
            while ((first) ? nav.MoveToFirstChild() : nav.MoveToNext())
            {
                first = false;
                if (Matches)
                {
                    position++;
                    return true;
                }
            }
            return false;
        }
    }
}
