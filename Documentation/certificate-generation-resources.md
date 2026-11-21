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

The IIS test host targets .NET Framework 4.7.2 for ASP.NET compilation and
runtime compatibility. This enables WCF's CNG certificate support by default,
without an explicit `Switch.System.ServiceModel.DisableCngCertificates`
override. Older compatibility targets can take a legacy CSP-only path and fail
with `Invalid provider type specified`, even when key permissions are correct.

The generator uses `RSA.Create`, which produces CNG-backed keys on Windows.
The final key is persisted in the machine key store so that a
certificate-store lookup in another process can reopen it. Do not use
`EphemeralKeySet` for certificates that IIS must retrieve from the store.

Run certificate installation and permission configuration from an elevated
Windows PowerShell (`powershell.exe`) shell on the test server. Grant only
private-key read access to the account actually running the application pool.
For an ApplicationPoolIdentity pool named `wcfservice123`, use:

```powershell
.\src\System.Private.ServiceModel\tools\scripts\CertificatePrivateKeyPermissions.ps1 -WcfServiceAccount 'IIS AppPool\wcfservice123'
```

For a pool running as a custom account, supply that account instead. Granting
`Everyone` full control neither fixes provider incompatibility nor follows
least privilege. The permissions script uses `GetRSAPrivateKey` and supports
legacy CSP containers and persisted Microsoft Software Key Storage Provider
CNG keys. It fails explicitly for unsupported key providers.
After updating the generator, regenerate the test certificates, reapply
permissions, and recycle affected application pools to discard cached
certificates.

The .NET generator reads `CertificateGenerator.dll.config`, not the apphost's
`CertificateGenerator.exe.config`. IIS setup writes the service name, validity
period, and CRL file location to the DLL configuration file. Verify that the
CRL distribution URL includes the IIS application path and is accessible from
the client machine.

IIS certificate setup also saves these settings automatically as
`C:\WCFTest\CertificateGenerator.dll.config`. This is the source of truth for
scheduled IIS renewal, outside the checkout cleaned by the scheduled restart.
Edit this file, not the disposable configuration under `artifacts`, to change
renewal settings. Setup protects this file for SYSTEM, administrators and the
setup identity; IIS workers do not need access to it. Grant read access
explicitly if scheduling renewal under a different, nonadministrator identity.

The refresh validates the saved settings before building, then reapplies them
to the generator output after the build and before any certificate or host
changes. Missing or invalid settings stop the refresh. Ordinary standalone
generator builds retain their existing defaults.

For an existing installation, first verify that the current generator DLL
configuration contains the correct IIS application name, validity period and
absolute CRL path, then migrate it **before rebuilding or cleaning**:

```powershell
.\src\System.Private.ServiceModel\tools\scripts\Copy-CertificateGeneratorConfiguration.ps1 `
    -SourcePath .\artifacts\bin\CertificateGenerator\Release\net10.0\CertificateGenerator.dll.config `
    -DestinationPath C:\WCFTest\CertificateGenerator.dll.config -ProtectDestination
```

Do not migrate build defaults with an empty `testserverbase`; these are rejected.
If the old settings have already been lost, restore the intended IIS settings
explicitly before migration. Adding another PR application validates and
preserves the shared settings instead of replacing them with the PR's path.
Full master setup and setup that adds a missing HTTPS binding recreate
`C:\WCFTest` and write fresh settings along with certificate generation.
Master cleanup-only removes that directory intentionally; complete setup again
before scheduling another renewal. Directory existence alone does not prove
that a previously interrupted setup completed successfully.

Run the configuration and batch-script regression cases without changing IIS
or certificate stores:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\src\System.Private.ServiceModel\tools\scripts\tests\CertificateGeneratorConfiguration.Tests.ps1
```

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

The refresh builds the generator before changing service state, then calls
`RefreshCertificatesWithRollback.ps1` to preserve the original certificate
contexts, CRL and running .NET Framework host state during generation and
private-key permission grants. HTTPS bindings are not changed until every
grant succeeds.

The refresh resolves each pool's configured process identity, including
`SpecificUser` and the built-in service accounts, rather than always granting
the virtual application-pool account. Unknown or unresolvable identities are
rejected before stopping hosts or generating certificates.

If generation or any grant fails, the helper restores the original
certificates with their existing persisted private keys and ACLs, restores
the CRL, recycles affected running IIS pools, and restarts any Framework hosts
it stopped using their original binaries. It does not call the scheduled
restart script during recovery because that script pulls and cleans the
repository. The refresh still returns a nonzero exit code after successful
recovery.

If recovery itself fails, the log reports the failure and the protected
backup directory containing serialized certificate contexts, the previous
CRL and host metadata. These contexts preserve key-provider associations,
not private-key material; do not delete the original persisted keys.
Backups are removed after success or successful recovery. This recovery
covers caught generation/ACL failures, not machine or process termination,
nor failures after the grants have committed. Restarting a host under a
different account or with an unexpected command line is rejected before
refresh starts. CoreWCF self-hosts are not managed by this scheduled script.

Run the isolated regression cases without modifying the certificate stores:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\src\System.Private.ServiceModel\tools\scripts\tests\RefreshCertificatesWithRollback.Tests.ps1
```
