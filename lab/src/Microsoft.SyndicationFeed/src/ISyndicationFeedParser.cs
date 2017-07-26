// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.SyndicationFeed
{
    public interface ISyndicationFeedParser
    {
        ISyndicationItem ParseItem(string value);

        ISyndicationLink ParseLink(string value);

        ISyndicationPerson ParsePerson(string value);

        ISyndicationCategory ParseCategory(string value);

        ISyndicationImage ParseImage(string value);

        ISyndicationContent ParseContent(string value);

        bool TryParseValue<T>(string value, out T result);
    }
}