Intro to WCF
================

Windows Communication Foundation (WCF) has been a part of the full .NET framework for years.  This WCF repository contains source code for all the client libraries originally available for the Windows Store but made compatible with the new [.NET Core Runtime](https://github.com/dotnet/coreclr).

Project Goals and Priorities
============================

Microsoft published WCF to GitHub, with the following goals:

- Establish a high-quality open source .NET implementation of the WCF client libraries.
- Work with the open source community to manage and extend the available libraries.

Reporting Issues
================
To report an issue or just to open the discussion for a proposed feature, open a [New Issue](https://github.com/dotnet/wcf/issues/new). Please assign labels to associate it with the appropriate library if you know it. Example: "System.ServiceModel.Http".

Contributing
============

Please read [Contributing](https://github.com/dotnet/wcf/blob/master/Documentation/contributing.md) to .NET Core before making your first contribution.

Building the repository
=======================

The WCF repository can be built from a regular, non-admin command prompt using build.cmd on Windows or build.sh on *nix.
This build produces assets including a single System.Private.ServiceModel assembly that implements the individual client libraries.

Microsoft uses this repository to create and publish separate NuGet packages for each library. 

The repository is a work in progress, and not all of the libraries are complete yet.

Running unit tests
======================
Unit tests are those that test aspects of the WCF libraries that don't require network interactions.
To build the product and run the unit tests, from the root of the repository simply execute this command on Windows:

```
    build.cmd
```
or this command on Linux or OS X:
```
    ./build.sh
```

Running scenario tests
==========================
Scenario tests are those tests that involve network activity between the client tests and WCF test services.
By default, they are not run with the normal build.cmd or build.sh.

To run the scenario tests on Windows, execute this CMD from the root of the repository. It will automatically start a
self-hosted WCF test service on the same Windows machine and shut it down at the end of execution of all tests.

```
    build.cmd /p:WithCategories=OuterLoop
```
To run scenario tests against an already running WCF test service, you will need to specify the base address 
of the WCF test service by using the `/p:ServiceUri` parameter. This is also necessary when running
scenrio tests on Linux or OS X as the WCF service can only run on Windows.

From the root of the repository, run this command to run scenario tests on Windows:
```
    build.cmd /p:WithCategories=OuterLoop /p:ServiceUri=[WCFTestServiceBaseAddress]
```
or run this command to run scenario tests on Linux or OS X.
Note some of security related tests require extra manual setup to work, 
see [Linux manual test guide](https://github.com/dotnet/wcf/blob/master/Documentation/manualtest-guide.md) for details.
```
    ./build.sh /p:WithCategories=OuterLoop /p:ServiceUri=[WCFTestServiceBaseAddress]
```

You will need to replace `[WCFTestServiceBaseAddress]` in above commands with an actual value.
See [Scenario test guide](https://github.com/dotnet/wcf/blob/master/Documentation/scenario-test-guide.md)
for further details including how to start a WCF test service.

Obtaining code coverage
=======================
You can also obtain detailed code coverage information by including an additional property
when you run the build script.  For example, this CMD runs the scenario tests and collects
code coverage numbers on Windows:
```
    build.cmd /p:WithCategories=OuterLoop /p:Coverage=true
```
