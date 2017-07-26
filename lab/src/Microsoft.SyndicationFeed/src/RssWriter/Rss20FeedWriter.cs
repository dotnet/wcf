// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    public class Rss20FeedWriter : ISyndicationFeedWriter
    {
        private XmlWriter _writer;

        public ISyndicationFeedSerializer Serializer { get; private set; }


        public Rss20FeedWriter(XmlWriter writer)
            : this(writer, new Rss20Serializer())
        {
        }

        public Rss20FeedWriter(XmlWriter writer, ISyndicationFeedSerializer serializer)
        {
            _writer = writer;
            Serializer = serializer;
        }

        public virtual Task Write(ISyndicationContent content)
        {
            string xml = Serializer.Serialize(content);
            return XmlUtils.WriteRaw(_writer, xml);
        }

        public virtual Task Write(ISyndicationCategory category)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public virtual Task Write(ISyndicationLink link)
        {
            throw new NotImplementedException();
        }
    }
}
