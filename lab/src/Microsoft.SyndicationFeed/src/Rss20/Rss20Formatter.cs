// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Microsoft.SyndicationFeed.Rss
{
    public class Rss20Formatter : ISyndicationFeedFormatter
    {
        XmlWriterSettings _settings;

        public Rss20Formatter(XmlWriterSettings settings)
        {
            _settings = settings?.Clone() ?? new XmlWriterSettings();

            _settings.Async = false;
            //_settings.CloseOutput = false;
            _settings.OmitXmlDeclaration = true;
        }

        public string Format(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            using (XmlWriter writer = CreateXmlWriter(out StringBuilder sb))
            {
                Write(content, writer);
                writer.Flush();
                return sb.ToString();
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

        protected virtual ISyndicationContent CreateContent(ISyndicationLink link)
        {
            if (link == null)
            {
                throw new ArgumentNullException(nameof(link));
            }

            if (link == null)
            {
                throw new ArgumentNullException(nameof(link));
            }

            switch (link.RelationshipType)
            {
                case Rss20Constants.LinkTag:
                    return CreateLinkContent(link);

                case Rss20Constants.EnclosureTag:
                    return CreateEnclosureContent(link);

                case Rss20Constants.CommentsTag:
                    return CreateCommentsContent(link);

                case Rss20Constants.SourceTag:
                    return CreateSourceContent(link);

                default:
                    return null;
            }
        }

        protected virtual void Write(ISyndicationContent content, XmlWriter writer)
        {
            //
            // Write opening name 
            writer.WriteStartElement(content.Name);

            //
            // Write attributes
            foreach (var attribute in content.Attributes)
            {
                writer.WriteAttributeString(attribute.Name, attribute.Namespace, attribute.Value);
            }

            var fields = (List<ISyndicationContent>)content.Fields;

            if (fields.Count == 0)
            {
                //
                // This element has no children.
                writer.WriteString(content.Value);
            }

            else
            {
                //
                // Write Fields
                foreach (var field in fields)
                {
                    writer.WriteRaw(Format(field));
                }
            }

            //
            // Write closing name 
            writer.WriteEndElement();
        }

        protected virtual ISyndicationContent CreateContent(ISyndicationCategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (string.IsNullOrEmpty(category.Name))
            {
                throw new FormatException("Invalid category name");
            }

            return new SyndicationContent(Rss20Constants.CategoryTag, category.Name);
        }

        protected virtual ISyndicationContent CreateContent(ISyndicationPerson person)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            if (string.IsNullOrEmpty(person.RelationshipType))
            {
                throw new ArgumentException("RelationshipType");
            }

            if (string.IsNullOrEmpty(person.Email))
            {
                throw new ArgumentException("Email");
            }

            return new SyndicationContent(person.RelationshipType, person.Email);
        }

        protected virtual ISyndicationContent CreateContent(ISyndicationImage image)
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

            var content = new SyndicationContent(Rss20Constants.ImageTag);

            //Write required contents of image
            content.AddField(new SyndicationContent(Rss20Constants.UrlTag, FormatValue(image.Url)));
            content.AddField(new SyndicationContent(Rss20Constants.TitleTag, image.Title));
            content.AddField(CreateContent(image.Link));


            //Write optional elements
            if (!string.IsNullOrEmpty(image.Description))
            {
                content.AddField(new SyndicationContent(Rss20Constants.DescriptionTag, image.Description));
            }

            return content;
        }

        protected virtual ISyndicationContent CreateContent(ISyndicationItem item)
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
            var content = new SyndicationContent(Rss20Constants.ItemTag);

            //
            // Title
            if (!string.IsNullOrEmpty(item.Title))
            {
                content.AddField(new SyndicationContent(Rss20Constants.TitleTag, item.Title));
            }

            //
            // Links
            if (item.Links != null)
            {
                foreach (var link in item.Links)
                {
                    content.AddField(CreateContent(link));
                }
            }

            //
            // Description
            if (!string.IsNullOrEmpty(item.Description))
            {
                content.AddField(new SyndicationContent(Rss20Constants.DescriptionTag, item.Description));
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
            if (!string.IsNullOrEmpty(item.Id))
            {
                content.AddField(new SyndicationContent(Rss20Constants.GuidTag, item.Id));
            }

            //
            // PubDate
            if (item.Published != DateTimeOffset.MinValue)
            {
                content.AddField(new SyndicationContent(Rss20Constants.PubDateTag, FormatValue(item.Published)));
            }

            return content;
        }

        private XmlWriter CreateXmlWriter(out StringBuilder sb)
        {
            sb = new StringBuilder();
            return XmlWriter.Create(sb, _settings.Clone());
        }
        
        private ISyndicationContent CreateEnclosureContent(ISyndicationLink link)
        {
            var content = new SyndicationContent(Rss20Constants.EnclosureTag);

            //
            // Url
            content.AddAttribute(new SyndicationAttribute(Rss20Constants.UrlTag, FormatValue(link.Uri)));

            //
            // Length
            if (link.Length == 0)
            {
                throw new ArgumentException("Enclosure requires length attribute");
            }

            content.AddAttribute(new SyndicationAttribute(Rss20Constants.LengthTag, FormatValue(link.Length)));

            //
            // MediaType
            if (string.IsNullOrEmpty(link.MediaType))
            {
                throw new ArgumentNullException("Enclosure requires a MediaType");
            }

            content.AddAttribute(new SyndicationAttribute(Rss20Constants.TypeTag, link.MediaType));
            return content;
        }

        private ISyndicationContent CreateLinkContent(ISyndicationLink link)
        {
            //Write attributes if exist
            var content = new SyndicationContent(link.RelationshipType);

            //
            // Lenght
            if (link.Length != 0)
            {
                content.AddAttribute(new SyndicationAttribute(Rss20Constants.LengthTag, FormatValue(link.Length)));
            }

            //
            // Type
            if (!string.IsNullOrEmpty(link.MediaType))
            {
                content.AddAttribute(new SyndicationAttribute(Rss20Constants.TypeTag, link.MediaType));
            }

            //
            // url attribute
            if (link.Uri.OriginalString != link.Title)
            {
                if (string.IsNullOrEmpty(link.Title))
                {
                    content.Value = FormatValue(link.Uri);
                }
                else
                {
                    content.AddAttribute(new SyndicationAttribute(Rss20Constants.UrlTag, FormatValue(link.Uri)));
                }
            }

            //
            // title 
            if (!string.IsNullOrEmpty(link.Title))
            {
                content.Value = link.Title;
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
            content.AddAttribute(new SyndicationAttribute(Rss20Constants.UrlTag, FormatValue(link.Uri)));

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