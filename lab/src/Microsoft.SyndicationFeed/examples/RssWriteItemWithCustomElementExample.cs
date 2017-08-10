// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;


/// <summary>
/// Create a SyndicationItem and add a custom field.
/// </summary>
class RssWriteItemWithCustomElement
{
    public static async Task WriteCustomItem()
    {
        var sw = new StringWriter();
        using (XmlWriter xmlWriter = XmlWriter.Create(sw))
        {
            var formatter = new Rss20Formatter(xmlWriter.Settings);
            var namespaces = new List<SyndicationAttribute>()
            {
                new SyndicationAttribute("xmlns:example", "http://contoso.com/syndication/feed/examples")
            };

            var writer = new Rss20FeedWriter(xmlWriter, formatter, namespaces);
              
            // Create item
            var item = new SyndicationItem()
            {
                Title = "Rss Writer Available",
                Description = "The new RSS Writer is now open source!",
                Id = "https://github.com/dotnet/wcf/tree/lab/lab/src/Microsoft.SyndicationFeed/src",
                Published = DateTimeOffset.UtcNow
            };

            item.AddCategory(new SyndicationCategory("Technology"));
            item.AddContributor(new SyndicationPerson(null, "test@mail.com"));

            //
            // Format the item as SyndicationContent
            var content = new SyndicationContent(formatter.CreateContent(item));

            // Add custom fields/attributes
            content.AddField(new SyndicationContent("example:customElement", "http://contoso.com/syndication/feed/examples", "Custom Value"));

            // Write 
            await writer.Write(content);

            // Done
            xmlWriter.Flush();
        }
        
        Console.WriteLine(sw.ToString());
    }
}
