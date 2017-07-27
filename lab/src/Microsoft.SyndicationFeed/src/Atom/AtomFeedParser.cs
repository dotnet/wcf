// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    public class AtomFeedParser : ISyndicationFeedParser
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

                if (reader.Name != AtomConstants.CategoryTag)
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

            using (XmlReader reader = XmlUtils.CreateXmlReader(value))
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

            using (XmlReader reader = XmlUtils.CreateXmlReader(value))
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

            using (XmlReader reader = XmlUtils.CreateXmlReader(value))
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

            using (XmlReader reader = XmlUtils.CreateXmlReader(value))
            {
                reader.MoveToContent();

                if (reader.Name != AtomConstants.AuthorTag && reader.Name != AtomConstants.ContributorTag)
                {
                    throw new FormatException("Invalid Atom person");
                }

                return ParsePerson(reader);
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
                throw new FormatException("Invalid image url");
            }

            return new SyndicationImage(uri)
            {
                RelationshipType = relationshipType
            };
        }

        private SyndicationCategory ParseCategory(XmlReader reader)
        {       
            string term = reader.GetAttribute("term");

            // term is required by the spec.
            if (string.IsNullOrEmpty(term))
            {
                throw new FormatException("Required attribute 'term'");
            }

            string scheme = reader.GetAttribute("scheme");
            string label = reader.GetAttribute("label");

            reader.Skip();

            return new SyndicationCategory()
            {
                Name = term,
                Scheme = scheme,
                Label = label
            };
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

            // type
            string rel = reader.GetAttribute("rel") ?? AtomConstants.AlternateTag;

            Uri uri = null;

            // href
            if (uri == null)
            {
                TryParseValue(reader.GetAttribute("href"), out uri);
            }

            // src
            if (uri == null)
            {
                TryParseValue(reader.GetAttribute("src"), out uri);
            }

            if (uri == null)
            {
                throw new FormatException("Invalid uri");
            }

            reader.Skip();

            return new SyndicationLink(uri)
            {
                Title = title,
                Length = length,
                MediaType = type,
                RelationshipType = rel 
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

            while (reader.IsStartElement())
            {

                //
                // Category
                if (reader.IsStartElement(AtomConstants.CategoryTag, AtomConstants.Atom10Namespace))
                {
                    categories.Add(ParseCategory(reader));
                }


                //
                // Content
                else if(reader.IsStartElement(AtomConstants.ContentTag, AtomConstants.Atom10Namespace))
                {
                    item.ContentType = reader.GetAttribute(AtomConstants.TypeTag) ?? AtomConstants.PlaintextType;

                    if (string.IsNullOrEmpty(reader.GetAttribute(AtomConstants.SourceTag)))
                    {
                        item.Description = reader.ReadInnerXml();
                    }
                    else
                    {
                        SyndicationLink src = ParseLink(reader);
                        src.RelationshipType = AtomConstants.ContentTag;
                        links.Add(src);
                    }
                }

                //
                // Author/Contributor
                else if(reader.IsStartElement(AtomConstants.AuthorTag, AtomConstants.Atom10Namespace) || reader.IsStartElement(AtomConstants.ContributorTag, AtomConstants.Atom10Namespace))
                {
                    contributors.Add(ParsePerson(reader));
                }

                //
                // Id
                else if(reader.IsStartElement(AtomConstants.IdTag, AtomConstants.Atom10Namespace))
                {
                    item.Id = reader.ReadElementContentAsString();
                }

                //
                // Link
                else if(reader.IsStartElement(AtomConstants.LinkTag, AtomConstants.Atom10Namespace))
                {
                    links.Add(ParseLink(reader));
                }

                //
                // Published
                else if(reader.IsStartElement(AtomConstants.PublishedTag, AtomConstants.Atom10Namespace))
                {
                    if (TryParseValue(reader.ReadElementContentAsString(), out DateTimeOffset published))
                    {
                        item.Published = published;
                    }
                }

                //
                // Rights
                else if(reader.IsStartElement(AtomConstants.RightsTag, AtomConstants.Atom10Namespace))
                {
                    item.Rights = reader.ReadElementContentAsString();
                }

                //
                // Source
                else if(reader.IsStartElement(AtomConstants.SourceFeedTag, AtomConstants.Atom10Namespace))
                {
                    links.Add(ParseSource(reader));
                }

                //
                // Summary
                else if(reader.IsStartElement(AtomConstants.SummaryTag, AtomConstants.Atom10Namespace))
                {
                    item.Summary = reader.ReadElementContentAsString();
                }

                //
                // Title
                else if (reader.IsStartElement(AtomConstants.TitleTag, AtomConstants.Atom10Namespace))
                {
                    item.Title = reader.ReadElementContentAsString();
                }

                //
                // Updated
                else if(reader.IsStartElement(AtomConstants.UpdatedTag, AtomConstants.Atom10Namespace))
                {
                    if (TryParseValue(reader.ReadElementContentAsString(), out DateTimeOffset updated))
                    {
                        item.LastUpdated = updated;
                    }
                }

                //
                // Unrecognized tags
                else
                {
                    reader.Skip();
                }
            }

            reader.ReadEndElement(); // read end of <entry>

            item.Categories = categories;
            item.Links = links;
            item.Contributors = contributors;
        }

        private ISyndicationLink ParseSource(XmlReader reader)
        {
            Uri url = null;
            string title = null;
            DateTimeOffset lastUpdated;

            reader.ReadStartElement();

            while (reader.IsStartElement())
            {
                switch (reader.LocalName)
                {
                    case AtomConstants.IdTag:
                        if (url == null)
                        {
                            TryParseValue(reader.ReadElementContentAsString(), out url);
                        }
                        else
                        {
                            reader.Skip();
                        }
                        break;

                    case AtomConstants.TitleTag:
                        title = reader.ReadElementContentAsString();
                        break;

                    case AtomConstants.UpdatedTag:
                        TryParseValue(reader.ReadElementContentAsString(), out lastUpdated);
                        break;

                    case AtomConstants.LinkTag:
                        if (url == null)
                        {
                            url = ParseLink(reader).Uri;
                        }
                        else
                        {
                            reader.Skip();
                        }
                        break;

                    //
                    // Unrecognized tags
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.ReadEndElement(); // end of source tag

            if (url == null)
            {
                throw new FormatException("Invalid source link");
            }

            return new SyndicationLink(url)
            {
                Title = title,
                RelationshipType = AtomConstants.SourceFeedTag,
                LastUpdated = lastUpdated
            };
        }
    }
}
