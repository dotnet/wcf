// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

/// <summary>
/// Create an RSS 2.0 feed
/// </summary>
class CreateSimpleRssFeed
{
    public static async Task WriteFeed()
    {
        var sw = new StringWriter();

        using (XmlWriter xmlWriter = XmlWriter.Create(sw, new XmlWriterSettings() { Async = true , Indent = true }))
        {
            var writer = new Rss20FeedWriter(xmlWriter);

            //
            // Add Title
            await writer.WriteValue(Rss20ElementNames.Title, "Example of Rss20FeedWriter");

            //
            // Add Description
            await writer.WriteValue(Rss20ElementNames.Description, "Hello World, RSS!");

            //
            // Add Link
            await writer.Write(new SyndicationLink(new Uri("https://github.com/dotnet/wcf")));

            //
            // Add managing editor
            await writer.Write(new SyndicationPerson(null, "managingeditor@contoso.com", Rss20ContributorTypes.ManagingEditor));

            //
            // Add publish date
            await writer.WriteValue(Rss20ElementNames.PubDate, DateTimeOffset.UtcNow);

            //
            // Add custom element
            var customElement = new SyndicationContent("customElement");

            customElement.AddAttribute(new SyndicationAttribute("attr1", "true"));
            customElement.AddField(new SyndicationContent("Company", "Contoso"));

            await writer.Write(customElement);

            //
            // Add Items
            for (int i = 0; i < 5; ++i)
            {
                var item = new SyndicationItem()
                {
                    Id = "https://github.com/dotnet/wcf/tree/lab/lab/src/Microsoft.SyndicationFeed/src",
                    Title = $"Item #{i + 1}",
                    Description = "The new RSS Writer is now open source!",
                    Published = DateTimeOffset.UtcNow
                };

                item.AddLink(new SyndicationLink(new Uri("https://github.com/dotnet/wcf")));
                item.AddCategory(new SyndicationCategory("Technology"));
                item.AddContributor(new SyndicationPerson(null, "user@contoso.com", Rss20ContributorTypes.Author));

                await writer.Write(item);
            }

            //
            // Done
            xmlWriter.Flush();
        }

        //
        // Ouput the feed
        Console.WriteLine(sw.ToString());
    }
}