// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    sealed class SyndicationContent : ISyndicationContent
    {
        private string _name;
        private IEnumerable<SyndicationAttribute> _attributes;
        private IEnumerable<ISyndicationContent> _children;

        public SyndicationContent(string rawContent)
        {
            RawContent = rawContent ?? throw new ArgumentNullException(nameof(rawContent));
        }

        public string RawContent { get; private set; }

        public string Name {
            get 
            {
                if (_name == null)
                {
                    _name = ParseName(RawContent);
                }

                return _name;
            }
        }

        public IEnumerable<SyndicationAttribute> Attributes
        {
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

        public IEnumerable<ISyndicationContent> Fields 
        {
            get 
            {
                if (_children == null)
                {
                    _children = ParseChildren(RawContent);
                }

                return _children;
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

        private static IEnumerable<ISyndicationContent> ParseChildren(string content)
        {
            var items = new List<ISyndicationContent>();

            if (!string.IsNullOrEmpty(content))
            {
                using (XmlReader reader = XmlReader.Create(new StringReader(content)))
                {
                    reader.ReadStartElement();

                    while (reader.IsStartElement())
                    {
                        items.Add(new SyndicationContent(reader.ReadOuterXml()));
                    }
                }
            }

            return items;
        }

        private static string ParseName(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                using (XmlReader reader = XmlReader.Create(new StringReader(content)))
                {
                    reader.MoveToContent();
                    return reader.Name;
                }
            }

            return string.Empty;
        }
    }
}
