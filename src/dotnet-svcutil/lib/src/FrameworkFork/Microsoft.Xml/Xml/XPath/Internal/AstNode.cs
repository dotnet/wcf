// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace MS.Internal.Xml.XPath {
    using System;
    using Microsoft.Xml.XPath;

    internal abstract class AstNode {
        public enum AstType {
            Axis            ,
            Operator        ,
            Filter          ,
            ConstantOperand ,
            Function        ,
            Group           ,
            Root            ,
            Variable        ,        
            Error           
        };

        public abstract AstType Type { get; }
        public abstract XPathResultType ReturnType { get; }
    }
}
