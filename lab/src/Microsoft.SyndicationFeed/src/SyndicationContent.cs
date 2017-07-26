// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Microsoft.SyndicationFeed
{
    public sealed class SyndicationContent : ISyndicationContent
    {
        private List<ISyndicationAttribute> _attributes = new List<ISyndicationAttribute>();
        private List<ISyndicationContent> _children = new List<ISyndicationContent>();

        public SyndicationContent(string name, string value = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Value = value;
        }

        public string Name { get; private set; }

        public string Value { get; set; }

        public IEnumerable<ISyndicationAttribute> Attributes 
        {
            get 
            {
                return _attributes;
            }
        }

        public IEnumerable<ISyndicationContent> Fields
        {
            get 
            {
                return _children;
            }
        }

        public void AddAttribute(ISyndicationAttribute attribute)
        {
            _attributes.Add(attribute ?? throw new ArgumentNullException(nameof(attribute)));
        }

        public void AddField(ISyndicationContent field)
        {
            _children.Add(field ?? throw new ArgumentNullException(nameof(field)));
        }
    }
}
