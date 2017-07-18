// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    public abstract class SyndicationFeedReaderBase : ISyndicationFeedReader
    {
        private readonly XmlReader _reader;
        private bool _currentSet;
        

        public SyndicationFeedReaderBase(XmlReader reader, ISyndicationFeedFormatter formatter)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));

            ElementType = SyndicationElementType.None;
        }

        public ISyndicationFeedFormatter Formatter { get; private set; }

        public SyndicationElementType ElementType { get; private set; }

        public string ElementName { get; private set; }

        public virtual async Task<bool> Read()
        {
            if (_currentSet) {
                //
                // The reader is already advanced, return status
                _currentSet = false;
                return !_reader.EOF;
            }
            else {
                if (ElementType != SyndicationElementType.None)
                {
                    await Skip();
                    return !_reader.EOF;
                }
                else
                {
                    //
                    // Advance the reader
                    return await MoveNext(false);
                }
            }
        }

        public virtual async Task<ISyndicationCategory> ReadCategory()
        {
            if (ElementType == SyndicationElementType.None)
            {
                await Read();
            }

            if (ElementType != SyndicationElementType.Category)
            {
                throw new XmlException("Unknown Category");
            }

            ISyndicationContent content = await ReadContent();

            return Formatter.ParseCategory(content.RawContent);
        }

        public virtual async Task<ISyndicationContent> ReadContent()
        {
            if (ElementType == SyndicationElementType.None)
            {
                await Read();
            }

            //
            // Any element can be read as ISyndicationContent
            if (ElementType == SyndicationElementType.None)
            {
                throw new XmlException("Unknown Content");
            }

            ISyndicationContent content = new SyndicationContent(await XmlUtils.ReadOuterXmlAsync(_reader));

            await MoveNext();

            return content;
        }

        public virtual async Task<ISyndicationItem> ReadItem()
        {
            if (ElementType == SyndicationElementType.None)
            {
                await Read();
            }

            if (ElementType != SyndicationElementType.Item)
            {
                throw new XmlException("Unknown Item");
            }

            ISyndicationContent content = await ReadContent();

            return Formatter.ParseItem(content.RawContent);
        }

        public virtual async Task<ISyndicationLink> ReadLink()
        {
            if (ElementType == SyndicationElementType.None)
            {
                await Read();
            }

            if (ElementType != SyndicationElementType.Link)
            {
                throw new XmlException("Unknown Link");
            }

            ISyndicationContent content = await ReadContent();

            return Formatter.ParseLink(content.RawContent);
        }

        public virtual async Task<ISyndicationPerson> ReadPerson()
        {
            if (ElementType == SyndicationElementType.None)
            {
                await Read();
            }

            if (ElementType != SyndicationElementType.Person)
            {
                throw new XmlException("Unknown Person");
            }

            ISyndicationContent content = await ReadContent();

            return Formatter.ParsePerson(content.RawContent);
        }

        public virtual async Task<ISyndicationImage> ReadImage()
        {
            if (ElementType == SyndicationElementType.None)
            {
                await Read();
            }

            if (ElementType != SyndicationElementType.Image)
            {
                throw new XmlException("Unknown Image");
            }

            ISyndicationContent content = await ReadContent();

            return Formatter.ParseImage(content.RawContent);
        }


        public virtual async Task<T> ReadValue<T>()
        {
            ISyndicationContent content = await ReadContent();

            if (!Formatter.TryParseValue(content.GetValue(), out T value))
            {
                throw new FormatException();
            }

            return value;
        }

        public virtual async Task Skip()
        {
            await XmlUtils.SkipAsync(_reader);
            await MoveNext(false);
        }

        private async Task<bool> MoveNext(bool setCurrent = true)
        {
            do
            {
                if (_reader.NodeType == XmlNodeType.Element)
                {
                    ElementType = MapElementType(_reader.Name);
                    ElementName = _reader.Name;

                    _currentSet = setCurrent;

                    return true;
                }
            }
            while (await XmlUtils.ReadAsync(_reader));

            //
            // Reset
            ElementType = SyndicationElementType.None;
            ElementName = null;
            _currentSet = false;

            return false;
        }

        protected abstract SyndicationElementType MapElementType(string elementName);
    }
}