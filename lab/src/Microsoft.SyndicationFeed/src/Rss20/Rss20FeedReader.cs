// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    public class Rss20FeedReader : SyndicationFeedReaderBase
    {
        private readonly XmlReader _reader;
        private bool _knownFeed;

        public Rss20FeedReader(XmlReader reader) 
            : this(reader, new Rss20FeedFormatter())
        {
        }

        public Rss20FeedReader(XmlReader reader, ISyndicationFeedFormatter formatter)
            : base(reader, formatter)
        {
            _reader = reader;
        }

        public override async Task<bool> Read()
        {
            if (!_knownFeed)
            {
                await InitRead();
                _knownFeed = true;
            }

            return await base.Read();
        }
        
        protected override SyndicationElementType MapElementType(string elementName)
        {
            switch (elementName)
            {
                case Rss20Constants.ItemTag:
                    return SyndicationElementType.Item;

                case Rss20Constants.LinkTag:
                    return SyndicationElementType.Link;

                case Rss20Constants.CategoryTag:
                    return SyndicationElementType.Category;

                case Rss20Constants.AuthorTag:
                case Rss20Constants.ManagingEditorTag:
                    return SyndicationElementType.Person;

                case Rss20Constants.ImageTag:
                    return SyndicationElementType.Image;

                default:
                    return SyndicationElementType.Content;
            }
        }


        private async Task InitRead()
        {
            // Check <rss>
            bool knownFeed = _reader.IsStartElement(Rss20Constants.RssTag, Rss20Constants.Rss20Namespace) &&
                             _reader.GetAttribute(Rss20Constants.VersionTag).Equals(Rss20Constants.Version);

            if (knownFeed) {
                // Read<rss>
                await XmlUtils.ReadAsync(_reader);

                // Check <channel>
                knownFeed = _reader.IsStartElement(Rss20Constants.ChannelTag, Rss20Constants.Rss20Namespace);
            }

            if (!knownFeed)
            {
                throw new XmlException("Unknown Rss Feed");
            }

            // Read <channel>
            await XmlUtils.ReadAsync(_reader);
        }
    }
}