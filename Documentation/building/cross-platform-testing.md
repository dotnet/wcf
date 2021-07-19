# Running XUnit tests cross platform

Running xUnit tests on the non-Windows platforms is very similar to running them on Windows.  The only differences are:

 - You execute a different script (`./build.sh` versus `build.cmd`)
 - Scenario tests (aka "Outerloop" tests) must use  WCF test services running on a different (Windows) machine
 - You cannot run the xUnit tests as UWP

For a description of building and running unit tests, please refer to the [Developer Guide](https://github.com/dotnet/wcf/blob/master/Documentation/developer-guide.md).

For a description of running the scenario tests, please refer to the [Scenario Test Guide](https://github.com/dotnet/wcf/blob/master/Documentation/scenario-test-guide.md)

Following are just quick summaries of those steps.

## Running unit tests cross platform

Building the WCF product and running the unit tests can be done with just these scripts from the root of the GitHub repo:

```
./build.sh
./build.sh -test
```

This will build the product from scratch, run all the unit tests, and place all the results in the file `msbuild.log`


## Running scenario tests cross platform

To run the scenario tests ("OuterLoops") cross platform, you must first ensure the WCF test services are running on a Windows machine.  This is described in more detail in the [Scenario Test Guide](https://github.com/dotnet/wcf/blob/master/Documentation/scenario-test-guide.md).  You can run that WCF service either as self-hosted or IIS-hosted.

Then on the non-Windows machine where you want to run the tests, use these scripts:

```
./build.sh -outerloop -- /p:ServiceUri={URL-to-WCF-service}
```

The "{URL-to-WCF-service}" should be replaced by the base-address of the WCF test service host.  It will be slightly different depending if you started it self-hosted or IIS-hosted.  For example, if you started the WCF service self-hosted on a machine called "MyWindowsMachine", the script would be:

```
./build.sh -outerloop -- /p:ServiceUri=MyWindowsMachine
```

And if you started the WCF test service IIS-hosted, the script would be something like this:

```
./build.sh -outerloop -- /p:ServiceUri=MyWindowsMachine/WcfService1
```


## Why you cannot run xunit tests as UWP cross platform

UWP applications are of course capable of being run cross-platform -- that is their purpose.  But the AOT (Ahead of Time) compilation step required to build a UWP application is currently supported only on Windows.  When run under UWP on a Windows machine, the xUnit test projects are all treated as UWP applications and go through the AOT compilation step before execution.  Not only are the tests pre-compiled this way, but so too is the xUnit framework itself.  It uses a version of xUnit made to run on NET Core.

But on a non-Windows machine, where the AOT step is not available, it is not possible to precompile the xUnit framework or the tests.  Microsoft ensures all the WCF packages and test projects are fully compatible for UWP by running them on Windows each night.