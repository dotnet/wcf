@echo off

setlocal

set VsTestDir=%~dp0

echo "This script has not been updated yet."
EXIT

pushd %VsTestDir%..\..\
set RepositoryRoot=%cd%\
popd
echo RepositoryRoot=%RepositoryRoot%

set TestResultsDir=%RepositoryRoot%bin\Debug\netcoreapp1.0\TestOutput\TestResults\
echo TestResultsDir=%TestResultsDir%

set BaselinesDir=%VsTestDir%Baselines\
echo BaselinesDir=%BaselinesDir%

if not exist "%TestResultsDir%" echo **** TestResultsDir not found! & goto :END
pushd %TestResultsDir%

xcopy /s/y/f/r *.cs %BaselinesDir%
xcopy /s/y/f/r nuget.config %BaselinesDir%
xcopy /s/y/f/r global.json %BaselinesDir%
xcopy /s/y/f/r *.params.json %BaselinesDir%
xcopy /s/y/f/r *ConnectedService.json %BaselinesDir%
xcopy /s/y/f/r *.log %BaselinesDir%
xcopy /s/y/f/r *.csproj %BaselinesDir%
xcopy /s/y/f/r *cmd.log %BaselinesDir%

popd

pushd %BaselinesDir%

REM TestGroup projects as with the starter project are not test-specific so are not tracked.
del /s/q *TestGroup.cs
del /s/q *TestGroup.csproj

REM del /s/q *.lock*
REM del /s/q *.deps.json
REM del /s/q nuget.config
REM for /f %%I in ('dir /s/b obj') do rd /s/q %%I
REM for /f %%I in ('dir /s/b bin') do rd /s/q %%I

popd

:End
endlocal