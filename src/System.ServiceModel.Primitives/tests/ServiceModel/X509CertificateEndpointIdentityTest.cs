// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public class X509CertificateEndpointIdentityTest : ConditionalWcfTest
{
    [WcfFact]
    public static void X509Certificate_RoundTrip_Succeeds()
    {
        // Create a self-signed certificate for testing
        X509Certificate2 certificate = CreateTestCertificate();
        
        // Manually construct the XML that would be sent from the server
        // This ensures we're testing deserialization independently of WriteContentsTo
        string certificateBase64 = Convert.ToBase64String(certificate.RawData);
        string xml = $@"<Identity xmlns=""http://schemas.xmlsoap.org/ws/2006/02/addressingidentity"">
    <KeyInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"">
        <X509Data>
            <X509Certificate>{certificateBase64}</X509Certificate>
        </X509Data>
    </KeyInfo>
</Identity>";
        
        // Deserialize from XML
        X509CertificateEndpointIdentity deserializedIdentity;
        using (var reader = XmlReader.Create(new StringReader(xml)))
        {
            // This uses the internal constructor that reads from XmlDictionaryReader
            // ReadIdentity expects to read the Identity element and its contents
            deserializedIdentity = (X509CertificateEndpointIdentity)typeof(EndpointIdentity)
                .GetMethod("ReadIdentity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .Invoke(null, new object[] { XmlDictionaryReader.CreateDictionaryReader(reader) });
        }
        
        // Verify the certificate was correctly deserialized
        Assert.NotNull(deserializedIdentity);
        X509Certificate2 deserializedCert = Assert.Single(deserializedIdentity.Certificates);
        Assert.Equal(certificate.Thumbprint, deserializedCert.Thumbprint);
        Assert.Equal(certificate.GetCertHash(), deserializedCert.GetCertHash());
    }
    
    [WcfFact]
    [Condition(nameof(Client_Certificate_Installed))]
    public static void X509Certificate_Multiple_RoundTrip_Succeeds()
    {
        // Create two test certificates
        X509Certificate2 primaryCert = CreateTestCertificate();
        X509Certificate2 supportingCert = CreateTestCertificate();
        
        // Manually construct the XML that would be sent from the server with multiple certificates
        // This ensures we're testing deserialization independently of WriteContentsTo
        string primaryCertBase64 = Convert.ToBase64String(primaryCert.RawData);
        string supportingCertBase64 = Convert.ToBase64String(supportingCert.RawData);
        string xml = $@"<Identity xmlns=""http://schemas.xmlsoap.org/ws/2006/02/addressingidentity"">
    <KeyInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"">
        <X509Data>
            <X509Certificate>{primaryCertBase64}</X509Certificate>
            <X509Certificate>{supportingCertBase64}</X509Certificate>
        </X509Data>
    </KeyInfo>
</Identity>";
        
        // Deserialize from XML
        X509CertificateEndpointIdentity deserializedIdentity;
        using (var reader = XmlReader.Create(new StringReader(xml)))
        {
            // This uses the internal constructor that reads from XmlDictionaryReader
            // ReadIdentity expects to read the Identity element and its contents
            deserializedIdentity = (X509CertificateEndpointIdentity)typeof(EndpointIdentity)
                .GetMethod("ReadIdentity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .Invoke(null, new object[] { XmlDictionaryReader.CreateDictionaryReader(reader) });
        }
        
        // Verify both certificates were correctly deserialized
        Assert.NotNull(deserializedIdentity);
        Assert.Equal(2, deserializedIdentity.Certificates.Count);
        Assert.Equal(primaryCert.Thumbprint, deserializedIdentity.Certificates[0].Thumbprint);
        Assert.Equal(supportingCert.Thumbprint, deserializedIdentity.Certificates[1].Thumbprint);
    }
    
    private static X509Certificate2 CreateTestCertificate()
    {
        // Create a simple self-signed certificate for testing
        using (RSA rsa = RSA.Create(2048))
        {
            var request = new CertificateRequest(
                "CN=Test Certificate",
                rsa,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
            
            return request.CreateSelfSigned(
                DateTimeOffset.UtcNow.AddDays(-1),
                DateTimeOffset.UtcNow.AddDays(365));
        }
    }
}
