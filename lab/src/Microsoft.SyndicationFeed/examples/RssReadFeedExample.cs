// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using System;
using System.Threading.Tasks;
using System.Xml;

/// <summary>
/// Rss 2.0 FeedReader that consumes an entire feed.
/// </summary>
class RssReadFeed
{
    // Read an RssFeed
    public static async Task CreateRssFeedReaderExample(string filePath)
    {
        // Create an XmlReader
        // Example: ..\tests\TestFeeds\rss20-2items.xml
        using (var xmlReader = XmlReader.Create(filePath, new XmlReaderSettings() { Async = true }))
        {
            // Instantiate an Rss20FeedReader using the XmlReader.
            // This will assign as default an Rss20FeedParser as the parser.
            var feedReader = new Rss20FeedReader(xmlReader);

            //
            // Read the feed
            while(await feedReader.Read())
            {
                switch (feedReader.ElementType)
                {
                    // Read category
                    case SyndicationElementType.Category:
                        ISyndicationCategory category = await feedReader.ReadCategory();
                        break;

                    // Read Image
                    case SyndicationElementType.Image:
                        ISyndicationImage image = await feedReader.ReadImage();
                        break;

                    // Read Item
                    case SyndicationElementType.Item:
                        ISyndicationItem item = await feedReader.ReadItem();
                        break;

                    // Read link
                    case SyndicationElementType.Link:
                        ISyndicationLink link = await feedReader.ReadLink();
                        break;

                    // Read Person
                    case SyndicationElementType.Person:
                        ISyndicationPerson person = await feedReader.ReadPerson();
                        break;

                    // Read content
                    default:
                        ISyndicationContent content = await feedReader.ReadContent();
                        break;
                }
            }
        }
    }
}
