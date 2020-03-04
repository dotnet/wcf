// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace MS.Internal.Xml.XPath {
    using System;
    using Microsoft.Xml;
    using Microsoft.Xml.XPath;
    using System.Diagnostics;

    internal class ForwardPositionQuery : CacheOutputQuery {
        
        public ForwardPositionQuery(Query input) : base(input) {
            Debug.Assert(input != null);
        }
        protected ForwardPositionQuery(ForwardPositionQuery other) : base(other) { }       
        
        public override object Evaluate(XPathNodeIterator context) {
            base.Evaluate(context);

            XPathNavigator node;
            while ((node = base.input.Advance()) != null) {
                outputBuffer.Add(node.Clone());
            }

            return this;
        }

        public override XPathNavigator MatchNode(XPathNavigator context) {
            return input.MatchNode(context);
        }

        public override XPathNodeIterator Clone() { return new ForwardPositionQuery(this); }
    }
}





