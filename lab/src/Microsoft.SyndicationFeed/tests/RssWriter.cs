// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Microsoft.SyndicationFeed.Tests
{
    public class RssWriter
    {
        [Fact]
        public async Task Rss20Writer_WriteContent()
        {

            StringBuilder sb = new StringBuilder();

            using (XmlWriter xmlWriter = XmlWriter.Create(sb))
            {
                Rss20FeedWriter writer = new Rss20FeedWriter(xmlWriter);
                SyndicationContent content = new SyndicationContent("<title>Hello World</title>");
                await writer.Write(content);
                xmlWriter.Flush();
            }

            string res = sb.ToString();
            Assert.True(res == "<?xml version=\"1.0\" encoding=\"utf-16\"?><title>Hello World</title>");
        }
    }
}
