// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1.X509;

namespace WcfTestCommon
{
    [Serializable]
    public class CertificateCreationSettings
    {
        public CertificateCreationSettings()
        {
            Subject = string.Empty;
            ValidityType = CertificateValidityType.Valid;
        }
        public string FriendlyName { get; set; }
        public string[] SubjectAlternativeNames { get; set; }
        public string Subject { get; set; }
        public DateTime ValidityNotBefore { get; set; }
        public DateTime ValidityNotAfter { get; set; }
        public CertificateValidityType ValidityType { get; set; }
        public bool IncludeCrlDistributionPoint { get; set; } = true;
        public List<KeyPurposeID> EKU { get; set; }
    }

    [Serializable]
    public enum CertificateValidityType
    {
        Valid = 0,                      // Valid and the authoritative and primary cert for the machine. This cert is retrievable by a client by using 
                                        // the EndCertificateResource endpoint and specifying the subject name

        // The following certificates are retrivable only by specifying the thumbprint to EndCertificateResource, but are not retrievable by using the 
        // subject name
        // This is because a cert may need to have the same subject name as the machine, but be in a expired or revoked state
        Expired = 1,                    // Expired certificate
        Revoked = 2,                    // Revoked certificate
        NonAuthoritativeForMachine = 3  // When the subject name is the same as the primary name of the machine
    }
}
