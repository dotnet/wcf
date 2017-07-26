// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.SyndicationFeed
{
    public interface ISyndicationFeedSerializer
    {
        string Serialize(ISyndicationContent content);

        string Serialize(ISyndicationCategory category);

        string Serialize(ISyndicationImage image);

        string Serialize(ISyndicationItem item);

        string Serialize(ISyndicationPerson person);

        string Serialize(ISyndicationLink link);
    }
}
