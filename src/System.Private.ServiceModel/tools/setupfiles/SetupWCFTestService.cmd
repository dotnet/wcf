echo off
setlocal

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

exit /b

