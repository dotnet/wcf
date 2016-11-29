Intro to WCF
================

Windows Communication Foundation (WCF) has been a part of the full .NET framework for years.  This WCF repository contains source code for all the client libraries originally available for the Windows Store but made compatible with the new [.NET Core Runtime](https://github.com/dotnet/coreclr).

Project Goals and Priorities
============================

Microsoft published WCF to GitHub with the following goals:

- Establish a high-quality open source .NET implementation of the WCF client libraries.
- Work with the open source community to manage and extend the available libraries.

Reporting Issues
================
To report an issue or just to open the discussion for a proposed feature, open a [New Issue](https://github.com/dotnet/wcf/issues/new). Please assign labels to associate it with the appropriate library if you know it. Example: "System.ServiceModel.Http".

Contributing
============

Please read [Contributing](https://github.com/dotnet/wcf/blob/master/Documentation/contributing.md) to .NET Core before making your first contribution.

Shared "Developer Workflow" Scripts
==================

Many of the [dotnet](https://github.com/dotnet) repositories share a common set of scripts to provide a unified developer experience across different repositories and operating systems. These scripts are known collectively as "Developer Workflow" and are described in the [CoreFx Developer Guide](https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/developer-guide.md).

This WCF repository uses those same Developer Workflow scripts. WCF also uses a few additional scripts for end-to-end testing.  The steps below demonstrate the use of these scripts to build and test the WCF product.

Building the WCF packages
=============

All WCF packages can be built from a regular, non-admin command prompt using this command in Windows, run from the [root](https://github.com/dotnet/wcf) of the repository:
```
build.cmd -- /p:SkipTests=true
```
or on a non-Windows OS:
```
./build.sh -- /p:SkipTests=true
```
This script builds the product code and produces all the WCF NuGet packages. Even when built on a Windows machine, these packages contain the correct assets for all supported platforms, such as Windows, Linux, OS X, etc.  The packages are created under the `bin\packages` folder.   These are the packages Microsoft ultimately publishes to [NuGet.org](https://www.nuget.org/). 

By default these scripts normally build the packages and run all the unit tests.  The `/p:SkipTests=true` option simply skips running the unit tests to make the build slightly faster.

More information about the build script as well as others to clean and sync the repository can be found in the [CoreFx Developer Guide](https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/developer-guide.md).

Running unit tests
=========
Unit tests validate the components of WCF that don't require network interactions or running WCF test services. To build the product and run all the unit tests on Windows, run this script from the root of the repository:
```
build.cmd
```
or this command on a non-windows OS:
```
./build.sh
```
Each separate WCF package has its own unique set of unit tests, and they live in a folder called `src\{WCF-package}\tests`.  For example, the folder:
[src\System.ServiceModel.Primitives\tests](https://github.com/dotnet/wcf/tree/master/src/System.ServiceModel.Primitives/tests)
contains the unit tests for the System.ServiceModel.Primitives NuGet package.

To run the unit tests for only a single test project, 'cd' to the folder containing the test project and use msbuild to run the 'BuildAndTest' target for the .csproj found there, like this:
```
cd src\System.ServiceModel.Primitives\tests

msbuild /t:BuildAndTest
```

You can force a rebuild by using `/t:RebuildAndTest` instead if you have made modifications to the code since the last test run.

These scripts automatically generate an `msbuild.log` file containing the output from msbuild.

**Note:** If building in a non-Windows environment, call `./Tools/msbuild.sh` instead of just `msbuild`.


Running scenario tests
===========
*Scenario* tests validate the WCF components using end-to-end scenario-based tests that involve network communications between the client tests and WCF test services running in a different process or different machine. By default, they are not run with the normal `build.cmd` or ` build.sh`.  The scenario tests are also referred to as "outerloop" tests in the common Developer Workflow.

To run the scenario tests on Windows, execute this script from the root of the repository:
```
build-tests.cmd -outerloop
```
This script will automatically start self-hosted WCF test services on the same Windows machine, run the scenario tests and then shut down the self-hosted WCF services. You will see a second window appear temporarily to self-host the WCF services, and then it will automatically close when done.

The test results will be written to the console as well as to the file `build-tests.log`.

This is the simplest way to run the scenario tests, but it works only on a Windows machine, and it uses only local self-hosted test services. To run the tests on a non-Windows OS or to run the tests cross-machine or using IIS-hosted test services, you will need to start the WCF test services separately.

See the [Scenario test guide](https://github.com/dotnet/wcf/blob/master/Documentation/scenario-test-guide.md)
for further details how to run the scenario tests using test services running self-hosted or IIS-hosted on another machine.



Obtaining code coverage
============
You can also obtain detailed code coverage information of the WCF product by including an additional property when you run the build script. For example, run this command on Windows to run all unit and scenario tests and to collect code coverage information:
```
build.cmd -outerloop -coverage
```
Once it is done, the result can be found at `bin\tests\coverage\index.html`.  Open this file in a browser to see detailed line-by-line code coverage.


Running tests as UWP (aka NET Native)
===================
WCF packages can be used in both ASP.NET Core and UWP applications. By default, the instructions given above for running tests effectively run the tests as if they were in an ASP.NET Core app. But the tests can also be run as if they were running inside a UWP application.

For more information about ASP.NET Core applications, see [ASP.NET Core](https://www.asp.net/core).

For more information about UWP applications, see [Getting Started with UWP Apps](https://msdn.microsoft.com/windows/uwp/get-started/universal-application-platform-guide).

Running unit tests as UWP
--------------
To run a unit test as UWP, you need to 'cd' to the folder containing the test project and use msbuild with some additional parameters.  For example, these steps will build and run the unit tests for the System.ServiceModel.Primitives package under UWP:
```
cd src\System.ServiceModel.Primitives\tests

msbuild /t:BuildAndTest /p:TargetGroup=netstandard1.3 /p:TestTFM=netcore50aot /p:TestNugetRuntimeId=win10-x64-aot /p:UseDotNetNativeToolchain=true
```

This will build the tests under UWP and include a special version of xunit that itself runs in NET Core. This way of running the tests takes slightly longer because it entails analyzing all test and product code to construct a single minimal sized executable.

When the test is complete, you can find the results in a `testresults.xml` file under the folder `bin\tests` folder.  For example, the results of the test run above can be found in `bin\tests\Windows_NT.AnyCPU.Debug\System.ServiceModel.Primitives.Tests\netcore50\testResults.xml`.

Running scenario tests as UWP
-----------------
The scenario tests can also be run as UWP.  They are run much like the unit tests, but you need to start the WCF test services manually.  The following steps will start the local self-hosted test services, run the security-based scenario tests as UWP, and then stop the test services:
```
src\System.Private.ServiceModel\tools\scripts\StartWCFSelfHostedSvc.cmd

pushd src\System.Private.ServiceModel\tests\Scenarios\Security\TransportSecurity

msbuild /t:BuildAndTest /p:TargetGroup=netstandard1.3 /p:TestTFM=netcore50aot /p:OuterLoop=true /p:TestNugetRuntimeId=win10-x64-aot /p:UseDotNetNativeToolchain=true Security.TransportSecurity.Tests.csproj

popd

src\System.Private.ServiceModel\tools\scripts\StopWCFSelfHostedSvc.cmd

```

When complete, the results would be found in 

`bin\tests\Windows_NT.AnyCPU.Debug\Security.TransportSecurity.Tests\netcore50\testresults.xml`

**Note:** If building in a non-Windows environment, call `./Tools/msbuild.sh` instead of just `msbuild`.

See the [Scenario test guide](https://github.com/dotnet/wcf/blob/master/Documentation/scenario-test-guide.md)
for further details how to run the scenario tests in UWP using test services running self-hosted or IIS-hosted on another machine.

Conditionally run tests
============
Most xunit tests are normally marked with [Fact] or [Theory] attributes to tell xunit they are tests.  However, WCF tests use [WcfFact] and [WcfTheory] instead.  These are unique to the WCF repository, and provide additional functionality over the normal [Fact] and [Theory].

This additional functionality includes:
* WCF tests marked with [WcfFact] or [WcfTheory] can also be marked with a [Condition] attribute that determines whether the test should be executed in the current environment or skipped.
* WCF tests marked with [WcfFact] or [WcfTheory] can also be marked with an [Issue] attribute if there is an active issue that would prevent the test from passing in a particular environment.

Based on the optional [Condition] or [Issue] attributes, the test may be executed or it may be skipped.  But whether it is skipped or executed, it will appear in the final test results.


