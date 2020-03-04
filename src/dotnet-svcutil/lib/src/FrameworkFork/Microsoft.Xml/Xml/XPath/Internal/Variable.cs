// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace MS.Internal.Xml.XPath {
    using System;
    using Microsoft.Xml;
    using Microsoft.Xml.XPath;

    internal class Variable : AstNode {
        private string localname;
	    private string prefix;

        public Variable(string name, string prefix) {
            this.localname = name;
	        this.prefix    = prefix;
        }

        public override AstType         Type       { get {return AstType.Variable   ; } }
        public override XPathResultType ReturnType { get {return XPathResultType.Any; } }
        
        public string Localname { get { return localname; } }
       	public string Prefix    { get { return prefix;    } }
    }
}
