// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.XPath
{
    using System;

    public enum XPathNodeType
    {
        Root,
        Element,
        Attribute,
        Namespace,
        Text,
        SignificantWhitespace,
        Whitespace,
        ProcessingInstruction,
        Comment,
        All,
    }
}
