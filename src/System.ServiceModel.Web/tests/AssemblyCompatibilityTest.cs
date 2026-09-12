// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Syndication;
using Infrastructure.Common;
using Xunit;

public static class AssemblyCompatibilityTest
{
    [WcfFact]
    public static void Assembly_PreservesFrameworkFacadeTypeForwards()
    {
        Type[] expected =
        {
            typeof(DataContractJsonSerializer),
            typeof(IXmlJsonReaderInitializer),
            typeof(IXmlJsonWriterInitializer),
            typeof(JsonReaderWriterFactory),
            typeof(Atom10FeedFormatter),
            typeof(Atom10FeedFormatter<>),
            typeof(Atom10ItemFormatter),
            typeof(Atom10ItemFormatter<>),
            typeof(AtomPub10CategoriesDocumentFormatter),
            typeof(AtomPub10ServiceDocumentFormatter),
            typeof(AtomPub10ServiceDocumentFormatter<>),
            typeof(CategoriesDocument),
            typeof(CategoriesDocumentFormatter),
            typeof(InlineCategoriesDocument),
            typeof(ReferencedCategoriesDocument),
            typeof(ResourceCollectionInfo),
            typeof(Rss20FeedFormatter),
            typeof(Rss20FeedFormatter<>),
            typeof(Rss20ItemFormatter),
            typeof(Rss20ItemFormatter<>),
            typeof(ServiceDocument),
            typeof(ServiceDocumentFormatter),
            typeof(SyndicationCategory),
            typeof(SyndicationContent),
            typeof(SyndicationElementExtension),
            typeof(SyndicationElementExtensionCollection),
            typeof(SyndicationFeed),
            typeof(SyndicationFeedFormatter),
            typeof(SyndicationItem),
            typeof(SyndicationItemFormatter),
            typeof(SyndicationLink),
            typeof(SyndicationPerson),
            typeof(SyndicationVersions),
            typeof(TextSyndicationContent),
            typeof(TextSyndicationContentKind),
            typeof(UrlSyndicationContent),
            typeof(Workspace),
            typeof(XmlSyndicationContent)
        };

        Type[] actual = typeof(WebHttpBinding).Assembly.GetForwardedTypes();

        Assert.Equal(
            expected.OrderBy(type => type.FullName),
            actual.OrderBy(type => type.FullName));
    }
}
