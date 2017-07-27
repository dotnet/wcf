// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    public class Rss20FeedWriter : ISyndicationFeedWriter
    {
        private XmlWriter _writer;
        public Rss20FeedWriter(XmlWriter writer)
            : this(writer, new Rss20Formatter(writer.Settings))
        {
        }

        public Rss20FeedWriter(XmlWriter writer, ISyndicationFeedFormatter formatter)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        }

        public ISyndicationFeedFormatter Formatter { get; private set; }

        public virtual Task Write(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            return XmlUtils.WriteRaw(_writer, Formatter.Format(content));
        }

        public virtual Task Write(ISyndicationCategory category)
        {
            if(category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }
            
            return XmlUtils.WriteRaw(_writer, Formatter.Format(category));
        }

        public virtual Task Write(ISyndicationImage image)
        {
            throw new NotImplementedException();
        }

        public virtual Task Write(ISyndicationItem item)
        {
            throw new NotImplementedException();
        }

        public virtual Task Write(ISyndicationPerson person)
        {

            if(person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            string res = Formatter.Format(person);

            XmlReader reader = XmlReader.Create(new StringReader(res));

            //_writer.WriteNode(reader, true);

            return XmlUtils.WriteRaw(_writer, res);
            //return Task.CompletedTask;
        }

        public virtual Task Write(ISyndicationLink link)
        {
            throw new NotImplementedException();
        }

        public Task WriteValue<T>(string name, T value)
        {
            throw new NotImplementedException();
        }

        public Task WriteElement(string content)
        {
            throw new NotImplementedException();
        }
    }
}
