// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace MS.Internal.Xml.XPath
{
    using System;
    using Microsoft.Xml;
    using Microsoft.Xml.XPath;
    using System.Diagnostics;

    internal sealed class ReversePositionQuery : ForwardPositionQuery
    {
        public ReversePositionQuery(Query input) : base(input) { }
        private ReversePositionQuery(ReversePositionQuery other) : base(other) { }

        public override XPathNodeIterator Clone() { return new ReversePositionQuery(this); }
        public override int CurrentPosition { get { return outputBuffer.Count - count + 1; } }
        public override QueryProps Properties { get { return base.Properties | QueryProps.Reverse; } }
    }
}





