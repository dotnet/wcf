// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.using System;

using System;

namespace Microsoft.SyndicationFeed
{
    public sealed class SyndicationImage : ISyndicationImage
    {
        public SyndicationImage(Uri url, string relationshipType = null)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            RelationshipType = relationshipType;
        }

        public string Title { get; set; }

        public Uri Url { get; private set; }

        public ISyndicationLink Link { get; set; }

        public string RelationshipType { get; set; }

        public string Description { get; set; }
    }
}