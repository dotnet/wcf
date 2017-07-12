// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    public class SyndicationAttribute
    {
        public SyndicationAttribute(XmlQualifiedName name, string value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public XmlQualifiedName Name { get; private set; }
        public string Value { get; private set; }
    }
}
