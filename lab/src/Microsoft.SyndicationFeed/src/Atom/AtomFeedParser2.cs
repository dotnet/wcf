// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.SyndicationFeed.Atom
{
    public class AtomFeedParser2 : ISyndicationFeedParser
    {
        public ISyndicationCategory ParseCategory(string value)
        {
            ISyndicationContent content = ParseContent(value);

            if (content.Name != AtomConstants.CategoryTag)
            {
                throw new FormatException("Invalid Atom Category");
            }

            return CreateCategory(content);
        }
        
        public ISyndicationImage ParseImage(string value)
        {
            ISyndicationContent content = ParseContent(value);

            if (content.Name != AtomConstants.LogoTag && content.Name != AtomConstants.IconTag)
            {
                throw new FormatException("Invalid Atom Image");
            }

            return CreateImage(content);
        }
        
        public ISyndicationItem ParseItem(string value)
        {
            return ParseEntry(value);
        }

        public IAtomEntry ParseEntry(string value)
        {
            ISyndicationContent content = ParseContent(value);

            if (content.Name != AtomConstants.EntryTag)
            {
                throw new FormatException("Invalid Atom feed");
            }

            return CreateItem(content);
        }
        
        public ISyndicationLink ParseLink(string value)
        {
            ISyndicationContent content = ParseContent(value);

            if (content.Name != AtomConstants.LinkTag)
            {
                throw new FormatException("Invalid Atom Link");
            }

            return CreateLink(content);
        }

        public ISyndicationPerson ParsePerson(string value)
        {
            ISyndicationContent content = ParseContent(value);

            if (content.Name != AtomConstants.AuthorTag && content.Name != AtomConstants.ContributorTag)
            {
                throw new FormatException("Invalid Atom person");
            }

            return CreatePerson(content);
        }

        public ISyndicationContent ParseContent(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = CreateXmlReader(value))
            {
                reader.MoveToContent();

                return XmlUtils.ReadSyndicationContent(reader);
            }
        }

        private XmlReader CreateXmlReader(string value)
        {
            return XmlUtils.CreateXmlReader(value);
        }

        public virtual bool TryParseValue<T>(string value, out T result)
        {
            return Converter.TryParseValue<T>(value, out result);
        }
        
        protected virtual ISyndicationCategory CreateCategory(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }
            
            if (content.Attributes.GetAtom(AtomConstants.TermTag) == null)
            {
                throw new FormatException("Invalid Atom category, requires Term attribute");
            }

            SyndicationCategory category = new SyndicationCategory(content.Attributes.GetAtom(AtomConstants.TermTag));

            category.Scheme = content.Attributes.GetAtom(AtomConstants.SchemeTag);
            category.Label = content.Attributes.GetAtom(AtomConstants.LabelTag);

            return category;
        }

        protected virtual ISyndicationImage CreateImage(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }


            if (!TryParseValue(content.Value, out Uri uri))
            {
                throw new FormatException("Invalid Atom image url");
            }

            SyndicationImage image = new SyndicationImage(uri);
            image.RelationshipType = content.Name;

            return image;
        }

        protected virtual ISyndicationLink CreateLink(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            //
            // title
            string title = content.Attributes.GetAtom(AtomConstants.TitleTag);

            // type
            string type = content.Attributes.GetAtom(AtomConstants.TypeTag);

            //
            // length
            long length = 0;
            TryParseValue(content.Attributes.GetAtom(AtomConstants.LengthTag), out length);

            //
            // rel
            string rel = content.Attributes.GetAtom(AtomConstants.RelativeTag) ?? ((content.Name == AtomConstants.LinkTag) ? AtomConstants.AlternateTag : content.Name);

            //
            // href
            TryParseValue(content.Attributes.GetAtom(AtomConstants.HrefTag), out Uri uri);

            //src
            if (uri == null)
            {
                TryParseValue(content.Attributes.GetAtom(AtomConstants.SourceTag), out uri);
            }

            if (uri == null)
            {
                throw new FormatException("Invalid uri");
            }

            return new SyndicationLink(uri, rel)
            {
                Title = title,
                Length = length,
                MediaType = type
            };
        }

        protected virtual ISyndicationPerson CreatePerson(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            var person = new SyndicationPerson();

            person.RelationshipType = content.Name;

            foreach (var field in content.Fields)
            {
                // content does not contain atom's namespace. So if we receibe a different namespace we will ignore it.
                if (field.Namespace != "" && field.Namespace != AtomConstants.Atom10Namespace)
                {
                    continue;
                }

                switch (field.Name)
                {
                    //
                    // Name
                    case AtomConstants.NameTag:
                        person.Name = field.Value;
                        break;

                    //
                    // Email
                    case AtomConstants.EmailTag:
                        person.Email = field.Value;
                        break;

                    //
                    // Uri
                    case AtomConstants.UriTag:
                        person.Uri = field.Value;
                        break;
                    //
                    // Unrecognized tag
                    default:
                        break;
                }
            }

            return person;
        }

        protected virtual IAtomEntry CreateItem(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            var item = new AtomEntry();


            foreach (var field in content.Fields)
            {
                // content does not contain atom's namespace. So if we receibe a different namespace we will ignore it.
                if (field.Namespace != "" && field.Namespace != AtomConstants.Atom10Namespace)
                {
                    continue;
                }

                switch (field.Name)
                {
                    //
                    // Category
                    case AtomConstants.CategoryTag:
                        item.AddCategory(CreateCategory(field));
                        break;

                    //
                    // Content
                    case AtomConstants.ContentTag:

                        item.ContentType = field.Attributes.GetAtom(AtomConstants.TypeTag) ?? AtomConstants.PlaintextType;

                        if (field.Attributes.GetAtom(AtomConstants.SourceTag) != null)
                        {
                            item.AddLink(CreateLink(field));
                        }

                        break;

                    //
                    // Author/Contributor
                    case AtomConstants.AuthorTag:
                    case AtomConstants.ContributorTag:
                        item.AddContributor(CreatePerson(field));
                        break;

                    //
                    // Id
                    case AtomConstants.IdTag:
                        item.Id = field.Value;
                        break;

                    //
                    // Link
                    case AtomConstants.LinkTag:
                        item.AddLink(CreateLink(field));
                        break;

                    //
                    // Published
                    case AtomConstants.PublishedTag:
                        if (TryParseValue(field.Value, out DateTimeOffset published))
                        {
                            item.Published = published;
                        }
                        break;

                    //
                    // Rights
                    case AtomConstants.RightsTag:
                        item.Rights = field.Value;
                        break;

                    //
                    // Source
                    case AtomConstants.SourceFeedTag:
                        item.AddLink(CreateSource(field));
                        break;
                    //
                    // Summary
                    case AtomConstants.SummaryTag:
                        item.Summary = field.Value;
                        break;

                    //
                    // Title
                    case AtomConstants.TitleTag:
                        item.Title = field.Value;
                        break;

                    //
                    // Updated
                    case AtomConstants.UpdatedTag:
                        if (TryParseValue(field.Value, out DateTimeOffset updated))
                        {
                            item.LastUpdated = updated;
                        }
                        break;

                    //
                    // Unrecognized tags
                    default:
                        break;
                }
            }


            return item;
        }

        protected virtual ISyndicationLink CreateSource(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            Uri url = null;
            string title = null;
            DateTimeOffset lastUpdated;

            foreach (var field in content.Fields)
            {
                // content does not contain atom's namespace. So if we receibe a different namespace we will ignore it.
                if (field.Namespace != "" && field.Namespace != AtomConstants.Atom10Namespace)
                {
                    continue;
                }

                switch (field.Name)
                {
                    //
                    // Id
                    case AtomConstants.IdTag:

                        if (url == null)
                        {
                            TryParseValue(field.Value, out url);
                        }

                        break;

                    //
                    // Title
                    case AtomConstants.TitleTag:
                        title = field.Value;
                        break;

                    //
                    // Updated
                    case AtomConstants.UpdatedTag:
                        TryParseValue(field.Value, out lastUpdated);
                        break;

                    //
                    // Link
                    case AtomConstants.LinkTag:
                        if(url == null)
                        {
                            url = CreateLink(field).Uri;
                        }
                        break;

                    //
                    // Unrecognized
                    default:
                        break;
                }
            }

            if (url == null)
            {
                throw new FormatException("Invalid source link");
            }

            return new SyndicationLink(url, AtomConstants.SourceFeedTag)
            {
                Title = title,
                LastUpdated = lastUpdated
            };
        }
    }

    static class AtomAttributeExtentions
    {
        public static string GetAtom(this IEnumerable<ISyndicationAttribute> attributes, string name)
        {
            return attributes.FirstOrDefault(a => a.Name == name && a.Namespace == "")?.Value;
        }
    }

}
