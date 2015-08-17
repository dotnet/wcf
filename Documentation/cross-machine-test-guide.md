The Bridge and cross-machine testing
====================================

Bridge overview
---------------
The basic OuterLoop scenario tests normally run against locally hosted
WCF test services.  But some scenarios require running the WCF test
service(s) on a machine other than the machine running the test
(example: cross-platform testing).

For this we have developed what we call the "Bridge." This Bridge is
a WebAPI application meant to run on a Windows OS using the full
.NET framework.  It offers a REST API that the scenario tests can
invoke through normal HTTP requests to launch WCF services on other
machines.

The Bridge itself is agnostic of WCF services. Instead, it is aware
of types that implement IResource, and it uses reflection to analyze
a collection of assemblies to discover them.  It then exposes each of these
as a "resource" to the Bridge REST API.  When a test requests a particular
resource, the Bridge invokes the appropriate IResource.  In this case, the
WCF tests provide IResources that know how to create and host WCF services.

When the tests start up, they inform the Bridge where to find these
resource assemblies.  This folder is known as the "Bridge Resource Folder".

There are several Bridge-specific properties that are meaningful to
both the Bridge and the tests.  They can be defined either as Environment
variables or passed into build.cmd as MSBuild properties.  They are:

  - **BuildHost**: the name of the machine on which the Bridge runs (default localhost)
  - **BridgePort**: the port to use to communicate with the Bridge (default 44283)
  - **BridgeResourceFolder**: the full path to the folder that contains the Bridge IResource assemblies (default bin\Wcf\Bridge\Resources)
  - **BridgeAllowRemote**: indicates whether the Bridge will accept requests from machines other than itself (default: false)

Starting the Bridge Locally
---------------------------
By default, the Bridge will self-start locally during OuterLoop tests and accept only
requests from the machine running the tests.  It will shutdown when all the OuterLoop
tests have completed.

If you want to run the Bridge on a different machine, you must follow the steps below.

Starting the Bridge on a different machine
------------------------------------------
On the Bridge where the machine will run, do these things:

cd to the root folder of a local WCF Git repository and run this command:

```
    startBridge.cmd {options}
```

The options can be any of these:

    -portNumber NNN
    -allowRemote true/false
    -remoteAddresses comma-separated-list

The 'portNumber' option allows you to choose a port other than
the default 44283.

The 'allowRemote' option tells the Bridge it is allowed to accept
requests from other machines.  If unspecified, it will accept only
from localhost.  It must be set to true if the Bridge is running
on a machine other than where the tests run.

The 'remoteAddresses' is a comma-separated list of IP addresses,
a range of IP's, or one of the supported predefined terms supported
for the Scope properties page in the Windows Firewall.
See https://technet.microsoft.com/en-us/library/dd759059.aspx .

If left unspecified, but 'allowRemote' is true, the 'remoteAddresses'
will default to "LocalSubnet".  The value "*" allows remote
access from all addresses, but for security reasons is not recommended.

The purpose of these options is to limit the scope applied to the
firewall rules the Bridge automatically generates while it is running.
It does this to open specific ports needed for the WCF test services.
The Bridge deletes these firewall rules when it exits.

After running 'startBridge.cmd', a PowerShell window will start Bridge.exe in elevated mode.
You will see a CMD window indicating what URL the Bridge is using and confirmation
whether it is enbled to receive remote requests.  It will remain running until you manually close it or type "exit" in that CMD window.

Running OuterLoop tests when the Bridge is remote
------------------------------------------------
On the machine where you want the tests to run, follow these steps:

Start the OuterLoop tests like this:

 * cd to the root folder
 * run this CMD

``` 
    build.cmd /p:WithCategories=OuterLoop /p:BridgeHost=bridge-host-name /p:BridgeResourceFolder=shared-folder-Bridge-can-access
```

   Alternatively, you could have set these as environment variables to skip passing them as MSBuild properties:


    set BridgeHost=bridge-host-name
    set BridgeResourceFolder=shared-folder-Bridge-can-access
    [optional] set BridgePort=NNN


BridgeHost must match the machine name where the Bridge is running.

BridgeResourceFolder must be the location of a folder that both the client
and Bridge machines can access.  The WCF services built for the tests will
be written to that folder, and the Bridge will be asked to read from it.

BridgePort needs to be set only if you started the Bridge on a different port.

After you started 'build.cmd' you will see a PowerShell window execute elevated, 
and it will ping the remotely running Bridge to verify it is available.  All OuterLoop tests
will then run against WCF test services running on the Bridge machine.

If you watch the CMD window running on the Bridge machine, you will see it
write to the console the incoming configuration and resource requests.
After all tests have run, the Bridge will continue to run for several minutes.
The timeout interval can be specified on the client test machine by setting
the BridgeMaxIdleTimeSpan to some valid TimeSpan value (either as an environment
variable or on the build.cmd line.  Default is "20:00" (twenty minutes).  After
no activity for this amount of time, the Bridge will close.  Alternatively, you
can type "exit" in the Bridge CMD window to close it manually.

You can control the amount of time the Bridge will remain alive when idle by setting the environment variable BridgeMaxIdleTimeSpan to any legal TimeSpan string (example: 'set BridgeMaxIdleTimeSpan=1.02:03:04' will allow it to run 1 day, 2 hours, 3 minutes, and 4 seconds).  This value should be set on the machine where the tests run, because they will configure the Bridge machine when they start.




