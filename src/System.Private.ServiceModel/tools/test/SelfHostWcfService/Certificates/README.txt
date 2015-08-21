Certificate generation
=======================

This file describes how the certificates were created
and how they are used.

The root certificate authority stored in the "Root" store
----------------------------------------------------------
This is a certificate authority used to create the self-signed certificate.
It was created with these commands:

    makecert -n "CN=DO_NOT_TRUST_RootCAWcfBridge, O=DO_NOT_TRUST, OU=Created by https://github.com/dotnet/wcf" -e "11/20/2015" -r -sv RootCAWcfBridge.pvk RootCAWcfBridge.cer

The password option "None" was selected.
At runtime, the Bridge will load this certificate and place it into the Root store.


The self-signed certificate for SSL stored in the "My" store
-------------------------------------------------------------
This is the self-signed certificate used for SSL testing.
It was created with this command:

    pvk2pfx -pvk RootCAWcfBridge.pvk -spc RootCAWcfBridge.cer -pfx RootCAWcfBridge.pfx

At runtime, the Bridge will import this .pfx file into the "My" store.
It will also configure Http to use this certificate for SSL.


