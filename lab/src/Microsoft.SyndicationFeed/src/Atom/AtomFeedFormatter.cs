// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    public class AtomFeedFormatter : ISyndicationFeedFormatter
    {
        public virtual ISyndicationCategory ParseCategory(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using(XmlReader reader = CreateXmlReader(value))
            {
                reader.MoveToContent();

                if(reader.Name != AtomConstants.CategoryTag)
                {
                    throw new FormatException("Invalid Atom category");
                }

                return ParseCategory(reader);
            }
        }

        public virtual ISyndicationImage ParseImage(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = CreateXmlReader(value))
            {
                reader.MoveToContent();
                if (reader.Name != AtomConstants.IconTag && reader.Name != AtomConstants.LogoTag)
                {
                    throw new FormatException("Invalid Atom image");
                }

                return ParseImage(reader);
            }
        }

        public virtual ISyndicationItem ParseItem(string value)
        {
            return ParseEntry(value);
        }

        public virtual IAtomEntry ParseEntry(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = CreateXmlReader(value))
            {
                reader.MoveToContent();

                if (reader.Name != AtomConstants.EntryTag)
                {
                    throw new FormatException("Invalid Atom entry");
                }

                return ParseEntry(reader);
            }
        }

        public virtual ISyndicationLink ParseLink(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = CreateXmlReader(value))
            {
                reader.MoveToContent();

                if (reader.Name != AtomConstants.LinkTag)
                {
                    throw new FormatException("Invalid Atom link");
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

            using (XmlReader reader = CreateXmlReader(value))
            {
                reader.MoveToContent();
                if (reader.Name != AtomConstants.AuthorTag && reader.Name != AtomConstants.ContributorTag)
                {
                    throw new FormatException("Invalid Atom person");
                }

                return ParsePerson(reader);
            }
        }

        public virtual bool TryParseValue<T>(string value, out T result)
        {
            result = default(T);

            if (value == null)
            {
                return false;
            }

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
                if (TryParseAtomDate(value, out dt))
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
                    case AtomConstants.NameTag:
                        person.Name = reader.ReadElementContentAsString();
                        break;

                    case AtomConstants.EmailTag:
                        person.Email = reader.ReadElementContentAsString();
                        break;

                    case AtomConstants.UriTag:
                        person.Uri = reader.ReadElementContentAsString();
                        break;

                    default:
                        reader.Skip();
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
            //
            // title
            string title = reader.GetAttribute("title");

            // type
            string type = reader.GetAttribute("type");

            // length
            long length = 0;
            TryParseValue(reader.GetAttribute("length"), out length);

            // href
            Uri uri = null;
            if (!TryParseValue(reader.GetAttribute("href"), out uri))
            {
                throw new FormatException("Invalid href attribute format");
            }

            // type
            string rel = reader.GetAttribute("rel");

            reader.Read(); // end

            return new SyndicationLink(uri)
            {
                Title = title,
                Length = length,
                MediaType = type,
                RelationshipType = rel ?? AtomConstants.AlternateTag
            };
        }

        private AtomEntry ParseEntry(XmlReader reader)
        {
            var item = new AtomEntry();

            bool isEmpty = reader.IsEmptyElement;

            if (!isEmpty)
            {
                FillItem(item, reader);
            }

            return item;
        }

        private void FillItem(AtomEntry item, XmlReader reader)
        {
            var categories = new List<ISyndicationCategory>();
            var contributors = new List<ISyndicationPerson>();
            var links = new List<ISyndicationLink>();

            reader.ReadStartElement();

            string date;
            DateTimeOffset dto;

            while (reader.IsStartElement())
            {
                switch (reader.LocalName)
                {
                    //
                    // Category
                    case AtomConstants.CategoryTag:
                        SyndicationCategory category = ParseCategory(reader);
                        break;
                    //
                    // Content
                    case AtomConstants.ContentTag:
                        item.Description = reader.ReadInnerXml();
                        break;

                    //
                    // Author/Contributor
                    case AtomConstants.AuthorTag:
                    case AtomConstants.ContributorTag:
                        SyndicationPerson person = ParsePerson(reader);
                        contributors.Add(person);
                        break;

                    //
                    // Id
                    case AtomConstants.IdTag:
                        item.Id = reader.ReadElementContentAsString();
                        break;

                    //
                    // Link
                    case AtomConstants.LinkTag:
                        SyndicationLink link = ParseLink(reader);
                        links.Add(link);
                        break;

                    //
                    // PublishedTag
                    case AtomConstants.PublishedTag:
                        date = reader.ReadElementContentAsString();
                        TryParseAtomDate(date, out dto);
                        item.Published = dto;
                        //parse the date
                        break;

                    //
                    // Rights
                    case AtomConstants.RightsTag:
                        item.Rights = reader.ReadElementContentAsString();
                        break;

                    //
                    // Source
                    case AtomConstants.SourceFeedTag:

                        links.Add(ParseSource(reader));

                        break;

                    //
                    // Summary
                    case AtomConstants.SummaryTag:
                        item.Summary = reader.ReadElementContentAsString();
                        break;

                    //
                    // Title
                    case AtomConstants.TitleTag:
                        item.Title = reader.ReadElementContentAsString();
                        break;

                    //
                    // Updated
                    case AtomConstants.UpdatedTag:
                        date = reader.ReadElementContentAsString();                        
                        TryParseAtomDate(date, out dto);
                        item.LastUpdated = dto;
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

        private SyndicationLink ParseSource(XmlReader reader)
        {
            SyndicationLink source = null;
            Uri id = null;
            string title = null;
            string updated = null;
            string relationship = reader.LocalName;

            reader.ReadStartElement();
            while (reader.IsStartElement())
            {
                switch (reader.LocalName)
                {
                    case AtomConstants.IdTag:
                        string url = reader.ReadElementContentAsString();
                        if (TryParseValue(url, out id))
                        {
                            source = new SyndicationLink(id);
                        }

                        break;

                    case AtomConstants.TitleTag:
                        title = reader.ReadElementContentAsString();
                        break;

                    case AtomConstants.UpdatedTag:
                        updated = reader.ReadElementContentAsString();
                        break;

                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.ReadEndElement(); // end of source tag

            if (source != null)
            {
                source.Title = title;
                source.RelationshipType = relationship;
                DateTimeOffset sourceDate;
                if (TryParseValue(updated, out sourceDate))
                {
                    source.LastUpdated = sourceDate;
                }
            }

            return source;
        }

        private XmlReader CreateXmlReader(string value)
        {
            return XmlReader.Create(new StringReader(value),
                                    new XmlReaderSettings()
                                    {
                                        IgnoreProcessingInstructions = true
                                    });
        }

        private bool TryParseAtomDate(string dateTimeString, out DateTimeOffset result)
        {
            const string Rfc3339LocalDateTimeFormat = "yyyy-MM-ddTHH:mm:sszzz";
            const string Rfc3339UTCDateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";

            dateTimeString = dateTimeString.Trim();

            if (dateTimeString[19] == '.')
            {
                // remove any fractional seconds, we choose to ignore them
                int i = 20;
                while (dateTimeString.Length > i && char.IsDigit(dateTimeString[i]))
                {
                    ++i;
                }
                dateTimeString = dateTimeString.Substring(0, 19) + dateTimeString.Substring(i);
            }

            DateTimeOffset localTime;
            if (DateTimeOffset.TryParseExact(dateTimeString, Rfc3339LocalDateTimeFormat,
                CultureInfo.InvariantCulture.DateTimeFormat,
                DateTimeStyles.None, out localTime))
            {
                result = localTime;
                return true;
            }
            DateTimeOffset utcTime;
            if (DateTimeOffset.TryParseExact(dateTimeString, Rfc3339UTCDateTimeFormat,
                CultureInfo.InvariantCulture.DateTimeFormat,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out utcTime))
            {
                result = utcTime;
                return true;
            }
            return false;
        }
    }
}
