// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
            var dir = Directory.GetCurrentDirectory();

            using (var xmlReader = XmlReader.Create(@"..\..\..\TestFeeds\rss20.xml", new XmlReaderSettings() { Async = true })) {
                var reader = new Rss20FeedReader(xmlReader);

                while (await reader.Read()) {
                    if (reader.ElementType == SyndicationElementType.Content) {
                        ISyndicationContent content = await reader.ReadContent();
                        Console.WriteLine(content);
                    }
                    if (reader.ElementType == SyndicationElementType.Item) {
                        ISyndicationItem item = await reader.ReadItem();
                        Console.WriteLine(item);
                    }
                    if (reader.ElementType == SyndicationElementType.Person) {
                        ISyndicationPerson person = await reader.ReadPerson();
                        Console.WriteLine(person);
                    }
                }
            }
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
    }
}
