// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.SyndicationFeed.Rss
{
    public class Rss20FeedWriter : XmlFeedWriter
    {
        private readonly XmlWriter _writer;
        private bool _feedStarted;
        private readonly IEnumerable<ISyndicationAttribute> _attributes;

        public Rss20FeedWriter(XmlWriter writer, IEnumerable<ISyndicationAttribute> attributes = null)
            : this(writer, attributes, null)
        {
        }

        public Rss20FeedWriter(XmlWriter writer, IEnumerable<ISyndicationAttribute> attributes, ISyndicationFeedFormatter formatter) :
            base(writer, formatter ?? new Rss20Formatter(attributes, writer.Settings))
        {
            _writer = writer;
            _attributes = attributes;
        }

        public virtual Task WriteTitle(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return WriteValue(Rss20ElementNames.Title, value);
        }

        public virtual Task WriteDescription(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return WriteValue(Rss20ElementNames.Description, value);
        }

        public virtual Task WriteLanguage(CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

            return WriteValue(Rss20ElementNames.Language, culture.Name);
        }

        public virtual Task WriteCopyright(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return WriteValue(Rss20ElementNames.Copyright, value);
        }

        public virtual Task WritePubDate(DateTimeOffset dt)
        {
            if (dt == default(DateTimeOffset))
            {
                throw new ArgumentException(nameof(dt));
            }

            return WriteValue(Rss20ElementNames.PubDate, dt);
        }

        public virtual Task WriteLastBuildDate(DateTimeOffset dt)
        {
            if (dt == default(DateTimeOffset))
            {
                throw new ArgumentException(nameof(dt));
            }

            return WriteValue(Rss20ElementNames.LastBuildDate, dt);
        }

        public virtual Task WriteGenerator(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return WriteValue(Rss20ElementNames.Generator, value);
        }

        public virtual Task WriteDocs()
        {
            return WriteValue(Rss20ElementNames.Docs, Rss20Constants.SpecificationLink);
        }

        public virtual Task WriteCloud(Uri uri, string registerProcedure, string protocol)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (!uri.IsAbsoluteUri)
            {
                throw new ArgumentException("Absolute uri required");
            }

            if (string.IsNullOrEmpty(registerProcedure))
            {
                throw new ArgumentNullException(nameof(registerProcedure));
            }

            var cloud = new SyndicationContent(Rss20ElementNames.Cloud);

            cloud.AddAttribute(new SyndicationAttribute("domain", uri.GetComponents(UriComponents.Host, UriFormat.UriEscaped)));
            cloud.AddAttribute(new SyndicationAttribute("port", uri.GetComponents(UriComponents.StrongPort, UriFormat.UriEscaped)));
            cloud.AddAttribute(new SyndicationAttribute("path", uri.GetComponents(UriComponents.PathAndQuery, UriFormat.UriEscaped)));
            cloud.AddAttribute(new SyndicationAttribute("registerProcedure", registerProcedure));
            cloud.AddAttribute(new SyndicationAttribute("protocol", protocol ?? "xml-rpc"));

            return Write(cloud);
        }

        public virtual Task WriteTimeToLive(TimeSpan ttl)
        {
            if (ttl == default(TimeSpan))
            {
                throw new ArgumentException(nameof(ttl));
            }

            return WriteValue(Rss20ElementNames.TimeToLive, (long) Math.Max(1, Math.Ceiling(ttl.TotalMinutes)));
        }

        public virtual Task WriteSkipHours(IEnumerable<byte> hours)
        {
            if (hours == null)
            {
                throw new ArgumentNullException(nameof(hours));
            }

            var skipHours = new SyndicationContent(Rss20ElementNames.SkipHours);

            foreach (var h in hours)
            {
                if (h < 0 || h > 23)
                {
                    throw new ArgumentOutOfRangeException("Hour value must be between 0 and 23");
                }

                skipHours.AddField(new SyndicationContent("hour", Formatter.FormatValue(h)));
            }

            return Write(skipHours);
        }

        public virtual Task WriteSkipDays(IEnumerable<DayOfWeek> days)
        {
            if (days == null)
            {
                throw new ArgumentNullException(nameof(days));
            }

            var skipDays = new SyndicationContent(Rss20ElementNames.SkipDays);

            foreach (var d in days)
            {
                skipDays.AddField(new SyndicationContent("day", Formatter.FormatValue(d)));
            }

            return Write(skipDays);
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