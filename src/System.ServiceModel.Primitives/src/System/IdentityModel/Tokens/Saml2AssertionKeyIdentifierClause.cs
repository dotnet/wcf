// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IdentityModel.Tokens
{
    /// <summary>
    /// A SecurityKeyIdentifierClause for referencing SAML2-based security tokens.
    /// </summary>
    public class Saml2AssertionKeyIdentifierClause : SecurityKeyIdentifierClause
    {
        /// <summary>
        /// Creates a Saml2AssertionKeyIdentifierClause for a given id.
        /// </summary>
        /// <param name="assertionId">The assertionId defining the clause to create.</param>
        public Saml2AssertionKeyIdentifierClause(string assertionId) : base(assertionId, null, 0)
        {
        }

        /// <summary>
        /// Indicates whether the <see cref="SecurityKeyIdentifierClause"/> for an assertion matches the specified <see cref="SecurityKeyIdentifierClause"/>.
        /// </summary>
        /// <param name="assertionId">Id of the assertion</param>
        /// <param name="keyIdentifierClause">A <see cref="SecurityKeyIdentifierClause"/> to match.</param>
        /// <returns>'True' if the keyIdentifier matches this. 'False' otherwise.</returns>
        public static bool Matches(string assertionId, SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (null == keyIdentifierClause)
                return false;

            // Prefer our own type
            if (keyIdentifierClause is Saml2AssertionKeyIdentifierClause saml2Clause && StringComparer.Ordinal.Equals(assertionId, saml2Clause.Id))
                return true;

            // For compatibility, match against the old WCF type.
            // WCF will read SAML2-based key identifier clauses if our 
            // SecurityTokenSerializer doesn't get the chance. Unfortunately,
            // the TokenTypeUri and ValueType properties are internal, so
            // we can't check if they're for SAML2 or not. We're just going
            // to go with the fact that SAML Assertion IDs, in both versions,
            // are supposed to be sufficiently random as to not intersect. 
            // So, if the AssertionID matches our Id, we'll say that's good 
            // enough.
            if (keyIdentifierClause is SamlAssertionKeyIdentifierClause samlClause && StringComparer.Ordinal.Equals(assertionId, samlClause.Id))
                return true;

            return false;
        }

        /// <summary>
        /// Indicates whether the <see cref="SecurityKeyIdentifierClause"/> for this instance is matches the specified <see cref="SecurityKeyIdentifierClause"/>.
        /// </summary>
        /// <param name="keyIdentifierClause">A <see cref="SecurityKeyIdentifierClause"/> to match.</param>
        /// <returns>True if the keyIdentifier matches this. False otherwise.</returns>
        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            return ReferenceEquals(this, keyIdentifierClause) || Matches(Id, keyIdentifierClause);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the current <see cref="Object"/>.
        /// </summary>
        /// <returns>The Id of this instance as a string.</returns>
        public override string ToString()
        {
            return "Saml2AssertionKeyIdentifierClause( Id = '" + Id + "' )";
        }
    }
}
