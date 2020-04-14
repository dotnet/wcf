// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace MS.Internal.Xml.XPath
{
    using System;
    using Microsoft.Xml;
    using Microsoft.Xml.XPath;
    using System.Diagnostics;
    using System.Globalization;
    using System.Collections.Generic;

    internal sealed class ParentQuery : CacheAxisQuery
    {
        public ParentQuery(Query qyInput, string Name, string Prefix, XPathNodeType Type) : base(qyInput, Name, Prefix, Type) { }
        private ParentQuery(ParentQuery other) : base(other) { }

        public override object Evaluate(XPathNodeIterator context)
        {
            base.Evaluate(context);

            // REVIEW: This algorithm doesn't use the fact that input is DOD.
            XPathNavigator input;
            while ((input = qyInput.Advance()) != null)
            {
                input = input.Clone();

                if (input.MoveToParent())
                {
                    if (matches(input))
                    {
                        Insert(outputBuffer, input);
                    }
                }
            }
            return this;
        }

        public override XPathNodeIterator Clone() { return new ParentQuery(this); }
    }
}
