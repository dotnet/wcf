// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace MS.Internal.Xml.XPath {
    using System;
    using Microsoft.Xml;
    using Microsoft.Xml.XPath;
    using System.Diagnostics;
    using Microsoft.Xml.Xsl;
    using System.Collections.Generic;

    internal sealed class MergeFilterQuery : CacheOutputQuery {
        private Query child;
        
        public MergeFilterQuery(Query input, Query child) : base(input) {
            this.child  = child;
        }
        private MergeFilterQuery(MergeFilterQuery other) : base(other) {
            this.child  = Clone(other.child);
        }
        
        public override void SetXsltContext(XsltContext xsltContext) {
            base.SetXsltContext(xsltContext);
            child.SetXsltContext(xsltContext);
        }

        public override object Evaluate(XPathNodeIterator nodeIterator) {
            base.Evaluate(nodeIterator);

            while (input.Advance() != null) {
                child.Evaluate(input);
                XPathNavigator node;
                while ((node = child.Advance()) != null) {
                    Insert(outputBuffer, node);
                }
            }
            return this;
        }

        public override XPathNavigator MatchNode(XPathNavigator current) {
            XPathNavigator context = child.MatchNode(current);
            if (context == null) {
                return null;
            }
            context = input.MatchNode(context);
            if (context == null) {
                return null;
            }
            Evaluate(new XPathSingletonIterator(context.Clone(), /*moved:*/true));
            XPathNavigator result = Advance();
            while (result != null) {
                if (result.IsSamePosition(current)) {
                    return context;
                }
                result = Advance();
            } 
            return null;
        }

        public override XPathNodeIterator Clone() { return new MergeFilterQuery(this); }

        public override void PrintQuery(XmlWriter w) {
            w.WriteStartElement(this.GetType().Name);
            input.PrintQuery(w);
            child.PrintQuery(w);
            w.WriteEndElement();
        }
    }
}
