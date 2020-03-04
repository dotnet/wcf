// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace MS.Internal.Xml.XPath {
    using System;
    using Microsoft.Xml;
    using Microsoft.Xml.XPath;
    using System.Diagnostics;
    using System.Globalization;

    internal class Filter : AstNode {
        private AstNode input;
        private AstNode condition;

        public Filter( AstNode input, AstNode condition) {
            this.input = input;
            this.condition = condition;
        }

        public override AstType         Type       { get { return AstType.Filter;          } }
        public override XPathResultType ReturnType { get { return XPathResultType.NodeSet; } }

        public AstNode Input     { get { return input;     } }
        public AstNode Condition { get { return condition; } }
    }
}
