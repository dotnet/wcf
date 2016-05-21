Scenario Testing Guide
======================

Overview
--------
WCF scenario tests are also referred to as outerloop tests. These are tests involving communication between WCF client tests and WCF test services. The WCF client tests running on .NET Core support cross platform, which means they can run on all supported platforms such as Windows, various Linux distros, Mac OS X as well as Universal Windows Platform (UWP). The WCF test services are built on full .NET Framework that is supported on Windows only.

Regardless which platform where you run WCF client tests, you will need a WCF test service for scenario tests to work. WCF Test services can be hosted in two ways like WCF users would usually do, ie., self-hosted or IIS-hosted on a Windows machine, for example, Windows Server 2012 R2 or Windows 10. The following sections provide instructions of how to host WCF test services and how to run WCF scenario tests.

Self-Hosted WCF Test Service
----------------------------
To start a self-hosted WCF test service on a Windows machine, execute this script that is located at directory `src\System.Private.ServiceModel\tools\scripts`.
```
    StartWCFSelfHostedSvc.cmd
```
This script will add necessary firewall rules, generate and install self-signed test certificates needed by tests, build and kick off a self-hosted WCF test service process from project at `src\System.Private.ServiceModel\tools\SelfHostedWcfService`. When you are done with this WCF test service, type `exit` and press `enter` in the window where the service was started. Upon shutting down the service, this script will also clean up firewall rules and remove test certificates it installed.

This script requires elevated privileges. If it is run from a non-elevated window, you may be prompted for elevated permission.

IIS-Hosted WCF Test Service
---------------------------
To host WCF services on IIS, the Windows machine has these prerequisites

1. Web Server (IIS) and Application Server roles are installed. This includes ASP.NET and WebSocket protocol.
2. .NET Framework 4.x features including WCF service and activation features are enabled.
3. Ports 80 (HTTP), 443 (HTTPS) and 808 (TCP) are opened.

Copy script `src\System.Private.ServiceModel\tools\scripts\SetupWcfIISHostedService.cmd` to the Windows machine and run it in an elevated command window.
```
    SetupWcfIISHostedService.cmd {id}
```

where `id` is an identifier that will be appended to the name of the IIS application to be created, for example, command `SetupWcfIISHostedService.cmd Test` will create an IIS aplication named "WCFService*Test*". This script will create a clone of WCF GitHub repository, an IIS application pool and an IIS application from `src\System.Private.ServiceModel\tools\IISHostedWcfService`. The script will also install necessary test certificates and add IIS HTTPS binding for testing.

This is one time setup. Once you set it up, you do not need to build or start the service manually. IIS will build and activate the service automatically upon the first request from tests. 

Running WCF Scenario Tests
-------------------------
As instructed in [Developer Guide](developer-guide.md#running-scenario-tests), to run scenario tests, you will need to specify the WCF service base address via `/p:ServiceUri` parameter to the build script.

The `ServiceUri` for self-hosted WCF service will be the name of the machine. For example, if you start a self-hosted WCF test service on a Windows machine named *myserver*, you will be able to run scenario tests on a Linux or a Mac machine using this command
```
    ./build.sh /p:WithCategories=OuterLoop /p:ServiceUri=myserver
```

The `ServiceUri` for IIS hosted WCF service will be `{MachineName}/WCFService{id}`. For example, if you set up an IIS hosted WCF service with id *Test* on a Windows machine named *myserver*, you will be able to run scenario tests on a Linux or a Mac machine using this command

```
    ./build.sh /p:WithCategories=OuterLoop /p:ServiceUri=myserver/WcfServiceTest
```
To run scenario tests from a Windows machine, replace `./build.sh` with `build.cmd` in above commands.