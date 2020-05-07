// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MS.Internal.Xml.XPath
{
    using System;
    using Microsoft.Xml;
    using Microsoft.Xml.XPath;
    using System.Diagnostics;
    using System.Globalization;
    using System.Collections;

    internal class XmlIteratorQuery : Query
    {
        private ResetableIterator _it;

        public XmlIteratorQuery(XPathNodeIterator it)
        {
            _it = it as ResetableIterator;
            if (_it == null)
            {
                _it = new XPathArrayIterator(it);
            }
        }
        protected XmlIteratorQuery(XmlIteratorQuery other) : base(other)
        {
            _it = (ResetableIterator)other._it.Clone();
        }

        public override XPathNavigator Current { get { return _it.Current; } }

        public override XPathNavigator Advance()
        {
            if (_it.MoveNext())
            {
                return _it.Current;
            }
            return null;
        }

        public override XPathResultType StaticType { get { return XPathResultType.NodeSet; } }

        public override void Reset()
        {
            _it.Reset();
        }

        public override XPathNodeIterator Clone() { return new XmlIteratorQuery(this); }

        public override int CurrentPosition { get { return _it.CurrentPosition; } }

        public override object Evaluate(XPathNodeIterator nodeIterator)
        {
            return _it;
        }
    }
}
