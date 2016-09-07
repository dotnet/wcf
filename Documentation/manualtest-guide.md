
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

The WCF Test Root CA certificate need to be retrieved from the server and installed on the local machine before tests can be run. This is automatically done when running build.sh / build.cmd. 

When running build.sh on a Linux machine, ensure that the user has permission to run the `InstallRootCertificate.sh` script with sudo (i.e., the user is listed in the sudoers file). For best results, add the following to the sudoers file
```
user ALL=(ALL) NOPASSWD:ALL
```
see the _visudo_ man page for details on how to do this 

Steps:

1. Ensure the service is running
2. On Linux, go to ${GitWcfRoot} and run `./build.sh -p:WithCategories=OuterLoop -p:ServiceUri=your-service-uri -p:` 

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
The self-hosted WCF Service or IIS Hosted service must be running under a LOCAL SYSTEM context

Furthermore, the WCF Test Root CA certificate needs to be retrieved from the service and installed on the local machine before tests can be run - this is automatically done when running tests if the user has the ability to run scripts as a superuser (sudo)


Requirements: 

* Requires a Windows machine running the WCF test service 
* * The Windows machine should be on a Windows domain 
* If using the self-hosted service (as opposed to the IIS-hosted service), a tool such as `psexec` is needed to start the self-hosted service as LOCAL SYSTEM. 
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
* ${ServiceUri} : WCF Service URI 
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

1. Ensure the service is running on a Windows machine - either IIS hosted or self-hosted 
2. On Linux, go to ${GitWcfRoot} and run `./build.sh /p:WithCategories=OuterLoop` to do a run and install the certificates 
3. Initialize Kerberos by running `kinit ${NegotiateTestUserName}@${NegotiateTestRealm}`; enter the password for the user, ${NegotiateTestPassword}
4. Run `klist` to see that the Kerberos ticket has been acquired 
5. Run tests as needed, such as with `${GitWcfRoot}/build.sh /p:WithCategories=OuterLoop /p:ServiceUri=${ServiceUri} /p:Negotiate_Available=true /p:Kerberos_Available=true`
See section titled "Test Conditions" to see what conditions are specifiable. 
6. Once tests are completed, run `kdestroy -A` on your Linux box

## Test conditions 
Tests that require special conditions to run are Conditioned to prevent running when the conditions are not true - for example, a Kerberos environment may not be available on all machines, so these tests will not run by default. In order to run tests, we must specify each condition we know to be true on the command line - such as 

`build.sh /p:WithCategories=OuterLoop /p:ServiceUri=${ServiceUri} /p:Negotiate_Available=true /p:Kerberos_Available=true`

Available conditions are: 

* SPN_Available
* Ambient_Credentials_Available
* Explicit_Credentials_Available
* Basic_Authentication_Available
* Digest_Authentication_Available
* Windows_Authentication_Available
* NTLM_Available
* SSL_Available

