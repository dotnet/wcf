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
    public class AtomFeedWriter : ISyndicationFeedWriter
    {
        private XmlWriter _writer;
        private bool _feedStarted;
        private IEnumerable<ISyndicationAttribute> _attributes;

        public AtomFeedWriter(XmlWriter writer, IEnumerable<ISyndicationAttribute> attributes = null)
            : this(writer, attributes, null)
        {
        }

        public AtomFeedWriter(XmlWriter writer, IEnumerable<ISyndicationAttribute> attributes, ISyndicationFeedFormatter formatter)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _attributes = EnsureXmlNs(attributes ?? Enumerable.Empty<ISyndicationAttribute>());

            Formatter = formatter ?? new AtomFormatter(_attributes, writer.Settings);
        }

        public ISyndicationFeedFormatter Formatter { get; private set; }

        public virtual Task WriteTitle(string value, string type = null)
        {
            return WriteText(AtomElementNames.Title, value, type);
        }

        public virtual Task WriteSubtitle(string value, string type = null)
        {
            return WriteText(AtomElementNames.Subtitle, value, type);
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

        public virtual Task WriteRights(string value, string type = null)
        {
            return WriteText(AtomElementNames.Rights, value, type);
        }

        public virtual Task WriteGenerator(string value, string uri = null, string version = null)
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

        public virtual Task Write(ISyndicationContent content)
        {
            return WriteRaw(Formatter.Format(content ?? throw new ArgumentNullException(nameof(content))));
        }

        public virtual Task Write(ISyndicationCategory category)
        {
            return WriteRaw(Formatter.Format(category ?? throw new ArgumentNullException(nameof(category))));
        }

        public virtual Task Write(ISyndicationImage image)
        {
            return WriteRaw(Formatter.Format(image ?? throw new ArgumentNullException(nameof(image))));
        }

        public virtual Task Write(ISyndicationItem item)
        {
            return WriteRaw(Formatter.Format(item ?? throw new ArgumentNullException(nameof(item))));
        }

        public virtual Task Write(ISyndicationPerson person)
        {
            return WriteRaw(Formatter.Format(person ?? throw new ArgumentNullException(nameof(person))));
        }

        public virtual Task Write(ISyndicationLink link)
        {
            return WriteRaw(Formatter.Format(link ?? throw new ArgumentNullException(nameof(link))));
        }

        public virtual Task WriteValue<T>(string name, T value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            string valueString = Formatter.FormatValue(value);

            if (valueString == null)
            {
                throw new FormatException(nameof(value));
            }
            
            return WriteRaw(Formatter.Format(new SyndicationContent(name, valueString)));
        }

        public virtual Task WriteRaw(string content)
        {
            if (!_feedStarted)
            {
                StartFeed();
            }

            return XmlUtils.WriteRawAsync(_writer, content);
        }

        public Task Flush()
        {
            return XmlUtils.FlushAsync(_writer);
        }

        private Task WriteText(string name, string value, string type)
        {
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