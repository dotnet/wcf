//------------------------------------------------------------------------------
// <copyright file="ValueQuery.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">sdub</owner>
//------------------------------------------------------------------------------

namespace MS.Internal.Xml.XPath {
    using System;
    using System.Globalization;
    using System.Text;
    using Microsoft.Xml;
    using Microsoft.Xml.XPath;
    using Microsoft.Xml.Xsl;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal abstract class ValueQuery : Query {
        public    ValueQuery() { }
        protected ValueQuery(ValueQuery other) : base(other) { }
        public sealed override void Reset() { }
        public sealed override XPathNavigator Current { get { throw XPathException.Create(ResXml.Xp_NodeSetExpected); } }
        public sealed override int CurrentPosition { get { throw XPathException.Create(ResXml.Xp_NodeSetExpected); } }
        public sealed override int Count { get { throw XPathException.Create(ResXml.Xp_NodeSetExpected); } }
        public sealed override XPathNavigator Advance() { throw XPathException.Create(ResXml.Xp_NodeSetExpected); }
    }
}
