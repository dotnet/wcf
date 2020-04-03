@echo off

setlocal

pushd %~dp0\..\..\

:buildsln
set buildlib=eng\common\cibuild.cmd -preparemachine -configuration Release -project src\dotnet-svcutil\lib\src\dotnet-svcutil-lib.csproj /p:Test=false /p:Sign=false
echo %buildlib%
call %buildlib%

set buildtool=eng\common\cibuild.cmd -preparemachine -configuration Release -project src\dotnet-svcutil\src\dotnet-svcutil.csproj /p:Test=false /p:Sign=false
echo %buildtool%
call %buildtool%

:End
popd
endlocal
