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
            var writer = new RssFeedWriter(xmlWriter);

            //
            // Add Title
            await writer.WriteTitle("Example of Rss20FeedWriter");

            //
            // Add Description
            await writer.WriteDescription("Hello World, RSS!");

            //
            // Add Link
            await writer.Write(new SyndicationLink(new Uri("https://github.com/dotnet/wcf")));

            //
            // Add managing editor
            await writer.Write(new SyndicationPerson("managingeditor", "managingeditor@contoso.com", RssContributorTypes.ManagingEditor));

            //
            // Add publish date
            await writer.WritePubDate(DateTimeOffset.UtcNow);

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
                    Id = "https://www.nuget.org/packages/Microsoft.SyndicationFeed",
                    Title = $"Item #{i + 1}",
                    Description = "The new RSS Writer is available as a NuGet package!",
                    Published = DateTimeOffset.UtcNow
                };

                item.AddLink(new SyndicationLink(new Uri("https://github.com/dotnet/wcf")));
                item.AddCategory(new SyndicationCategory("Technology"));
                item.AddContributor(new SyndicationPerson(null, "user@contoso.com", RssContributorTypes.Author));

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