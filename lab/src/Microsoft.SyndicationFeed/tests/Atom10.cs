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
                while (await reader.Read())
                {
                    if (reader.ElementType == SyndicationElementType.Image)
                    {
                        ISyndicationImage image = await reader.ReadImage();
                        imagesRead++;
                    }
                }
                Assert.True(imagesRead == 2);
            }
        }

    }
}
