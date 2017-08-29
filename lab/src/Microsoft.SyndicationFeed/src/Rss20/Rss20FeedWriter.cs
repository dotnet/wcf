// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.SyndicationFeed.Rss
{
    public class Rss20FeedWriter : ISyndicationFeedWriter
    {
        private XmlWriter _writer;
        private bool _feedStarted;
        private IEnumerable<ISyndicationAttribute> _attributes;

        public Rss20FeedWriter(XmlWriter writer, IEnumerable<ISyndicationAttribute> attributes = null)
            : this(writer, attributes, new Rss20Formatter(attributes, writer.Settings))
        {
        }

        public Rss20FeedWriter(XmlWriter writer, IEnumerable<ISyndicationAttribute> attributes, ISyndicationFeedFormatter formatter)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            _attributes = attributes;
        }

        public ISyndicationFeedFormatter Formatter { get; private set; }

        public virtual Task Write(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            return WriteRaw(Formatter.Format(content));
        }

        public virtual Task Write(ISyndicationCategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            return WriteRaw(Formatter.Format(category));
        }

        public virtual Task Write(ISyndicationImage image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            return WriteRaw(Formatter.Format(image));
        }

        public virtual Task Write(ISyndicationItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return WriteRaw(Formatter.Format(item));
        }

        public virtual Task Write(ISyndicationPerson person)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            return WriteRaw(Formatter.Format(person));
        }

        public virtual Task Write(ISyndicationLink link)
        {
            if (link == null)
            {
                throw new ArgumentNullException(nameof(link));
            }

            return WriteRaw(Formatter.Format(link));
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

        private void StartFeed()
        {
            // Write <rss version="2.0">
            _writer.WriteStartElement(Rss20ElementNames.Rss);

            // Write attributes if exist
            if (_attributes != null)
            {
                foreach (var a in _attributes)
                {
                    _writer.WriteSyndicationAttribute(a);
                }
            }

            _writer.WriteAttributeString(Rss20ElementNames.Version, Rss20Constants.Version);
            _writer.WriteStartElement(Rss20ElementNames.Channel);
            _feedStarted = true;
        }
    }
}
