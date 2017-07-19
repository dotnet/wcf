// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;

namespace Microsoft.SyndicationFeed.Atom10
{
    class Atom10FeedReader : SyndicationFeedReaderBase
    {
        public Atom10FeedReader(XmlReader reader, ISyndicationFeedFormatter formatter) 
            : base(reader, formatter)
        {
        }

        protected override SyndicationElementType MapElementType(string elementName)
        {
            throw new NotImplementedException();
        }
    }
}
