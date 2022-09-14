// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Security
{
    using System.Globalization;
    using System.IdentityModel.Tokens;
    using System.Runtime.CompilerServices;
    using System.Xml;
    using DiagnosticUtility = DiagnosticUtility;

    [TypeForwardedFrom("System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public class SecurityContextKeyIdentifierClause : SecurityKeyIdentifierClause
    {
        private readonly UniqueId _generation;

        public SecurityContextKeyIdentifierClause(UniqueId contextId)
            : this(contextId, null)
        {
        }

        public SecurityContextKeyIdentifierClause(UniqueId contextId, UniqueId generation)
            : this(contextId, generation, null, 0)
        {
        }

        public SecurityContextKeyIdentifierClause(UniqueId contextId, UniqueId generation, byte[] derivationNonce, int derivationLength)
            : base(null, derivationNonce, derivationLength)
        {
            ContextId = contextId ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(contextId));
            _generation = generation;
        }

        public UniqueId ContextId { get; }

        public UniqueId Generation
        {
            get { return _generation; }
        }

        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            SecurityContextKeyIdentifierClause that = keyIdentifierClause as SecurityContextKeyIdentifierClause;
            return ReferenceEquals(this, that) || (that != null && that.Matches(ContextId, _generation));
        }

        public bool Matches(UniqueId contextId, UniqueId generation)
        {
            return contextId == ContextId && generation == _generation;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "SecurityContextKeyIdentifierClause(ContextId = '{0}', Generation = '{1}')",
                ContextId, Generation);
        }
    }
}
