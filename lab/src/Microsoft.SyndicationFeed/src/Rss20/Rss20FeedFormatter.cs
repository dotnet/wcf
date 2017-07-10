// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    public class Rss20FeedFormatter : ISyndicationFeedFormatter
    {
        public ISyndicationCategory ParseCategory(string value)
        {
            throw new NotImplementedException();
        }

        public ISyndicationContent ParseContent(string value)
        {
            XmlReader reader = XmlReader.Create(new StringReader(value));
            reader.MoveToContent();
            return ParseContent(reader);
        }

        public ISyndicationItem ParseItem(string value)
        {
            XmlReader reader = XmlReader.Create(new StringReader(value));
            return ParseItem(reader);
        }

        public ISyndicationLink ParseLink(string value)
        {
            throw new NotImplementedException();
        }

        public ISyndicationPerson ParsePerson(string value)
        {
            XmlReader reader = XmlReader.Create(new StringReader(value));
            return ParsePerson(reader);
        }


        private SyndicationItem ParseItem(XmlReader reader)
        {
            var item = new SyndicationItem();

            bool isEmpty = reader.IsEmptyElement;

            if (reader.HasAttributes)
            {
                FillItemAttributes(item, reader);
            }

            if (!isEmpty)
            {
                reader.ReadStartElement();
                FillItem(item, reader);
            }

            return item;
        }

        private SyndicationPerson ParsePerson(XmlReader reader)
        {
            var person = new SyndicationPerson();

            bool isEmpty = reader.IsEmptyElement;

            reader.ReadStartElement();
            if (!isEmpty) {
                string email = reader.ReadString();
                reader.ReadEndElement();
                person.Email = email;
            }

            return person;
        }

        private SyndicationContent ParseContent(XmlReader reader)
        {
            SyndicationContent content = new SyndicationContent();

            content.Value = reader.ReadElementContentAsString();

            return content;
        }

        private void FillItemAttributes(SyndicationItem item, XmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                string ns = reader.NamespaceURI;
                string name = reader.LocalName;

                if (name == "base" && ns == XmlUtils.XmlNs)
                {
                    item.BaseUri = UriUtils.Combine(item.BaseUri, reader.Value);
                    continue;
                }

                if (XmlUtils.IsXmlns(name, ns) || XmlUtils.IsXmlSchemaType(name, ns))
                {
                    continue;
                }

                string val = reader.Value;

                // Extentions are not supported yet

                /*
                if (!TryParseAttribute(name, ns, val, result, this.Version))
                {
                    if (_preserveAttributeExtensions)
                    {
                        result.AttributeExtensions.Add(new XmlQualifiedName(name, ns), val);
                    }
                    else
                    {
                        TraceSyndicationElementIgnoredOnRead(reader);
                    }
                }
                */
            }
        }

        private void FillItem(SyndicationItem item, XmlReader reader)
        {
            string fallbackAlternateLink = null;
            bool readAlternateLink = false;

            var links = new List<ISyndicationLink>();
            var authors = new List<ISyndicationPerson>();
            var categories = new List<ISyndicationCategory>();

            while (reader.IsStartElement())
            {
                if (reader.IsStartElement(Rss20Constants.TitleTag, Rss20Constants.Rss20Namespace))
                {
                    item.Title = reader.ReadElementString();
                }
                else if (reader.IsStartElement(Rss20Constants.LinkTag, Rss20Constants.Rss20Namespace))
                {
                    throw new NotImplementedException();

                    //links.Add(ReadAlternateLink(reader, result.BaseUri));
                    //readAlternateLink = true;
                }
                else if (reader.IsStartElement(Rss20Constants.DescriptionTag, Rss20Constants.Rss20Namespace))
                {
                    item.Description = reader.ReadElementString();
                }
                else if (reader.IsStartElement(Rss20Constants.AuthorTag, Rss20Constants.Rss20Namespace))
                {
                    throw new NotImplementedException();
                    
                    //authors.Add(ReadPerson(reader, result));
                }
                else if (reader.IsStartElement(Rss20Constants.CategoryTag, Rss20Constants.Rss20Namespace))
                {
                    throw new NotImplementedException();
                    
                    //categories.Add(ReadCategory(reader, result));
                }
                else if (reader.IsStartElement(Rss20Constants.EnclosureTag, Rss20Constants.Rss20Namespace))
                {
                    throw new NotImplementedException();

                    //links.Add(ReadMediaEnclosure(reader, result.BaseUri));
                }
                else if (reader.IsStartElement(Rss20Constants.GuidTag, Rss20Constants.Rss20Namespace))
                {
                    bool isPermalink = true;
                    string permalinkString = reader.GetAttribute(Rss20Constants.IsPermaLinkTag, Rss20Constants.Rss20Namespace);

                    if ((permalinkString != null) && (permalinkString.ToUpperInvariant() == "FALSE"))
                    {
                        isPermalink = false;
                    }

                    item.Id = reader.ReadElementString();

                    if (isPermalink)
                    {
                        fallbackAlternateLink = item.Id;
                    }
                }
                else if (reader.IsStartElement(Rss20Constants.PubDateTag, Rss20Constants.Rss20Namespace))
                {
                    bool canReadContent = !reader.IsEmptyElement;
                    reader.ReadStartElement();

                    if (canReadContent)
                    {
                        string str = reader.ReadString();
                        if (!string.IsNullOrEmpty(str))
                        {
                            item.PublishDate = DateTimeUtils.Parse(str);
                        }

                        reader.ReadEndElement();
                    }
                }
                else if (reader.IsStartElement(Rss20Constants.SourceTag, Rss20Constants.Rss20Namespace))
                {
                    throw new NotImplementedException();

                    /*
                    SyndicationFeed feed = new SyndicationFeed();

                    if (reader.HasAttributes)
                    {
                        while (reader.MoveToNextAttribute())
                        {
                            string ns = reader.NamespaceURI;
                            string name = reader.LocalName;
                            if (FeedUtils.IsXmlns(name, ns))
                            {
                                continue;
                            }
                            string val = reader.Value;
                            if (name == Rss20Constants.UrlTag && ns == Rss20Constants.Rss20Namespace)
                            {
                                feed.Links.Add(SyndicationLink.CreateSelfLink(new Uri(val, UriKind.RelativeOrAbsolute)));
                            }
                            else if (!FeedUtils.IsXmlns(name, ns))
                            {
                                if (_preserveAttributeExtensions)
                                {
                                    feed.AttributeExtensions.Add(new XmlQualifiedName(name, ns), val);
                                }
                                else
                                {
                                    TraceSyndicationElementIgnoredOnRead(reader);
                                }
                            }
                        }
                    }

                    string feedTitle = reader.ReadElementString();
                    feed.Title = new TextSyndicationContent(feedTitle);
                    result.SourceFeed = feed;
                    */
                }
                else
                {
                    // Extentions are not supported yet
                        
                    reader.Skip();

                    /*
                    bool parsedExtension = _serializeExtensionsAsAtom && _atomSerializer.TryParseItemElementFrom(reader, result);
                    if (!parsedExtension)
                    {
                        parsedExtension = TryParseElement(reader, result, this.Version);
                    }
                    if (!parsedExtension)
                    {
                        if (_preserveElementExtensions)
                        {
                            CreateBufferIfRequiredAndWriteNode(ref buffer, ref extWriter, reader, _maxExtensionSize);
                        }
                        else
                        {
                            TraceSyndicationElementIgnoredOnRead(reader);
                            reader.Skip();
                        }
                    }
                    */
                }
            }

            // Extentions are not supported yet

            // XmlBuffer buffer = null;
            // LoadElementExtensions(buffer, extWriter, result);

            reader.ReadEndElement(); // end

            if (!readAlternateLink && fallbackAlternateLink != null)
            {
                throw new NotImplementedException();

                //links.Add(SyndicationLink.CreateAlternateLink(new Uri(fallbackAlternateLink, UriKind.RelativeOrAbsolute)));
                //readAlternateLink = true;
            }

            // if there's no content and no alternate link set the summary as the item content
            if (item.Content == null && !readAlternateLink)
            {
                item.Content = item.Description;
                item.Description = null;
            }

            item.Links = links;
            item.Authors = authors;
            item.Categories = categories;
        }
    }
}
