// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace MS.Internal.Xml.XPath
{
    using System;
    using Microsoft.Xml;
    using Microsoft.Xml.XPath;

    internal class Root : AstNode
    {
        public Root() { }

        public override AstType Type { get { return AstType.Root; } }
        public override XPathResultType ReturnType { get { return XPathResultType.NodeSet; } }
    }
}
