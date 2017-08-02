// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.SyndicationFeed
{
    public sealed class SyndicationPerson : ISyndicationPerson
    {
        public string Email { get; set; }

        public string Name { get; set; }

        public string Uri { get; set; }

        public string RelationshipType { get; set; }
    }
}