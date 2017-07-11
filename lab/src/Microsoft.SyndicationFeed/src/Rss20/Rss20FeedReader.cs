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
        private bool _currentSet;

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

            string xml = await _reader.ReadOuterXmlAsync();

            ISyndicationCategory category = _formatter.ParseCategory(xml);

            await MoveNext();

            return category;
        }

        public async Task<ISyndicationContent> ReadContent()
        {
            string xml = await _reader.ReadOuterXmlAsync();
            
            ISyndicationContent content = _formatter.ParseContent(xml);

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

            ISyndicationItem item = _formatter.ParseItem(xml);

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

            ISyndicationLink link = _formatter.ParseLink(xml);

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

            ISyndicationPerson person = _formatter.ParsePerson(content);

            await MoveNext();

            return person;
        }

        public async Task<T> ReadValue<T>()
        {

            string value = await _reader.ReadElementContentAsStringAsync();

            await MoveNext();

            return _formatter.ParseValue<T>(value);
        }

        public Task Skip()
        {
            return _reader.SkipAsync();
            //throw new NotImplementedException();
        }

        private async Task<bool> MoveNext(bool setCurrent = true)
        {

            await EnsureRead();

            while (await _reader.ReadAsync()) {
                switch (_reader.NodeType) {
                    case XmlNodeType.Element:
                        ElementType = GetElementType();
                        ElementName = _reader.Name;
                        _currentSet = setCurrent;
                        return true;

                    default:
                        // Keep reading
                        break;
                }
            }

            //
            // Reset
            ElementType = SyndicationElementType.None;
            ElementName = null;
            _currentSet = false;

            return false;
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
            }
        }

        private void ResetCurrent()
        {
            ElementType = SyndicationElementType.None;
            ElementName = null;
        }
    }
}
