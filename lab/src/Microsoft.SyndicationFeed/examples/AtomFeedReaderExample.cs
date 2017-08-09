// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Atom;
using System.Threading.Tasks;
using System.Xml;

/// <summary>
/// Consumes an entire atom feed using the AtomFeedReader.
/// </summary>
class AtomFeedReaderExample
{
    public static async Task ReadAtomFeed(string filePath)
    {
        //
        // Create an XmlReader from file
        // Example: ..\tests\TestFeeds\rss20-2items.xml
        using (XmlReader xmlReader = XmlReader.Create(filePath))
        {
            //
            // Create an AtomFeedReader
            var reader = new AtomFeedReader(xmlReader);

            //
            // Read the feed
            while (await reader.Read())
            {
                //
                // Check the type of the current element.
                switch (reader.ElementType)
                {
                    //
                    // Read category
                    case SyndicationElementType.Category:
                        ISyndicationCategory category = await reader.ReadCategory();
                        break;

                    //
                    // Read image
                    case SyndicationElementType.Image:
                        ISyndicationImage image = await reader.ReadImage();
                        break;
                        
                    //
                    // Read entry 
                    case SyndicationElementType.Item:
                        IAtomEntry entry = await reader.ReadEntry();                            
                        break;

                    //
                    // Read link
                    case SyndicationElementType.Link:
                        ISyndicationLink link = await reader.ReadLink();
                        break;

                    //
                    // Read person
                    case SyndicationElementType.Person:
                        ISyndicationPerson person = await reader.ReadPerson();
                        break;

                    //
                    // Read content
                    default:
                        ISyndicationContent content = await reader.ReadContent();
                        break;
                }
            }
        }
    }
}