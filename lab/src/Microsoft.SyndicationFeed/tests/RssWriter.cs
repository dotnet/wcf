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
                SyndicationLink link = new SyndicationLink(urlForLink, Rss20LinkTypes.Link);

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
                SyndicationLink link = new SyndicationLink(urlForLink, Rss20LinkTypes.Link);
                
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
                SyndicationLink link = new SyndicationLink(urlForLink, Rss20LinkTypes.Link)
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
                SyndicationLink link = new SyndicationLink(urlForLink, Rss20LinkTypes.Link)
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
                SyndicationLink link = new SyndicationLink(url, Rss20LinkTypes.Link);

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
    }
}
