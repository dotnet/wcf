echo off
setlocal

tasklist /FI "IMAGENAME eq Bridge.exe" 2>NUL | find /I /N "Bridge.exe">NUL
if "%ERRORLEVEL%"=="0" (
  echo The Bridge is already running.
  goto running
)

pushd %~dp0
echo Building the Bridge...
call BuildWCFTestService.cmd
popd

if '%BridgeResourceFolder%' == '' (
   set _bridgeResourceFolder=..\..\Bridge\Resources
) else (
   set _bridgeResourceFolder=%BridgeResourceFolder%
)

pushd %~dp0..\..\..\..\bin\wcf\tools\Bridge
echo Invoking Bridge.exe with arguments: /BridgeResourceFolder:%_bridgeResourceFolder% %*
start /MIN Bridge.exe /BridgeResourceFolder:%_bridgeResourceFolder% %*
popd

:running
exit /b

