using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Microsoft.SyndicationFeed.Tests
{
    public class Atom10
    {

        [Fact]
        public async Task AtomTest()
        {
            using (XmlReader xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\simpleAtomFeed.xml",new XmlReaderSettings { Async = true }))
            {
                var reader = new Atom10FeedReader(xmlReader);
                while(await reader.Read())
                {
                    ISyndicationContent content = await reader.ReadContent();
                }
            }
        }

        [Fact]
        public async Task AtomReader_ReadPerson()
        {
            using (XmlReader xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\simpleAtomFeed.xml", new XmlReaderSettings { Async = true }))
            {
                var reader = new Atom10FeedReader(xmlReader);
                while (await reader.Read())
                {
                    if(reader.ElementType == SyndicationElementType.Person)
                    {
                        ISyndicationPerson person = await reader.ReadPerson();
                    }
                }
            }
        }

        [Fact]
        public async Task AtomReader_ReadImage()
        {
            using (XmlReader xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\simpleAtomFeed.xml", new XmlReaderSettings { Async = true }))
            {
                var reader = new Atom10FeedReader(xmlReader);
                int imagesRead = 0;

                List<String> contentsOfImages = new List<string>();

                while (await reader.Read())
                {
                    if (reader.ElementType == SyndicationElementType.Image)
                    {
                        ISyndicationImage image = await reader.ReadImage();
                        imagesRead++;
                        contentsOfImages.Add(image.Url.OriginalString);
                    }
                }
                Assert.True(imagesRead == 2);
                Assert.True(contentsOfImages[0] == "/icon.jpg");
                Assert.True(contentsOfImages[1] == "/logo.jpg");
            }
        }

        [Fact]
        public async Task AtomReader_ReadCategory()
        {
            using (XmlReader xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\simpleAtomFeed.xml", new XmlReaderSettings { Async = true }))
            {
                var reader = new Atom10FeedReader(xmlReader);
                while (await reader.Read())
                {
                    if (reader.ElementType == SyndicationElementType.Category)
                    {
                        ISyndicationCategory category = await reader.ReadCategory();
                        Assert.True(category.Name == "sports");
                    }
                }
            }
        }

        [Fact]
        public async Task AtomReader_ReadLink()
        {
            using (XmlReader xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\simpleAtomFeed.xml", new XmlReaderSettings { Async = true }))
            {
                var reader = new Atom10FeedReader(xmlReader);
                List<string> hrefs = new List<string>();
                while (await reader.Read())
                {
                    if (reader.ElementType == SyndicationElementType.Link)
                    {
                        ISyndicationLink link = await reader.ReadLink();
                        hrefs.Add(link.Uri.OriginalString);
                    }
                }
                Assert.True(hrefs[0] == "http://example.org/");
                Assert.True(hrefs[1] == "http://example.org/feed.atom");
            }
        }

    }
}
