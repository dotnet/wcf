# Collecting and Analyzing ETW Traces 

## Supported
ETW logs are available on Windows platform only currently.

## Collect ETW logs for WCF Server
* If you use your own WCF Server, it is recommended to follow https://msdn.microsoft.com/en-us/library/ms733025(v=vs.110).aspx.
* If you run the scenario tests, a locally-hosted WCF service will be started automatically, and it is recommended
 to follow "How to Collect ETW logs uses logman" below.

## How to Collect ETW logs use logman

For logman parameters usage, please see https://www.microsoft.com/resources/documentation/windows/xp/all/proddocs/en-us/nt_command_logman.mspx?mfr=true 

Logman collects ETW logs from all WCF applications on the machine. 
* If you run both WCF Client and Server on the same machine, you will get ETW logs for both processes in the same file. It 
  can be differentiated from process IDs.

* If you run WCF Client and Server on different machines. You need to collect the logs using the same command on both machines.

### Start the logging

 * Option 1: Generate a single file that does circular logging with Verbose level, For example
 
 ```
logman start WCFETWTracing -p "Microsoft-Windows-Application Server-Applications" 0xFFFFFFFF  0x5 -bs 64 -nb 120 320 -ets -ct perf -f bincirc -max 500 -o 

c:\temp\WCFEtwTrace.etl
```
 * Option 2: Generate a single file that does circular logging with Informational level, For example
 
 ```
logman start WCFETWTracing -p "Microsoft-Windows-Application Server-Applications" 0xFFFFFFFF  0x4 -bs 64 -nb 120 320 -ets -ct perf -f bincirc -max 500 -o 

c:\temp\WCFEtwTrace.etl
```
 * Option 3: Generate multiple files that rolls logging every 10 minutes with Informational level, For example
 
```
logman create trace WCFETWTracing -p  "Microsoft-Windows-Application Server-Applications" 0xFFFFFFFF 0x4 -o C:\temp\WCFETWTrace -f bin -cnf 00:10:00 -v 

mmddhhmm

logman start WCFETWTracing 
```
 * 	Option 4: Generate a single file that does circular logging with Warning level, For example
 
 ```
 logman start WCFETWTracing -p "Microsoft-Windows-Application Server-Applications" 0xFFFFFFFF 0x3 -bs 64 -nb 120 320 -ets -ct perf -f bincirc -max 500 -o 

c:\temp\WCFEtwTrace.etl
```

### Stopping the logging when done
	logman stop WCFETWTracing -ets

## Tools for viewing/analyzing ETW events
 * eventvwr
   1. Launch eventvwr.
   2. Window Logs -> Saved Logs, then open the event log
 * perfview
   1. Download perfview from https://github.com/Microsoft/perfview/blob/master/documentation/Downloading.md 
   2. Open perfview.exe
       * Open the event log file by typing the path into the file path window.
       * Click on Events from the list.
 * svcperf, see instructions on http://svcperf.codeplex.com/
