echo off
setlocal

echo BridgeKeepRunning=%BridgeKeepRunning%
if '%BridgeKeepRunning%' neq 'true' (
    echo Stopping the Bridge...
    pushd %~dp0..\..\..\..\bin\wcf\tools\Bridge
    call Bridge.exe -stopIfLocal %*
    popd
) else (
    echo The Bridge was left running because BridgeKeepRunning is true
)

exit /b
