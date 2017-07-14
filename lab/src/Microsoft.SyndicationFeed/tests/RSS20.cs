// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Microsoft.SyndicationFeed.Tests
{
    public class RSS20
    {
        [Fact]
        public async Task ReadWhile()
        {
            await ReadWhile(@"..\..\..\TestFeeds\rss20.xml");
        }

        [Fact]
        public async Task ReadWhile_3GB()
        {
            await ReadWhile(@"\\funbox\Share\Personal\jconde\Microsoft.ServiceModel.Syndication\tests\bin\Debug\netcoreapp2.0\feed3Gb.xml");
        }

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
        public async Task ReadWithExtentions()
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
                        Console.WriteLine(content.Name);

                        // Enuemrate children
                        foreach (var c in content.Fields)
                        {
                            Console.WriteLine("\t" + c.Name);

                            if (c.Name == "title" ||
                                c.Name == "description")
                            {
                                Console.WriteLine("\t" + c.GetValue());
                            }
                        }

                        // Process as Item
                        ISyndicationItem item = reader.Formatter.ParseItem(content.RawContent);
                        Console.WriteLine(item);
                    }
                }
            }
        }

        private async Task ReadWhile(string filePath)
        {
            using (var xmlReader = XmlReader.Create(filePath, new XmlReaderSettings() { Async = true }))
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
