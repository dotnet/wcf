@echo off

setlocal

pushd %~dp0..\..\..\..\

:buildsln
set buildtool=eng\common\cibuild.cmd -warnAsError 0 -configuration Release -projects src\System.Private.ServiceModel\tools\SelfHostedWcfService\SelfHostedWCFService.sln /p:Sign=false
echo %buildtool%
call %buildtool%

:End
popd
endlocal

:: Kill dotnet.exe process started by eng\common\cibuild.cmd which might remain running
TASKKILL /F /IM dotnet.exe