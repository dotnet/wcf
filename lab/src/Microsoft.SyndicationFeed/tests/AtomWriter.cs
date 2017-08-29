// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.SyndicationFeed.Atom;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Microsoft.SyndicationFeed.Tests.Atom
{
    public class AtomWriter
    {
        [Fact]
        public async Task WriteCategory()
        {
            var category = new SyndicationCategory("Test Category");

            var sw = new StringWriterWithEncoding(Encoding.UTF8);

            using (var xmlWriter = XmlWriter.Create(sw))
            {
                var writer = new AtomFeedWriter(xmlWriter);

                await writer.Write(category);
                await writer.Flush();
            }

            string res = sw.ToString();
            Assert.True(CheckResult(res, $"<category term=\"{category.Name}\" />"));
        }

        [Fact]
        public async Task WritePerson()
        {
            var sw = new StringWriterWithEncoding(Encoding.UTF8);

            var p1 = new SyndicationPerson("John Doe", "johndoe@contoso.com");
            var p2 = new SyndicationPerson("Jane Doe", "janedoe@contoso.com", AtomContributorTypes.Contributor)
            {
                Uri = "www.contoso.com/janedoe"
            };

            using (var xmlWriter = XmlWriter.Create(sw))
            {
                var writer = new AtomFeedWriter(xmlWriter);

                await writer.Write(p1);
                await writer.Write(p2);

                await writer.Flush();
            }

            string res = sw.ToString();
            Assert.True(CheckResult(res, $"<author><name>{p1.Name}</name><email>{p1.Email}</email></author><contributor><name>{p2.Name}</name><email>{p2.Email}</email><uri>{p2.Uri}</uri></contributor>"));
        }

        [Fact]
        public async Task WriteImage()
        {
            var icon = new SyndicationImage(new Uri("http://contoso.com/icon.ico"), AtomImageTypes.Icon);
            var logo = new SyndicationImage(new Uri("http://contoso.com/logo.png"), AtomImageTypes.Logo);

            var sw = new StringWriterWithEncoding(Encoding.UTF8);

            using (var xmlWriter = XmlWriter.Create(sw))
            {
                var writer = new AtomFeedWriter(xmlWriter);

                await writer.Write(icon);
                await writer.Write(logo);

                await writer.Flush();
            }

            string res = sw.ToString();
            Assert.True(CheckResult(res, $"<icon>{icon.Url}</icon><logo>{logo.Url}</logo>"));
        }


        [Fact]
        public async Task WriteLink()
        {
            var sw = new StringWriterWithEncoding(Encoding.UTF8);

            var link = new SyndicationLink(new Uri("http://contoso.com"))
            {
                Title = "Test title",
                Length = 123,
                MediaType = "mp3/video"
            };

            using (var xmlWriter = XmlWriter.Create(sw))
            {
                var writer = new AtomFeedWriter(xmlWriter);

                await writer.Write(link);

                await writer.Flush();
            }

            string res = sw.ToString();
            Assert.True(CheckResult(res, $"<link title=\"{link.Title}\" href=\"{link.Uri}\" type=\"{link.MediaType}\" length=\"{link.Length}\" />"));
        }


        [Fact]
        public async Task WriteEntry()
        {
            var link = new SyndicationLink(new Uri("https://contoso.com/alternate"));
            var related = new SyndicationLink(new Uri("https://contoso.com/related"), AtomLinkTypes.Related);
            var self = new SyndicationLink(new Uri("https://contoso.com/28af09b3"), AtomLinkTypes.Self);
            var enclosure = new SyndicationLink(new Uri("https://contoso.com/podcast"), AtomLinkTypes.Enclosure)
            {
                Title = "Podcast",
                MediaType = "audio/mpeg",
                Length = 4123
            };
            var source = new SyndicationLink(new Uri("https://contoso.com/source"), AtomLinkTypes.Source)
            {
                Title = "Blog",
                LastUpdated = DateTimeOffset.UtcNow.AddDays(-10)
            };
            var author = new SyndicationPerson("John Doe", "johndoe@email.com");
            var category = new SyndicationCategory("Lorem Category");

            // 
            // Construct entry
            var entry = new AtomEntry()
            {
                Id = "https://contoso.com/28af09b3",
                Title = "Lorem Ipsum",
                Description = "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit...",
                LastUpdated = DateTimeOffset.UtcNow,
                ContentType = "text/html",
                Summary = "Proin egestas sem in est feugiat, id laoreet massa dignissim",
                Rights = $"copyright (c) {DateTimeOffset.UtcNow.Year}"
            };

            entry.AddLink(link);
            entry.AddLink(enclosure);
            entry.AddLink(related);
            entry.AddLink(source);
            entry.AddLink(self);

            entry.AddContributor(author);

            entry.AddCategory(category);

            //
            // Write
            var sw = new StringWriterWithEncoding(Encoding.UTF8);

            using (var xmlWriter = XmlWriter.Create(sw))
            {
                var writer = new AtomFeedWriter(xmlWriter);

                await writer.Write(entry);
                await writer.Flush();
            }

            string res = sw.ToString();
            Assert.True(CheckResult(res, $"<entry><id>{entry.Id}</id><title>{entry.Title}</title><updated>{entry.LastUpdated.ToString("r")}</updated><link href=\"{link.Uri}\" /><link title=\"{enclosure.Title}\" href=\"{enclosure.Uri}\" rel=\"{enclosure.RelationshipType}\" type=\"{enclosure.MediaType}\" length=\"{enclosure.Length}\" /><link href=\"{related.Uri}\" rel=\"{related.RelationshipType}\" /><source><title>{source.Title}</title><link href=\"{source.Uri}\" /><updated>{source.LastUpdated.ToString("r")}</updated></source><link href=\"{self.Uri}\" rel=\"{self.RelationshipType}\" /><author><name>{author.Name}</name><email>{author.Email}</email></author><category term=\"{category.Name}\" /><content type=\"{entry.ContentType}\">{entry.Description}</content><summary>{entry.Summary}</summary><rights>{entry.Rights}</rights></entry>"));
        }

        [Fact]
        public async Task WriteValue()
        {
            const string title = "Example Feed";
            Guid id = Guid.NewGuid();
            DateTimeOffset updated = DateTimeOffset.UtcNow.AddDays(-21);

            var sw = new StringWriterWithEncoding(Encoding.UTF8);

            using (var xmlWriter = XmlWriter.Create(sw))
            {
                var writer = new AtomFeedWriter(xmlWriter);

                await writer.WriteTitle(title);
                await writer.WriteId(id.ToString());
                await writer.WriteUpdated(updated);

                await writer.Flush();
            }

            string res = sw.ToString();
            Assert.True(CheckResult(res, $"<title>{title}</title><id>{id}</id><updated>{updated.ToString("r")}</updated>"));
        }

        [Fact]
        public async Task WriteContent()
        {
            const string uri = "https://contoso.com/generator";
            const string version = "1.0";
            const string generator = "Example Toolkit";

            var sw = new StringWriterWithEncoding(Encoding.UTF8);

            using (var xmlWriter = XmlWriter.Create(sw))
            {
                var writer = new AtomFeedWriter(xmlWriter);

                await writer.WriteGenerator(generator, uri, version);

                await writer.Flush();
            }

            string res = sw.ToString();
            Assert.True(CheckResult(res, $"<generator uri=\"{uri}\" version=\"{version}\">{generator}</generator>"));
        }

        [Fact]
        public async Task WritePrefixedAtomNs()
        {
            const string title = "Example Feed";
            const string uri = "https://contoso.com/generator";
            const string generator = "Example Toolkit";

            var sw = new StringWriterWithEncoding(Encoding.UTF8);

            using (var xmlWriter = XmlWriter.Create(sw))
            {
                var writer = new AtomFeedWriter(xmlWriter, 
                                                new ISyndicationAttribute[] { new SyndicationAttribute("xmlns:atom", "http://www.w3.org/2005/Atom") });

                await writer.WriteTitle(title);
                await writer.WriteGenerator(generator, uri);

                await writer.Flush();
            }

            string res = sw.ToString();
            Assert.True(CheckResult(res, $"<atom:title>{title}</atom:title><atom:generator uri=\"{uri}\">{generator}</atom:generator>", "atom"));
        }

        [Fact]
        public async Task EmbededAtomInRssFeed()
        {
            var author = new SyndicationPerson("john doe", "johndoe@contoso.com");
            var entry = new AtomEntry()
            {
                Id = "https://contoso.com/28af09b3",
                Title = "Atom Entry",
                Description = "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit...",
                LastUpdated = DateTimeOffset.UtcNow
            };
            entry.AddContributor(author);

            var sw = new StringWriterWithEncoding(Encoding.UTF8);

            using (var xmlWriter = XmlWriter.Create(sw))
            {
                var attributes = new ISyndicationAttribute[] { new SyndicationAttribute("xmlns:atom", "http://www.w3.org/2005/Atom") };
                var writer = new Rss.Rss20FeedWriter(xmlWriter, attributes);
                var formatter = new AtomFormatter(attributes, xmlWriter.Settings);

                //
                // Write Rss elements
                await writer.WriteValue(Rss.Rss20ElementNames.Title, "Rss Title");
                await writer.Write(author);
                await writer.Write(new SyndicationItem()
                {
                    Title = "Rss Item",
                    Id = "https://contoso.com/rss/28af09b3",
                    Description = "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium",
                    LastUpdated = DateTimeOffset.UtcNow
                });

                //
                // Write atom entry
                await writer.WriteRaw(formatter.Format(entry));

                await writer.Flush();
            }

            string res = sw.ToString();
            Assert.True(res.Contains($"<atom:entry><atom:id>{entry.Id}</atom:id><atom:title>{entry.Title}</atom:title><atom:updated>{entry.LastUpdated.ToString("r")}</atom:updated><atom:author><atom:name>{author.Name}</atom:name><atom:email>{author.Email}</atom:email></atom:author><atom:content>{entry.Description}</atom:content></atom:entry>"));
        }

        private static bool CheckResult(string result, string expected)
        {
            return result == $"<?xml version=\"1.0\" encoding=\"utf-8\"?><feed xmlns=\"http://www.w3.org/2005/Atom\">{expected}</feed>";
        }

        private static bool CheckResult(string result, string expected, string prefix)
        {
            return result == $"<?xml version=\"1.0\" encoding=\"utf-8\"?><feed xmlns:{prefix}=\"http://www.w3.org/2005/Atom\">{expected}</feed>";
        }
    }


    sealed class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding _encoding;

        public StringWriterWithEncoding(Encoding encoding)
        {
            this._encoding = encoding;
        }

        public override Encoding Encoding {
            get { return _encoding; }
        }
    }
}
