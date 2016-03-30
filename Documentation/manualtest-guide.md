Manual Tests Instruction
========================

**Windows: Windows Authentication tests**

Tests:  
```csharp
    WindowsAuthentication_RoundTrips_Echo
```

Requirements:

1. Must run on a domain machine.

Steps:

1. Enable the test
in src\System.Private.ServiceModel\tests\Scenarios\Security\TransportSecurity\Https\ClientCredentialTypeTests.cs,
Remove  [ActiveIssue(53)] above test method WindowsAuthentication_RoundTrips_Echo()
2. Go to the root of WCF source, run all scenario tests.
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

* $(GitWcfRoot) is the root of the WCF Core git repo
* $(BridgeHost) is the Bridge host machine name  

Steps:

1. Ensure the Bridge is running on a Windows machine with remote access allowed: `startBridge.cmd -allowRemote`
2. On Linux, `pushd $(GitWcfRoot)/src/System.Private.ServiceModel/tools/setupfiles`
3. Install the certificate file `sudo ./BridgeCertificateInstaller.sh --bridge-host $(BridgeHost) --cert-file ~/tmp/wcfca.crt`. Note that the --cert-file should be a file in the the user tmp directory and the filename must have a .crt extension
4. Specify the Bridge host location `export BridgeHost=$(BridgeHost)`
5. Run tests as needed, such as with `$(GitWcfRoot)/build.sh /p:WithCategories=OuterLoop`

**Linux: NegotiateStream tests** 

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
* A tool such as `psexec` is needed to start the Bridge as LOCAL SYSTEM 

Assumes: 

* $(GitWcfRoot) is the root of the WCF Core git repo
* $(BridgeHost) is the Bridge host machine name  

Steps:

1. Ensure the Bridge is running on a Windows machine and as LOCAL MACHINE with `-allowRemote`
2. On Linux, `pushd $(GitWcfRoot)/src/System.Private.ServiceModel/tools/setupfiles`
3. Install the certificate file `sudo ./BridgeCertificateInstaller.sh --bridge-host $(BridgeHost) --cert-file ~/tmp/wcfca.crt`. Note that the --cert-file should be a file in the the user tmp directory and the filename must have a .crt extension
4. Specify the Bridge host location `export BridgeHost=$(BridgeHost)`
5. Initialize Kerberos `kinit user@DC.DOMAIN.COM` and enter the password
6. Run `klist` to see that the Kerberos ticket has been acquired 
7. Run tests as needed, such as with `$(GitWcfRoot)/build.sh /p:WithCategories=OuterLoop`
8. Once tests are completed, run `kdestroy -A`

