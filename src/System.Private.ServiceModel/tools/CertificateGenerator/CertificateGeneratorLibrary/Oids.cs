// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;

namespace WcfTestCommon
{
    // Centralized OID constants used throughout the CertificateGenerator project.
    // Keep raw OID strings in one place so callers reference these instead of
    // sprinkling numeric literals across the codebase.
    internal static class Oids
    {
        // Extended Key Usage OIDs (RFC 5280)
        public const string ServerAuthEku = "1.3.6.1.5.5.7.3.1";
        public const string ClientAuthEku = "1.3.6.1.5.5.7.3.2";

        // X.509 v3 extension OIDs (RFC 5280)
        public const string SubjectKeyIdentifierExtension   = "2.5.29.14";
        public const string CrlDistributionPointsExtension  = "2.5.29.31";
        public const string AuthorityInfoAccessExtension    = "1.3.6.1.5.5.7.1.1";

        // AuthorityInfoAccess accessMethod OIDs (RFC 5280 4.2.2.1)
        public const string IdAdOcsp        = "1.3.6.1.5.5.7.48.1";
        public const string IdAdCaIssuers   = "1.3.6.1.5.5.7.48.2";

        // Friendly names used when surfacing OIDs in cert viewers.
        public const string ServerAuthEkuFriendlyName                  = "TLS Web Server Authentication";
        public const string ClientAuthEkuFriendlyName                  = "TLS Web Client Authentication";
        public const string CrlDistributionPointsExtensionFriendlyName = "X509v3 CRL Distribution Points";
        public const string AuthorityInfoAccessExtensionFriendlyName   = "Authority Information Access";

        // Strongly-typed Oid instances (include friendly names that show up in tools
        // that surface Oid.FriendlyName, e.g. certificate viewers).
        public static readonly Oid ServerAuthEkuOid                  = new Oid(ServerAuthEku,                  ServerAuthEkuFriendlyName);
        public static readonly Oid ClientAuthEkuOid                  = new Oid(ClientAuthEku,                  ClientAuthEkuFriendlyName);
        public static readonly Oid CrlDistributionPointsExtensionOid = new Oid(CrlDistributionPointsExtension, CrlDistributionPointsExtensionFriendlyName);
        public static readonly Oid AuthorityInfoAccessExtensionOid   = new Oid(AuthorityInfoAccessExtension,   AuthorityInfoAccessExtensionFriendlyName);
    }
}
