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
        private readonly XmlReader _reader;
        private bool _knownFeed;
        private bool _currentSet;

        public Rss20FeedReader(XmlReader reader) 
            : this(reader, new Rss20FeedFormatter())
        {
        }

        public Rss20FeedReader(XmlReader reader, ISyndicationFeedFormatter formatter)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));

            ElementType = SyndicationElementType.None;

            if (!_reader.Settings.Async)
            {
                throw new ArgumentException("Synchronous XmlReader is not supported", nameof(reader));
            }
        }

        public ISyndicationFeedFormatter Formatter { get; private set; }

        public SyndicationElementType ElementType { get; private set; }

        public string ElementName { get; private set; }

        public async Task<bool> Read()
        {
            if (_currentSet) {
                //
                // The reader is already advanced, return status
                _currentSet = false;
                return !_reader.EOF;
            }
            else {
                //
                // Advance the reader
                return await MoveNext(false);
            }
        }

        public async Task<ISyndicationCategory> ReadCategory()
        {
            if (ElementType != SyndicationElementType.Category)
            {
                throw new XmlException("Unknown Category");
            }

            ISyndicationCategory category = Formatter.ParseCategory(await _reader.ReadOuterXmlAsync());

            await MoveNext();

            return category;
        }

        public async Task<ISyndicationContent> ReadContent()
        {
            //
            // Any element can be read as ISyndicationContent
            
            ISyndicationContent content = Formatter.ParseContent(await _reader.ReadOuterXmlAsync());

            await MoveNext();

            return content;
        }

        public async Task<ISyndicationItem> ReadItem()
        {
            if (ElementType != SyndicationElementType.Item)
            {
                throw new XmlException("Unknown Item");
                //throw new XmlException(SR.GetString(SR.UnknownItemXml, reader.LocalName, reader.NamespaceURI));
            }

            string xml = await _reader.ReadOuterXmlAsync();

            ISyndicationItem item = Formatter.ParseItem(xml);

            await MoveNext();

            return item;
        }

        public async Task<ISyndicationLink> ReadLink()
        {
            if (ElementType != SyndicationElementType.Link)
            {
                throw new XmlException("Unknown Link");
                //throw new XmlException(SR.GetString(SR.UnknownItemXml, reader.LocalName, reader.NamespaceURI));
            }

            string xml = await _reader.ReadOuterXmlAsync();

            ISyndicationLink link = Formatter.ParseLink(xml);

            await MoveNext();

            return link;
        }

        public async Task<ISyndicationPerson> ReadPerson()
        {
            if (ElementType != SyndicationElementType.Person)
            {
                throw new XmlException("Unknown Person");
                //throw new XmlException(SR.GetString(SR.UnknownItemXml, reader.LocalName, reader.NamespaceURI));
            }

            string content = await _reader.ReadOuterXmlAsync();

            ISyndicationPerson person = Formatter.ParsePerson(content);

            await MoveNext();

            return person;
        }

        public async Task<ISyndicationImage> ReadImage()
        {
            if (ElementType != SyndicationElementType.Image)
            {
                throw new XmlException("Unknown Image");
                //throw new XmlException(SR.GetString(SR.UnknownItemXml, reader.LocalName, reader.NamespaceURI));
            }

            ISyndicationImage image = Formatter.ParseImage(await _reader.ReadOuterXmlAsync());

            await MoveNext();

            return image;
        }


        public async Task<T> ReadValue<T>()
        {
            ISyndicationContent content = await ReadContent();

            if (!Formatter.TryParseValue(content.GetValue(), out T value))
            {
                throw new FormatException();
            }

            return value;
        }

        public Task Skip()
        {
            return _reader.SkipAsync();
        }

        protected async Task<bool> MoveNext(bool setCurrent = true)
        {
            await EnsureRead();

            do
            {
                switch (_reader.NodeType)
                {
                    case XmlNodeType.Element:
                        ElementType = MapElementType();
                        ElementName = _reader.Name;
                        _currentSet = setCurrent;
                        return true;

                    default:
                        // Keep reading
                        break;
                }
            }
            while (await _reader.ReadAsync());

            /*
            while (await _reader.ReadAsync()) {
                switch (_reader.NodeType) {
                    case XmlNodeType.Element:
                        ElementType = MapElementType();
                        ElementName = _reader.Name;
                        _currentSet = setCurrent;
                        return true;

                    default:
                        // Keep reading
                        break;
                }
            }
            */

            //
            // Reset
            ElementType = SyndicationElementType.None;
            ElementName = null;
            _currentSet = false;

            return false;
        }

        protected virtual SyndicationElementType MapElementType()
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
                case Rss20Constants.ManagingEditorTag:
                    return SyndicationElementType.Person;

                case Rss20Constants.ImageTag:
                    return SyndicationElementType.Image;

                default:
                    return SyndicationElementType.Content;
            }
        }

        private async Task EnsureRead()
        {
            if (!_knownFeed)
            {
                _knownFeed = _reader.IsStartElement(Rss20Constants.RssTag, Rss20Constants.Rss20Namespace)
                                && _reader.GetAttribute(Rss20Constants.VersionTag).Equals(Rss20Constants.Version);

                //
                // Read <rss>
                if (_knownFeed) {
                    await _reader.ReadAsync();
                    _knownFeed = _reader.IsStartElement(Rss20Constants.ChannelTag, Rss20Constants.Rss20Namespace);
                }

                if (!_knownFeed)
                {
                    throw new XmlException("Unknown Feed");
                    //throw new XmlException(SR.GetString(SR.UnknownFeedXml, reader.LocalName, reader.NamespaceURI));
                }

                // Read <channel>
                await _reader.ReadAsync();
            }
        }
    }
}
