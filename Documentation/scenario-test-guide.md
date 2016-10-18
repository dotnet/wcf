Scenario Testing Guide
======================

Overview
--------
WCF *scenario* tests use end-to-end scenarios that involve network communication between WCF client tests and WCF test services. They are also referred to as "outerloop" tests. The WCF client tests themselves run on .NET Core, which means they can run on all supported platforms such as Windows, various Linux distros, Mac OS X as well as Universal Windows Platform (UWP). The WCF test *services* used by the tests are built on the full .NET Framework and run only on Windows.

Regardless which platform where you run WCF client tests, you will need a WCF test service running on a Windows machine for scenario tests to work. The [Developer Guide](https://github.com/dotnet/wcf/blob/master/Documentation/developer-guide.md) describes the simplest way to run the scenario tests on Windows using self-starting locally self-hosted test services. However, that simple way is not sufficient when testing on a non-Windows OS or in a real-world cross-machine configuration.  This document describes how to run the scenario tests for those situations.

WCF Test services can be hosted in two ways like WCF users would usually do, i.e. self-hosted or IIS-hosted on a Windows machine, such as Windows Server 2012 R2, Windows Server 2016 or Windows 10. The following sections provide instructions of how to host WCF test services and how to run WCF scenario tests.

Self-Hosted WCF Test Service
----------------------------
To start a self-hosted WCF test service on a Windows machine, execute this script that is located at directory [src\System.Private.ServiceModel\tools\scripts](https://github.com/dotnet/wcf/tree/master/src/System.Private.ServiceModel/tools/scripts).
```
StartWCFSelfHostedSvc.cmd
```
This script will add necessary firewall rules, generate and install self-signed test certificates needed by tests, build and start a self-hosted WCF test service process in a new CMD window.  When you are done with this WCF test service, type `exit` and press `enter` in the window where the service was started. Upon shutting down the service, this script will also clean up firewall rules and remove test certificates it installed.

This script requires elevated privileges. If it is run from a non-elevated window, you may be prompted for elevated permission.

**Note:** this is exactly the same script used by the simple auto-starting scenario tests.  The only difference is that now we are running that script on a different machine than the tests.

The source code for this self-hosted test program can be found in the project at [src\System.Private.ServiceModel\tools\SelfHostedWcfService](https://github.com/dotnet/wcf/tree/master/src/System.Private.ServiceModel/tools/SelfHostedWcfService).   The source code for the WCF services themselves is shared with the IIS-hosted test service found at [src\System.Private.ServiceModel\tools\IISHostedWcfService](https://github.com/dotnet/wcf/tree/master/src/System.Private.ServiceModel/tools/IISHostedWcfService).   

IIS-Hosted WCF Test Service
---------------------------
To host WCF services on IIS, the Windows machine has these prerequisites

1. Web Server (IIS) and Application Server roles are installed. This includes ASP.NET and WebSocket protocol.
2. .NET Framework 4.x features including WCF service and activation features are enabled.
3. Ports 80 (HTTP), 443 (HTTPS) and 808 (TCP) are opened.

Copy the script [src\System.Private.ServiceModel\tools\scripts\SetupWcfIISHostedService.cmd](https://github.com/dotnet/wcf/blob/master/src/System.Private.ServiceModel/tools/scripts/SetupWcfIISHostedService.cmd) to the Windows machine with IIS and run it in an elevated command window.
```
SetupWcfIISHostedService.cmd {id}
```

where `id` is an identifier that will be appended to the name of the IIS application to be created.  For example, command `SetupWcfIISHostedService.cmd Test` will create an IIS application named "WCFService*Test*". By default, this script will create a clone of WCF GitHub repository, an IIS application pool and an IIS application from [src\System.Private.ServiceModel\tools\IISHostedWcfService](https://github.com/dotnet/wcf/tree/master/src/System.Private.ServiceModel/tools/IISHostedWcfService). The script will also install necessary test certificates and add IIS HTTPS binding for testing.

If you wish to use an existing WCF repository on the IIS machine instead of cloning a new copy, run the script like this:
```
SetupWcfIISHostedService.cmd {id} /p:Path-to-WCF-repo
```
To see a list of options and sample usage, run the command like this:
```
SetupWcfIISHostedService.cmd /?
```
This is one time setup. Once you set it up, you do not need to build or start the service manually. IIS will build and activate the service automatically upon the first request from tests.

Running WCF Scenario Tests
-------------------------
As described in the [Developer Guide](https://github.com/dotnet/wcf/blob/master/Documentation/developer-guide.md), the simplest way to run scenario tests self-hosted on the same machine is:
```
build-tests.cmd -outerloop
```

But to run scenario tests against an IIS-hosted test service or a self-hosted test service on a different machine, you will need to specify the WCF service base address via `/p:ServiceUri` parameter to the build script.

If you started the self-hosted service as described above on a machine called "myServer", you just need to set `\p:ServiceUri` to be the name of that machine when you run the tests on some other client machine. So for this self-hosted example, you would run the scenario tests like this from the client machine:
```
build-tests.cmd -outerloop -- /p:ServiceUri=myServer
```
or similarly, you can run the tests from a non-Windows OS with:
```
./build-tests.sh -outerloop -- /p:ServiceUri=myServer
```

To run the tests against an IIS hosted WCF service `ServiceUri` needs to be of the form `{MachineName}/WCFService{id}`. For example, if you set up an IIS hosted WCF service with id *Test* on a Windows machine named *myserver*, you will be able to run scenario tests using this command on a Windows machine:
```
build-tests.cmd -outerloop -- /p:ServiceUri=myServer/WcfServiceTest
```
or similarly, you can run the tests from a non-Windows OS with:
```
./build-tests.sh -outerloop -- /p:ServiceUri=myServer/WcfServiceTest
```

**Note:** You can also set ServiceUri as an environment variable.  In that case, you don't need to pass the `/p:ServiceUri` parameter to the script. But if you do choose to use `/p:ServiceUri` on the command line and then change the value to point to different machines or endpoints between test runs, you should first delete the `bin\Windows_NT.AnyCPU.Debug\Infrastructure.Common` folder, because the `ServiceUri` value is compiled into the generated test infrastructure assembly.

