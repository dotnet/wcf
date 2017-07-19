// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;

namespace Microsoft.SyndicationFeed.Atom10
{
    class Atom10FeedReader : SyndicationFeedReaderBase
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
                    return SyndicationElementType.Image;

                case Atom10Constants.AuthorTag:
                case Atom10Constants.ContributorTag:
                    return SyndicationElementType.Person;

                default:
                    return SyndicationElementType.Content;
            }
        }
    }
}
