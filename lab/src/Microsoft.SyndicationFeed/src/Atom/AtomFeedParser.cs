// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.SyndicationFeed.Atom
{
    public class AtomFeedParser : ISyndicationFeedParser
    {
        public ISyndicationCategory ParseCategory(string value)
        {
            ISyndicationContent content = ParseContent(value);

            if (content.Name != AtomElementNames.Category)
            {
                throw new FormatException("Invalid Atom Category");
            }

            return CreateCategory(content);
        }
        
        public ISyndicationImage ParseImage(string value)
        {
            ISyndicationContent content = ParseContent(value);

            if (content.Name != AtomElementNames.Logo && content.Name != AtomElementNames.Icon)
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

            if (content.Name != AtomElementNames.Entry)
            {
                throw new FormatException("Invalid Atom feed");
            }

            return CreateEntry(content);
        }
        
        public ISyndicationLink ParseLink(string value)
        {
            ISyndicationContent content = ParseContent(value);

            if (content.Name != AtomElementNames.Link)
            {
                throw new FormatException("Invalid Atom Link");
            }

            return CreateLink(content);
        }

        public ISyndicationPerson ParsePerson(string value)
        {
            ISyndicationContent content = ParseContent(value);

            if (content.Name != AtomContributorTypes.Author && content.Name != AtomContributorTypes.Contributor)
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

                return reader.ReadSyndicationContent();
            }
        }

        public virtual bool TryParseValue<T>(string value, out T result)
        {
            return Converter.TryParseValue<T>(value, out result);
        }

        public virtual ISyndicationCategory CreateCategory(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            string term = content.Attributes.GetAtom(AtomConstants.Term);

            if (term == null)
            {
                throw new FormatException("Invalid Atom category, requires Term attribute");
            }

            return new SyndicationCategory(term)
            {
                Scheme = content.Attributes.GetAtom(AtomConstants.Scheme),
                Label = content.Attributes.GetAtom(AtomConstants.Label)
            };
        }

        public virtual ISyndicationImage CreateImage(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (!TryParseValue(content.Value, out Uri uri))
            {
                throw new FormatException("Invalid Atom image url");
            }

            return new SyndicationImage(uri, content.Name);
        }

        public virtual ISyndicationLink CreateLink(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            //
            // title
            string title = content.Attributes.GetAtom(AtomElementNames.Title);

            // type
            string type = content.Attributes.GetAtom(AtomConstants.Type);

            //
            // length
            long length = 0;
            TryParseValue(content.Attributes.GetAtom(AtomConstants.Length), out length);

            //
            // rel
            string rel = content.Attributes.GetAtom(AtomConstants.Rel) ?? ((content.Name == AtomElementNames.Link) ? AtomLinkTypes.Alternate : content.Name);

            //
            // href
            TryParseValue(content.Attributes.GetAtom(AtomConstants.Href), out Uri uri);

            // src
            if (uri == null)
            {
                TryParseValue(content.Attributes.GetAtom(AtomConstants.Source), out uri);
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

        public virtual ISyndicationPerson CreatePerson(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            string name = null;
            string email = null;
            string uri = null;

            foreach (var field in content.Fields)
            {
                // content does not contain atom's namespace. So if we receibe a different namespace we will ignore it.
                if (field.Namespace != AtomConstants.Atom10Namespace)
                {
                    continue;
                }

                switch (field.Name)
                {
                    //
                    // Name
                    case AtomElementNames.Name:
                        name = field.Value;
                        break;

                    //
                    // Email
                    case AtomElementNames.Email:
                        email = field.Value;
                        break;

                    //
                    // Uri
                    case AtomElementNames.Uri:
                        uri = field.Value;
                        break;
                    //
                    // Unrecognized field
                    default:
                        break;
                }
            }

            return new SyndicationPerson(name, email, content.Name)
            {
                Uri = uri
            };
        }

        public virtual IAtomEntry CreateEntry(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            var item = new AtomEntry();


            foreach (var field in content.Fields)
            {
                // content does not contain atom's namespace. So if we receibe a different namespace we will ignore it.
                if (field.Namespace != AtomConstants.Atom10Namespace)
                {
                    continue;
                }

                switch (field.Name)
                {
                    //
                    // Category
                    case AtomElementNames.Category:
                        item.AddCategory(CreateCategory(field));
                        break;

                    //
                    // Content
                    case AtomElementNames.Content:

                        item.ContentType = field.Attributes.GetAtom(AtomConstants.Type) ?? AtomConstants.PlainTextContentType;

                        if (field.Attributes.GetAtom(AtomConstants.Source) != null)
                        {
                            item.AddLink(CreateLink(field));
                        }
                        else
                        {
                            item.Description = field.Value;
                        }

                        break;

                    //
                    // Author/Contributor
                    case AtomContributorTypes.Author:
                    case AtomContributorTypes.Contributor:
                        item.AddContributor(CreatePerson(field));
                        break;

                    //
                    // Id
                    case AtomElementNames.Id:
                        item.Id = field.Value;
                        break;

                    //
                    // Link
                    case AtomElementNames.Link:
                        item.AddLink(CreateLink(field));
                        break;

                    //
                    // Published
                    case AtomElementNames.Published:
                        if (TryParseValue(field.Value, out DateTimeOffset published))
                        {
                            item.Published = published;
                        }
                        break;

                    //
                    // Rights
                    case AtomElementNames.Rights:
                        item.Rights = field.Value;
                        break;

                    //
                    // Source
                    case AtomElementNames.SourceFeed:
                        item.AddLink(CreateSource(field));
                        break;
                    //
                    // Summary
                    case AtomElementNames.Summary:
                        item.Summary = field.Value;
                        break;

                    //
                    // Title
                    case AtomElementNames.Title:
                        item.Title = field.Value;
                        break;

                    //
                    // Updated
                    case AtomElementNames.Updated:
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

        public virtual ISyndicationLink CreateSource(ISyndicationContent content)
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
                if (field.Namespace != AtomConstants.Atom10Namespace)
                {
                    continue;
                }

                switch (field.Name)
                {
                    //
                    // Id
                    case AtomElementNames.Id:

                        if (url == null)
                        {
                            TryParseValue(field.Value, out url);
                        }

                        break;

                    //
                    // Title
                    case AtomElementNames.Title:
                        title = field.Value;
                        break;

                    //
                    // Updated
                    case AtomElementNames.Updated:
                        TryParseValue(field.Value, out lastUpdated);
                        break;

                    //
                    // Link
                    case AtomElementNames.Link:
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

            return new SyndicationLink(url, AtomElementNames.SourceFeed)
            {
                Title = title,
                LastUpdated = lastUpdated
            };
        }

        private XmlReader CreateXmlReader(string value)
        {
            return XmlUtils.CreateXmlReader(value);
        }
    }

    static class AtomAttributeExtentions
    {
        public static string GetAtom(this IEnumerable<ISyndicationAttribute> attributes, string name)
        {
            return attributes.FirstOrDefault(a => a.Name == name && a.Namespace == string.Empty)?.Value;
        }
    }
}
