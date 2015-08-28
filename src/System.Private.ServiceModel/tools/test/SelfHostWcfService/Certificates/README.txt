Certificate generation
=======================

This file describes how the certificates were created
and how they are used.  Basic steps are at:
    https://msdn.microsoft.com/en-US/library/Ff648498.aspx


The root certificate authority stored in the "Root" store
----------------------------------------------------------
This is a certificate authority used to create the self-signed certificate.
It was created with these commands:

Template:
    makecert -n "CN=RootCATest" -r -sv RootCATest.pvk RootCATest.cer
Actual:
    makecert -n "CN=DO_NOT_TRUST_RootCAWcfBridge, O=DO_NOT_TRUST, OU=Created by https://github.com/dotnet/wcf" -e "11/20/2015" -r -sv RootCAWcfBridge.pvk RootCAWcfBridge.cer

The password option "test" was selected when one was requested for the private key.
This password appears necessary or the private key cannot be exported.

At runtime, the Bridge will load this certificate and place it into the Root store.


The self-signed certificate for SSL stored in the "My" store
-------------------------------------------------------------
This is the self-signed certificate used for SSL testing.
It was created with this command:

Template:
    makecert -sk <<UniqueKeyName>> -iv RootCATest.pvk -n "CN=<<MachineName>>" -ic RootCATest.cer -sr localmachine -ss my -sky exchange -pe 
Actual:
    Run this to create the certificate *and put it into the store* (critical step):
      makecert -sk "RootCAWcfBridge" -iv RootCAWcfBridge.pvk -n "CN=localhost" -ic RootCAWcfBridge.cer -sr localmachine -ss my -sky exchange -pe
    Then start the certificate manager and do these steps:
      1. Under the "Personal"folder find the "localhost" certificate whose issuer is the "DO_NOT_TRUST_RootCAWcfBridge" above
      2. Right-click | All Tasks | Export
      3. Check "Export private keys" | NEXT
      4. NEXT
      5. When asked for a name to save, choose RootCAWcfBridge.pfx in the same folder as RootCAWcfBridge.cer

The reason for this use of the certificate manager UI is that makecert writes the .pfx file as UNICODE,
but certificate manager writews it as ASCII. If written as UNICODE it fails to load properly.

At runtime, the Bridge will import this .pfx file into the "My" store.
It will also configure Http to use this certificate for SSL.


