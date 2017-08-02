// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Microsoft.SyndicationFeed.Rss
{
    public class RssWriter
    {
        [Fact]
        public async Task Rss20Writer_WriteCategory()
        {

            StringBuilder sb = new StringBuilder();

            using (XmlWriter xmlWriter = XmlWriter.Create(sb))
            {
                Rss20FeedWriter writer = new Rss20FeedWriter(xmlWriter, new Rss20Formatter(xmlWriter.Settings));
                SyndicationCategory category = new SyndicationCategory("Test Category");
                await writer.Write(category);
                xmlWriter.Flush();
            }

            string res = sb.ToString();
            Assert.True(res == "<?xml version=\"1.0\" encoding=\"utf-16\"?><rss version=\"2.0\"><channel><category>Test Category</category></channel></rss>");
        }

        [Fact]
        public async Task Rss20Writer_WritePerson()
        {

            StringBuilder sb = new StringBuilder();

            using (XmlWriter xmlWriter = XmlWriter.Create(sb))
            {
                xmlWriter.WriteStartElement("document");
                Rss20FeedWriter writer = new Rss20FeedWriter(xmlWriter, new Rss20Formatter(xmlWriter.Settings));
                SyndicationPerson author = new SyndicationPerson()
                {
                    Email = "author@email.com",
                    RelationshipType = Rss20ContributorTypes.Author
                };

                SyndicationPerson managingEditor = new SyndicationPerson()
                {
                    Email = "mEditor@email.com",
                    RelationshipType = Rss20ContributorTypes.ManagingEditor
                };

                await writer.Write(author);
                await writer.Write(managingEditor);

                xmlWriter.WriteEndElement();
                xmlWriter.Flush();
            }

            string res = sb.ToString();
            Assert.True(res == "<?xml version=\"1.0\" encoding=\"utf-16\"?><document><rss version=\"2.0\"><channel><author>author@email.com</author><managingEditor>mEditor@email.com</managingEditor></channel></rss></document>");
        }

        [Fact]
        public async Task Rss20Writer_WriteImage()
        {

            StringBuilder sb = new StringBuilder();

            using (XmlWriter xmlWriter = XmlWriter.Create(sb))
            {

                Rss20FeedWriter writer = new Rss20FeedWriter(xmlWriter, new Rss20Formatter(xmlWriter.Settings));

                Uri url = new Uri("http://testuriforimage.com");
                Uri urlForLink = new Uri("http://testuriforlink.com");
                SyndicationLink link = new SyndicationLink(urlForLink, Rss20LinkTypes.Alternate);

                SyndicationImage image = new SyndicationImage(url)
                {
                    Title = "Testing image title",
                    Description = "testing image description",
                    Link = link
                };

                await writer.Write(image);
                
                xmlWriter.Flush();
            }

            string res = sb.ToString();
            Assert.True(res == "<?xml version=\"1.0\" encoding=\"utf-16\"?><rss version=\"2.0\"><channel><image><url>http://testuriforimage.com/</url><title>Testing image title</title><link>http://testuriforlink.com/</link><description>testing image description</description></image></channel></rss>");
        }

        [Fact]
        public async Task Rss20Writer_WriteLink_onlyUrl()
        {

            StringBuilder sb = new StringBuilder();

            using (XmlWriter xmlWriter = XmlWriter.Create(sb))
            {
                
                Rss20FeedWriter writer = new Rss20FeedWriter(xmlWriter, new Rss20Formatter(xmlWriter.Settings));
                
                Uri urlForLink = new Uri("http://testuriforlink.com");
                SyndicationLink link = new SyndicationLink(urlForLink, Rss20LinkTypes.Alternate);
                
                await writer.Write(link);
                
                xmlWriter.Flush();
            }

            string res = sb.ToString();
            Assert.True(res == "<?xml version=\"1.0\" encoding=\"utf-16\"?><rss version=\"2.0\"><channel><link>http://testuriforlink.com/</link></channel></rss>");
        }

        [Fact]
        public async Task Rss20Writer_WriteLink_allElements()
        {

            StringBuilder sb = new StringBuilder();

            using (XmlWriter xmlWriter = XmlWriter.Create(sb))
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

            string res = sb.ToString();
            Assert.True(res == "<?xml version=\"1.0\" encoding=\"utf-16\"?><rss version=\"2.0\"><channel><link length=\"123\" type=\"mp3/video\" url=\"http://testuriforlink.com/\">Test title</link></channel></rss>");
        }

        [Fact]
        public async Task Rss20Writer_WriteLink_uriEqualsTitle()
        {

            StringBuilder sb = new StringBuilder();

            using (XmlWriter xmlWriter = XmlWriter.Create(sb))
            {

                Rss20FeedWriter writer = new Rss20FeedWriter(xmlWriter, new Rss20Formatter(xmlWriter.Settings));

                Uri urlForLink = new Uri("http://testuriforlink.com");
                SyndicationLink link = new SyndicationLink(urlForLink, Rss20LinkTypes.Alternate)
                {
                    Title = "http://testuriforlink.com"
                };

                await writer.Write(link);

                xmlWriter.Flush();
            }

            string res = sb.ToString();
            Assert.True(res == "<?xml version=\"1.0\" encoding=\"utf-16\"?><rss version=\"2.0\"><channel><link>http://testuriforlink.com</link></channel></rss>");
        }

        [Fact]
        public async Task Rss20Writer_WriteItem()
        {

            StringBuilder sb = new StringBuilder();

            using (XmlWriter xmlWriter = XmlWriter.Create(sb))
            {

                Rss20FeedWriter writer = new Rss20FeedWriter(xmlWriter, new Rss20Formatter(xmlWriter.Settings));

                Uri url = new Uri("http://testuriforlinks.com");
                SyndicationLink link = new SyndicationLink(url, Rss20LinkTypes.Alternate);

                SyndicationLink enclosureLink = new SyndicationLink(url, Rss20LinkTypes.Enclosure)
                {
                    Title = "http://enclosurelink.com",
                    Length = 4123,
                    MediaType = "audio/mpeg"
                };

                SyndicationLink commentsLink = new SyndicationLink(url, Rss20LinkTypes.Comments);

                SyndicationLink sourceLink = new SyndicationLink(url, Rss20LinkTypes.Source)
                {
                    Title = "Anonymous Blog"
                };

                SyndicationLink guidLink = new SyndicationLink(url, Rss20LinkTypes.Guid);

                SyndicationItem item = new SyndicationItem();

                item.Title = "First item on ItemWriter";
                item.AddLink(link);
                item.AddLink(enclosureLink);
                item.AddLink(commentsLink);
                item.AddLink(sourceLink);
                item.AddLink(guidLink);


                item.Description = "Brief description of an item";

                item.AddContributor(new SyndicationPerson()
                {
                    Email = "person@email.com",
                    RelationshipType =  Rss20ContributorTypes.Author
                });

                item.Id = "Unique ID for this item";

                DateTimeOffset time;
                DateTimeOffset.TryParse("Fri, 28 Jul 2017 19:07:32 GMT",out time);

                item.Published = time;
                
                await writer.Write(item);

                xmlWriter.Flush();
            }

            string res = sb.ToString();
            Assert.True(res == "<?xml version=\"1.0\" encoding=\"utf-16\"?><rss version=\"2.0\"><channel><item><title>First item on ItemWriter</title><link>http://testuriforlinks.com/</link><enclosure url=\"http://testuriforlinks.com/\" length=\"4123\" type=\"audio/mpeg\" /><comments>http://testuriforlinks.com/</comments><source url=\"http://testuriforlinks.com/\">Anonymous Blog</source><description>Brief description of an item</description><author>person@email.com</author><guid isPermaLink=\"true\">Unique ID for this item</guid><pubDate>Fri, 28 Jul 2017 19:07:32 GMT</pubDate></item></channel></rss>", res);
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
        public async Task Rss20Writer_ReadAndWriteFeed()
        {
            string res = null;
            using (XmlReader xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\rss20-2items.xml", new XmlReaderSettings() { Async = true }))
            {
                var reader = new Rss20FeedReader(xmlReader);

                var sb = new StringBuilder();

                using (XmlWriter xmlWriter = XmlWriter.Create(sb))
                {

                    var writer = new Rss20FeedWriter(xmlWriter);


                    while (await reader.Read())
                    {
                        switch (reader.ElementType)
                        {
                            case SyndicationElementType.Link:
                                ISyndicationLink link = await reader.ReadLink();
                                await writer.Write(link);
                                break;

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
                res = sb.ToString();
                Assert.True(res == "<?xml version=\"1.0\" encoding=\"utf-16\"?><rss version=\"2.0\"><channel><title asd=\"123\">Lorem ipsum feed for an interval of 1 minutes</title><description>This is a constantly updating lorem ipsum feed</description><link length=\"123\" type=\"testType\">http://example.com/</link><image><url>http://2.bp.blogspot.com/-NA5Jb-64eUg/URx8CSdcj_I/AAAAAAAAAUo/eCx0irI0rq0/s1600/bg_Microsoft_logo3-20120824073001907469-620x349.jpg</url><title>Microsoft News</title><link>http://www.microsoft.com/news</link><description>Test description</description></image><generator>RSS for Node</generator><lastBuildDate>Thu, 06 Jul 2017 20:25:17 GMT</lastBuildDate><managingEditor>John Smith</managingEditor><pubDate>Thu, 06 Jul 2017 20:25:00 GMT</pubDate><copyright>Michael Bertolacci, licensed under a Creative Commons Attribution 3.0 Unported License.</copyright><ttl>60</ttl><item><title>Lorem ipsum 2017-07-06T20:25:00+00:00</title><enclosure url=\"http://www.scripting.com/mp3s/weatherReportSuite.mp3\" length=\"12216320\" type=\"audio/mpeg\" /><link>http://example.com/test/1499372700</link><description>Exercitation sit dolore mollit et est eiusmod veniam aute officia veniam ipsum.</description><guid isPermaLink=\"true\">http://example.com/test/1499372700</guid><pubDate>Thu, 06 Jul 2017 20:25:00 GMT</pubDate></item><item><title>Lorem ipsum 2017-07-06T20:24:00+00:00</title><link>http://example.com/test/1499372640</link><enclosure url=\"http://www.scripting.com/mp3s/weatherReportSuite.mp3\" length=\"12216320\" type=\"audio/mpeg\" /><description>Do ipsum dolore veniam minim est cillum aliqua ea.</description><guid isPermaLink=\"true\">http://example.com/test/1499372640</guid><pubDate>Thu, 06 Jul 2017 20:24:00 GMT</pubDate></item></channel></rss>");
            }
        }
    }
}
