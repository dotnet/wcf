﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.SyndicationFeed
{
    class SyndicationLink : ISyndicationLink
    {
        public Uri Uri { get; set; }

        public string Title { get; set; }

        public string MediaType { get; set; }

        public string RelationshipType { get; set; }

        public long Length { get; set; }
    }
}