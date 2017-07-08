// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    public class Rss20FeedReader : ISyndicationFeedReader
    {
        private XmlReader _reader;
        private ISyndicationFeedFormatter _formatter;
        private bool _knownFeed;


        public Rss20FeedReader(XmlReader reader) 
            : this(reader, new Rss20FeedFormatter())
        {
        }

        private Rss20FeedReader(XmlReader reader, ISyndicationFeedFormatter formatter)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));

            ElementType = SyndicationElementType.None;

            if (!_reader.Settings.Async)
            {
                throw new ArgumentException("Synchronous XmlReader is not supported", nameof(reader));
            }
        }

        public SyndicationElementType ElementType { get; private set; }

        public string ElementName { get; private set; }

        public async Task<bool> Read()
        {
            EnsureRead();

            while (await _reader.ReadAsync())
            {
                switch (_reader.NodeType)
                {
                    case XmlNodeType.Element:
                        ElementType = GetElementType();
                        ElementName = _reader.Name;
                        return true;

                    default:
                        // Keep reading
                        await _reader.SkipAsync();
                        break;
                }
            }

            ElementType = SyndicationElementType.None;
            ElementName = null;

            return false;
        }

        public Task<ISyndicationCategory> ReadCategory()
        {
            throw new System.NotImplementedException();
        }

        public Task<ISyndicationContent> ReadContent()
        {
            throw new System.NotImplementedException();
        }

        public async Task<ISyndicationItem> ReadItem()
        {
            if (ElementType != SyndicationElementType.Item)
            {
                throw new XmlException("Unknown Item");
                //throw new XmlException(SR.GetString(SR.UnknownItemXml, reader.LocalName, reader.NamespaceURI));
            }

            await _reader.MoveToContentAsync();

            string content = await _reader.ReadOuterXmlAsync();

            return _formatter.ParseItem(content);
        }

        public Task<ISyndicationLink> ReadLink()
        {
            if (ElementType != SyndicationElementType.Link)
            {
                throw new XmlException("Unknown Link");
                //throw new XmlException(SR.GetString(SR.UnknownItemXml, reader.LocalName, reader.NamespaceURI));
            }

            throw new NotImplementedException();
        }

        public Task<ISyndicationPerson> ReadPerson()
        {
            if (ElementType != SyndicationElementType.Person)
            {
                throw new XmlException("Unknown Link");
                //throw new XmlException(SR.GetString(SR.UnknownItemXml, reader.LocalName, reader.NamespaceURI));
            }

            throw new NotImplementedException();
        }

        public Task Skip()
        {
            return _reader.SkipAsync();
            //throw new NotImplementedException();
        }

        private SyndicationElementType GetElementType()
        {
            switch (_reader.Name)
            {
                case Rss20Constants.ItemTag:
                    return SyndicationElementType.Item;
                case Rss20Constants.LinkTag:
                    return SyndicationElementType.Link;
                case Rss20Constants.CategoryTag:
                    return SyndicationElementType.Category;
                case Rss20Constants.AuthorTag:
                    return SyndicationElementType.Person;
                default:
                    return SyndicationElementType.Content;
            }
        }

        private void EnsureRead()
        {
            if (!_knownFeed)
            {
                _knownFeed = _reader.IsStartElement(Rss20Constants.RssTag, Rss20Constants.Rss20Namespace);

                if (!_knownFeed)
                {
                    throw new XmlException("Unknown Feed");
                    //throw new XmlException(SR.GetString(SR.UnknownFeedXml, reader.LocalName, reader.NamespaceURI));
                }
            }
        }
    }
}
