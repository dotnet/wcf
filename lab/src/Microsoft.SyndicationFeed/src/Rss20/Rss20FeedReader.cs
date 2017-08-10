// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.SyndicationFeed.Rss
{
    public class Rss20FeedReader : SyndicationFeedReaderBase
    {
        private readonly XmlReader _reader;
        private bool _knownFeed;

        public Rss20FeedReader(XmlReader reader) 
            : this(reader, new Rss20FeedParser())
        {
        }

        public Rss20FeedReader(XmlReader reader, ISyndicationFeedParser parser)
            : base(reader, parser)
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
                case Rss20ElementNames.Item:
                    return SyndicationElementType.Item;

                case Rss20ElementNames.Link:
                    return SyndicationElementType.Link;

                case Rss20ElementNames.Category:
                    return SyndicationElementType.Category;

                case Rss20ElementNames.Author:
                case Rss20ElementNames.ManagingEditor:
                    return SyndicationElementType.Person;

                case Rss20ElementNames.Image:
                    return SyndicationElementType.Image;

                default:
                    return SyndicationElementType.Content;
            }
        }
        
        private async Task InitRead()
        {
            // Check <rss>
            bool knownFeed = _reader.IsStartElement(Rss20ElementNames.Rss, Rss20Constants.Rss20Namespace) &&
                             _reader.GetAttribute(Rss20ElementNames.Version).Equals(Rss20Constants.Version);

            if (knownFeed)
            {
                // Read<rss>
                await XmlUtils.ReadAsync(_reader);

                // Check <channel>
                knownFeed = _reader.IsStartElement(Rss20ElementNames.Channel, Rss20Constants.Rss20Namespace);
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