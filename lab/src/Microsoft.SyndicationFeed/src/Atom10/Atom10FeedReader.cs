// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    public class Atom10FeedReader : SyndicationFeedReaderBase
    {
        private readonly XmlReader _reader;
        private bool _knownFeed;

        public Atom10FeedReader(XmlReader reader)
            : this(reader, new Atom10FeedFormatter())
        {                
        }

        public Atom10FeedReader(XmlReader reader, ISyndicationFeedFormatter formatter) 
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
                case Atom10Constants.EntryTag:
                    return SyndicationElementType.Item;

                case Atom10Constants.LinkTag:
                    return SyndicationElementType.Link;

                case Atom10Constants.CategoryTag:
                    return SyndicationElementType.Category;

                case Atom10Constants.LogoTag:
                case Atom10Constants.IconTag:
                    return SyndicationElementType.Image;

                case Atom10Constants.AuthorTag:
                case Atom10Constants.ContributorTag:
                    return SyndicationElementType.Person;

                default:
                    return SyndicationElementType.Content;
            }
        }

        private async Task InitRead()
        {
            // Check <feed>
            bool knownFeed = _reader.IsStartElement(Atom10Constants.FeedTag, Atom10Constants.Atom10Namespace);

            if (knownFeed)
            {
                //Read <feed>
                await XmlUtils.ReadAsync(_reader);
            }
            else
            {
                throw new XmlException("Unkown Atom Feed");
            }
        }

    }
}
