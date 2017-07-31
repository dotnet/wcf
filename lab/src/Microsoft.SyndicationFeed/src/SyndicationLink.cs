// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.SyndicationFeed
{
    public class SyndicationLink : ISyndicationLink
    {
        public SyndicationLink(Uri url, string relationshipType = null)
        {
            Uri = url ?? throw new ArgumentNullException(nameof(url));
            RelationshipType = relationshipType;
        }

        public Uri Uri { get; private set; }

        public string Title { get; set; }

        public string MediaType { get; set; }

        public string RelationshipType { get; }

        public long Length { get; set; }

        public DateTimeOffset LastUpdated { get; set; }
    }
}