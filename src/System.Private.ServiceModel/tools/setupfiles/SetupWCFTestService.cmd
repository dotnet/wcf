echo off
setlocal

pushd %~dp0

if '%BridgeHost%' neq '' (
   set _bridgeHostArg=-hostName %BridgeHost%
)

if '%BridgePort%' neq '' (
   set _bridgePortArg=-portNumber %BridgePort%
)

if '%BridgeAllowRemote%' neq '' (
   set _bridgeAllowRemoteArg=-allowRemote %BridgeAllowRemote%
)

echo Executing: start powershell -ExecutionPolicy Bypass -File ..\test\Bridge\bin\ensureBridge.ps1 %_bridgeHostArg% %_bridgePortArg% %_bridgeAllowRemoteArg% %*

start powershell -ExecutionPolicy Bypass -File ..\test\Bridge\bin\ensureBridge.ps1 %_bridgeHostArg% %_bridgePortArg% %_bridgeAllowRemoteArg% %*
exit /b
