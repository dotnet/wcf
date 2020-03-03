// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.IdentityModel.Tokens
{
    /// <summary>
    /// 
    /// </summary>
    public class SamlAssertionKeyIdentifierClause : SecurityKeyIdentifierClause
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assertionId"></param>
        public SamlAssertionKeyIdentifierClause(string assertionId) : base(assertionId, null, 0)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assertionId"></param>
        /// <param name="keyIdentifierClause"></param>
        /// <returns></returns>
        public static bool Matches(string assertionId, SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (null == keyIdentifierClause)
                return false;

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
        /// -
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "SamlAssertionKeyIdentifierClause(AssertionId = '{0}')", Id);
        }
    }
}
