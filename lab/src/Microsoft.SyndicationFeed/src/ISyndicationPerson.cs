// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.SyndicationFeed
{
    public interface ISyndicationPerson
    {
        string Email { get; }

        string Name { get; }

        string Uri { get; }

        string RelationshipType { get; }
    }
}