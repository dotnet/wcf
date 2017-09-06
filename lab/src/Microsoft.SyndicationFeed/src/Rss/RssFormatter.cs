// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Microsoft.SyndicationFeed.Rss
{
    public class RssFormatter : ISyndicationFeedFormatter
    {
        private readonly XmlWriter _writer;
        private readonly StringBuilder _buffer;

        public RssFormatter()
            : this(null, null)
        {
        }

        public RssFormatter(IEnumerable<ISyndicationAttribute> knownAttributes, XmlWriterSettings settings)
        {
            _buffer = new StringBuilder();
            _writer = XmlUtils.CreateXmlWriter(settings?.Clone() ?? new XmlWriterSettings(), knownAttributes, _buffer);
        }

        public string Format(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            try
            {
                _writer.WriteSyndicationContent(content, null);

                _writer.Flush();

                return _buffer.ToString();
            }
            finally
            {
                _buffer.Clear();
            }
        }

        public string Format(ISyndicationCategory category)
        {
            ISyndicationContent content = CreateContent(category);

            return Format(content);
        }
        
        public string Format(ISyndicationImage image)
        {
            ISyndicationContent content = CreateContent(image);

            return Format(content);
        }

        public string Format(ISyndicationPerson person)
        {
            ISyndicationContent content = CreateContent(person);

            return Format(content);
        }

        public string Format(ISyndicationItem item)
        {
            ISyndicationContent content = CreateContent(item);

            return Format(content);
        }

        public string Format(ISyndicationLink link)
        {
            ISyndicationContent content = CreateContent(link);

            return Format(content);
        }

        public virtual string FormatValue<T>(T value)
        {
            return Converter.FormatValue(value);
        }

        public virtual ISyndicationContent CreateContent(ISyndicationLink link)
        {
            if (link == null)
            {
                throw new ArgumentNullException(nameof(link));
            }

            if (link.Uri == null)
            {
                throw new ArgumentNullException("Invalid link uri");
            }

            switch (link.RelationshipType)
            {
                case RssElementNames.Enclosure:
                    return CreateEnclosureContent(link);

                case RssElementNames.Comments:
                    return CreateCommentsContent(link);

                case RssElementNames.Source:
                    return CreateSourceContent(link);

                default:
                    return CreateLinkContent(link);
            }
        }

        public virtual ISyndicationContent CreateContent(ISyndicationCategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (string.IsNullOrEmpty(category.Name))
            {
                throw new FormatException("Invalid category name");
            }

            return new SyndicationContent(RssElementNames.Category, category.Name);
        }

        public virtual ISyndicationContent CreateContent(ISyndicationPerson person)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            //
            // RSS requires Email
            if (string.IsNullOrEmpty(person.Email))
            {
                throw new ArgumentNullException("Invalid person Email");
            }

            return new SyndicationContent(person.RelationshipType ?? RssElementNames.Author, person.Email);
        }

        public virtual ISyndicationContent CreateContent(ISyndicationImage image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            // Required URL - Title - Link
            if (string.IsNullOrEmpty(image.Title))
            {
                throw new ArgumentNullException("Image requires a title");
            }

            if (image.Link == null)
            {
                throw new ArgumentNullException("Image requires a link");
            }

            if (image.Url == null)
            {
                throw new ArgumentNullException("Image requires an url");
            }

            var content = new SyndicationContent(RssElementNames.Image);

            // Write required contents of image
            content.AddField(new SyndicationContent(RssElementNames.Url, FormatValue(image.Url)));
            content.AddField(new SyndicationContent(RssElementNames.Title, image.Title));
            content.AddField(CreateContent(image.Link));


            // Write optional elements
            if (!string.IsNullOrEmpty(image.Description))
            {
                content.AddField(new SyndicationContent(RssElementNames.Description, image.Description));
            }

            return content;
        }

        public virtual ISyndicationContent CreateContent(ISyndicationItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            // Spec requires to have at least one title or description
            if (string.IsNullOrEmpty(item.Title) && string.IsNullOrEmpty(item.Description))
            {
                throw new ArgumentNullException("RSS Item requires a title or a description");
            }

            // Write <item> tag
            var content = new SyndicationContent(RssElementNames.Item);

            //
            // Title
            if (!string.IsNullOrEmpty(item.Title))
            {
                content.AddField(new SyndicationContent(RssElementNames.Title, item.Title));
            }

            //
            // Links
            ISyndicationLink guidLink = null;

            if (item.Links != null)
            {
                foreach (var link in item.Links)
                {
                    if (link.RelationshipType == RssElementNames.Guid)
                    {
                        guidLink = link;
                    }

                    content.AddField(CreateContent(link));
                }
            }

            //
            // Description
            if (!string.IsNullOrEmpty(item.Description))
            {
                content.AddField(new SyndicationContent(RssElementNames.Description, item.Description));
            }

            //
            // Authors (persons)
            if (item.Contributors != null)
            {
                foreach (var person in item.Contributors)
                {
                    content.AddField(CreateContent(person));
                }
            }

            //
            // Cathegory
            if (item.Categories != null)
            {
                foreach (var category in item.Categories)
                {
                    content.AddField(CreateContent(category));
                }
            }

            //
            // Guid (id)
            if (guidLink == null && !string.IsNullOrEmpty(item.Id))
            {
                var guid = new SyndicationContent(RssElementNames.Guid, item.Id);

                guid.AddAttribute(new SyndicationAttribute(RssConstants.IsPermaLink, "false"));

                content.AddField(guid);
            }

            //
            // PubDate
            if (item.Published != DateTimeOffset.MinValue)
            {
                content.AddField(new SyndicationContent(RssElementNames.PubDate, FormatValue(item.Published)));
            }

            return content;
        }

        
        private ISyndicationContent CreateEnclosureContent(ISyndicationLink link)
        {
            var content = new SyndicationContent(RssElementNames.Enclosure);

            //
            // Url
            content.AddAttribute(new SyndicationAttribute(RssElementNames.Url, FormatValue(link.Uri)));

            //
            // Length
            if (link.Length == 0)
            {
                throw new ArgumentException("Enclosure requires length attribute");
            }

            content.AddAttribute(new SyndicationAttribute(RssConstants.Length, FormatValue(link.Length)));

            //
            // MediaType
            if (string.IsNullOrEmpty(link.MediaType))
            {
                throw new ArgumentNullException("Enclosure requires a MediaType");
            }

            content.AddAttribute(new SyndicationAttribute(RssConstants.Type, link.MediaType));
            return content;
        }

        private ISyndicationContent CreateLinkContent(ISyndicationLink link)
        {
            SyndicationContent content;

            if (string.IsNullOrEmpty(link.RelationshipType) || 
                link.RelationshipType == RssLinkTypes.Alternate)
            {
                // Regular <link>
                content = new SyndicationContent(RssElementNames.Link);
            }
            else
            {
                // Custom
                content = new SyndicationContent(link.RelationshipType);
            }

            //
            // title 
            if (!string.IsNullOrEmpty(link.Title))
            {
                content.Value = link.Title;
            }

            //
            // url
            string url = FormatValue(link.Uri);

            if (content.Value != null)
            {
                content.AddAttribute(new SyndicationAttribute(RssElementNames.Url, url));
            }
            else
            {
                content.Value = url;
            }

            //
            // Type
            if (!string.IsNullOrEmpty(link.MediaType))
            {
                content.AddAttribute(new SyndicationAttribute(RssConstants.Type, link.MediaType));
            }

            //
            // Lenght
            if (link.Length != 0)
            {
                content.AddAttribute(new SyndicationAttribute(RssConstants.Length, FormatValue(link.Length)));
            }

            return content;
        }

        private ISyndicationContent CreateCommentsContent(ISyndicationLink link)
        {
            return new SyndicationContent(link.RelationshipType)
            {
                Value = FormatValue(link.Uri)
            };
        }

        private ISyndicationContent CreateSourceContent(ISyndicationLink link)
        {
            var content = new SyndicationContent(link.RelationshipType);

            //
            // Url
            string url = FormatValue(link.Uri);
            if (link.Title != url)
            {
                content.AddAttribute(new SyndicationAttribute(RssElementNames.Url, url));
            }

            //
            // Title
            if (!string.IsNullOrEmpty(link.Title))
            {
                content.Value = link.Title;
            }

            return content;
        }
    }
}