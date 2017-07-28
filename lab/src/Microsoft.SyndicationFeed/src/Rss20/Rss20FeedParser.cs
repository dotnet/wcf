// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    public class Rss20FeedParser : ISyndicationFeedParser
    {
        public virtual ISyndicationCategory ParseCategory(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = XmlUtils.CreateXmlReader(value))
            {
                reader.MoveToContent();

                if(reader.Name != Rss20Constants.CategoryTag)
                {
                    throw new FormatException("Invalid Rss category");
                }

                return ParseCategory(reader);
            }
        }

        public virtual ISyndicationItem ParseItem(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = XmlUtils.CreateXmlReader(value))
            {
                reader.MoveToContent();

                if (reader.Name != Rss20Constants.ItemTag)
                {
                    throw new FormatException("Invalid Rss item");
                }

                return ParseItem(reader);
            }
        }

        public virtual ISyndicationLink ParseLink(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = XmlUtils.CreateXmlReader(value))
            {
                reader.MoveToContent();

                if(reader.Name != Rss20Constants.LinkTag)
                {
                    throw new FormatException("Invalid Rss Link");
                }

                return ParseLink(reader);
            }
        }

        public virtual ISyndicationPerson ParsePerson(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = XmlUtils.CreateXmlReader(value))
            {
                reader.MoveToContent();

                if(reader.Name != Rss20Constants.AuthorTag && reader.Name != Rss20Constants.ManagingEditorTag)
                {
                    throw new FormatException("Invalid Rss Person");
                }

                return ParsePerson(reader);
            }
        }

        public virtual ISyndicationImage ParseImage(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = XmlUtils.CreateXmlReader(value))
            {
                reader.MoveToContent();

                if(reader.Name != Rss20Constants.ImageTag)
                {
                    throw new FormatException("Invalid Rss Image");
                }

                return ParseImage(reader);
            }
        }

        public virtual ISyndicationContent ParseContent(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = XmlUtils.CreateXmlReader(value))
            {
                reader.MoveToContent();

                return XmlUtils.ReadXmlNode(reader);
            }
        }


        public virtual bool TryParseValue<T>(string value, out T result)
        {
            return Converter.TryParseValue<T>(value, out result);
        }

        private SyndicationImage ParseImage(XmlReader reader)
        {
            string title = string.Empty;
            string description = string.Empty;
            Uri url = null;
            ISyndicationLink link = null;

            if (!reader.IsEmptyElement)
            {
                reader.ReadStartElement();

                while (reader.IsStartElement())
                {
                    //
                    // Url
                    if (reader.IsStartElement(Rss20Constants.UrlTag, Rss20Constants.Rss20Namespace))
                    {
                        if (!TryParseValue(reader.ReadElementContentAsString(), out url)) 
                        {
                            throw new FormatException("Invalid image url.");
                        }
                    }

                    //
                    // Title
                    if(reader.IsStartElement(Rss20Constants.TitleTag, Rss20Constants.Rss20Namespace))
                    {
                        title = reader.ReadElementContentAsString();
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
                        description = reader.ReadElementContentAsString();
                    }
                }

                reader.ReadEndElement(); //image end
            }

            if (url == null)
            {
                throw new FormatException("Invalid image url");
            }

            return new SyndicationImage(url) {
                Title = title,
                Description = description,
                Link = link,
                RelationshipType = Rss20Constants.ImageTag
            };
        }

        private SyndicationLink ParseLink(XmlReader reader)
        {

            //
            // Url
            Uri uri = null;
            string url = reader.GetAttribute("url");
            if (url != null)
            {
                if (!TryParseValue(url,out uri))
                {
                    throw new FormatException("Invalid url attribute format.");
                }
            }

            //
            // Length
            long length = 0;
            TryParseValue(reader.GetAttribute("length"), out length);

            //
            // Type
            string type = string.Empty;
            TryParseValue(reader.GetAttribute("type"), out type);
            

            //
            // Title
            string title = string.Empty;
            if (!reader.IsEmptyElement)
            {
                title = reader.ReadElementContentAsString();

                // Url is the content, if not set as attribute
                if (uri == null && !string.IsNullOrEmpty(title))
                {
                    TryParseValue(title, out uri);
                }
            }
            else
            {
                reader.Skip();
            }

            return new SyndicationLink(uri) {
                Title = title,
                Length = length,
                MediaType = type,
            };
        }

        private SyndicationCategory ParseCategory(XmlReader reader)
        {
            var category = new SyndicationCategory();

            category.Name = reader.ReadElementContentAsString();

            return category;
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

        private SyndicationItem ParseItem(XmlReader reader)
        {
            SyndicationItem item = new SyndicationItem();

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
                    item.Title = reader.ReadElementContentAsString();
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
                    item.Description = reader.ReadElementContentAsString();
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
                    bool isPermalink = (permalinkString != null) && permalinkString.Equals("true", StringComparison.OrdinalIgnoreCase);

                    item.Id = reader.ReadElementContentAsString();

                    if (isPermalink)
                    {
                        fallbackAlternateLink = item.Id;
                    }
                }
                //
                // PubDate
                else if (reader.IsStartElement(Rss20Constants.PubDateTag, Rss20Constants.Rss20Namespace))
                {
                    if (TryParseValue(reader.ReadElementContentAsString(), out DateTimeOffset dt))
                    {
                        item.Published = dt;
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
                Uri url;
                if (TryParseValue(fallbackAlternateLink, out url))
                {
                    var link = new SyndicationLink(url);
                    links.Add(link);
                }
            }

            item.Links = links;
            item.Contributors = contributors;
            item.Categories = categories;

            return item;
        }
    }
}