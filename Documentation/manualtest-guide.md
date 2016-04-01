Manual Tests Instruction
========================

**Windows: Windows Authentication tests**

Tests:  
```csharp
    WindowsAuthentication_RoundTrips_Echo
```

Requirements:

1. Must run on a domain machine

Steps:

1. Enable the test <br/>
In src\System.Private.ServiceModel\tests\Scenarios\Security\TransportSecurity\Https\ClientCredentialTypeTests.cs,
Remove  `[ActiveIssue(53)]` above the test method 
2. Go to the root of WCF source, run all scenario tests
```
    .\build.cmd /p:WithCategories=OuterLoop
```


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
2. On Linux, `pushd ${GitWcfRoot}/src/System.Private.ServiceModel/tools/setupfiles`
3. Install the certificate file `sudo ./BridgeCertificateInstaller.sh --bridge-host ${BridgeHost} --cert-file ~/tmp/wcfca.crt`. Note that the --cert-file should be a file in the the user tmp directory and the filename must have a .crt extension
4. Specify the Bridge host location `export BridgeHost=${BridgeHost}`
5. Run tests as needed, such as with `${GitWcfRoot}/build.sh /p:WithCategories=OuterLoop`

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
  `psexec -s -h <command>` will start a command as an elevated LOCAL SYSTEM user

Specific terminology: 

* Where a user `user` has access to a domain controller `DC.DOMAIN.COM` that can issue Kerberos tickets <br/> 
  _Note: Capitalization of the domain controller FQDN is required_ 
* The Kerberos realm is `DC.DOMAIN.COM`
* User's Kerberos credential is `user@DC.DOMAIN.COM`

Assumes: 

* A user has access to a domain controller `DC.DOMAIN.COM` that can issue Kerberos tickets <br/> 
  _Note: Capitalization of the domain controller FQDN is required_  
  
* ${GitWcfRoot} : root of the WCF Core git repo
* ${BridgeHost} : Bridge host machine name  
* ${NegotiateTestRealm} :  Kerberos realm on which server is running
* ${NegotiateTestUserName} : Valid user name on Kerberos realm specified by ${NegotiateTestRealm} 
* ${NegotiateTestPassword} : User's password on the Kerberos realm

Steps:

1. Ensure the Bridge is running on a Windows machine and as LOCAL MACHINE with `-allowRemote`
2. On Linux, `pushd $(GitWcfRoot)/src/System.Private.ServiceModel/tools/setupfiles`
3. Install the certificate file `sudo ./BridgeCertificateInstaller.sh --bridge-host ${BridgeHost} --cert-file ~/tmp/wcfca.crt`. Note that the --cert-file should be a file in the the user tmp directory and the filename must have a .crt extension
4. Specify the Bridge host location `export BridgeHost=${BridgeHost}`. <br/> 
   _Note: this step is not needed if running tests using 'build.sh'_
5. Initialize Kerberos by running `kinit ${NegotiateTestUserName}@${NegotiateTestRealm}`; enter the password for the user, ${NegotiateTestPassword}
6. Run `klist` to see that the Kerberos ticket has been acquired 
7. Run tests as needed, such as with `${GitWcfRoot}/build.sh /p:WithCategories=OuterLoop /p:BridgeHost=${BridgeHost}`
8. Once tests are completed, run `kdestroy -A`

