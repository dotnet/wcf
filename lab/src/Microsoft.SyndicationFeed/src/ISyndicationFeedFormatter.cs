// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.SyndicationFeed
{
    public interface ISyndicationFeedFormatter
    {
        string Format(ISyndicationContent content);

        string Format(ISyndicationCategory category);

        string Format(ISyndicationImage image);

        string Format(ISyndicationItem item);

        string Format(ISyndicationPerson person);

        string Format(ISyndicationLink link);

        string FormatValue<T>(T value);
    }
}
