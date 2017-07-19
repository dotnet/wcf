// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    public class Atom10FeedFormatter : ISyndicationFeedFormatter
    {
        public ISyndicationCategory ParseCategory(string value)
        {
            throw new NotImplementedException();
        }

        public ISyndicationImage ParseImage(string value)
        {
            throw new NotImplementedException();
        }

        public ISyndicationItem ParseItem(string value)
        {
            throw new NotImplementedException();
        }

        public ISyndicationLink ParseLink(string value)
        {
            throw new NotImplementedException();
        }

        public ISyndicationPerson ParsePerson(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            using(XmlReader reader = XmlReader.Create(new StringReader(value)))
            {
                reader.MoveToContent();
                return ParsePerson(reader);
            }

        }

        public bool TryParseValue<T>(string value, out T result)
        {
            throw new NotImplementedException();
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
                    case Atom10Constants.NameTag:
                        person.Name = reader.ReadElementContentAsString();
                        break;

                    case Atom10Constants.EmailTag:
                        person.Email = reader.ReadElementContentAsString();
                        break;

                    case Atom10Constants.UriTag:
                        person.Uri = reader.ReadElementContentAsString();
                        break;
                }
            }

            reader.ReadEndElement(); //end of author / contributor

            return person;
        }

    }
}
