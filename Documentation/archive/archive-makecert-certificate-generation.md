(Archive) Certificate generation with makecert
=======================

## This document is now deprecated

This document is kept as a reference for how to generate certificates using makecert.exe. We do not use makecert-generated certificates for tests anymore.  

Certificates are now generated using Bridge [CertificateResources](https://github.com/dotnet/wcf/blob/master/Documentation/certificate-generation-resources.md).

------


This file describes how the certificates were created
and how they are used.  Basic steps are at:
    https://msdn.microsoft.com/en-US/library/Ff648498.aspx


The root certificate authority stored in the "Root" store
----------------------------------------------------------
This is a certificate authority used to create the self-signed certificate.
It was created with these commands from an elevated prompt: 

Template:

```
    makecert -n "CN=<<CanonicalName>>" -pe -r -sky exchange -cy authority -sk PrivateKey -eku <<Enhanced key usages>> -ss <<Subject cert store>> -sr <<Subject cert store location>> -m <<months to expiry>> <<filename>> 
```

Actual:

```
    makecert -n "CN=DO_NOT_TRUST_WcfBridgeRootCA, O=DO_NOT_TRUST, OU=Created by https://github.com/dotnet/wcf" -pe -r -sky exchange -cy authority -sk PrivateKey -eku 1.3.6.1.5.5.7.3.2,1.3.6.1.5.5.7.3.1 -ss Root -sr LocalMachine -m 2 WcfBridge_RootCA.cer
```

The EKUs are specified are for Client Authentication (1.3.6.1.5.5.7.3.2) and Server Authentication (1.3.6.1.5.5.7.3.1). 

The certificate gets placed into the *Local Machine*'s *Root Certificate* store, as well as in the current directory's *WcfBridge_RootCA.cer*. 

If prompted for a private key, use the password "test".
This password is necessary or the private key cannot be exported. 

Note that we do not commit the root .pfx file, as it allows creation of other certs. 

At runtime, the Bridge will load this certificate and place it into the Root store.

The self-signed certificate for SSL stored in the "My" (Personal) store
-------------------------------------------------------------
This is the self-signed certificate used for SSL testing.
It was created with this command:

Template:

```
    makecert -n "CN=<<Machine Name>>" -pe -sky exchange -cy end -eku <<Enhanced key usages>> -ss <<Subject store location>> -sr <<Subject cert store>> -ic <<Issuer certificate>> -is <<Issuer cert location>> -ir <<Issuer cert store>> -m <<months to expiry>> WcfBridge_localhost.cer
```

Actual:

Run this to create the certificate *and put it into the store* (critical step):

```
    makecert -n "CN=localhost" -pe -sky exchange -cy end -eku 1.3.6.1.5.5.7.3.2,1.3.6.1.5.5.7.3.1 -ss My -sr LocalMachine -ic .\WcfBridge_RootCA.cer -is Root -ir LocalMachine -m 2 WcfBridge_localhost.cer
```

Then start the certificate manager (certmgr.msc) and do these steps:

1. Under the "Personal" folder find the "localhost" certificate whose issuer is the "DO_NOT_TRUST_WcfBridgeRootCA" above
2. Right-click | All Tasks | Export
3. Check "Export private keys" | NEXT
4. NEXT
5. When asked for a name to save, choose WcfBridge_localhost.pfx in the same folder as WcfBridge_RootCA.cer

The reason for this use of the certificate manager UI is that makecert writes the .pfx file as UNICODE, but certificate manager writes it as ASCII. If written as UNICODE it fails to load properly.

At runtime, the Bridge will import this .pfx file into the "My" (Personal) store. It will also configure HTTP.SYS to use this certificate for SSL.