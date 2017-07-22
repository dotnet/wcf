// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Microsoft.SyndicationFeed.Tests
{
    public class RSS20
    {
        [Fact]
        public async Task ReadSequential()
        {
            using (var xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\rss20.xml", new XmlReaderSettings() { Async = true })) {
                var reader = new Rss20FeedReader(xmlReader);

                await reader.Read();

                ISyndicationContent content = await reader.ReadContent();
                content = await reader.ReadContent();
                content = await reader.ReadContent();
            }
        }

        [Fact]
        public async Task ReadItemAsContent()
        {
            using (var xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\rss20.xml", new XmlReaderSettings() { Async = true })) {
                var reader = new Rss20FeedReader(xmlReader);

                while (await reader.Read()) {
                    if (reader.ElementType == SyndicationElementType.Item) {
                        // Read as content
                        ISyndicationContent content = await reader.ReadContent();

                        var fields = content.Fields.ToArray();
                        Assert.Equal(fields.Length, 6);

                        Assert.Equal("title", fields[0].Name);
                        Assert.False(string.IsNullOrEmpty(fields[0].GetValue()));

                        Assert.Equal("description", fields[1].Name);
                        Assert.False(string.IsNullOrEmpty(fields[1].GetValue()));

                        Assert.Equal("link", fields[2].Name);
                        Assert.False(string.IsNullOrEmpty(fields[2].GetValue()));

                        Assert.Equal("guid", fields[3].Name);
                        Assert.Equal(fields[3].Attributes.Count(), 1);
                        Assert.False(string.IsNullOrEmpty(fields[3].GetValue()));

                        Assert.Equal("dc:creator", fields[4].Name);
                        Assert.False(string.IsNullOrEmpty(fields[4].GetValue()));

                        Assert.Equal("pubDate", fields[5].Name);
                        Assert.False(string.IsNullOrEmpty(fields[5].GetValue()));
                    }
                }
            }
        }

        [Fact]
        public async Task CountItems()
        {
            int itemCount = 0;

            using (var xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\rss20.xml", new XmlReaderSettings() { Async = true })) {
                var reader = new Rss20FeedReader(xmlReader);

                while (await reader.Read()) {
                    if (reader.ElementType == SyndicationElementType.Item) {
                        itemCount++;
                    }
                }
            }

            Assert.Equal(itemCount, 10);
        }

        [Fact]
        private async Task ReadWhile()
        {
            using (var xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\rss20.xml", new XmlReaderSettings() { Async = true }))
            {
                var reader = new Rss20FeedReader(xmlReader);

                while (await reader.Read())
                {
                    switch (reader.ElementType)
                    {
                        case SyndicationElementType.Link:
                            ISyndicationLink link = await reader.ReadLink();
                            Console.WriteLine(link);
                            break;

                        case SyndicationElementType.Item:
                            ISyndicationItem item = await reader.ReadItem();
                            Console.WriteLine(item);
                            break;

                        case SyndicationElementType.Person:
                            ISyndicationPerson person = await reader.ReadPerson();
                            Console.WriteLine(person);
                            break;

                        case SyndicationElementType.Image:
                            ISyndicationImage image = await reader.ReadImage();
                            Console.WriteLine(image);
                            break;

                        default:
                            ISyndicationContent content = await reader.ReadContent();
                            Console.WriteLine(content);
                            break;
                    }
                }
            }
        }
    }
}
