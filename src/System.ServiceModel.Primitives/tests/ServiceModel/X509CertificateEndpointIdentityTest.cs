// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public static class X509CertificateEndpointIdentityTest
{
    [WcfFact]
    public static void X509Certificate_RoundTrip_Succeeds()
    {
        // Create a self-signed certificate for testing
        X509Certificate2 certificate = CreateTestCertificate();
        
        // Create an X509CertificateEndpointIdentity with the certificate
        var identity = new X509CertificateEndpointIdentity(certificate);
        
        // Serialize to XML
        var sb = new StringBuilder();
        var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
        using (var writer = XmlWriter.Create(sb, settings))
        {
            // Write the Identity element
            writer.WriteStartElement("Identity", "http://schemas.xmlsoap.org/ws/2006/02/addressingidentity");
            identity.WriteContentsTo(XmlDictionaryWriter.CreateDictionaryWriter(writer));
            writer.WriteEndElement();
        }
        
        string xml = sb.ToString();
        
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
        Assert.Equal(1, deserializedIdentity.Certificates.Count);
        Assert.Equal(certificate.Thumbprint, deserializedIdentity.Certificates[0].Thumbprint);
        Assert.Equal(certificate.GetCertHash(), deserializedIdentity.Certificates[0].GetCertHash());
    }
    
    [WcfFact]
    public static void X509Certificate_Multiple_RoundTrip_Succeeds()
    {
        // Create two test certificates
        X509Certificate2 primaryCert = CreateTestCertificate();
        X509Certificate2 supportingCert = CreateTestCertificate();
        
        var supportingCerts = new X509Certificate2Collection { supportingCert };
        
        // Create an X509CertificateEndpointIdentity with primary and supporting certificates
        var identity = new X509CertificateEndpointIdentity(primaryCert, supportingCerts);
        
        // Serialize to XML
        var sb = new StringBuilder();
        var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
        using (var writer = XmlWriter.Create(sb, settings))
        {
            writer.WriteStartElement("Identity", "http://schemas.xmlsoap.org/ws/2006/02/addressingidentity");
            identity.WriteContentsTo(XmlDictionaryWriter.CreateDictionaryWriter(writer));
            writer.WriteEndElement();
        }
        
        string xml = sb.ToString();
        
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
            
            var certificate = request.CreateSelfSigned(
                DateTimeOffset.UtcNow.AddDays(-1),
                DateTimeOffset.UtcNow.AddDays(365));
            
            return certificate;
        }
    }
}
