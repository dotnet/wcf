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

## Certificate revocation list

A certificate revocation list is generated every time certificates are generated; the CRL is valid for the duration of the CA certificate. 

Each certificate generated has a CRL Distribution Point of _base_address_/Crl - this is automatically set up when run using the scripts above, but if not using the scripts, then the endpoint need to be set up accordingly so that the CRL can be accessed. If this is not set up, certificates may fail to validate due to the CRL being inaccessible 

## Certificate validity

The default validity period of certificates generated is 90 days. In order to deal with potential time skew, certificates are valid for five minutes *prior* to the generation time of the certificate.

## Certificate refresh

Certificates must be refreshed at the end of the certificate expiry - there is no provision for extension of the certificate validity date. 

`RefreshServerCertificates.cmd` can be set up as a scheduled task to automatically perform these functions

