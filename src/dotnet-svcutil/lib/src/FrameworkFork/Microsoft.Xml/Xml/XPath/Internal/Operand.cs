// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace MS.Internal.Xml.XPath
{
    using System;
    using Microsoft.Xml;
    using Microsoft.Xml.XPath;
    using System.Diagnostics;
    using System.Globalization;

    internal class Operand : AstNode
    {
        private XPathResultType _type;
        private object _val;

        public Operand(string val)
        {
            _type = XPathResultType.String;
            _val = val;
        }

        public Operand(double val)
        {
            _type = XPathResultType.Number;
            _val = val;
        }

        public Operand(bool val)
        {
            _type = XPathResultType.Boolean;
            _val = val;
        }

        public override AstType Type { get { return AstType.ConstantOperand; } }
        public override XPathResultType ReturnType { get { return _type; } }

        public object OperandValue { get { return _val; } }
    }
}
