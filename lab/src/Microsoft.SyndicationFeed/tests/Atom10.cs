using System;
using System.Collections.Generic;
using System.Linq;
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
                var reader = new AtomFeedReader(xmlReader);
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
                var persons = new List<ISyndicationPerson>();
                var reader = new AtomFeedReader(xmlReader);
                while (await reader.Read())
                {
                    if(reader.ElementType == SyndicationElementType.Person)
                    {
                        ISyndicationPerson person = await reader.ReadPerson();
                        persons.Add(person);
                    }
                }

                Assert.True(persons.Count() == 2);
                Assert.True(persons[0].Name == "Mark Pilgrim");
                Assert.True(persons[1].Name == "Sam Ruby");

            }
        }

        [Fact]
        public async Task AtomReader_ReadImage()
        {
            using (XmlReader xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\simpleAtomFeed.xml", new XmlReaderSettings { Async = true }))
            {
                var reader = new AtomFeedReader(xmlReader);
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
                var reader = new AtomFeedReader(xmlReader);
                while (await reader.Read())
                {
                    if (reader.ElementType == SyndicationElementType.Category)
                    {
                        ISyndicationCategory category = await reader.ReadCategory();
                        Assert.True(category.Name == "sports");
                        Assert.True(category.Label == "testLabel");
                        Assert.True(category.Scheme == "testScheme");
                    }
                }
            }
        }

        [Fact]
        public async Task AtomReader_ReadLink()
        {
            using (XmlReader xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\simpleAtomFeed.xml", new XmlReaderSettings { Async = true }))
            {
                var reader = new AtomFeedReader(xmlReader);
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

        [Fact]
        public async Task AtomReader_ReadItem()
        {
            using (XmlReader xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\simpleAtomFeed.xml", new XmlReaderSettings { Async = true }))
            {
                var reader = new AtomFeedReader(xmlReader);
                while (await reader.Read())
                {
                    if (reader.ElementType == SyndicationElementType.Item)
                    {
                        IAtomEntry item = await reader.ReadEntry();
                        
                        //Assert content of item
                        Assert.True(item.Title == "Atom draft-07 snapshot");
                        Assert.True(item.Links.Count() == 3);
                        Assert.True(item.Contributors.Count() == 3);
                        Assert.True(item.Rights == "All rights Reserved. Contoso.");
                        Assert.True(item.Id == "tag:example.org,2003:3.2397");
                    }
                }
            }
        }

        [Fact]
        public async Task AtomReader_ReadItemContent()
        {
            using( XmlReader xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\simpleAtomFeed.xml", new XmlReaderSettings { Async = true }))
            {
                var reader = new AtomFeedReader(xmlReader);

                while (await reader.Read())
                {
                    if (reader.ElementType == SyndicationElementType.Item)
                    {
                        ISyndicationContent content = await reader.ReadContent();

                        var fields = content.Fields.ToArray();

                        Assert.True(fields.Length == 12);

                        Assert.True(fields[0].Name == "title");
                        Assert.False(string.IsNullOrEmpty(fields[0].Value));

                        Assert.True(fields[1].Name == "link");
                        Assert.True(fields[1].Attributes.Count() > 0);

                        Assert.True(fields[2].Name == "link");
                        Assert.True(fields[2].Attributes.Count() > 0);

                        Assert.True(fields[3].Name == "id");
                        Assert.False(string.IsNullOrEmpty(fields[3].Value));

                        Assert.True(fields[4].Name == "updated");
                        Assert.False(string.IsNullOrEmpty(fields[4].Value));

                        Assert.True(fields[5].Name == "published");
                        Assert.False(string.IsNullOrEmpty(fields[5].Value));

                        Assert.True(fields[6].Name == "source");
                        Assert.True(fields[6].Fields.Count() > 0);

                        Assert.True(fields[7].Name == "author");
                        Assert.True(fields[7].Fields.Count() > 0);

                        Assert.True(fields[8].Name == "contributor");
                        Assert.True(fields[8].Fields.Count() > 0);

                        Assert.True(fields[9].Name == "contributor");
                        Assert.True(fields[9].Fields.Count() > 0);

                        Assert.True(fields[10].Name == "rights");
                        Assert.False(string.IsNullOrEmpty(fields[10].Value));

                        Assert.True(fields[11].Name == "content");
                        Assert.True(fields[11].Fields.Count() > 0);

                    }
                }
            }
        }
    }
}
