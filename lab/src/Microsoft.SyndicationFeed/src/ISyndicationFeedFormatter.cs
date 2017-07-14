// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Microsoft.SyndicationFeed
{
    public interface ISyndicationFeedFormatter
    {
        ISyndicationItem ParseItem(string value);

        ISyndicationLink ParseLink(string value);

        ISyndicationPerson ParsePerson(string value);

        ISyndicationCategory ParseCategory(string value);

        ISyndicationContent ParseContent(string value);

        ISyndicationImage ParseImage(string value);

        IEnumerable<ISyndicationContent> ParseChildren(string content);

        bool TryParseValue<T>(string value, out T result);
    }
}