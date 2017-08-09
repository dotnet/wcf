// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.SyndicationFeed.Tests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Microsoft.SyndicationFeed.Rss
{
    public class RssWriter
    {
        sealed class StringWriterWithEncoding : StringWriter
        {
            private readonly Encoding _encoding;

            public StringWriterWithEncoding(Encoding encoding)
            {
                this._encoding = encoding;
            }

            public override Encoding Encoding
            {
                get { return _encoding; }
            }
        }

        [Fact]
        public async Task Rss20Writer_WriteCategory()
        {

            var sw = new StringWriterWithEncoding(Encoding.UTF8);

            using (XmlWriter xmlWriter = XmlWriter.Create(sw))
            {
                Rss20FeedWriter writer = new Rss20FeedWriter(xmlWriter, new Rss20Formatter(xmlWriter.Settings));
                SyndicationCategory category = new SyndicationCategory("Test Category");
                await writer.Write(category);
                xmlWriter.Flush();
            }

            string res = sw.ToString();
            Assert.True(res == "<?xml version=\"1.0\" encoding=\"utf-8\"?><rss version=\"2.0\"><channel><category>Test Category</category></channel></rss>");
        }

        [Fact]
        public async Task Rss20Writer_WritePerson()
        {
            var sw = new StringWriterWithEncoding(Encoding.UTF8);

            using (XmlWriter xmlWriter = XmlWriter.Create(sw))
            {                
                var writer = new Rss20FeedWriter(xmlWriter, new Rss20Formatter());

                await writer.Write(new SyndicationPerson("author", "author@email.com"));
                await writer.Write(new SyndicationPerson("mEditor", "mEditor@email.com", Rss20ContributorTypes.ManagingEditor));

                xmlWriter.WriteEndElement();
                xmlWriter.Flush();
            }

            string res = sw.ToString();
            Assert.True(res == "<?xml version=\"1.0\" encoding=\"utf-8\"?><rss version=\"2.0\"><channel><author>author@email.com</author><managingEditor>mEditor@email.com</managingEditor></channel></rss>");
        }

        [Fact]
        public async Task Rss20Writer_WriteImage()
        {
            Uri uri = new Uri("http://testuriforlink.com");

            var sw = new StringWriterWithEncoding(Encoding.UTF8);

            using (XmlWriter xmlWriter = XmlWriter.Create(sw))
            {

                Rss20FeedWriter writer = new Rss20FeedWriter(xmlWriter, new Rss20Formatter(xmlWriter.Settings));

                var image = new SyndicationImage(uri)
                {
                    Title = "Testing image title",
                    Description = "testing image description",
                    Link = new SyndicationLink(uri)
                };

                await writer.Write(image);
                
                xmlWriter.Flush();
            }

            string res = sw.ToString();
            Assert.True(res == $"<?xml version=\"1.0\" encoding=\"utf-8\"?><rss version=\"2.0\"><channel><image><url>{uri}</url><title>Testing image title</title><link>{uri}</link><description>testing image description</description></image></channel></rss>");
        }

        [Fact]
        public async Task Rss20Writer_WriteLink_onlyUrl()
        {

            var sw = new StringWriterWithEncoding(Encoding.UTF8);

            using (XmlWriter xmlWriter = XmlWriter.Create(sw))
            {
                
                Rss20FeedWriter writer = new Rss20FeedWriter(xmlWriter, new Rss20Formatter(xmlWriter.Settings));
                
                Uri urlForLink = new Uri("http://testuriforlink.com");
                SyndicationLink link = new SyndicationLink(urlForLink, Rss20LinkTypes.Alternate);
                
                await writer.Write(link);
                
                xmlWriter.Flush();
            }

            string res = sw.ToString();
            Assert.True(res == "<?xml version=\"1.0\" encoding=\"utf-8\"?><rss version=\"2.0\"><channel><link>http://testuriforlink.com/</link></channel></rss>");
        }

        [Fact]
        public async Task Rss20Writer_WriteLink_allElements()
        {

            var sw = new StringWriterWithEncoding(Encoding.UTF8);

            using (XmlWriter xmlWriter = XmlWriter.Create(sw))
            {

                Rss20FeedWriter writer = new Rss20FeedWriter(xmlWriter, new Rss20Formatter(xmlWriter.Settings));

                Uri urlForLink = new Uri("http://testuriforlink.com");
                SyndicationLink link = new SyndicationLink(urlForLink, Rss20LinkTypes.Alternate)
                {
                    Title = "Test title",
                    Length = 123,
                    MediaType = "mp3/video"
                };

                await writer.Write(link);

                xmlWriter.Flush();
            }

            string res = sw.ToString();
            Assert.True(res == "<?xml version=\"1.0\" encoding=\"utf-8\"?><rss version=\"2.0\"><channel><link length=\"123\" type=\"mp3/video\" url=\"http://testuriforlink.com/\">Test title</link></channel></rss>");
        }

             
        [Fact]
        public async Task Rss20Writer_WriteItem()
        {
            var url = new Uri("https://contoso.com/");

            // 
            // Construct item
            var item = new SyndicationItem()
            {
                Id = "https://contoso.com/28af09b3-86c7-4dd6-b56f-58aaa17cff62",
                Title = "First item on ItemWriter",
                Description = "Brief description of an item",
                Published = DateTimeOffset.UtcNow
            };

            item.AddLink(new SyndicationLink(url));
            item.AddLink(new SyndicationLink(url, Rss20LinkTypes.Enclosure)
            {
                Title = "https://contoso.com/",
                Length = 4123,
                MediaType = "audio/mpeg"
            });
            item.AddLink(new SyndicationLink(url, Rss20LinkTypes.Comments));
            item.AddLink(new SyndicationLink(url, Rss20LinkTypes.Source)
            {
                Title = "Anonymous Blog"
            });

            item.AddLink(new SyndicationLink(new Uri(item.Id), Rss20LinkTypes.Guid));

            item.AddContributor(new SyndicationPerson("person", "person@email.com"));

            //
            // Write
            var sw = new StringWriterWithEncoding(Encoding.UTF8);

            using (XmlWriter xmlWriter = XmlWriter.Create(sw))
            {
                await new Rss20FeedWriter(xmlWriter).Write(item);
                xmlWriter.Flush();
            }

            string res = sw.ToString();
            Assert.True(res == $"<?xml version=\"1.0\" encoding=\"utf-8\"?><rss version=\"2.0\"><channel><item><title>First item on ItemWriter</title><link>{url}</link><enclosure url=\"{url}\" length=\"4123\" type=\"audio/mpeg\" /><comments>{url}</comments><source url=\"{url}\">Anonymous Blog</source><guid>{item.Id}</guid><description>Brief description of an item</description><author>person@email.com</author><pubDate>{item.Published.ToString("r")}</pubDate></item></channel></rss>", res);
        }

        [Fact]
        public async Task Rss20Writer_WriteContent()
        {
            SyndicationContent content = null;

            using (XmlReader xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\CustomXml.xml"))
            {
                Rss20FeedReader reader = new Rss20FeedReader(xmlReader);
                content = (SyndicationContent)await reader.ReadContent();
            }

            StringBuilder sb = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(sb))
            {
                Rss20FeedWriter writer = new Rss20FeedWriter(xmlWriter, new Rss20Formatter(xmlWriter.Settings));

                await writer.Write(content);
                xmlWriter.Flush();
            }

            string res = sb.ToString();
            Assert.True(res == "<?xml version=\"1.0\" encoding=\"utf-16\"?><rss version=\"2.0\"><channel><NewItem><enclosure url=\"http://www.scripting.com/mp3s/weatherReportSuite.mp3\" length=\"12216320\" type=\"audio/mpeg\" /><title>Lorem ipsum 2017-07-06T20:25:00+00:00</title><description>Exercitation sit dolore mollit et est eiusmod veniam aute officia veniam ipsum.</description><link>http://example.com/test/1499372700</link><guid isPermaLink=\"true\">http://example.com/test/1499372700</guid><pubDate>Thu, 06 Jul 2017 20:25:00 GMT</pubDate></NewItem></channel></rss>");
        }

        [Fact]
        public async Task Rss20Writer_WriteValue()
        {
            var sb = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(sb))
            {
                var writer = new Rss20FeedWriter(xmlWriter, new Rss20Formatter(xmlWriter.Settings));
                await writer.WriteValue("CustomTag", "Custom Content");
                xmlWriter.Flush();
            }

            var res = sb.ToString();
            Assert.True(res == "<?xml version=\"1.0\" encoding=\"utf-16\"?><rss version=\"2.0\"><channel><CustomTag>Custom Content</CustomTag></channel></rss>");
        }
        
        [Fact]
        public async Task Rss20Writer_ReadAndWriteFeed_TestResultWithRss20Test()
        {
            string res = null;
            using (XmlReader xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\rss20-2items.xml", new XmlReaderSettings() { Async = true }))
            {
                var reader = new Rss20FeedReader(xmlReader);

                var sw = new StringWriterWithEncoding(Encoding.UTF8);

                using (XmlWriter xmlWriter = XmlWriter.Create(sw))
                {

                    var writer = new Rss20FeedWriter(xmlWriter);


                    while (await reader.Read())
                    {
                        switch (reader.ElementType)
                        {
                            case SyndicationElementType.Item:
                                ISyndicationItem item = await reader.ReadItem();
                                await writer.Write(item);
                                break;

                            case SyndicationElementType.Person:
                                ISyndicationPerson person = await reader.ReadPerson();
                                await writer.Write(person);
                                break;

                            case SyndicationElementType.Image:
                                ISyndicationImage image = await reader.ReadImage();
                                await writer.Write(image);
                                break;

                            default:
                                ISyndicationContent content = await reader.ReadContent();
                                await writer.Write(content);
                                break;
                        }
                    }

                    xmlWriter.Flush();
                }
                res = sw.ToString();
                Assert.True(res == "<?xml version=\"1.0\" encoding=\"utf-8\"?><rss version=\"2.0\"><channel><title asd=\"123\">Lorem ipsum feed for an interval of 1 minutes</title><description>This is a constantly updating lorem ipsum feed</description><link length=\"123\" type=\"testType\">http://example.com/</link><image><url>http://2.bp.blogspot.com/-NA5Jb-64eUg/URx8CSdcj_I/AAAAAAAAAUo/eCx0irI0rq0/s1600/bg_Microsoft_logo3-20120824073001907469-620x349.jpg</url><title>Microsoft News</title><link>http://www.microsoft.com/news</link><description>Test description</description></image><generator>RSS for Node</generator><lastBuildDate>Thu, 06 Jul 2017 20:25:17 GMT</lastBuildDate><managingEditor>John Smith</managingEditor><pubDate>Thu, 06 Jul 2017 20:25:00 GMT</pubDate><copyright>Michael Bertolacci, licensed under a Creative Commons Attribution 3.0 Unported License.</copyright><ttl>60</ttl><item><title>Lorem ipsum 2017-07-06T20:25:00+00:00</title><enclosure url=\"http://www.scripting.com/mp3s/weatherReportSuite.mp3\" length=\"12216320\" type=\"audio/mpeg\" /><link>http://example.com/test/1499372700</link><guid>http://example.com/test/1499372700</guid><description>Exercitation sit dolore mollit et est eiusmod veniam aute officia veniam ipsum.</description><author>John Smith</author><pubDate>Thu, 06 Jul 2017 20:25:00 GMT</pubDate></item><item><title>Lorem ipsum 2017-07-06T20:24:00+00:00</title><link>http://example.com/test/1499372640</link><guid>http://example.com/test/1499372640</guid><enclosure url=\"http://www.scripting.com/mp3s/weatherReportSuite.mp3\" length=\"12216320\" type=\"audio/mpeg\" /><description>Do ipsum dolore veniam minim est cillum aliqua ea.</description><author>John Smith</author><pubDate>Thu, 06 Jul 2017 20:24:00 GMT</pubDate></item></channel></rss>");
            }

            XmlReader newReader = XmlReader.Create(new StringReader(res));
            await RSS20.TestReadFeedElements(newReader);
        }

        [Fact]
        public async Task Rss20Writer_CompareContents()
        {
            string filePath = @"..\..\..\TestFeeds\internetRssFeed.xml";
            string res = null;
            using (XmlReader xmlReader = XmlReader.Create(filePath, new XmlReaderSettings() { Async = true }))
            {
                var reader = new Rss20FeedReader(xmlReader);

                var sw = new StringWriterWithEncoding(Encoding.UTF8);

                using (XmlWriter xmlWriter = XmlWriter.Create(sw))
                {
                    var writer = new Rss20FeedWriter(xmlWriter);

                    while (await reader.Read())
                    {
                        switch (reader.ElementType)
                        {
                            case SyndicationElementType.Item:
                                ISyndicationItem item = await reader.ReadItem();
                                await writer.Write(item);
                                break;

                            case SyndicationElementType.Person:
                                ISyndicationPerson person = await reader.ReadPerson();
                                await writer.Write(person);
                                break;

                            case SyndicationElementType.Image:
                                ISyndicationImage image = await reader.ReadImage();
                                await writer.Write(image);
                                break;

                            default:
                                ISyndicationContent content = await reader.ReadContent();
                                await writer.Write(content);
                                break;
                        }
                    }

                    xmlWriter.Flush();
                }
                res = sw.ToString();
            }

            
            var originalReader = XmlReader.Create(filePath);
            var resReader = XmlReader.Create(new StringReader(res));

            await CompareFeeds(new Rss20FeedReader(originalReader), new Rss20FeedReader(resReader));
            
        }

        private async Task CompareFeeds(ISyndicationFeedReader f1, ISyndicationFeedReader f2)
        {
            while (await f1.Read() && await f2.Read())
            {
                Assert.True(f1.ElementType == f2.ElementType);

                switch (f1.ElementType)
                {
                    case SyndicationElementType.Item:
                        ISyndicationItem item1 = await f1.ReadItem();
                        ISyndicationItem item2 = await f2.ReadItem();
                        CompareItem(item1, item2);
                        break;

                    case SyndicationElementType.Person:
                        ISyndicationPerson person1 = await f1.ReadPerson();
                        ISyndicationPerson person2 = await f2.ReadPerson();
                        ComparePerson(person1, person2);
                        break;

                    case SyndicationElementType.Image:
                        ISyndicationImage image1 = await f1.ReadImage();
                        ISyndicationImage image2 = await f2.ReadImage();
                        CompareImage(image1, image2);
                        break;

                    default:
                        ISyndicationContent content1 = await f1.ReadContent();
                        ISyndicationContent content2 = await f2.ReadContent();
                        CompareContent(content1, content2);
                        break;
                }
            }
        }

        [Fact]
        public async Task Rss20Writer_WriteNamespaces()
        {

            StringWriterWithEncoding sw = new StringWriterWithEncoding(Encoding.UTF8);

            using (XmlWriter xmlWriter = XmlWriter.Create(sw))
            {

                var list = new List<SyndicationAttribute>()
                {
                    new SyndicationAttribute("xmlns:content", "http://purl.org/rss/1.0/modules/content/"),
                    new SyndicationAttribute("xmlns:media", "http://search.yahoo.com/mrss/"),
                };

                Rss20FeedWriter writer = new Rss20FeedWriter(xmlWriter, new Rss20Formatter(), list);

                await writer.WriteValue("hello", "world");
            }

            string res = sw.ToString();
            Assert.True(res == "<?xml version=\"1.0\" encoding=\"utf-8\"?><rss xmlns:content=\"http://purl.org/rss/1.0/modules/content/\" xmlns:media=\"http://search.yahoo.com/mrss/\" version=\"2.0\"><channel><hello>world</hello></channel></rss>");
        }

        void ComparePerson(ISyndicationPerson person1, ISyndicationPerson person2)
        {
            Assert.True(person1.Email == person2.Email);
            Assert.True(person1.RelationshipType == person2.RelationshipType);
        }

        void CompareImage(ISyndicationImage image1, ISyndicationImage image2)
        {
            Assert.True(image1.RelationshipType == image2.RelationshipType);
            Assert.True(image1.Url == image2.Url);
            Assert.True(image1.Link.Uri.ToString() == image2.Link.Uri.ToString());
            Assert.True(image1.Description == image2.Description);
        }

        void CompareItem(ISyndicationItem item1, ISyndicationItem item2)
        {
            Assert.True(item1.Id == item2.Id);
            Assert.True(item1.Title == item2.Title);
            Assert.True(item1.LastUpdated == item2.LastUpdated);

        }

        void CompareContent(ISyndicationContent content1, ISyndicationContent content2)
        {
            Assert.True(content1.Name == content2.Name);

            //Compare attributes
            foreach (var a in content1.Attributes)
            {
                var a2 = content2.Attributes.Single(att => att.Name == a.Name);
                Assert.True(a.Name == a2.Name);
                Assert.True(a.Namespace == a2.Namespace);
                Assert.True(a.Value == a2.Value);
            }

            //Compare fields
            foreach (var f in content1.Fields)
            {
                var f2 = content2.Fields.Single(field => field.Name == f.Name && field.Value == f.Value);
                CompareContent(f, f2);
            }

            Assert.True(content1.Value == content2.Value);
        }
    }
}
