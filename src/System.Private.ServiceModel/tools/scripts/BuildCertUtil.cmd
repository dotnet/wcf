@echo off

setlocal

pushd %~dp0..\..\..\..\

:buildsln
set buildtool=eng\common\cibuild.cmd -warnAsError 0 -configuration Release -projects src\System.Private.ServiceModel\tools\CertificateGenerator\CertificateGenerator.sln /p:Sign=false
echo %buildtool%
call %buildtool%

:End
popd
endlocal
