The Bridge and cross-machine testing
====================================

Bridge overview
---------------
The basic OuterLoop scenario tests normally run against locally hosted
WCF test services.  But some scenarios require running the WCF test
service(s) on a machine other than the machine running the test
(example: cross-platform testing).

For this we have developed what we call the "Bridge." This Bridge is
a self-elevating .exe that hosts a WebAPI application capable of
starting WCF services on demand.  Currently this exe must be run on a
Windows OS using the full .NET Framework.

When Outerloop tests are run, the Bridge is automatically started at
the beginning of the run and closed at the end.

Starting the Bridge manually
---------------------------
By default, the Bridge will self-start locally during OuterLoop tests and accept only
requests from the machine running the tests.  It will shutdown when all the OuterLoop
tests have completed.

However, the Bridge can be started manually.  This is useful if you want to run it on a different machine or attach a debugger to it before the tests run.  Executing this CMD from the repository root will start the Bridge locally:

```
  startBridge.cmd {options}
```

See below for a description of available options.

If you start the Bridge manually this way you must also stop it manually.
This manual start sets an Environment variable 'BridgeKeepRunning' to true,
and this prevents the Bridge from being shutdown when OuterLoop tests are complete.
To shutdown the Bridge manually, type "exit" into the CMD window in which the Bridge is running.
As long as 'BridgeKeepRunning' is set to true, the OuterLoop tests will not stop the Bridge when complete, even if the tests caused the Bridge to start.

The Bridge.exe program starts minimized.  If you want to see its
output as it runs, restore its CMD window from the taskbar.

Security options to consider when starting the Bridge
-----------------------------------------------------
Because the Bridge offers the ability to exercise arbitrary
code, it is important that it not be exposed publically.
There are 2 options to 'startBridge' to limit its visibility:

    -allowRemote
    -remoteAddresses:comma-separated-list

The 'allowRemote' option tells the Bridge it is allowed to accept
requests from other machines.  If unspecified, it will accept only requests
from localhost.  It must be set explicitly if the Bridge is running
on a machine other than where the tests run.

The 'remoteAddresses' is a comma-separated list of IP addresses,
a range of IP's, or one of the supported predefined terms supported
for the Scope properties page in the Windows Firewall.
See https://technet.microsoft.com/en-us/library/dd759059.aspx .

This option is used only if 'allowRemote' has also been specified.
Its default value is "LocalSubnet".  This means setting 'allowRemote' will by default allow only requests on the same local subnet. The value "*" allows remote
access from all addresses, but for security reasons is not recommended.


Running OuterLoop tests when the Bridge is remote
------------------------------------------------
On the machine where you want the Bridge to run, run this
command from the repository root:
```
  startBridge.cmd -allowRemote
```

On the machine where you want the tests to run, follow these steps to execute the OuterLoop tests using the remote Bridge:

``` 
    build.cmd /p:WithCategories=OuterLoop /p:BridgeHost={bridge host name} /p:BridgeResourceFolder={shared folder Bridge can access}
```

Alternatively, you could have set these as environment variables to skip passing them as MSBuild properties:

    set BridgeHost=bridge-host-name
    set BridgeResourceFolder=shared-folder-Bridge-can-access

Another way to run the Bridge and specify multiple options at the same time is to create a file in json format and specify it using the BridgeConfig optionm like this:

```
    build.cmd  /p:WithCategories=OuterLoop /p:BridgeConfig={my config file}
```

BridgeHost and BridgePort must match the machine name where the Bridge is running.

BridgeResourceFolder must be the location of a folder that both the client
and Bridge machines can access.  The WCF services built for the tests will
be written to that folder, and the Bridge will be asked to read from it.


How the Bridge works
--------------------
The Bridge is a WebAPI application that can run on any Windows
machine using the full .NET framework.  The Bridge can launch
WCF services requested by tests running on the same or a different
machine.

This allows the tests to be run in environments (such as NET Native
or CoreCLR) or on other operating systems (such as Linux or the Mac)
that are not able to host their own WCF services.

The Bridge is agnostic of WCF but instead supports the notion of
named "resources".  A resource is any type that implements the IResource
interface and resides in the "Bridge Resource Folder" when the Bridge
is configured.  The Bridge can be reconfigured on-the-fly without being
stopped and restarted.

Each resource has a name, which is just the simple class name of the
type implementing IResource.  In this way, a remote application can
request the Bridge to invoke the Get or Put method of any named resource.
For WCF tests, there are a number of IResources that know how to start
and host specific WCF services.  This allows a test running in a .NET
Core enviromnent to say "I need the URL to reach a running instance of
the XYZ WCF service" and have the Bridge automatically start that service and return its URL to the client.

The Bridge offers 3 endpoints:
  - http://{host}:{port}/**Bridge**
  - http://{host}:{port}/**Config**
  - http://{host}:{port}/**Resource**

The 'Bridge' endpoint supports these Http requests:

- GET -- returns the current Bridge configuration as a set of name/value pairs
- DELETE -- Terminates the Bridge process cleanly

The 'Config' endpoint supports these Http requests:

- GET -- returns the current Bridge configuration as a set of name/value pairs
- POST -- reconfigures the Bridge configuration with a new set of name/value pairs
-  DELETE -- releases all resources currently used by the Bridge but remains running

The 'Resource' endpoint supports these Http requests:

  - GET -- return the result of the IResource.Get for the resource of the given name
  - POST -- returns the result of the IResource.Put for the resource of the given name

Bridge.exe
-----------
The Bridge.exe is a self-elevating executable capable of starting and
stopping the Bridge WebAPI application.  If started from a non-elevated
process, it will ask for elevation confirmation depending on UAC settings.

Usage is: Bridge.exe [/ping] [/stop] [/stopIfLocal] [/allowRemote] [/remoteAddresses:x,y,z] [/{BridgeProperty}:value]

   - **ping**                       Pings the Bridge to check if it is running
   - **stop**                       Stops the Bridge if it is running
   - **stopIfLocal**                Stops the Bridge if it is running locally
   - **allowRemote**                If starting the Bridge, allows access from other than localHost (default is localhost only)
   - **remoteAddresses**            If starting the Bridge, comma-separated list of addresses firewall rules will accept (default is 'LocalSubnet')
   - **BridgeConfig:file**         Treat file as json name/value pairs to initialize any or all other options
   - **BridgeResourceFolder**       The folder containing the Bridge 'resources'
   - **BridgeHost**                 The machine on which the Bridge is running
   - **BridgePort**                 The port on which the Bridge is listening
   - **BridgeHttpPort**             The port used for Http tests
   - **BridgeHttpsPort**            The port used for Https tests
   - **BridgeTcpPort**              The port used for TCP tests
   - **BridgeWebSocketPort**        The port used for web socket tests
   - **BridgeCertificateAuthority** The name of the certificate file to serve as the certificate authorithy
   - **BridgeHttpsCertificate**     The name of the certificate file to import for Https tests
   - **BridgeMaxIdleTimeSpan**      The maximum TimeSpan the Bridge can stay idle before shutting down

Any of the options starting with 'Bridge' can also be specified by setting an Environment
variable with that name.  If passed explicitly on the command line, the command line value takes precedence over the Environment variable.

Any of these options can also be specified by placing them into a json-formatted file and specifying the 'BridgeConfig' property.  An example of such a file might be:
```
{
   allowRemote : "",
   BridgePort : "44289",
   BridgeMaxIdleTimeSpan : "24:00:00"
}

```

Options stated explicitly on the command line take precedence over Environment variables or values read from the BridgeConfig file.

When the Outerloop tests are run, Bridge.exe is started automatically running on localhost. When the Outerloop tests complete 'Bridge /stopIfLocal' is invoked to close the Bridge.

While running, the Bridge will add firewall rules to allow access to specific ports.
It may also load certificates as they are needed by tests.  When the Bridge closes,
it removes all those firewall rules or certificates.


