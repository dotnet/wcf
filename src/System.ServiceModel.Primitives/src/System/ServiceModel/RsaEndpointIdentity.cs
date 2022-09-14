// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IdentityModel.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace System.ServiceModel
{
    public class RsaEndpointIdentity : EndpointIdentity
    {
        public RsaEndpointIdentity(string publicKey)
        {
            if (publicKey == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(publicKey));

            Initialize(Claim.CreateRsaClaim(ToRsa(publicKey)));
        }

        public RsaEndpointIdentity(X509Certificate2 certificate)
        {
            if (certificate == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(certificate));

            RSA rsa = certificate.GetRSAPublicKey();
            if (rsa == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.PublicKeyNotRSA)));

            Initialize(Claim.CreateRsaClaim(rsa));
        }

        public RsaEndpointIdentity(Claim identity)
        {
            if (identity == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(identity));

            if (!identity.ClaimType.Equals(ClaimTypes.Rsa))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.UnrecognizedClaimTypeForIdentity, identity.ClaimType, ClaimTypes.Rsa));

            Initialize(identity);
        }

        internal RsaEndpointIdentity(XmlDictionaryReader reader)
        {
            reader.ReadStartElement(XD.XmlSignatureDictionary.RsaKeyValue, XD.XmlSignatureDictionary.Namespace);
            byte[] modulus = Convert.FromBase64String(reader.ReadElementString(XD.XmlSignatureDictionary.Modulus.Value, XD.XmlSignatureDictionary.Namespace.Value));
            byte[] exponent = Convert.FromBase64String(reader.ReadElementString(XD.XmlSignatureDictionary.Exponent.Value, XD.XmlSignatureDictionary.Namespace.Value));
            reader.ReadEndElement();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RSAParameters parameters = new RSAParameters();
            parameters.Exponent = exponent;
            parameters.Modulus = modulus;
            rsa.ImportParameters(parameters);
            Initialize(Claim.CreateRsaClaim(rsa));
        }

        internal override void WriteContentsTo(XmlDictionaryWriter writer)
        {
            if (writer == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(writer));

            writer.WriteStartElement(XD.XmlSignatureDictionary.Prefix.Value, XD.XmlSignatureDictionary.KeyInfo, XD.XmlSignatureDictionary.Namespace);
            writer.WriteStartElement(XD.XmlSignatureDictionary.Prefix.Value, XD.XmlSignatureDictionary.RsaKeyValue, XD.XmlSignatureDictionary.Namespace);
            RSA rsa = (RSA)this.IdentityClaim.Resource;
            RSAParameters parameters = rsa.ExportParameters(false);
            writer.WriteElementString(XD.XmlSignatureDictionary.Prefix.Value, XD.XmlSignatureDictionary.Modulus, XD.XmlSignatureDictionary.Namespace, Convert.ToBase64String(parameters.Modulus));
            writer.WriteElementString(XD.XmlSignatureDictionary.Prefix.Value, XD.XmlSignatureDictionary.Exponent, XD.XmlSignatureDictionary.Namespace, Convert.ToBase64String(parameters.Exponent));
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private static RSA ToRsa(string keyString)
        {
            if (keyString == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(keyString));

            RSA rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(keyString);

            return rsa;
        }
    }
}
