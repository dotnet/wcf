// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Microsoft.SyndicationFeed
{
    public interface ISyndicationItem
    {
        string Id { get; }

        string Title { get; }

        string Description { get; }

        IEnumerable<ISyndicationCategory> Categories { get; }

        IEnumerable<ISyndicationPerson> Contributors { get; }

        IEnumerable<ISyndicationLink> Links { get; }

        DateTimeOffset LastUpdated { get; }

        DateTimeOffset Published { get; }
    }
}