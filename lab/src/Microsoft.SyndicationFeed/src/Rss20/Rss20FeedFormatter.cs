// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    public class Rss20FeedFormatter : ISyndicationFeedFormatter
    {
        public virtual ISyndicationCategory ParseCategory(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = CreateXmlReader(value))
            {
                reader.MoveToContent();

                if(reader.Name != Rss20Constants.CategoryTag)
                {
                    throw new FormatException("Invalid Rss category");
                }

                return ParseCategory(reader);
            }
        }

        public virtual ISyndicationItem ParseItem(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (XmlReader reader = CreateXmlReader(value))
            {
                reader.MoveToContent();

                if (reader.Name != Rss20Constants.ItemTag)
                {
                    throw new FormatException("Invalid Rss item");
                }

                return ParseItem(reader);
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

                if(reader.Name != Rss20Constants.LinkTag)
                {
                    throw new FormatException("Invalid Rss Link");
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

                if(reader.Name != Rss20Constants.AuthorTag && reader.Name != Rss20Constants.ManagingEditorTag)
                {
                    throw new FormatException("Invalid Rss Person");
                }

                return ParsePerson(reader);
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

                if(reader.Name != Rss20Constants.ImageTag)
                {
                    throw new FormatException("Invalid Rss Image");
                }

                return ParseImage(reader);
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
                if (TryParseRssDate(value, out dt))
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
            return (result = (T) Convert.ChangeType(value, typeof(T))) != null;
        }

        private SyndicationImage ParseImage(XmlReader reader)
        {
            string title = string.Empty;
            string description = string.Empty;
            Uri url = null;
            ISyndicationLink link = null;

            if (!reader.IsEmptyElement)
            {
                reader.ReadStartElement();

                while (reader.IsStartElement())
                {
                    //
                    // Url
                    if (reader.IsStartElement(Rss20Constants.UrlTag, Rss20Constants.Rss20Namespace))
                    {
                        if (!TryParseValue(reader.ReadElementContentAsString(), out url)) 
                        {
                            throw new FormatException("Invalid image url.");
                        }
                    }

                    //
                    // Title
                    if(reader.IsStartElement(Rss20Constants.TitleTag, Rss20Constants.Rss20Namespace))
                    {
                        title = reader.ReadElementContentAsString();
                    }

                    //
                    // Link
                    if (reader.IsStartElement(Rss20Constants.LinkTag, Rss20Constants.Rss20Namespace))
                    {
                        link = ParseLink(reader.ReadOuterXml());
                    }

                    //
                    // Description
                    if (reader.IsStartElement(Rss20Constants.DescriptionTag, Rss20Constants.Rss20Namespace))
                    {
                        description = reader.ReadElementContentAsString();
                    }
                }

                reader.ReadEndElement(); //image end
            }

            if (url == null)
            {
                throw new FormatException("Invalid image url");
            }

            return new SyndicationImage(url) {
                Title = title,
                Desciption = description,
                Link = link,
                RelationshipType = Rss20Constants.ImageTag
            };
        }

        private SyndicationLink ParseLink(XmlReader reader)
        {

            //
            // Url
            Uri uri = null;
            string url = reader.GetAttribute("url");
            if(url != null)
            {
                if (!TryParseValue(url,out uri))
                {
                    throw new FormatException("Invalid url attribute format.");
                }
            }

            //
            // Length
            long length = 0;
            TryParseValue(reader.GetAttribute("length"), out length);

            //
            // Type
            string type = string.Empty;
            TryParseValue(reader.GetAttribute("type"), out type);
            
            reader.ReadStartElement();

            //
            // Title
            string title = string.Empty;
            if (!reader.IsEmptyElement)
            {
                title = reader.ReadContentAsString();

                // Url is the content, if not set as attribute
                if (uri == null && !string.IsNullOrEmpty(title))
                {
                    TryParseValue(title, out uri);
                }
            }

            reader.ReadEndElement();

            return new SyndicationLink(uri) {
                Title = title,
                Length = length,
                MediaType = type,
            };
        }

        private SyndicationCategory ParseCategory(XmlReader reader)
        {
            var category = new SyndicationCategory();

            if (!reader.IsEmptyElement)
            {
                category.Name = reader.ReadElementContentAsString();
            }

            return category;
        }

        private SyndicationItem ParseItem(XmlReader reader)
        {
            var item = new SyndicationItem();

            bool isEmpty = reader.IsEmptyElement;

            if (!isEmpty)
            {
                FillItem(item, reader);
            }

            return item;
        }

        private SyndicationPerson ParsePerson(XmlReader reader)
        {
            var person = new SyndicationPerson();

            if (!reader.IsEmptyElement)
            {
                person.Email = reader.ReadElementContentAsString();
            }

            return person;
        }

        private void FillItem(SyndicationItem item, XmlReader reader)
        {
            string fallbackAlternateLink = null;
            bool readAlternateLink = false;

            var links = new List<ISyndicationLink>();
            var contributors = new List<ISyndicationPerson>();
            var categories = new List<ISyndicationCategory>();

            reader.ReadStartElement();

            while (reader.IsStartElement())
            {
                //
                // Title
                if (reader.IsStartElement(Rss20Constants.TitleTag, Rss20Constants.Rss20Namespace))
                {
                    item.Title = reader.ReadElementContentAsString();
                }
                //
                // Link
                else if (reader.IsStartElement(Rss20Constants.LinkTag, Rss20Constants.Rss20Namespace))
                {
                    SyndicationLink link = ParseLink(reader);
                    link.RelationshipType = Rss20Constants.AlternateLink;

                    links.Add(link);
                    readAlternateLink = true;
                }
                //
                // Description
                else if (reader.IsStartElement(Rss20Constants.DescriptionTag, Rss20Constants.Rss20Namespace))
                {
                    item.Description = reader.ReadElementContentAsString();
                }
                //
                // Author
                else if (reader.IsStartElement(Rss20Constants.AuthorTag, Rss20Constants.Rss20Namespace))
                {
                    SyndicationPerson person = ParsePerson(reader);
                    person.RelationshipType = Rss20Constants.AuthorTag;

                    contributors.Add(person);
                }
                //
                // Category
                else if (reader.IsStartElement(Rss20Constants.CategoryTag, Rss20Constants.Rss20Namespace))
                {
                    categories.Add(ParseCategory(reader));
                }
                //
                // Comments
                else if (reader.IsStartElement(Rss20Constants.CommentsTag, Rss20Constants.Rss20Namespace))
                {
                    SyndicationLink link = ParseLink(reader);
                    link.RelationshipType = Rss20Constants.CommentsTag;
                    links.Add(link);
                }
                //
                // Enclosure
                else if (reader.IsStartElement(Rss20Constants.EnclosureTag, Rss20Constants.Rss20Namespace))
                {
                    SyndicationLink link = ParseLink(reader);
                    link.RelationshipType = Rss20Constants.EnclosureTag;
                    links.Add(link);
                }
                //
                // Guid
                else if (reader.IsStartElement(Rss20Constants.GuidTag, Rss20Constants.Rss20Namespace))
                {
                    string permalinkString = reader.GetAttribute(Rss20Constants.IsPermaLinkTag, Rss20Constants.Rss20Namespace);
                    bool isPermalink = (permalinkString == null) || permalinkString.Equals("false", StringComparison.OrdinalIgnoreCase);

                    item.Id = reader.ReadElementContentAsString();

                    if (isPermalink)
                    {
                        fallbackAlternateLink = item.Id;
                    }
                }
                //
                // PubDate
                else if (reader.IsStartElement(Rss20Constants.PubDateTag, Rss20Constants.Rss20Namespace))
                {
                    if (!reader.IsEmptyElement)
                    {
                        DateTimeOffset dt;
                        if (TryParseValue(new SyndicationContent(reader.ReadOuterXml()).GetValue(), out dt))
                        {
                            item.Published = dt;
                        }
                    }
                }
                //
                // Source
                else if (reader.IsStartElement(Rss20Constants.SourceTag, Rss20Constants.Rss20Namespace))
                {
                    SyndicationLink link = ParseLink(reader);
                    link.RelationshipType = Rss20Constants.SourceTag;
                    links.Add(link);
                }
                else
                {
                    // Skip Unknown tags
                    reader.Skip();
                }
            }

            reader.ReadEndElement(); // end

            // Add a fallback Link
            if (!readAlternateLink && fallbackAlternateLink != null)
            {
                Uri url;
                if (TryParseValue(fallbackAlternateLink, out url))
                {
                    var link = new SyndicationLink(url);
                    links.Add(link);
                }
            }

            item.Links = links;
            item.Contributors = contributors;
            item.Categories = categories;
        }
        
        private XmlReader CreateXmlReader(string value)
        {
            return XmlReader.Create(new StringReader(value), 
                                    new XmlReaderSettings()
                                    {
                                        IgnoreProcessingInstructions = true
                                    });
        }

        private bool TryParseRssDate(string value, out DateTimeOffset result)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            StringBuilder sb = new StringBuilder(value.Trim());

            if (sb.Length < 18)
            {
                return false;
            }

            if (sb[3] == ',')
            {
                // There is a leading (e.g.) "Tue, ", strip it off
                sb.Remove(0, 4);

                // There's supposed to be a space here but some implementations dont have one
                TrimStart(sb);
            }

            CollapseWhitespaces(sb);

            if (!char.IsDigit(sb[1]))
            {
                sb.Insert(0, '0');
            }

            if (sb.Length < 19)
            {
                return false;
            }

            bool thereAreSeconds = (sb[17] == ':');
            int timeZoneStartIndex = thereAreSeconds ? 21 : 18;

            string timeZoneSuffix = sb.ToString().Substring(timeZoneStartIndex);
            sb.Remove(timeZoneStartIndex, sb.Length - timeZoneStartIndex);

            bool isUtc;
            sb.Append(NormalizeTimeZone(timeZoneSuffix, out isUtc));

            string wellFormattedString = sb.ToString();

            string parseFormat = thereAreSeconds ? "dd MMM yyyy HH:mm:ss zzz" : "dd MMM yyyy HH:mm zzz";

            return DateTimeOffset.TryParseExact(wellFormattedString,
                                                parseFormat,
                                                CultureInfo.InvariantCulture.DateTimeFormat,
                                                isUtc ? DateTimeStyles.AdjustToUniversal : DateTimeStyles.None,
                                                out result);
        }

        private string NormalizeTimeZone(string rfc822TimeZone, out bool isUtc)
        {
            isUtc = false;
            // return a string in "-08:00" format
            if (rfc822TimeZone[0] == '+' || rfc822TimeZone[0] == '-')
            {
                // the time zone is supposed to be 4 digits but some feeds omit the initial 0
                StringBuilder result = new StringBuilder(rfc822TimeZone);
                if (result.Length == 4)
                {
                    // the timezone is +/-HMM. Convert to +/-HHMM
                    result.Insert(1, '0');
                }
                result.Insert(3, ':');
                return result.ToString();
            }
            switch (rfc822TimeZone)
            {
                case "UT":
                case "Z":
                    isUtc = true;
                    return "-00:00";
                case "GMT":
                    return "-00:00";
                case "A":
                    return "-01:00";
                case "B":
                    return "-02:00";
                case "C":
                    return "-03:00";
                case "D":
                case "EDT":
                    return "-04:00";
                case "E":
                case "EST":
                case "CDT":
                    return "-05:00";
                case "F":
                case "CST":
                case "MDT":
                    return "-06:00";
                case "G":
                case "MST":
                case "PDT":
                    return "-07:00";
                case "H":
                case "PST":
                    return "-08:00";
                case "I":
                    return "-09:00";
                case "K":
                    return "-10:00";
                case "L":
                    return "-11:00";
                case "M":
                    return "-12:00";
                case "N":
                    return "+01:00";
                case "O":
                    return "+02:00";
                case "P":
                    return "+03:00";
                case "Q":
                    return "+04:00";
                case "R":
                    return "+05:00";
                case "S":
                    return "+06:00";
                case "T":
                    return "+07:00";
                case "U":
                    return "+08:00";
                case "V":
                    return "+09:00";
                case "W":
                    return "+10:00";
                case "X":
                    return "+11:00";
                case "Y":
                    return "+12:00";
                default:
                    return "";
            }
        }

        private void TrimStart(StringBuilder sb)
        {
            int i = 0;
            while (i < sb.Length)
            {
                if (!char.IsWhiteSpace(sb[i]))
                {
                    break;
                }
                ++i;
            }
            if (i > 0)
            {
                sb.Remove(0, i);
            }
        }

        private void CollapseWhitespaces(StringBuilder builder)
        {
            int index = 0;
            int whiteSpaceStart = -1;
            while (index < builder.Length)
            {
                if (char.IsWhiteSpace(builder[index]))
                {
                    if (whiteSpaceStart < 0)
                    {
                        whiteSpaceStart = index;
                        // normalize all white spaces to be ' ' so that the date time parsing works
                        builder[index] = ' ';
                    }
                }
                else if (whiteSpaceStart >= 0)
                {
                    if (index > whiteSpaceStart + 1)
                    {
                        // there are at least 2 spaces... replace by 1
                        builder.Remove(whiteSpaceStart, index - whiteSpaceStart - 1);
                        index = whiteSpaceStart + 1;
                    }
                    whiteSpaceStart = -1;
                }
                ++index;
            }
            // we have already trimmed the start and end so there cannot be a trail of white spaces in the end
            //Fx.Assert(builder.Length == 0 || builder[builder.Length - 1] != ' ', "The string builder doesnt end in a white space");
        }
    }
}