// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.SyndicationFeed.Rss;
using System;
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
            using (var xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\rss20.xml", new XmlReaderSettings() { Async = true }))
            {
                var reader = new Rss20FeedReader(xmlReader);

                while (await reader.Read())
                {
                    if (reader.ElementType == SyndicationElementType.Item)
                    {

                        // Read as content
                        ISyndicationContent content = await reader.ReadContent();
                        //ISyndicationItem item = await reader.ReadItem();

                        var fields = content.Fields.ToArray();
                        Assert.Equal(fields.Length, 6);

                        Assert.Equal("title", fields[0].Name);
                        Assert.False(string.IsNullOrEmpty(fields[0].Value));

                        Assert.Equal("description", fields[1].Name);
                        Assert.False(string.IsNullOrEmpty(fields[1].Value));

                        Assert.Equal("link", fields[2].Name);
                        Assert.False(string.IsNullOrEmpty(fields[2].Value));

                        Assert.Equal("guid", fields[3].Name);
                        Assert.Equal(fields[3].Attributes.Count(), 1);
                        Assert.False(string.IsNullOrEmpty(fields[3].Value));

                        Assert.Equal("dc:creator", fields[4].Name);
                        Assert.False(string.IsNullOrEmpty(fields[4].Value));

                        Assert.Equal("pubDate", fields[5].Name);
                        Assert.False(string.IsNullOrEmpty(fields[5].Value));
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
                            break;

                        case SyndicationElementType.Item:
                            ISyndicationItem item = await reader.ReadItem();
                            break;

                        case SyndicationElementType.Person:
                            ISyndicationPerson person = await reader.ReadPerson();
                            break;

                        case SyndicationElementType.Image:
                            ISyndicationImage image = await reader.ReadImage();
                            break;

                        default:
                            ISyndicationContent content = await reader.ReadContent();
                            break;
                    }
                }
            }
        }

        [Fact]
        public async Task ReadFeedElements()
        {
            using (var xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\rss20-2items.xml", new XmlReaderSettings() { Async = true }))
            {
                var reader = new Rss20FeedReader(xmlReader);
                int items = 0;
                while (await reader.Read())
                {
                    switch (reader.ElementType)
                    {
                        case SyndicationElementType.Person:
                            ISyndicationPerson person = await reader.ReadPerson();
                            Assert.True(person.Email == "John Smith");
                            break;

                        case SyndicationElementType.Link:
                            ISyndicationLink link = await reader.ReadLink();
                            Assert.True(link.Length == 123);
                            Assert.True(link.MediaType == "testType");
                            Assert.True(link.Uri.OriginalString == "http://example.com/");
                            break;

                        case SyndicationElementType.Image:
                            ISyndicationImage image = await reader.ReadImage();
                            Assert.True(image.Title == "Microsoft News");
                            Assert.True(image.Description == "Test description");
                            Assert.True(image.Url.OriginalString == "http://2.bp.blogspot.com/-NA5Jb-64eUg/URx8CSdcj_I/AAAAAAAAAUo/eCx0irI0rq0/s1600/bg_Microsoft_logo3-20120824073001907469-620x349.jpg");
                            break;

                        case SyndicationElementType.Item:
                            items++;
                            ISyndicationItem item = await reader.ReadItem();

                            if (items == 1)
                            {
                                Assert.True(item.Title == "Lorem ipsum 2017-07-06T20:25:00+00:00");
                                Assert.True(item.Description == "Exercitation sit dolore mollit et est eiusmod veniam aute officia veniam ipsum.");
                                Assert.True(item.Links.Count() == 2);
                            }
                            else if(items == 2)
                            {
                                Assert.True(item.Title == "Lorem ipsum 2017-07-06T20:24:00+00:00");
                                Assert.True(item.Description == "Do ipsum dolore veniam minim est cillum aliqua ea.");
                                Assert.True(item.Links.Count() == 2);
                            }

                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }
}
