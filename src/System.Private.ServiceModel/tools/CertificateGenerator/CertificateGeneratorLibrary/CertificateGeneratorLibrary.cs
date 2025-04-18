// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using WcfTestCommon;
using X509Certificate2 = System.Security.Cryptography.X509Certificates.X509Certificate2;
using System.IO;
using System.Net;

public class CertificateGeneratorLibrary
{
    private const string ClientCertificateSubject = "WCF Client Certificate";
    private const string CertificateIssuer = "DO_NOT_TRUST_WcfBridgeRootCA";

    private static string s_fqdn = Dns.GetHostEntry("127.0.0.1").HostName;
    private static string s_hostname = Dns.GetHostEntry("127.0.0.1").HostName.Split('.')[0];
    private static string s_testserverbase = string.Empty;
    private static string s_crlFileLocation = string.Empty;
    private static TimeSpan s_validatePeriod;

    private static void RemoveCertificatesFromStore(StoreName storeName, StoreLocation storeLocation)
    {
        X509Store store = CertificateHelper.GetX509Store(storeName, storeLocation);
        Console.WriteLine("  Checking StoreName '{0}', StoreLocation '{1}'", storeName, store.Location);
        {
            if (!CertificateHelper.CurrentOperatingSystem.IsMacOS()) 
            {
                store.Open(OpenFlags.ReadWrite | OpenFlags.IncludeArchived);
            }

            foreach (var cert in store.Certificates.Find(X509FindType.FindByIssuerName, CertificateIssuer, false))
            {
                Console.Write("    {0}. Subject: '{1}'", cert.Thumbprint, cert.SubjectName.Name);
                store.Remove(cert);
                Console.WriteLine(" ... removed");
            }
        }
        Console.WriteLine();
    }

    public static void UninstallAllCerts()
    {
        RemoveCertificatesFromStore(StoreName.My, StoreLocation.CurrentUser);
        RemoveCertificatesFromStore(StoreName.My, StoreLocation.LocalMachine);

        RemoveCertificatesFromStore(StoreName.Root, StoreLocation.LocalMachine);
        RemoveCertificatesFromStore(StoreName.Root, StoreLocation.CurrentUser);

        RemoveCertificatesFromStore(StoreName.TrustedPeople, StoreLocation.LocalMachine);
        RemoveCertificatesFromStore(StoreName.TrustedPeople, StoreLocation.CurrentUser);
    }

    public static int SetupCerts(string testserverbase, TimeSpan validatePeriod, string crlFileLocation, int httpPort=80)
    {
        s_testserverbase = testserverbase;
        s_validatePeriod = validatePeriod;
        s_crlFileLocation = crlFileLocation;

        UninstallAllCerts();

        CertificateGenerator certificateGenerate = new CertificateGenerator();
        certificateGenerate.CertificatePassword = "test";

        certificateGenerate.CrlServiceUri = $"{s_fqdn}:{httpPort}";
        certificateGenerate.ValidityPeriod = s_validatePeriod;

        if (!string.IsNullOrEmpty(s_testserverbase))
        {
            certificateGenerate.CrlUriRelativePath += "/" + s_testserverbase;
        }
        certificateGenerate.CrlUriRelativePath += "/TestHost.svc/Crl";

        //Create and install root and server cert
        CertificateManager.CreateAndInstallLocalMachineCertificates(certificateGenerate);

        //Create and Install expired cert
        CertificateCreationSettings certificateCreationSettings = new CertificateCreationSettings()
        {
            FriendlyName = "WCF Bridge - TcpExpiredServerCertResource",
            ValidityType = CertificateValidityType.Expired,
            ValidityNotBefore = DateTime.UtcNow - TimeSpan.FromDays(4),
            ValidityNotAfter = DateTime.UtcNow - TimeSpan.FromDays(2),
            //If you specify multiple subjects, the first one becomes the subject, and all of them become Subject Alt Names.
            //In this case, the certificate subject is  CN=fqdn, OU=..., O=... , and SANs will be  fqdn, hostname, localhost
            //We do this so that a single WCF service setup can deal with all the possible addresses that a client might use.
            Subject = s_fqdn,
            SubjectAlternativeNames = new string[] { s_fqdn, s_hostname, "localhost" }
        };
        CreateAndInstallMachineCertificate(certificateGenerate, certificateCreationSettings);


        //Create and Install TcpCertificateWithServerAltName
        certificateCreationSettings = new CertificateCreationSettings()
        {
            FriendlyName = "WCF Bridge - TcpCertificateWithServerAltNameResource",
            Subject = "not-real-subject-name",
            SubjectAlternativeNames = new string[] { "not-real-subject-name", "not-real-subject-name.example.com", s_fqdn, s_hostname, "localhost" }
        };
        CreateAndInstallMachineCertificate(certificateGenerate, certificateCreationSettings);

        //TcpCertificateWithSubjectCanonicalNameDomainName
        certificateCreationSettings = new CertificateCreationSettings()
        {
            FriendlyName = "WCF Bridge - TcpCertificateWithSubjectCanonicalNameDomainNameResource",
            Subject = s_hostname,
            SubjectAlternativeNames = new string[0],
            ValidityType = CertificateValidityType.NonAuthoritativeForMachine
        };
        CreateAndInstallMachineCertificate(certificateGenerate, certificateCreationSettings);

        //WCF Bridge - TcpCertificateWithSubjectCanonicalNameFqdn
        certificateCreationSettings = new CertificateCreationSettings()
        {
            FriendlyName = "WCF Bridge - TcpCertificateWithSubjectCanonicalNameFqdnResource",
            Subject = s_fqdn,
            SubjectAlternativeNames = new string[0],
            ValidityType = CertificateValidityType.NonAuthoritativeForMachine
        };
        CreateAndInstallMachineCertificate(certificateGenerate, certificateCreationSettings);

        //TcpCertificateWithSubjectCanonicalNameLocalhost
        certificateCreationSettings = new CertificateCreationSettings()
        {
            FriendlyName = "WCF Bridge - TcpCertificateWithSubjectCanonicalNameLocalhostResource",
            Subject = "localhost",
            SubjectAlternativeNames = new string[0],
            ValidityType = CertificateValidityType.NonAuthoritativeForMachine
        };
        CreateAndInstallMachineCertificate(certificateGenerate, certificateCreationSettings);

        //TcpRevokedServerCert
        certificateCreationSettings = new CertificateCreationSettings()
        {
            FriendlyName = "WCF Bridge - TcpRevokedServerCertResource",
            ValidityType = CertificateValidityType.Revoked,
            Subject = s_fqdn,
            SubjectAlternativeNames = new string[] { s_fqdn, s_hostname, "localhost" }
        };
        CreateAndInstallMachineCertificate(certificateGenerate, certificateCreationSettings);

        //TcpInvalidEkuServerCert
        certificateCreationSettings = new CertificateCreationSettings()
        {
            FriendlyName = "WCF Bridge - TcpInvalidEkuServerCert",
            ValidityType = CertificateValidityType.Valid,
            Subject = s_fqdn,
            SubjectAlternativeNames = new string[] { s_fqdn, s_hostname, "localhost" },
            EKU = new List<Org.BouncyCastle.Asn1.X509.KeyPurposeID> { Org.BouncyCastle.Asn1.X509.KeyPurposeID.id_kp_clientAuth }
        };
        CreateAndInstallMachineCertificate(certificateGenerate, certificateCreationSettings);

        //STSMetaData
        certificateCreationSettings = new CertificateCreationSettings()
        {
            FriendlyName = "WCF Bridge - STSMetaData",
            ValidityType = CertificateValidityType.Valid,
            Subject = "STSMetaData",
            EKU = new List<Org.BouncyCastle.Asn1.X509.KeyPurposeID>()
        };
        CreateAndInstallMachineCertificate(certificateGenerate, certificateCreationSettings);

        //Create and install client cert
        certificateCreationSettings = new CertificateCreationSettings()
        {
            FriendlyName = "WCF Bridge - UserCertificateResource",
            Subject = "WCF Client Certificate",
        };
        X509Certificate2 certificate = certificateGenerate.CreateUserCertificate(certificateCreationSettings).Certificate;
        CertificateManager.AddToStoreIfNeeded(StoreName.My, StoreLocation.LocalMachine, certificate);

        //Create CRL and save it
        FileInfo file = new FileInfo(s_crlFileLocation);
        file.Directory.Create();

        File.WriteAllBytes(s_crlFileLocation, certificateGenerate.CrlEncoded);

        return 0;
    }

    private static void CreateAndInstallMachineCertificate(CertificateGenerator certificateGenerate, CertificateCreationSettings certificateCreationSettings)
    {
        X509Certificate2 certificate = certificateGenerate.CreateMachineCertificate(certificateCreationSettings).Certificate;
        CertificateManager.AddToStoreIfNeeded(StoreName.My, StoreLocation.LocalMachine, certificate);
    }
}
