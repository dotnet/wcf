@echo off

setlocal

pushd %~dp0\..\..\

:buildsln
set buildtool=eng\common\cibuild.cmd -preparemachine -configuration Release -projects dotnet-svcutil.sln /p:Test=false /p:Sign=false
echo %buildtool%
call %buildtool%

:End
popd
endlocal
