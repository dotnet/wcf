// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.ServiceModel;

namespace System.IdentityModel.Tokens
{
    public abstract class SecurityToken
    {
        public abstract string Id { get; }
        public abstract ReadOnlyCollection<SecurityKey> SecurityKeys { get; }
        public abstract DateTime ValidFrom { get; }
        public abstract DateTime ValidTo { get; }

        public virtual bool CanCreateKeyIdentifierClause<T>() where T : SecurityKeyIdentifierClause
        {
            return ((typeof(T) == typeof(LocalIdKeyIdentifierClause)) && CanCreateLocalKeyIdentifierClause());
        }

        public virtual T CreateKeyIdentifierClause<T>() where T : SecurityKeyIdentifierClause
        {
            if ((typeof(T) == typeof(LocalIdKeyIdentifierClause)) && CanCreateLocalKeyIdentifierClause())
                return new LocalIdKeyIdentifierClause(this.Id, this.GetType()) as T;

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(
                System.SRServiceModel.Format(System.SRServiceModel.TokenDoesNotSupportKeyIdentifierClauseCreation, GetType().Name, typeof(T).Name)));
        }

        public virtual bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            LocalIdKeyIdentifierClause localKeyIdentifierClause = keyIdentifierClause as LocalIdKeyIdentifierClause;
            if (localKeyIdentifierClause != null)
                return localKeyIdentifierClause.Matches(this.Id, this.GetType());

            return false;
        }

        public virtual SecurityKey ResolveKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (this.SecurityKeys.Count != 0 && MatchesKeyIdentifierClause(keyIdentifierClause))
                return this.SecurityKeys[0];

            return null;
        }

        bool CanCreateLocalKeyIdentifierClause()
        {
            return (this.Id != null);
        }
    }
}
