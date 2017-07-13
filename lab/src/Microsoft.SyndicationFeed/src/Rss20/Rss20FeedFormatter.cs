// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    public class Rss20FeedFormatter : ISyndicationFeedFormatter
    {
        public ISyndicationCategory ParseCategory(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = XmlReader.Create(new StringReader(value)))
            {
                reader.MoveToContent();
                return ParseCategory(reader);
            }
        }

        public ISyndicationContent ParseContent(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = XmlReader.Create(new StringReader(value)))
            {
                reader.MoveToContent();
                return ParseContent(reader);
            }
        }

        public ISyndicationItem ParseItem(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = XmlReader.Create(new StringReader(value)))
            { 
                reader.MoveToContent();
                return ParseItem(reader);
            }
        }

        public ISyndicationLink ParseLink(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = XmlReader.Create(new StringReader(value)))
            {
                reader.MoveToContent();
                return ParseLink(reader);
            }
        }

        public ISyndicationPerson ParsePerson(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = XmlReader.Create(new StringReader(value)))
            {
                reader.MoveToContent();
                return ParsePerson(reader);
            }
        }

        public ISyndicationImage ParseImage(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = XmlReader.Create(new StringReader(value)))
            {
                reader.MoveToContent();
                return ParseImage(reader);
            }
        }

        public bool TryParseValue<T>(string value, out T result)
        {
            result = default(T);

            Type type = typeof(T);

            //
            // String
            if (type == typeof(string))
            {
                result = (T)(object)value;
                return true;
            }

            //
            // DateTimeOffset
            if (type == typeof(DateTimeOffset))
            {
                DateTimeOffset dt;
                if (DateTimeUtils.TryParse(value, out dt))
                {
                    result = (T)(object)dt;
                    return true;
                }
            }

            //
            // Enum
            if (type.IsEnum)
            {
                object o;
                if (Enum.TryParse(typeof(T), value, true, out o))
                {
                    result = (T)o;
                    return true;
                }
            }

            //
            // Uri
            if (type == typeof(Uri))
            {
                Uri uri;
                if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out uri))
                {
                    result = (T)(object)uri;
                    return true;
                }
            }

            //
            // Fall back default
            return (result = (T) Convert.ChangeType(value, typeof(T))) != null;
        }


        public ISyndicationImage ParseImage(XmlReader reader)
        {
            SyndicationImage image = null;

            if (!reader.IsEmptyElement)
            {
                reader.ReadStartElement();

                string title = null;
                Uri url = null;
                ISyndicationLink link = null;
                string description = null;

                while (reader.IsStartElement())
                {
                    //
                    // Url
                    if (reader.IsStartElement(Rss20Constants.UrlTag, Rss20Constants.Rss20Namespace))
                    {
                        string uri = reader.ReadElementString();
                        if(!TryParseValue(uri, out url)) 
                        {
                            //Image parse failed
                            throw new ArgumentException("The image can't be constructed with an invalid url");
                        }
                    }

                    //
                    // Title
                    if(reader.IsStartElement(Rss20Constants.TitleTag, Rss20Constants.Rss20Namespace))
                    {
                        title = reader.ReadElementString();
                    }

                    //
                    // Link
                    if (reader.IsStartElement(Rss20Constants.LinkTag, Rss20Constants.Rss20Namespace))
                    {
                        link = ParseLink(reader.ReadOuterXml());
                    }

                    //
                    // Description
                    if (reader.IsStartElement(Rss20Constants.DescriptionTag, Rss20Constants.Rss20Namespace))
                    {
                        description = reader.ReadElementString();
                    }
                }

                reader.ReadEndElement(); //image end
                
                image = new SyndicationImage(url);
                image.Desciption = description;
                image.RelationshipType = Rss20Constants.ImageTag;
                image.Title = title;
                image.Link = link;
            }

            return image;
        }
        
        public IEnumerable<ISyndicationContent> ParseChildren(string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            var items = new List<ISyndicationContent>();

            if (!string.IsNullOrEmpty(content))
            {
                XmlReader reader = XmlReader.Create(new StringReader(content));
                reader.ReadStartElement();

                while (reader.IsStartElement())
                {
                    items.Add(ParseContent(reader));
                }
            }

            return items;
        }

        private SyndicationLink ParseLink(XmlReader reader)
        {
            var link = new SyndicationLink();

            IEnumerable<SyndicationAttribute> attrs = XmlUtils.ReadAttributes(reader);

            //
            // Url
            SyndicationAttribute attrUrl = FindAttribute(attrs, "url");
            if (attrUrl != null)
            {
                Uri uri;
                if (TryParseValue(attrUrl.Value, out uri))
                {
                    link.Uri = uri;
                }
            }

            //
            // Length
            SyndicationAttribute attrLength = FindAttribute(attrs, "length");
            if (attrLength != null)
            {
                int length;
                if (TryParseValue(attrLength.Value, out length))
                {
                    link.Length = length;
                }
            }

            //
            // Type
            SyndicationAttribute attrType = FindAttribute(attrs, "type");
            if (attrType != null)
            {
                link.MediaType = attrType.Value;
            }


            reader.ReadStartElement();

            if (!reader.IsEmptyElement)
            {
                // Title
                link.Title = reader.ReadContentAsString();

                // Url is the content, if not set as attribute
                Uri uri;
                if (link.Uri == null && 
                    !string.IsNullOrEmpty(link.Title) &&
                    TryParseValue(link.Title, out uri))
                {
                    link.Uri = uri;
                }
            }
            
            return link;
        }

        private static ISyndicationCategory ParseCategory(XmlReader reader)
        {
            var category = new SyndicationCategory();

            if (!reader.IsEmptyElement)
            {
                category.Name = reader.ReadElementContentAsString();
            }

            return category;
        }

        private SyndicationItem ParseItem(XmlReader reader)
        {
            var item = new SyndicationItem();

            bool isEmpty = reader.IsEmptyElement;

            if (!isEmpty)
            {
                FillItem(item, reader);
            }

            return item;
        }

        private SyndicationPerson ParsePerson(XmlReader reader)
        {
            var person = new SyndicationPerson();

            if (!reader.IsEmptyElement)
            {
                person.Email = reader.ReadElementContentAsString();
            }

            return person;
        }

        private SyndicationContent ParseContent(XmlReader reader)
        {
            return new SyndicationContent(reader.Name, 
                                          reader.IsStartElement() ? reader.ReadOuterXml() : string.Empty);
        }


        private void FillItem(SyndicationItem item, XmlReader reader)
        {
            string fallbackAlternateLink = null;
            bool readAlternateLink = false;

            var links = new List<ISyndicationLink>();
            var contributors = new List<ISyndicationPerson>();
            var categories = new List<ISyndicationCategory>();

            reader.ReadStartElement();

            while (reader.IsStartElement())
            {
                //
                // Title
                if (reader.IsStartElement(Rss20Constants.TitleTag, Rss20Constants.Rss20Namespace))
                {
                    item.Title = reader.ReadElementString();
                }
                //
                // Link
                else if (reader.IsStartElement(Rss20Constants.LinkTag, Rss20Constants.Rss20Namespace))
                {
                    SyndicationLink link = ParseLink(reader);
                    link.RelationshipType = Rss20Constants.AlternateLink;

                    links.Add(link);
                    readAlternateLink = true;
                }
                //
                // Description
                else if (reader.IsStartElement(Rss20Constants.DescriptionTag, Rss20Constants.Rss20Namespace))
                {
                    item.Description = reader.ReadElementString();
                }
                //
                // Author
                else if (reader.IsStartElement(Rss20Constants.AuthorTag, Rss20Constants.Rss20Namespace))
                {
                    SyndicationPerson person = ParsePerson(reader);
                    person.RelationshipType = Rss20Constants.AuthorTag;

                    contributors.Add(person);
                }
                //
                // Category
                else if (reader.IsStartElement(Rss20Constants.CategoryTag, Rss20Constants.Rss20Namespace))
                {
                    categories.Add(ParseCategory(reader));
                }
                //
                // Comments
                else if (reader.IsStartElement(Rss20Constants.CommentsTag, Rss20Constants.Rss20Namespace))
                {
                    SyndicationLink link = ParseLink(reader);
                    link.RelationshipType = Rss20Constants.CommentsTag;
                    links.Add(link);
                }
                //
                // Enclosure
                else if (reader.IsStartElement(Rss20Constants.EnclosureTag, Rss20Constants.Rss20Namespace))
                {
                    SyndicationLink link = ParseLink(reader);
                    link.RelationshipType = Rss20Constants.EnclosureTag;
                    links.Add(link);
                }
                //
                // Guid
                else if (reader.IsStartElement(Rss20Constants.GuidTag, Rss20Constants.Rss20Namespace))
                {
                    string permalinkString = reader.GetAttribute(Rss20Constants.IsPermaLinkTag, Rss20Constants.Rss20Namespace);
                    bool isPermalink = (permalinkString == null) || permalinkString.Equals("false", StringComparison.OrdinalIgnoreCase);

                    item.Id = reader.ReadElementString();

                    if (isPermalink)
                    {
                        fallbackAlternateLink = item.Id;
                    }
                }
                //
                // PubDate
                else if (reader.IsStartElement(Rss20Constants.PubDateTag, Rss20Constants.Rss20Namespace))
                {
                    if (!reader.IsEmptyElement)
                    {
                        DateTimeOffset dt;
                        if (TryParseValue(ParseContent(reader).GetValue(), out dt))
                        {
                            item.Published = dt;
                        }
                    }
                }
                //
                // Source
                else if (reader.IsStartElement(Rss20Constants.SourceTag, Rss20Constants.Rss20Namespace))
                {
                    SyndicationLink link = ParseLink(reader);
                    link.RelationshipType = Rss20Constants.SourceTag;
                    links.Add(link);
                }
                else
                {
                    // Skip Unknown tags
                    reader.Skip();
                }
            }

            reader.ReadEndElement(); // end

            // Add a fallback Link
            if (!readAlternateLink && fallbackAlternateLink != null)
            {
                var link = new SyndicationLink();
                link.Uri = new Uri(fallbackAlternateLink, UriKind.RelativeOrAbsolute);

                links.Add(link);
                readAlternateLink = true;
            }

            item.Links = links;
            item.Contributors = contributors;
            item.Categories = categories;
        }


        private static SyndicationAttribute FindAttribute(IEnumerable<SyndicationAttribute> collection, string name, string ns = null)
        {
            if (ns != null)
            {
                return collection.FirstOrDefault(a => a.Name.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                                                      a.Name.Namespace.Equals(XmlUtils.XmlNs, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                return collection.FirstOrDefault(a => a.Name.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}