// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.using System;

using System;

namespace Microsoft.SyndicationFeed
{
    public interface ISyndicationImage
    {
        string Title { get; }

        Uri Url { get;  }

        ISyndicationLink Link { get; }

        string RelationshipType { get; }

        string Desciption { get; }
    }
}
