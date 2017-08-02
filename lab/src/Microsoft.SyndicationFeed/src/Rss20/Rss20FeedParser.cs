// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.SyndicationFeed.Rss
{
    public class Rss20FeedParser : ISyndicationFeedParser
    {
        public ISyndicationCategory ParseCategory(string value)
        {
            ISyndicationContent content = ParseContent(value);

            if (content.Name != Rss20Constants.CategoryTag || 
                content.Namespace != Rss20Constants.Rss20Namespace)
            {
                throw new FormatException("Invalid Rss category");
            }

            return CreateCategory(content);
        }

        public ISyndicationItem ParseItem(string value)
        {
            ISyndicationContent content = ParseContent(value);

            if (content.Name != Rss20Constants.ItemTag || 
                content.Namespace != Rss20Constants.Rss20Namespace)
            {
                throw new FormatException("Invalid Rss item");
            }

            return CreateItem(content);
        }

        public ISyndicationLink ParseLink(string value)
        {
            ISyndicationContent content = ParseContent(value);

            if (content.Name != Rss20Constants.LinkTag || 
                content.Namespace != Rss20Constants.Rss20Namespace)
            {
                throw new FormatException("Invalid Rss link");
            }

            return CreateLink(content);
        }

        public ISyndicationPerson ParsePerson(string value)
        {
            ISyndicationContent content = ParseContent(value);

            if ((content.Name != Rss20Constants.AuthorTag && 
                 content.Name != Rss20Constants.ManagingEditorTag) ||
                content.Namespace != Rss20Constants.Rss20Namespace)
            {
                throw new FormatException("Invalid Rss Person");
            }

            return CreatePerson(content);
        }

        public ISyndicationImage ParseImage(string value)
        {
            ISyndicationContent content = ParseContent(value);

            if (content.Name != Rss20Constants.ImageTag ||
                content.Namespace != Rss20Constants.Rss20Namespace)
            {
                throw new FormatException("Invalid Rss Image");
            }

            return CreateImage(content);
        }

        public ISyndicationContent ParseContent(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = XmlUtils.CreateXmlReader(value))
            {
                reader.MoveToContent();

                return XmlUtils.ReadSyndicationContent(reader);
            }
        }

        public virtual bool TryParseValue<T>(string value, out T result)
        {
            return Converter.TryParseValue<T>(value, out result);
        }

        protected virtual ISyndicationItem CreateItem(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            var item = new SyndicationItem();

            string fallbackAlternateLink = null;
            bool hasAlternateLink = false;

            foreach (var field in content.Fields)
            {
                if (field.Namespace != Rss20Constants.Rss20Namespace)
                {
                    continue;
                }

                switch (field.Name)
                {
                    //
                    // Title
                    case Rss20Constants.TitleTag:
                        item.Title = field.Value;
                        break;

                    //
                    // Link
                    case Rss20Constants.LinkTag:
                        item.AddLink(CreateLink(field));
                        hasAlternateLink = true;
                        break;

                    // Description
                    case Rss20Constants.DescriptionTag:
                        item.Description = field.Value;
                        break;

                    //
                    // Author
                    case Rss20Constants.AuthorTag:
                        item.AddContributor(CreatePerson(field));
                        break;

                    //
                    // Category
                    case Rss20Constants.CategoryTag:
                        break;

                    //
                    // Links
                    case Rss20Constants.CommentsTag:
                    case Rss20Constants.EnclosureTag:
                    case Rss20Constants.SourceTag:
                        item.AddLink(CreateLink(field));
                        break;

                    //
                    // Guid
                    case Rss20Constants.GuidTag:
                        item.Id = field.Value;

                        if (!hasAlternateLink && 
                            TryParseValue(field.Attributes.GetRss(Rss20Constants.IsPermaLinkTag), out bool isPermalink) &&
                            isPermalink)
                        {
                            fallbackAlternateLink = field.Value;
                        }
                        break;

                    //
                    // PubDate
                    case Rss20Constants.PubDateTag:
                        if (TryParseValue(field.Value, out DateTimeOffset dt))
                        {
                            item.Published = dt;
                        }
                        break;

                    default:
                        break;
                }
            }

            //
            // Add a fallback Link
            if (!hasAlternateLink &&
                fallbackAlternateLink != null &&
                TryParseValue(fallbackAlternateLink, out Uri url))
            {
                item.AddLink(new SyndicationLink(url, Rss20Constants.AlternateLink));
            }

            return item;
        }

        protected virtual ISyndicationLink CreateLink(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            //
            // Url
            Uri uri = null;
            string url = content.Attributes.GetRss("url");

            if (url != null)
            {
                if (!TryParseValue(url, out uri))
                {
                    throw new FormatException("Invalid url attribute");
                }
            }
            else
            {
                if (!TryParseValue(content.Value, out uri))
                {
                    throw new FormatException("Invalid url");
                }
            }

            //
            // Length
            long length = 0;
            TryParseValue(content.Attributes.GetRss("length"), out length);

            //
            // Type
            string type = content.Attributes.GetRss("type");
            
            //
            // rel
            string rel = (content.Name == Rss20Constants.LinkTag) ? Rss20Constants.AlternateLink : content.Name;

            return new SyndicationLink(uri, rel)
            {
                Title = content.Value,
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

            return new SyndicationPerson()
            {
                Email = content.Value,
                RelationshipType = content.Name
            };
        }

        protected virtual ISyndicationImage CreateImage(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            string title = null;
            string description = null;
            Uri url = null;
            ISyndicationLink link = null;

            foreach (var field in content.Fields)
            {
                if (field.Namespace != Rss20Constants.Rss20Namespace)
                {
                    continue;
                }

                switch (field.Name)
                {
                    //
                    // Title
                    case Rss20Constants.TitleTag:
                        title = field.Value;
                        break;
                        
                    //
                    // Url
                    case Rss20Constants.UrlTag:
                        if (!TryParseValue(field.Value, out url))
                        {
                            throw new FormatException($"Invalid image url '{field.Value}'");
                        }
                        break;

                    //
                    // Link
                    case Rss20Constants.LinkTag:
                        link = CreateLink(field);
                        break;
                        
                    //
                    // Description
                    case Rss20Constants.DescriptionTag:
                        description = field.Value;
                        break;

                    default:
                        break;
                }
            }
  
            if (url == null)
            {
                throw new FormatException("Image url not found");
            }

            return new SyndicationImage(url, Rss20Constants.ImageTag)
            {
                Title = title,
                Description = description,
                Link = link
            };
        }

        protected virtual ISyndicationCategory CreateCategory(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (content.Value == null)
            {
                throw new FormatException("Invalid Rss category name");
            }

            return new SyndicationCategory(content.Value);
        }
    }


    static class RssAttributeExtentions
    {
        public static string GetRss(this IEnumerable<ISyndicationAttribute> attributes, string name)
        {
            return attributes.FirstOrDefault(a => a.Name == name && a.Namespace == Rss20Constants.Rss20Namespace)?.Value;
        }
    }
}