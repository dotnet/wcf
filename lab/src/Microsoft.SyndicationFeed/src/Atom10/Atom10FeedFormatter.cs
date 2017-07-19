// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    public class Atom10FeedFormatter : ISyndicationFeedFormatter
    {
        public ISyndicationCategory ParseCategory(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using(XmlReader reader = XmlReader.Create(new StringReader(value)))
            {
                reader.MoveToContent();
                return ParseCategory(reader);
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
            //if (type == typeof(DateTimeOffset))
            //{
            //    DateTimeOffset dt;
            //    if (DateTimeUtils.TryParse(value, out dt))
            //    {
            //        result = (T)(object)dt;
            //        return true;
            //    }
            //}

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
            return (result = (T)Convert.ChangeType(value, typeof(T))) != null;
        }

        private SyndicationPerson ParsePerson(XmlReader reader)
        {
            var person = new SyndicationPerson();

            person.RelationshipType = reader.Name;

            //Read inner elements of person
            reader.ReadStartElement();

            while (reader.IsStartElement())
            {
                switch (reader.LocalName)
                {
                    case Atom10Constants.NameTag:
                        person.Name = reader.ReadElementContentAsString();
                        break;

                    case Atom10Constants.EmailTag:
                        person.Email = reader.ReadElementContentAsString();
                        break;

                    case Atom10Constants.UriTag:
                        person.Uri = reader.ReadElementContentAsString();
                        break;
                }
            }

            reader.ReadEndElement(); //end of author / contributor

            return person;
        }

        private SyndicationImage ParseImage(XmlReader reader)
        {
            //
            // Atom Icon and Logo only contain one string with an Uri.
            string relationshipType = reader.Name;
            Uri uri = null;
            if (!TryParseValue(reader.ReadElementContentAsString(), out uri))
            {
                throw new FormatException("Invalid image url.");
            }

            SyndicationImage image = new SyndicationImage(uri);
            image.RelationshipType = relationshipType;
            return image;
        }

        private SyndicationCategory ParseCategory(XmlReader reader)
        {
            if (!reader.HasAttributes)
            {
                throw new FormatException("The category doesn't contain any attribute.");
            }

            string term = reader.GetAttribute("term");

            // term is required by the spec.
            if (string.IsNullOrEmpty(term))
            {
                throw new FormatException("The category doesn't contain term attribute.");
            }

            var category = new SyndicationCategory()
            {
                Name = term,
                Scheme = reader.GetAttribute("schme"),
                Label = reader.GetAttribute("label")
            };

            reader.Read();
            return category;
        }

        private SyndicationLink ParseLink(XmlReader reader)
        {

            Uri uri = null;
            long length = 0;
            string relationshipType = reader.Name;

            string lenghtRead = reader.GetAttribute("length");
            if (!string.IsNullOrEmpty(lenghtRead))
            {
                TryParseValue(lenghtRead, out length);
            }

            string href = reader.GetAttribute("href");
            if(string.IsNullOrEmpty(href))
            {
                throw new ArgumentNullException("The link does not contain href attribute.");
            }

            if (!TryParseValue(href ,out uri))
            {
                throw new FormatException("Unrecognized href format.");
            }



            SyndicationLink link = new SyndicationLink(uri)
            {
                Title = reader.GetAttribute("title"),
                Length = length,
                MediaType = reader.GetAttribute("type"),
                RelationshipType = reader.GetAttribute("rel")
            };
            reader.Read();
            return link;            
        }

        private SyndicationItem ParseItem(XmlReader reader)
        {
            SyndicationItem item = new SyndicationItem();

            bool isEmpty = reader.IsEmptyElement;

            if (!isEmpty)
            {
                FillItems(item, reader);
            }

            return item;
        }

        private void FillItems(SyndicationItem item, XmlReader reader)
        {
            var categories = new List<ISyndicationCategory>();
            var contributors = new List<ISyndicationPerson>();
            var links = new List<ISyndicationLink>();

            reader.ReadStartElement();

            string date;

            while (reader.IsStartElement())
            {
                switch (reader.LocalName)
                {
                    //
                    // Category
                    case Atom10Constants.CategoryTag:
                        SyndicationCategory category = ParseCategory(reader);
                        break;
                    //
                    // Content
                    case Atom10Constants.ContentTag:
                        reader.ReadOuterXml(); // Needs to be discussed.
                        break;

                    //
                    // Author/Contributor
                    case Atom10Constants.AuthorTag:
                    case Atom10Constants.ContributorTag:
                        SyndicationPerson person = ParsePerson(reader);
                        contributors.Add(person);
                        break;

                    //
                    // Id
                    case Atom10Constants.IdTag:
                        item.Id = reader.ReadElementContentAsString();
                        break;

                    //
                    // Link
                    case Atom10Constants.LinkTag:
                        SyndicationLink link = ParseLink(reader);
                        links.Add(link);
                        break;

                    //
                    // PublishedTag
                    case Atom10Constants.PublishedTag:
                        date = reader.ReadElementContentAsString();
                        //parse the date
                        break;

                    //
                    // Rights
                    case Atom10Constants.RightsTag:
                        reader.ReadOuterXml();
                        break;

                    //
                    // Source
                    case Atom10Constants.SourceTag:
                        reader.ReadOuterXml();
                        break;

                    //
                    // Summary
                    case Atom10Constants.SummaryTag:
                        item.Description = reader.ReadElementContentAsString();
                        break;

                    //
                    // Title
                    case Atom10Constants.TitleTag:
                        item.Title = reader.ReadElementContentAsString();
                        break;

                    //
                    // Updated
                    case Atom10Constants.UpdatedTag:
                        date = reader.ReadElementContentAsString();
                        //parse the date
                        break;

                    //
                    // Unrecognized tags
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.ReadEndElement(); // read end of <entry>

            item.Categories = categories;
            item.Links = links;
            item.Contributors = contributors;
        }
    }
}
