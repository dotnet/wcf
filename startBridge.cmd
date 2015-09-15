@echo off

set BridgeKeepRunning=true

pushd %~dp0src\System.Private.ServiceModel\tools\setupfiles
call SetupWCFTestService.cmd %*
if ERRORLEVEL 1 goto error
popd

echo Because you started the Bridge manually, it will remain running until you close it manually.
echo Set the BridgeKeepRunning environment variable to 'false' to allow it to be closed by OuterLoop tests.
goto done

:error
echo An error occurred starting the Bridge
popd
exit /b l

:done
popd
