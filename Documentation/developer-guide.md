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

The WCF repository can be built from a regular, non-admin command prompt using build.cmd. This build produces a single System.Private.ServiceModel assembly that implements the individual client libraries.

Microsoft uses this repository to create and publish separate NuGet packages for each library. 

The repository is a work in progress, and not all of the libraries are complete yet.

Running the unit tests
======================
Unit tests are those that test aspects of the WCF libraries that don't require network interactions.
To build the product and run the unit tests, simply execute this CMD from the root of the repository:

```
    build.cmd
```

Running the scenario tests
==========================
Scenario tests are those tests that involve network activity between the tests and one or
more running WCF services.  By default, they are not run with the normal build.cmd.
To run the scenario tests, execute this CMD from the root of the repository:

```
    build.cmd /p:WithCategories=OuterLoop
```

Obtaining code coverage
=======================
You can also obtain detailed code coverage information by including an additional property
when you run the build script.  For example, this CMD runs the scenario tests and collects
code coverage numbers:
```
    build.cmd /p:WithCategories=OuterLoop /p:Coverage=true
```

Running tests with latest dependency versions
=============================================
Running tests against each dependency's latest version can catch functional issues. The
`FloatingTestRuntimeDependencies` property enables this. To run unit tests against the
latest dependencies:
```
    build.cmd /p:FloatingTestRuntimeDependencies=true
```
To run the scenario tests with latest dependencies:
```
    build.cmd /p:FloatingTestRuntimeDependencies=true /p:WithCategories=OuterLoop
```

Running scenario tests across multiple machines
===============================================
By default scenario tests run against WCF services that are locally self-hosted.
To run these tests against WCF services running on a separate machine, see:
[Cross-machine test guide](https://github.com/dotnet/wcf/blob/master/Documentation/cross-machine-test-guide.md).



