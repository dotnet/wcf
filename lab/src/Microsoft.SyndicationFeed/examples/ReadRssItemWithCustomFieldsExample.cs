// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

/// <summary>
/// Reads RSS items with custom fields
/// </summary>
class ReadRssItemWithCustomFields
{
    public static async Task ReadFeed(string filepath)
    {
        //
        // Create an XmlReader from file
        // Example: ..\tests\TestFeeds\rss20-2items.xml
        using (var xmlReader = XmlReader.Create(filepath))
        {
            //
            // Instantiate an Rss20FeedReader
            var parser = new Rss20FeedParser();
            var feedReader = new Rss20FeedReader(xmlReader, parser);

            //
            // Read the feed
            while (await feedReader.Read())
            {
                if (feedReader.ElementType == SyndicationElementType.Item)
                {
                    //
                    // Read the item as generic content
                    ISyndicationContent content = await feedReader.ReadContent();

                    //
                    // Parse the item if needed (unrecognized tags aren't available)
                    // Utilize the existing parser
                    ISyndicationItem item = parser.CreateItem(content);

                    Console.WriteLine($"Item: {item.Title}");

                    //
                    // Get <example:customElement> field
                    ISyndicationContent customElement = content.Fields.FirstOrDefault(f => f.Name == "example:customElement");

                    if (customElement != null)
                    {
                        Console.WriteLine($"{customElement.Name}: {customElement.Value}");
                    }
                }
            }
        }
    }
}
