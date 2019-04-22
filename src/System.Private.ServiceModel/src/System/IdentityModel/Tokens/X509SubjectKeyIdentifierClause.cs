// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace System.IdentityModel.Tokens
{
    public class X509SubjectKeyIdentifierClause : BinaryKeyIdentifierClause
    {
        private const string SubjectKeyIdentifierOid = "2.5.29.14";
        private const int SkiDataOffset = 2;

        public X509SubjectKeyIdentifierClause(byte[] ski)
            : this(ski, true)
        {
        }

        internal X509SubjectKeyIdentifierClause(byte[] ski, bool cloneBuffer)
            : base(null, ski, cloneBuffer)
        {
        }

        private static byte[] GetSkiRawData(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(certificate));
            }

            X509SubjectKeyIdentifierExtension skiExtension =
                certificate.Extensions[SubjectKeyIdentifierOid] as X509SubjectKeyIdentifierExtension;
            if (skiExtension != null)
            {
                return skiExtension.RawData;
            }
            else
            {
                return null;
            }
        }

        public byte[] GetX509SubjectKeyIdentifier()
        {
            return GetBuffer();
        }

        public bool Matches(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                return false;
            }

            // This would fail if anyone used a Subject Key Identifier in excess of 127 bytes (because then the prefix grows to a 3rd byte, or more).
            // Large values are not technically illegal, but will probably never happen. If this is ever an issue, then this code will need to be revised.
            byte[] data = GetSkiRawData(certificate);
            return data != null && Matches(data, SkiDataOffset);
        }

        public static bool TryCreateFrom(X509Certificate2 certificate, out X509SubjectKeyIdentifierClause keyIdentifierClause)
        {
            byte[] data = GetSkiRawData(certificate);
            keyIdentifierClause = null;
            if (data != null)
            {
                byte[] ski = SecurityUtils.CloneBuffer(data, SkiDataOffset, data.Length - SkiDataOffset);
                keyIdentifierClause = new X509SubjectKeyIdentifierClause(ski, false);
            }
            return keyIdentifierClause != null;
        }

        public static bool CanCreateFrom(X509Certificate2 certificate)
        {
            return null != GetSkiRawData(certificate);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "X509SubjectKeyIdentifierClause(SKI = 0x{0})", ToHexString());
        }
    }
}
