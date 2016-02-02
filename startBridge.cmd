@echo off

REM StartBridge explicitly sets BridgeAutoStart false to prevent
REM test scripts from closing it.  It must be closed manually.
pushd %~dp0src\System.Private.ServiceModel\tools\setupfiles
call SetupWCFTestService.cmd /BridgeAutoStart:false %*
if ERRORLEVEL 1 goto error

echo Because you started the Bridge manually, it will remain running until you stop it manually.
echo To stop the Bridge manually, use 'Bridge.exe -stop' or type 'exit' into the Bridge console window.
goto done

:error
echo An error occurred starting the Bridge
popd
exit /b l

:done
popd
