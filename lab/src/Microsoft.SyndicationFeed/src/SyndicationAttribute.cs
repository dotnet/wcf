// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.SyndicationFeed
{
    public sealed class SyndicationAttribute : ISyndicationAttribute
    {
        public SyndicationAttribute(string name, string value) :
            this(name, string.Empty, value)
        {
        }

        public SyndicationAttribute(string name, string nameSpace, string value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Namespace = nameSpace ?? throw new ArgumentNullException(nameof(nameSpace));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Name { get; private set; }
        public string Namespace { get; private set; }
        public string Value { get; private set; }
    }
}