// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.SyndicationFeed
{
    public sealed class SyndicationPerson : ISyndicationPerson
    {
        public SyndicationPerson(string name, string email, string relationshipType = null)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("Valid name or email is required");
            }

            Name = name;
            Email = email;
            RelationshipType = relationshipType;
        }

        public string Email { get; private set; }

        public string Name { get; private set; }

        public string Uri { get; set; }

        public string RelationshipType { get; set; }
    }
}