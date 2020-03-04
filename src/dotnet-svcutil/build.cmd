@echo off

setlocal

pushd %~dp0\src

if not exist Packages call restore.cmd

set logParams=
set logParams=/flp:Summary;Verbosity=normal;logFile=obj\%~n0.log
set logParams=%logParams% /flp1:warningsonly;logfile=obj\%~n0.wrn
set logParams=%logParams% /flp2:errorsonly;logfile=obj\%~n0.err

if not exist obj\ md obj\

:buildsln
set runcmd=msbuild %logParams% ..\tools\master.msbuild /v:m %*
echo %runcmd%
call %runcmd%

:End
popd
endlocal
