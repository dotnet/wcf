echo off
setlocal

pushd %~dp0

certutil -addstore -f root RootCATest.cer
certutil -f -p test -importpfx "WcfTestServer.pfx"
netsh http add sslcert ipport=0.0.0.0:44285 certhash=1d85a3f6cd2c022c5ca54e5cb200a47f89ba0d3d appid={00000000-0000-0000-0000-000000000000}

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
