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

            if (content.Name != Rss20ElementNames.Category || 
                content.Namespace != Rss20Constants.Rss20Namespace)
            {
                throw new FormatException("Invalid Rss category");
            }

            return CreateCategory(content);
        }

        public ISyndicationItem ParseItem(string value)
        {
            ISyndicationContent content = ParseContent(value);

            if (content.Name != Rss20ElementNames.Item || 
                content.Namespace != Rss20Constants.Rss20Namespace)
            {
                throw new FormatException("Invalid Rss item");
            }

            return CreateItem(content);
        }

        public ISyndicationLink ParseLink(string value)
        {
            ISyndicationContent content = ParseContent(value);

            if (content.Name != Rss20ElementNames.Link || 
                content.Namespace != Rss20Constants.Rss20Namespace)
            {
                throw new FormatException("Invalid Rss link");
            }

            return CreateLink(content);
        }

        public ISyndicationPerson ParsePerson(string value)
        {
            ISyndicationContent content = ParseContent(value);

            if ((content.Name != Rss20ElementNames.Author && 
                 content.Name != Rss20ElementNames.ManagingEditor) ||
                content.Namespace != Rss20Constants.Rss20Namespace)
            {
                throw new FormatException("Invalid Rss Person");
            }

            return CreatePerson(content);
        }

        public ISyndicationImage ParseImage(string value)
        {
            ISyndicationContent content = ParseContent(value);

            if (content.Name != Rss20ElementNames.Image ||
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

                return reader.ReadSyndicationContent();
            }
        }

        public virtual bool TryParseValue<T>(string value, out T result)
        {
            return Converter.TryParseValue<T>(value, out result);
        }

        public virtual ISyndicationItem CreateItem(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            var item = new SyndicationItem();

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
                    case Rss20ElementNames.Title:
                        item.Title = field.Value;
                        break;

                    //
                    // Link
                    case Rss20ElementNames.Link:
                        item.AddLink(CreateLink(field));
                        break;

                    // Description
                    case Rss20ElementNames.Description:
                        item.Description = field.Value;
                        break;

                    //
                    // Author
                    case Rss20ElementNames.Author:
                        item.AddContributor(CreatePerson(field));
                        break;

                    //
                    // Category
                    case Rss20ElementNames.Category:
                        break;

                    //
                    // Links
                    case Rss20ElementNames.Comments:
                    case Rss20ElementNames.Enclosure:
                    case Rss20ElementNames.Source:
                        item.AddLink(CreateLink(field));
                        break;

                    //
                    // Guid
                    case Rss20ElementNames.Guid:
                        item.Id = field.Value;

                        // permaLink
                        if (TryParseValue(field.Attributes.GetRss(Rss20Constants.IsPermaLinkTag), out bool isPermalink) &&
                            isPermalink &&
                            TryParseValue(field.Value, out Uri permaLink))
                        {
                            item.AddLink(new SyndicationLink(permaLink, Rss20ElementNames.Guid));
                        }
                        break;

                    //
                    // PubDate
                    case Rss20ElementNames.PubDate:
                        if (TryParseValue(field.Value, out DateTimeOffset dt))
                        {
                            item.Published = dt;
                        }
                        break;

                    default:
                        break;
                }
            }

            return item;
        }

        public virtual ISyndicationLink CreateLink(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            //
            // Title
            string title = content.Value;

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

                title = null;
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
            string rel = (content.Name == Rss20ElementNames.Link) ? Rss20LinkTypes.Alternate : content.Name;

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

            return new SyndicationPerson()
            {
                Email = content.Value,
                RelationshipType = content.Name
            };
        }

        public virtual ISyndicationImage CreateImage(ISyndicationContent content)
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
                    case Rss20ElementNames.Title:
                        title = field.Value;
                        break;
                        
                    //
                    // Url
                    case Rss20ElementNames.Url:
                        if (!TryParseValue(field.Value, out url))
                        {
                            throw new FormatException($"Invalid image url '{field.Value}'");
                        }
                        break;

                    //
                    // Link
                    case Rss20ElementNames.Link:
                        link = CreateLink(field);
                        break;
                        
                    //
                    // Description
                    case Rss20ElementNames.Description:
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

            return new SyndicationImage(url, Rss20ElementNames.Image)
            {
                Title = title,
                Description = description,
                Link = link
            };
        }

        public virtual ISyndicationCategory CreateCategory(ISyndicationContent content)
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