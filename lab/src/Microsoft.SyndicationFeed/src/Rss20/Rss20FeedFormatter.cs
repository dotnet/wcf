// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    public class Rss20FeedFormatter : ISyndicationFeedFormatter
    {
        public virtual ISyndicationCategory ParseCategory(string value)
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

        public virtual ISyndicationItem ParseItem(string value)
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

        public virtual ISyndicationLink ParseLink(string value)
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

        public virtual ISyndicationPerson ParsePerson(string value)
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

        public virtual ISyndicationImage ParseImage(string value)
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

        public virtual bool TryParseValue<T>(string value, out T result)
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
            // Todo being added in netstandard 2.0
            //if (type.GetTypeInfo().IsEnum)
            //{
            //    if (Enum.TryParse(typeof(T), value, true, out T o)) {
            //        result = (T)(object)o;
            //        return true;
            //    }
            //}

            //
            // Uri
            if (type == typeof(Uri))
            {
                Uri uri;
                if (UriUtils.TryParse(value, out uri))
                {
                    result = (T)(object)uri;
                    return true;
                }
            }

            //
            // Fall back default
            return (result = (T) Convert.ChangeType(value, typeof(T))) != null;
        }

        private ISyndicationImage ParseImage(XmlReader reader)
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
                            throw new FormatException("Invalid image url");
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
                Desciption = description,
                Link = link,
                RelationshipType = Rss20Constants.ImageTag
            };
        }

        private SyndicationLink ParseLink(XmlReader reader)
        {
            Uri uri = null;
            string title = string.Empty;
            long length = 0;
            string type = string.Empty;

            IEnumerable<SyndicationAttribute> attrs = XmlUtils.ReadAttributes(reader);

            //
            // Url
            SyndicationAttribute attrUrl = FindAttribute(attrs, "url");
            if (attrUrl != null)
            {
                TryParseValue(attrUrl.Value, out uri);
            }

            //
            // Length
            SyndicationAttribute attrLength = FindAttribute(attrs, "length");
            if (attrLength != null)
            {
                TryParseValue(attrLength.Value, out length);
            }

            //
            // Type
            SyndicationAttribute attrType = FindAttribute(attrs, "type");
            if (attrType != null)
            {
                type = attrType.Value;
            }


            reader.ReadStartElement();

            if (!reader.IsEmptyElement)
            {
                // Title
                title = reader.ReadContentAsString();

                // Url is the content, if not set as attribute
                if (uri == null && !string.IsNullOrEmpty(title))
                {
                    TryParseValue(title, out uri);
                }
            }

            reader.ReadEndElement();

            return new SyndicationLink(uri) {
                Title = title,
                Length = length,
                MediaType = type,
            };
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
                    bool isPermalink = (permalinkString == null) || permalinkString.Equals("false", StringComparison.OrdinalIgnoreCase);

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
                    if (!reader.IsEmptyElement)
                    {
                        DateTimeOffset dt;
                        if (TryParseValue(new SyndicationContent(reader.ReadOuterXml()).GetValue(), out dt))
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
        }


        private static SyndicationAttribute FindAttribute(IEnumerable<SyndicationAttribute> collection, string name, string ns = null)
        {
            if (ns != null)
            {
                return collection.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                                                      a.Namespace.Equals(XmlUtils.XmlNs, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                return collection.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}