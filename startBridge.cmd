@echo off

pushd %~dp0src\System.Private.ServiceModel\tools\setupfiles

echo Building the Bridge...
call start /wait BuildWCFTestService.cmd

echo Launching the Bridge (elevated) ...
start /wait RunElevated.vbs SetupWCFTestService.cmd %*

set BridgeKeepRunning=true

:done
popd