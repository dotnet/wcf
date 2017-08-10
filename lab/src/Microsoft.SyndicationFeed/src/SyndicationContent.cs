// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SyndicationFeed
{
    public class SyndicationContent : ISyndicationContent
    {
        private ICollection<ISyndicationAttribute> _attributes;
        private ICollection<ISyndicationContent> _children;

        public SyndicationContent(string name, string value = null)
            : this(name, string.Empty, value)
        {
        }

        public SyndicationContent(string name, string ns, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }


            Name = name;
            Value = value;
            Namespace = ns ?? throw new ArgumentNullException(nameof(ns));

            _attributes = new List<ISyndicationAttribute>();
            _children = new List<ISyndicationContent>();
        }

        public SyndicationContent(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            Name = content.Name;
            Namespace = content.Namespace;
            Value = content.Value;

            // Copy collections only if needed
            _attributes = content.Attributes as ICollection<ISyndicationAttribute> ?? content.Attributes.ToList();
            _children = content.Fields as ICollection<ISyndicationContent> ?? content.Fields.ToList();
        }

        public string Name { get; private set; }

        public string Namespace { get; private set; }

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
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            if (_attributes.IsReadOnly)
            {
                _attributes = _attributes.ToList();
            }

            _attributes.Add(attribute);
        }

        public void AddField(ISyndicationContent field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            if (_children.IsReadOnly)
            {
                _children = _children.ToList();
            }

            _children.Add(field);
        }
    }
}
