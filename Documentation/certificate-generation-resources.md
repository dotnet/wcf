# Dynamic test certificate generation

## Background

The Bridge has an endpoint that allows the generation of certificates for test purposes. The Bridge generates the following types of certs on demand: 

* Root Certificate Authority (CA) 
* Machine certificate
* Certificate Revocation List (CRL) 

When the bridge is running in single machine mode (i.e., Bridge and tests are running on the same machine), the Bridge is responsbile for installing and removing certificates from the test machine. 

When the bridge is running in multi-machine mode (i.e., Bridge is running with `-allowRemote`, and tests are running on another machine), the Bridge will install certificates the CA and machine certificate on the Bridge host machine, and the Bridge client will install and remove the machine certificates on the client side. 

The Bridge implements certificate generation as a series of endpoints modelled as "Resources". A Resource is any type that implementes the IResource interface - for example, each endpoint available to test is modelled as a Resource. 

Parameters are passed to/from the endpoints as JSON key-value pairs. 


## Settings on Bridge initialization

* `BridgeCertificatePassword` - string - password for certificates generated/exported. Default is "test"
* `BridgeCertificateValidityPeriod` - TimeSpan - valid timespan for certificates generated. Default is 24 hours


## Resources exposed

### Certificate Authority Resource

name: `WcfService.CertificateResources.CertificateAuthorityResource`

|HTTP Verb|Action|
|---------|------|
|`PUT`| Creates a root CA certificate <br/> Parameters: none <br/> Returns: `thumbprint` - thumbprint of the root CA <br/> *Note: This PUT doesn't need to be issued as this is initialized automatically as part of  any other PUT/GET action on all \*CertificateResources.* <br/> |
|`GET`| Retrieves the Root CA certificate from the Bridge <br/> Returns: `thumbprint` - thumbprint of the Root CA, `certificate` - Base64 encoded X509 Certificate |

### Machine Certificate Resource

name: `WcfService.CertificateResources.MachineCertificateResource`

|HTTP Verb|Action|
|---------|------|
|`PUT`| Creates a machine certificate <br/> Parameters: `subject` - comma-separated list of subject names (the first subject will be the CN of the certificate; all others will be listed as DNS Subject Alternative Names) <br/> Returns: `thumbprint` - thumbprint of the machine or user certificate; `isLocal` - if the certificate was generated for a machine name local to the Bridge |
|`GET`| No parameters <br/> Returns: `subjects` - list of certificate subjects; `thumbprints` - corresponding list of certificate thumbprints |
|`GET`| Retrieves the certificate with a given `subject` or `thumbprint` from the Bridge <br/> Parameters: `thumbprint` - thumbprint of the certificate; OR `subject` - subject name of the cert to retrieve.  If both are specified, `thumbprint` takes precedence <br/> Returns: `thumbprint` - thumbprint of the certificate, `certificate` - Base64 encoded X509 Certificate |

### Certificate Revocation List Resource 

name: `WcfService.CertificateResources.CertificateRevocationListResource`

|HTTP Verb|Action|
|---------|------|
|`PUT`| No parameters <br/> Creates a Certificate Revocation List <br/> Returns: `revokedCertificates` - comma-separated list of revoked certificate serial numbers |
|`PUT`| Parameters: `revoke` - serial number of the certificate to revoke <br/> Returns: `revokedCertificates` - comma-separated list of revoked certificate serial numbers|
|`GET`| Retrieves the CRL from the Bridge as an application/octet-stream <br/> *Not returned as a JSON key-val pair* |

Note that for the certificate revocation list `GET` action, the return value is the raw Certificate Revocation List. All certs created will list */resource/WcfService.CertificateResources.CertificateRevocationListResource* as the CRL distribution point. 

## Usage pattern

The expected usage pattern is to first `PUT` to the requested resource, followed by `GET`ting the resource. Some resources allow the use of `GET` without `PUT`, but it's recommended to follow this practice in case future changes enforce the `PUT`-then-`GET` semantic.

## Certificate validity

The default validity period of certificates generated is 24 hours. In order to deal with potential time skew, certificates are valid for one hour *prior* to the generation time of the certificate.

Certificate Revocation Lists have a "Next Update" field of two minutes after the request time. This helps with time skew issues when testing cross machines. This does mean, however, that Windows will not check the CRL again until the "Next Update" time is hit, which may result in oddness when testing CRL revocation. 