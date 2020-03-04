// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace MS.Internal.Xml.XPath {
    using System;
    using Microsoft.Xml;
    using Microsoft.Xml.XPath;
    using System.Diagnostics;
    using System.Globalization;
    using Microsoft.Xml.Xsl;
    using System.Collections;

    internal sealed class EmptyQuery : Query {
        public override XPathNavigator Advance() { return null; }
        public override XPathNodeIterator Clone() { return this; }
        public override object Evaluate(XPathNodeIterator context) { return this; }
        public override int CurrentPosition { get { return 0; } }
        public override int Count { get { return 0; } }
        public override QueryProps Properties { get { return QueryProps.Merge | QueryProps.Cached | QueryProps.Position | QueryProps.Count; } }
        public override XPathResultType StaticType { get { return XPathResultType.NodeSet; } }
        public override void Reset() { }
        public override XPathNavigator Current { get { return null; } }
    }
}
