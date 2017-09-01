// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.SyndicationFeed.Atom
{
    public class AtomFeedWriter : XmlFeedWriter
    {
        private readonly XmlWriter _writer;
        private readonly IEnumerable<ISyndicationAttribute> _attributes;
        private bool _feedStarted;

        public AtomFeedWriter(XmlWriter writer, IEnumerable<ISyndicationAttribute> attributes = null)
            : this(writer, attributes, null)
        {
        }

        public AtomFeedWriter(XmlWriter writer, IEnumerable<ISyndicationAttribute> attributes, ISyndicationFeedFormatter formatter) :
            this(writer, formatter, EnsureXmlNs(attributes ?? Enumerable.Empty<ISyndicationAttribute>()))
        {
        }

        private AtomFeedWriter(XmlWriter writer, ISyndicationFeedFormatter formatter, IEnumerable<ISyndicationAttribute> attributes) :
            base(writer, formatter ?? new AtomFormatter(attributes, writer.Settings))
        {
            _writer = writer;
            _attributes = attributes;
        }

        public virtual Task WriteTitle(string value)
        {
            return WriteText(AtomElementNames.Title, value, null);
        }

        public virtual Task WriteSubtitle(string value)
        {
            return WriteText(AtomElementNames.Subtitle, value, null);
        }

        public virtual Task WriteId(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return WriteValue(AtomElementNames.Id, value);
        }

        public virtual Task WriteUpdated(DateTimeOffset dt)
        {
            if (dt == default(DateTimeOffset))
            {
                throw new ArgumentException(nameof(dt));
            }

            return WriteValue(AtomElementNames.Updated, dt);
        }

        public virtual Task WriteRights(string value)
        {
            return WriteText(AtomElementNames.Rights, value, null);
        }

        public virtual Task WriteGenerator(string value, string uri, string version)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var generator = new SyndicationContent(AtomElementNames.Generator, value);

            if (!string.IsNullOrEmpty(uri))
            {
                generator.AddAttribute(new SyndicationAttribute("uri", uri));
            }

            if (!string.IsNullOrEmpty(version))
            {
                generator.AddAttribute(new SyndicationAttribute("version", version));
            }

            return Write(generator);
        }

        public virtual Task WriteText(string name, string value, string type)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var content = new SyndicationContent(name, value);

            if (!string.IsNullOrEmpty(type))
            {
                content.AddAttribute(new SyndicationAttribute(AtomConstants.Type, type));
            }

            return Write(content);
        }

        public override Task WriteRaw(string content)
        {
            if (!_feedStarted)
            {
                StartFeed();
            }

            return XmlUtils.WriteRawAsync(_writer, content);
        }

        private void StartFeed()
        {
            ISyndicationAttribute xmlns = _attributes.FirstOrDefault(a => a.Name == "xmlns");

            // Write <feed>
            if (xmlns != null)
            {
                _writer.WriteStartElement(AtomElementNames.Feed, xmlns.Value);
            }
            else
            {
                _writer.WriteStartElement(AtomElementNames.Feed);
            }

            // Add attributes
            foreach (var a in _attributes)
            {
                if (a != xmlns)
                {
                    _writer.WriteSyndicationAttribute(a);
                }
            }

            _feedStarted = true;
        }

        private static IEnumerable<ISyndicationAttribute> EnsureXmlNs(IEnumerable<ISyndicationAttribute> attributes)
        {
            ISyndicationAttribute xmlnsAttr = attributes.FirstOrDefault(a => a.Name.StartsWith("xmlns") && a.Value == AtomConstants.Atom10Namespace);

            //
            // Insert Atom namespace if it doesn't already exist
            if (xmlnsAttr == null)
            {
                var list = new List<ISyndicationAttribute>(attributes);
                list.Insert(0, new SyndicationAttribute("xmlns", AtomConstants.Atom10Namespace));

                attributes = list;
            }

            return attributes;
        }
    }
}