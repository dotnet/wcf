@echo off

set BridgeKeepRunning=true

pushd %~dp0src\System.Private.ServiceModel\tools\setupfiles
call SetupWCFTestService.cmd %*
popd

echo Because you started the Bridge manually, it will remain running until you close it manually.
echo Set the BridgeKeepRunning environment variable to 'false' to allow it to be closed by OuterLoop tests.

:done
popd
