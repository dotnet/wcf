// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


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
            {
                return new LocalIdKeyIdentifierClause(Id, GetType()) as T;
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(
                SRP.Format(SRP.TokenDoesNotSupportKeyIdentifierClauseCreation, GetType().Name, typeof(T).Name)));
        }

        public virtual bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            LocalIdKeyIdentifierClause localKeyIdentifierClause = keyIdentifierClause as LocalIdKeyIdentifierClause;
            if (localKeyIdentifierClause != null)
            {
                return localKeyIdentifierClause.Matches(Id, GetType());
            }

            return false;
        }

        public virtual SecurityKey ResolveKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (SecurityKeys.Count != 0 && MatchesKeyIdentifierClause(keyIdentifierClause))
            {
                return SecurityKeys[0];
            }

            return null;
        }

        private bool CanCreateLocalKeyIdentifierClause()
        {
            return (Id != null);
        }
    }
}
