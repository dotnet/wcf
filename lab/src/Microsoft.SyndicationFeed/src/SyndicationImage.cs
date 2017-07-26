// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.using System;

using System;

namespace Microsoft.SyndicationFeed
{
    public sealed class SyndicationImage : ISyndicationImage
    {
        public SyndicationImage(Uri url)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public string Title { get; set; } = String.Empty;

        public Uri Url { get; private set; }

        public ISyndicationLink Link { get; set; }

        public string RelationshipType { get; set; }

        public string Desciption { get; set; }
    }
}