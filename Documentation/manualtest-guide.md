Manual Tests Instruction
========================

**Linux: HTTPS tests** 

Tests:
```csharp
   HttpsTests.SameBinding_Soap12_EchoString 
   HttpsTests.CrossBinding_Soap11_EchoString
   HttpsTests.SameBinding_Soap11_EchoString 
   HttpsTests.ServerCertificateValidation_EchoString
   Https_ClientCredentialTypeTests.BasicAuthenticationInvalidPwd_throw_MessageSecurityException
   Https_ClientCredentialTypeTests.BasicAuthentication_RoundTrips_Echo
```

The WCF Test Root CA certificate need to be retrieved from the Bridge and installed on the local machine before tests can be run

Requirements: 

* Requires a Windows machine running the Bridge  

Assumes: 

* ${GitWcfRoot} is the root of the WCF Core git repo
* ${BridgeHost} is the Bridge host machine name  

Steps:

1. Ensure the Bridge is running on a Windows machine with remote access allowed: `startBridge.cmd -allowRemote` <br/> 
   If running Bridge on a machine not on the same subnet as the test client machine: `startBridge.cmd -allowRemote -remoteAddresses:*` 
2. On Linux, go to ${GitWcfRoot} and run `./build.sh` 
3. On Linux, `pushd ${GitWcfRoot}/src/System.Private.ServiceModel/tools/setupfiles`
4. Install the certificate file `sudo ./BridgeCertificateInstaller.sh --bridge-host ${BridgeHost} --cert-file ~/tmp/wcfca.crt`. Note that the --cert-file should be a file in the the user tmp directory and the filename must have a .crt extension
5. Specify the Bridge host location `export BridgeHost=${BridgeHost}`
6. Run tests as needed, such as with `${GitWcfRoot}/build.sh /p:WithCategories=OuterLoop`

**Linux: NegotiateStream tests with ambient credentials** 

Tests:
```csharp
   StreamingTests.NetTcp_TransportSecurity_StreamedRequest_RoundTrips_String
   StreamingTests.NetTcp_TransportSecurity_Streamed_RoundTrips_String_WithSingleThreadedSyncContext
   StreamingTests.NetTcp_TransportSecurity_Streamed_RoundTrips_String
   StreamingTests.NetTcp_TransportSecurity_Streamed_Async_RoundTrips_String
   StreamingTests.NetTcp_TransportSecurity_StreamedRequest_Async_RoundTrips_String
   StreamingTests.NetTcp_TransportSecurity_StreamedResponse_Async_RoundTrips_String
   StreamingTests.NetTcp_TransportSecurity_StreamedResponse_RoundTrips_String
   StreamingTests.NetTcp_TransportSecurity_Streamed_TimeOut_Long_Running_Operation
   StreamingTests.NetTcp_TransportSecurity_Streamed_Async_RoundTrips_String_WithSingleThreadedSyncContext
   Tcp_ClientCredentialTypeTests.SameBinding_DefaultSettings_EchoString
   Tcp_ClientCredentialTypeTests.SameBinding_SecurityModeTransport_EchoString
   NegotiateStream_Http_Tests.NegotiateStream_Http_AmbientCredentials

```

The WCF Test Root CA certificate need to be retrieved from the Bridge and installed on the local machine before tests can be run.
In addition, the Bridge must be running under a LOCAL SYSTEM context

Requirements: 

* Requires a Windows machine running the Bridge
* The Windows machine should be on a Windows domain 
* A tool such as `psexec` is needed to start the Bridge as LOCAL SYSTEM. <br/> 
  Download the tool from https://technet.microsoft.com/en-us/sysinternals/psexec.aspx <br />
  `psexec -s -h <command>` will start a command as an elevated LOCAL SYSTEM user

Specific terminology: 

* Where a user `user` has access to a domain controller `DC.REALM.DOMAIN.COM` that can issue Kerberos tickets <br/> 
  _Note: Capitalization of the domain controller FQDN is required_ 
* The Kerberos realm is `DC.REALM.DOMAIN.COM`
* User's Kerberos credential is `user@DC.REALM.DOMAIN.COM`

Assumes: 

* A user has access to a domain controller `DC.REALM.DOMAIN.COM` that can issue Kerberos tickets <br/> 
  _Note: The domain controller FQDN must be in uppercase_
  
* ${GitWcfRoot} : root of the WCF Core git repo
* ${BridgeHost} : Bridge host machine name  
* ${NegotiateTestRealm} :  Kerberos realm on which server is running
* ${NegotiateTestUserName} : Valid user name on Kerberos realm specified by ${NegotiateTestRealm} 
* ${NegotiateTestPassword} : User's password on the Kerberos realm

One time install steps: 

For developers who have never installed Kerberos on their Linux distribution, you need to ensure that Kerberos is installed. 

On Ubuntu/Debian: install the krb5-config and krb5-user packages: `sudo apt-get install krb5-config krb5-user`

1. You will be presented with a screen to set your default authentication realm. Set the default realm to be the domain controller, in all capital letters. For example, if you have a domain `domain.com` and a realm at `realm.domain.com`, the default realm should be `REALM.DOMAIN.COM`. You can find a good default value to set by logging into the machine where you intend to run the Bridge (Windows), opening a `cmd` prompt, and then typing `set USERDNSDOMAIN`. This is the name of your realm.

2. You will be presented with a screen for the Authentication server. Set this to be an actual server name e.g., `DC1.REALM.DOMAIN.COM`. You can find a good default value to set by logging into the machine where you intend to run the Bridge (Windows), opening a `cmd` prompt, and then typing `set LOGONSERVER`. This is the name of your domain controller. 

3. Run `kinit user@REALM.DOMAIN.COM` (where user == a username you have a password to, and REALM.DOMAIN.COM == actual realm), type in your password. You can verify that Kerberos was initialized properly by running `klist` and checking for output like the following
```
Ticket cache: FILE:/tmp/krb5cc_1000
Default principal: user@REALM.DOMAIN.COM

Valid starting       Expires              Service principal
30/03/2016 16:23:42  31/03/2016 02:23:42  krbtgt/REALM.DOMAIN.COM@REALM.DOMAIN.COM
        renew until 31/03/2016 16:23:36
```

If your configuration is incorrect, you can run `sudo dpkg-reconfigure krb5-config` to reconfigure the package. 

Note: if your authentication server changes, you should manually edit `/etc/krb5.conf` and check for an entry like the following. Edit them to appropriate values

```
[realms]
        REALM.DOMAIN.COM = {
                kdc = DC1.REALM.DOMAIN.COM
                admin_server = DC1.REALM.DOMAIN.COM 
        }

```

On RedHat/CentOS: (to be investigated)

Steps:

1. Ensure the Bridge is running on a Windows machine and as LOCAL SYSTEM with `psexec -s -h ${GitWcfRoot}\startBridge.cmd -allowRemote -remoteAddresses:*`
2. On Linux, go to ${GitWcfRoot} and run `./build.sh` 
3. On Linux, `pushd $(GitWcfRoot)/src/System.Private.ServiceModel/tools/setupfiles`
4. Install the certificate file `sudo ./BridgeCertificateInstaller.sh --bridge-host ${BridgeHost} --cert-file ~/tmp/wcfca.crt`. Note that the --cert-file should be a file in the the user tmp directory and the filename must have a .crt extension
5. Specify the Bridge host location `export BridgeHost=${BridgeHost}`. <br/> 
   _Note: this step is not needed if running tests using 'build.sh'_
6. Initialize Kerberos by running `kinit ${NegotiateTestUserName}@${NegotiateTestRealm}`; enter the password for the user, ${NegotiateTestPassword}
7. Run `klist` to see that the Kerberos ticket has been acquired 
8. Run tests as needed, such as with `${GitWcfRoot}/build.sh /p:WithCategories=OuterLoop /p:BridgeHost=${BridgeHost}`
9. Once tests are completed, run `kdestroy -A` on your Linux box
10. Shut down the bridge by running `${GitWcfRoot}\stopBridge.cmd`



