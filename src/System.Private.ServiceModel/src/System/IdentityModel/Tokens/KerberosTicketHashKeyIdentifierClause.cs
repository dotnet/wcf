// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.IdentityModel.Tokens
{
    public sealed class KerberosTicketHashKeyIdentifierClause : BinaryKeyIdentifierClause
    {
        public KerberosTicketHashKeyIdentifierClause(byte[] ticketHash)
            : this(ticketHash, null, 0)
        {
        }

        public KerberosTicketHashKeyIdentifierClause(byte[] ticketHash, byte[] derivationNonce, int derivationLength)
            : this(ticketHash, true, derivationNonce, derivationLength)
        {
        }
        
        internal KerberosTicketHashKeyIdentifierClause(byte[] ticketHash, bool cloneBuffer, byte[] derivationNonce, int derivationLength)
            : base(null, ticketHash, cloneBuffer, derivationNonce, derivationLength)
        {           
        }

        public byte[] GetKerberosTicketHash()
        {
            return GetBuffer();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "KerberosTicketHashKeyIdentifierClause(Hash = {0})", ToBase64String());
        }        
    }
}

