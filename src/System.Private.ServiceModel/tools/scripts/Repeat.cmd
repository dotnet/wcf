@echo off
setlocal

if [%2]==[] goto :Usage
if [%1]==[/?] goto :Usage
if [%1]==[-?] goto :Usage
if [%1]==[/help] goto :Usage
if [%1]==[-help] goto :Usage

set _n=%1
set _exitCode=0
set _cmd=
set _cmd1=

:Loop
shift
if [%1]==[] goto :Continue
if [%1]==[#] (
    set _cmd1=%_cmd1% %_cmd%
    set _cmd=
    goto :Loop
)
set _cmd=%_cmd% %1
goto :Loop
:Continue

for /L %%i in (1,1,%_n%) do (
    echo Iteration: %%i
    if defined _cmd1 (
        call :Run %_cmd1% %%i %_cmd%
    ) else (
        call :Run %_cmd%
    )
    if ERRORLEVEL 1 goto :Failed
    echo.
)

echo All %_n% iterations completed successfully!
goto :Done

:Usage
echo.
echo Usage: 
echo   %~n0 n command [parameters]
echo            n: [Required] The number of iterations to repeat the command
echo      command: [Required] The command to run for n iterations
echo   parameters: [Optional] The parameters of the command
echo               #: When parameters contain #, the last occurance in the parameter
echo               will be replace with the number of iteration.
echo.
echo Example:
echo   %~n0 42 work.cmd # param
echo   :The script 'work.cmd' will be called 42 times like
echo    work.cmd 1 param, work.cmd 2 param, ..., work.cmd 42 param 
goto :Done

:Run
set cmdLine=%*
echo %cmdLine%
call %cmdLine%
exit /b

:Failed
set _exitCode=1
echo.
echo Aborted! Failed to complete all iterations.

:Done
exit /b %_exitCode%
endlocal
