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

        public Rss20FeedWriter(XmlWriter writer)
            : this(writer, new Rss20Formatter(writer.Settings))
        {
        }

        public Rss20FeedWriter(XmlWriter writer, IEnumerable<ISyndicationAttribute> attributes)
            : this(writer, new Rss20Formatter(writer.Settings), attributes)
        {
        }

        public Rss20FeedWriter(XmlWriter writer, ISyndicationFeedFormatter formatter)
            : this(writer, formatter, null)
        {
        }

        public Rss20FeedWriter(XmlWriter writer, ISyndicationFeedFormatter formatter, IEnumerable<ISyndicationAttribute> namespaces)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            _attributes = namespaces; // optional
        }

        public ISyndicationFeedFormatter Formatter { get; private set; }

        public virtual Task Write(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (!_feedStarted)
            {
                StartFeed();
            }

            return XmlUtils.WriteRaw(_writer, Formatter.Format(content));
        }

        public virtual Task Write(ISyndicationCategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (!_feedStarted)
            {
                StartFeed();
            }

            return XmlUtils.WriteRaw(_writer, Formatter.Format(category));
        }

        public virtual Task Write(ISyndicationImage image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (!_feedStarted)
            {
                StartFeed();
            }

            return XmlUtils.WriteRaw(_writer, Formatter.Format(image));
        }

        public virtual Task Write(ISyndicationItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!_feedStarted)
            {
                StartFeed();
            }

            return XmlUtils.WriteRaw(_writer, Formatter.Format(item));
        }

        public virtual Task Write(ISyndicationPerson person)
        {

            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            if (!_feedStarted)
            {
                StartFeed();
            }

            return XmlUtils.WriteRaw(_writer, Formatter.Format(person));
        }

        public virtual Task Write(ISyndicationLink link)
        {
            if (link == null)
            {
                throw new ArgumentNullException(nameof(link));
            }

            if (!_feedStarted)
            {
                StartFeed();
            }

            return XmlUtils.WriteRaw(_writer, Formatter.Format(link));
        }

        public Task WriteValue<T>(string name, T value)
        {

            if (!_feedStarted)
            {
                StartFeed();
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var valueString = Converter.FormatValue(value);

            if (string.IsNullOrEmpty(valueString))
            {
                throw new ArgumentNullException(nameof(value));
            }

            SyndicationContent content = new SyndicationContent(name, valueString);

            return XmlUtils.WriteRaw(_writer, Formatter.Format(content));
        }

        public Task WriteElement(string content)
        {
            return XmlUtils.WriteRaw(_writer, content);
        }

        private void StartFeed()
        {
            //Write <rss version="2.0">
            _writer.WriteStartElement(Rss20ElementNames.Rss);

            //Write namespaces if exist
            if (_attributes != null)
            {
                foreach (var ns in _attributes)
                {
                    if(ns.Namespace != null)
                    {
                        XmlUtils.SplitName(ns.Name,out string prefix, out string localname);
                        _writer.WriteAttributeString(prefix, localname, null, ns.Value);
                    }
                }
            }

            _writer.WriteAttributeString(Rss20ElementNames.Version, Rss20Constants.Version);
            _writer.WriteStartElement(Rss20ElementNames.Channel);
            _feedStarted = true;
        }
    }
}
