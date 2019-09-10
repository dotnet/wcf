@echo off

setlocal

pushd %~dp0

set runcmd=dotnet restore dotnet-svcutil.sln
echo %runcmd%
call %runcmd%

:End
popd
endlocal
