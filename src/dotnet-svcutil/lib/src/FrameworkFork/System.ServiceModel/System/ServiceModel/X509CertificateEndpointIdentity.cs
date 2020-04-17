// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    using System;
    using System.IdentityModel.Claims;
    using System.IdentityModel.Policy;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.Xml;
    using Microsoft.Xml.Serialization;

    public class X509CertificateEndpointIdentity : EndpointIdentity
    {
        private X509Certificate2Collection _certificateCollection = new X509Certificate2Collection();

        public X509CertificateEndpointIdentity(X509Certificate2 certificate)
        {
            if (certificate == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("certificate");

            base.Initialize(new Claim(ClaimTypes.Thumbprint, certificate.GetCertHash(), Rights.PossessProperty));

            _certificateCollection.Add(certificate);
        }

        public X509CertificateEndpointIdentity(X509Certificate2 primaryCertificate, X509Certificate2Collection supportingCertificates)
        {
            if (primaryCertificate == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("primaryCertificate");

            if (supportingCertificates == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("supportingCertificates");

            base.Initialize(new Claim(ClaimTypes.Thumbprint, primaryCertificate.GetCertHash(), Rights.PossessProperty));

            _certificateCollection.Add(primaryCertificate);

            for (int i = 0; i < supportingCertificates.Count; ++i)
            {
                _certificateCollection.Add(supportingCertificates[i]);
            }
        }

        internal X509CertificateEndpointIdentity(XmlDictionaryReader reader)
        {
            if (reader == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");

            reader.MoveToContent();
            if (reader.IsEmptyElement)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(string.Format(SRServiceModel.UnexpectedEmptyElementExpectingClaim, XD.AddressingDictionary.X509v3Certificate.Value, XD.AddressingDictionary.IdentityExtensionNamespace.Value)));

            reader.ReadStartElement(XD.XmlSignatureDictionary.X509Data, XD.XmlSignatureDictionary.Namespace);
            while (reader.IsStartElement(XD.XmlSignatureDictionary.X509Certificate, XD.XmlSignatureDictionary.Namespace))
            {
                X509Certificate2 certificate = new X509Certificate2(Convert.FromBase64String(reader.ReadElementString()));
                if (_certificateCollection.Count == 0)
                {
                    // This is the first certificate. We assume this as the primary 
                    // certificate and initialize the base class.
                    base.Initialize(new Claim(ClaimTypes.Thumbprint, certificate.GetCertHash(), Rights.PossessProperty));
                }

                _certificateCollection.Add(certificate);
            }

            reader.ReadEndElement();

            if (_certificateCollection.Count == 0)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(string.Format(SRServiceModel.UnexpectedEmptyElementExpectingClaim, XD.AddressingDictionary.X509v3Certificate.Value, XD.AddressingDictionary.IdentityExtensionNamespace.Value)));
        }

        public X509Certificate2Collection Certificates
        {
            get { return _certificateCollection; }
        }

        internal override void WriteContentsTo(XmlDictionaryWriter writer)
        {
            if (writer == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");

            writer.WriteStartElement(XD.XmlSignatureDictionary.Prefix.Value, XD.XmlSignatureDictionary.KeyInfo, XD.XmlSignatureDictionary.Namespace);
            writer.WriteStartElement(XD.XmlSignatureDictionary.Prefix.Value, XD.XmlSignatureDictionary.X509Data, XD.XmlSignatureDictionary.Namespace);
            for (int i = 0; i < _certificateCollection.Count; ++i)
            {
                writer.WriteElementString(XD.XmlSignatureDictionary.X509Certificate, XD.XmlSignatureDictionary.Namespace, Convert.ToBase64String(_certificateCollection[i].RawData));
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}
