// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Microsoft.SyndicationFeed
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

        public virtual string Format(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            using (XmlWriter writer = CreateXmlWriter(out StringBuilder sb))
            {
                Write(content, writer);

                return sb.ToString();
            }
        }

        public virtual string Format(ISyndicationCategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            using (XmlWriter writer = CreateXmlWriter(out StringBuilder sb))
            {
                Write(category, writer);

                return sb.ToString();
            }
        }

        public virtual string Format(ISyndicationImage image)
        {
            if(image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            using (XmlWriter writer = CreateXmlWriter(out StringBuilder sb))
            {
                Write(image, writer);
                return sb.ToString();
            }
        }
        

        public virtual string Format(ISyndicationItem item)
        {
            throw new NotImplementedException();
        }

        public virtual string Format(ISyndicationPerson person)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            using (XmlWriter writer = CreateXmlWriter(out StringBuilder sb))
            {
                Write(person, writer);

                return sb.ToString();
            }
        }

        public virtual string Format(ISyndicationLink link)
        {
            throw new NotImplementedException();
        }

        public string FormatValue<T>(T value)
        {
            throw new NotImplementedException();
        }
        
        private XmlWriter CreateXmlWriter(out StringBuilder sb)
        {
            sb = new StringBuilder();
            return XmlWriter.Create(sb, _settings.Clone());
        }

        private void Write(ISyndicationContent content, XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        private void Write(ISyndicationCategory category, XmlWriter writer)
        {

            if (string.IsNullOrEmpty(category.Name))
            {
                throw new ArgumentNullException(nameof(category.Name));
            }

            writer.WriteElementString(Rss20Constants.CategoryTag,category.Name);
            writer.Flush();
        }

        private void Write(ISyndicationPerson person, XmlWriter writer)
        {
            if (string.IsNullOrEmpty(person.RelationshipType))
            {
                throw new ArgumentException(nameof(person.RelationshipType));
            }

            if (string.IsNullOrEmpty(person.Email))
            {
                throw new ArgumentException(nameof(person.Email));
            }

            if(person.RelationshipType == Rss20Constants.AuthorTag)
            {
                writer.WriteElementString(Rss20Constants.AuthorTag, person.Email);
            }
            else if (person.RelationshipType == Rss20Constants.ManagingEditorTag)
            {
                writer.WriteElementString(Rss20Constants.ManagingEditorTag, person.Email);
            }
            else
            {
                throw new ArgumentException("Invalid relationshipType");
            }

            writer.Flush();
        }

        private void Write(ISyndicationImage image, XmlWriter writer)
        {
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

            //Write <image>
            writer.WriteStartElement(Rss20Constants.ImageTag);

            //Write required contents of image
            writer.WriteElementString(Rss20Constants.UrlTag, image.Url.OriginalString);
            writer.WriteElementString(Rss20Constants.TitleTag, image.Title);
            writer.WriteElementString(Rss20Constants.LinkTag, image.Link.Uri.OriginalString); // THIS MUST BE CHANGED TO USE LINK PARSER, WAITING FOR IMPLEMENTATION

            //Write optional elements
            if (!string.IsNullOrEmpty(image.Desciption))
            {
                writer.WriteElementString(Rss20Constants.DescriptionTag, image.Desciption);
            }

            //Close image tag </image>
            writer.WriteEndElement();
            writer.Flush();
        }
    }
}