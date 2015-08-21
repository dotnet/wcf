echo off
setlocal
pushd %~dp0

echo BridgeKeepRunning=%BridgeKeepRunning%
if '%BridgeKeepRunning%' neq 'true' (
    echo Stopping the Bridge.exe task locally...
    Taskkill /IM bridge.exe /F
) else (
    echo Bridge is left running because BridgeKeepRunning is true
)

exit /b
