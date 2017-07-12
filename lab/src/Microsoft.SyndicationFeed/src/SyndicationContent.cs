// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SyndicationFeed
{
    sealed class SyndicationContent : ISyndicationContent
    {
        private IEnumerable<SyndicationAttribute> _attributes;

        public SyndicationContent(string name, string rawContent)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            RawContent = rawContent ?? throw new ArgumentNullException(nameof(rawContent));
        }

        public string RawContent { get; private set; }

        public string Name { get; private set; }

        public IEnumerable<SyndicationAttribute> Attributes {
            get 
            {
                if (_attributes == null)
                {
                    _attributes = !string.IsNullOrEmpty(RawContent) ? XmlUtils.ReadAttributes(RawContent) :
                                                                      Enumerable.Empty<SyndicationAttribute>();
                }

                return _attributes;
            }
        }

        public string GetValue()
        {
            if (string.IsNullOrEmpty(RawContent))
            {
                throw new ArgumentNullException(nameof(RawContent));
            }

            return XmlUtils.GetValue(RawContent);
        }
    }
}
