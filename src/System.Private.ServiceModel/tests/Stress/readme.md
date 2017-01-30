### What is SharedPoolsOfWCFObjects?
When presented with a task to find stress issues in WCF client code we looked for an approach which would be both an efficient way of finding stress bugs (leaks, deadlocks, threading issues, data corruption, etc) and a good representation of some of the typical customer use cases. Generally, when talking about framework stress scenarios, we would like to avoid only targeting “typical customer use cases” especially for the frameworks as complex as WCF  (and .NET in general) which allow for an extremely wide range of scenarios. Each of them might be the only “typical use case” for a particular customer. There are however some patterns that show up in various app designs built on top of WCF. One of them is pooling of communication objects. Pooling is common because in many cases WCF communication objects are reusable and are relatively expensive to create. The other common pattern belongs to the nature of communication objects – they periodically get faulted, closed, or simply dereferenced. SharedPoolsOfWCFObjects is trying to do both: pool WCF communication objects, let multiple threads use them, and periodically change their state while being used.

This application by no means is a good example of how to use WCF. Rather than writing a solid code to pool communication objects or to recover from unexpected events the goal was to increase the concurrency inside the most crucial pieces of code. We also wanted to avoid extra locks outside of the framework to prevent contentions that may otherwise not occur in real customer scenarios.

We also realized that the same scenarios are actually applicable in performance measurements: concurrently use pooled instances by many threads and try to achieve the maximum possible throughput. As the result this app is not a typical micro benchmark perf program that many other frameworks have. While both micro perf tests and more complex scenarios are useful, the more complex frameworks tend to have more potential perf impact from bottlenecks inside the framework rather than from total code path length. Therefore we chose to not focus on micro benchmarks for now. Given the dynamic nature of the communication systems, SharedPoolsOfWCFObjects app has some means to ensure that the perf measurements are both accurate and reproducible. 

It was not a goal for this app to provide an extensive code coverage, so the current number of tests is very limited. The primary focus was to include all available bindings and communication modes and to make it easy to add more bindings and tests later.

### What is SharedPoolsOfWCFObjectsServer?
The only purpose of SharedPoolsOfWCFObjectsServer is to provide the corresponding server end points for the tests used in SharedPoolsOfWCFObjects. It can be used for both stress and perf runs. It can only run on Windows platform since the server part of WCF is available for Windows only. It is recommended to use one of the Windows Server SKU since it does not have throttling of number of http connections. 

### How to build WCF Stress&Perf tests.
1.	Follow links on https://www.microsoft.com/net/download/core#/current to install the latest released .NET Core SDK.
2.	Make a local clone of WCF repo
3.	Go to src\System.Private.ServiceModel\tests\Stress\SharedPoolsOfWCFObjects
4.	Run “dotnet restore” (before running this you may want to edit the project file to target a different version)
5.	Run “dotnet build”
6.	You may want to run “dotnet publish” to be able to run this app on other machines without installing the core framework. Follow the “dotnet help publish” guidelines to target the desired options. Typically I’d run something like 
~~~
dotnet publish -o MyPublishedAppFolder -r win7-x64
~~~

### Client setup:
.NET Core apps don’t require any setup – simply copy the contents of the folder produced by the “dotnet publish” command. However some client cert authentication tests will require installing client certificate on stress machines. The details vary depending on the platform and how those client certificates are issued. If your client machine already ran the functional tests then it will likely have the client cert installed properly. In general there are 2 requirements for the client:
1.	The client has to have both private and public keys of the client certificate (and the server should be able to validate the client cert)
2.	The client has to have the server cert from step 3 below

### Server setup: 
1.	Create an IIS app, name it WcfService1 (I hope your server doesn’t already have an app with this name) 
2.	Copy over the contents of wcf/src/System.Private.ServiceModel/tests/Stress/SharedPoolsOfWCFObjectsServer (no build required)
3.	If this server already ran some other WCF security tests it probably already has the server certificate – find its thumbprint.
4.	Open web.config in WcfService1, find <serviceCertificate findValue ="xxxxxxxxxx" x509FindType="FindByThumbprint"
5.	Replace xxxxx… with your server certificate thumbprint and save the config file
6.	You may need to tweak the ACLs on the server certificate so that IIS worker process will have access to it (unless you run IIS as system or admin account). The easiest way to do it is to open cert mmc -> select your server cert -> All Tasks -> Manage Private Keys -> Add -> Give full control to the identity used to run IIS app pool (e.g. 'IIS APPPOOL\DefaultAppPool').
7.	Goto IIS manager / wcfService1 app / SSL settings and change the Client certificates to “Accept”
8.	You will also need to add 808 binding (if it hasn’t been added before), via Inetmgr or here is the part from applicationHost.config:
~~~
        <sites> <site name="Default Web Site" id="1"> <bindings>  <binding protocol="net.tcp" bindingInformation="808:*" />
~~~
9.	The server should have the https binding setup.
Now you’re ready to run both stress and perf tests.

### Running stress tests:
Simply go to the SharedPoolsOfWCFObjects folder and run “dotnet run”. If you previously published the app you can run SharedPoolsOfWCFObjects.exe. Typically you want to specify the service host name, the test name, the binding, and the stress run duration. 
~~~
SharedPoolsOfWCFObjects.exe Binding:NetTcp Async:true Test:HelloWorld HostName:yourserverhostname StressRunDuration:1
~~~
SharedPoolsOfWCFObjects will print a full list of its parameters. I am going to skip this list here as it is ever changing and so the copy inside this readme will become obsolete very fast. I should only note that the app has default values for pretty much every parameter. Whenever there is a list of possible values the default values will be placed first in the list.
In some cases you may want to run it under debuggers – even though the timing will be different it helps to investigate stress bugs. The app will also attempt to break into the debugger when some of the preconditions are broken. This helps to preserve the state of the process at the moment when the problem was detected.

### What to expect, what to monitor and typical stress problems and work arounds:
Expect the CPU usage to stay high (unless you specified a low stress level). It is a good idea to also monitor the request rate on the server side (via WCF service perf counters). There should be no continuous growth in memory (private bytes, committed bytes, virtual bytes) and handle count after initial warmup. There should be no exceptions printed during the stress run. After finishing the required duration the tool should report finishing all the tasks. This should be a good start for stress.
One typical problem is port exhaustion – the app may reset the connections too fast and will eventually run out of the ports. There is a parameter “RecycleFrequencyThrottle” to throttle the recycling frequency. Experiment with the numbers (start with 10K and go up). 

### How to run WCF Perf tests?
The list of possible parameters is a lot longer for perf. Most notable parameters are:  how fast we increase the load, whether we share channels across multiple threads, and what the maximum load is.  You want to specify: binding, test, and maximum load. You may also want to compare sync and async versions of the test.
~~~
SharedPoolsOfWCFObjects.exe Program2Run:Perf PerfMaxThroughputTasks:90 Binding:NetTcp Async:true Test:HelloWorld HostName:yourserverhostname 
~~~
### What to monitor and what to expect:
Perf is a vast area and there is a lot to discuss while measuring perf results. I am only going to highlight some initial steps to make sure you’re on the right track.
1.	Look for the client machine CPU usage. It should increase as the load increases and it should eventually saturate to 100% in every test that we have. Some of the possible reasons for not saturating CPU are: 
-	Not enough network bandwidth. In our perf lab the streaming tests running on 24 core machines are able to send & receive up to 1 gigabyte both ways in duplex streaming. This would typically require a relatively high end network card.
-	Too many (>32) CPU cores
-	Not using server GC for the client process (this will likely start affecting the machines with 4+ CPU cores)
-	Not fast enough server. It should be at least 50% faster than the client machine and should be able to scale well.
2.	Look for exceptions during the run – there should be none.
3.	Look for consistency of the results. This is a good idea to rerun them at least twice and compare the results.

There are many more things to look for but these are good starting key points for perf.
Thanks for reading!
