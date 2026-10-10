# Dynamic test certificate generation

## Background

In order to test certificate-based security scenarios, we generate test certificates for: 

* Root Certificate Authority (CA) 
* Machine certificate
* Certificate Revocation List (CRL) 

This happens using the CertificateGenerator tool, located at `src/System.Private.ServiceModel/tools/CertificateGenerator`

This tool is automatically called on the following occasions: 

* Creating a WCF IIS Hosted Service using `SetupWcfIISHostedService.cmd`
* Starting a WCF Self Hosted Service using `StartWCFSelfHostedSvc.cmd`
* Running `RefreshServerCertificates.cmd`

Certificate generation happens using the CertificateGenerator tool, Certificate generation happens only on Windows machines

Upon calling, the CertificateGenerator generates the following certificates: 

* Root Certificate
* Client certificate
* Machine certificates
  * localhost
  * machine name 
  * machine fully qualified domain name
  * revoked certificate
  * expired certificate
  * with server alt names 

The certificate revocation list is published to (by default) C:\wcftest\test.crl, and can be changed via app.config

Certificates expire in 90 days

The CA certificates get installed into the machine trusted certificate store
Machine certificates get installed into the machine My store 

## IIS-hosted .NET Framework compatibility

On Windows, the generator uses the Microsoft Enhanced RSA and AES Cryptographic
Provider (CSP provider type 24) with an exchange key (`AT_KEYEXCHANGE`). The
.NET Framework WCF test services use the legacy `X509Certificate2.PrivateKey`
API and CSP key-container information. A CNG-backed certificate can fail in
this path with `Invalid provider type specified`, even when its private-key
permissions are correct. This is a compatibility requirement of these test
services, not a general recommendation to replace CNG in modern applications.

The generator uses a temporary named CSP container and preserves its storage
provider metadata during the PFX import with
`Pkcs12LoaderLimits.PreserveStorageProvider`. The other loader limits remain
enabled. The final key is persisted in the machine key store so that a
certificate-store lookup in another process can reopen it. Do not use
`EphemeralKeySet` for certificates that IIS must retrieve from the store.
Creating an RSA CSP key alone is insufficient if its provider metadata is lost
when attaching or importing the key.

Modern .NET can expose a CSP-backed certificate through `RSACng`; verify the
persisted provider and key specification with .NET Framework or `certutil`
rather than relying only on the managed RSA type.

Run certificate installation and permission configuration from an elevated
Windows PowerShell (`powershell.exe`) shell on the test server. Grant only
private-key read access to the account actually running the application pool.
For an ApplicationPoolIdentity pool named `wcfservice123`, use:

```powershell
.\src\System.Private.ServiceModel\tools\scripts\CertificatePrivateKeyPermissions.ps1 -WcfServiceAccount 'IIS AppPool\wcfservice123'
```

For a pool running as a custom account, supply that account instead. Granting
`Everyone` full control neither fixes provider incompatibility nor follows
least privilege. The permissions script is intentionally CSP-specific; it
does not make existing CNG certificates compatible with .NET Framework WCF.
After updating the generator, regenerate the test certificates, reapply
permissions, and recycle affected application pools to discard cached
certificates.

The .NET generator reads `CertificateGenerator.dll.config`, not the apphost's
`CertificateGenerator.exe.config`. IIS setup writes the service name, validity
period, and CRL file location to the DLL configuration file. Verify that the
CRL distribution URL includes the IIS application path and is accessible from
the client machine.

These are disposable test certificates, including intentionally invalid ones.
Do not use their test CA, fixed PFX password, or exportable keys in production.
For production certificates, use a trusted issuer, appropriate DNS SANs and
EKUs, protected private-key storage, and only the export permissions actually
required by the application.

## Certificate revocation list

A certificate revocation list is generated every time certificates are generated; the CRL is valid for the duration of the CA certificate. 

Each certificate generated has a CRL Distribution Point of _base_address_/Crl. By default the CertificateGenerator uses HTTP port 80, but it can also be given an explicit HTTP port for hosting scenarios such as CoreWCF on Kestrel where port sharing is not available. This is automatically set up when run using the scripts above, but if not using the scripts, then the endpoint needs to be set up accordingly so that the CRL can be accessed. If this is not set up, certificates may fail to validate due to the CRL being inaccessible.

## Certificate validity

The default validity period of certificates generated is 90 days. In order to deal with potential time skew, certificates are valid for five minutes *prior* to the generation time of the certificate.

## Certificate refresh

Certificates must be refreshed at the end of the certificate expiry - there is no provision for extension of the certificate validity date. 

`RefreshServerCertificates.cmd` can be set up as a scheduled task to automatically perform these functions

