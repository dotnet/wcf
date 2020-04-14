// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace MS.Internal.Xml.XPath
{
    using System;
    using Microsoft.Xml;
    using Microsoft.Xml.XPath;
    using System.Diagnostics;

    internal sealed class XPathSelfQuery : BaseAxisQuery
    {
        public XPathSelfQuery(Query qyInput, string Name, string Prefix, XPathNodeType Type) : base(qyInput, Name, Prefix, Type) { }
        private XPathSelfQuery(XPathSelfQuery other) : base(other) { }

        public override XPathNavigator Advance()
        {
            while ((currentNode = qyInput.Advance()) != null)
            {
                if (matches(currentNode))
                {
                    position = 1;
                    return currentNode;
                }
            }
            return null;
        }

        public override XPathNodeIterator Clone() { return new XPathSelfQuery(this); }
    }
}
