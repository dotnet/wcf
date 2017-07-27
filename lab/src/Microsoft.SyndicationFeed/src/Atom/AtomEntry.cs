// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Microsoft.SyndicationFeed
{
    sealed class AtomEntry : IAtomEntry
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public IEnumerable<ISyndicationCategory> Categories { get; set; }
       
        public IEnumerable<ISyndicationPerson> Contributors { get; set; }

        public IEnumerable<ISyndicationLink> Links { get; set; }

        public DateTimeOffset LastUpdated { get; set; }

        public DateTimeOffset Published { get; set; }

        public string ContentType { get; set; }

        public string Summary { get; set; }

        public string Rights { get; set; }
    }
}
