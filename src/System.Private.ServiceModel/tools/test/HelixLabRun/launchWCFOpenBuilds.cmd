REM
REM This script sync to the latest WCF source, build and submit Helix lab runs.
REM Arguments:
REM %1: Lab scripts location
REM %2: WCF local git source location
REM %3: Response files location

@echo off
setlocal

set BridgeHost=
pushd %2
pushd bin\wcf\tools\Bridge
if [%ERRORLEVEL%]==[0] (
    call Bridge.exe -stop
    popd
)

#REM remove bin directory to ensure a rebuild
call git clean -xdf
if NOT [%ERRORLEVEL%]==[0] (
    echo ERROR: An error occurred while cleanning Git.
    exit /b
  )

REM sync
git checkout master
if NOT [%ERRORLEVEL%]==[0] (
    echo ERROR: An error occurred while checking out master.
    exit /b
  )
git pull
if NOT [%ERRORLEVEL%]==[0] (
    echo ERROR: An error occurred while syncing master.
    exit /b
  )
call startbridge.cmd  /allowRemote /remoteAddresses:*
if NOT [%ERRORLEVEL%]==[0] (
    echo ERROR: An error occurred while starting the bridge.
    exit /b
  )
REM replaceBuildNumber.ps1 will update build number for all response files.
copy /y %1\* .
copy /y %3\* .
powershell.exe  %1\replaceBuildNumber.ps1
if NOT [%ERRORLEVEL%]==[0] (
    echo ERROR: An error occurred while updating build number.
    exit /b
  )
set BridgeHost=Wcfcoresrv4.cloudapp.net
@echo kick off mac debug
call build.cmd   @osx_debug.rsp
if NOT [%ERRORLEVEL%]==[0] (
    echo ERROR: An error occurred while kicking off mac debug.
    echo Continue submitting the rest of the jobs
  )
@echo kick off mac release
call build.cmd   @osx_release.rsp
if NOT [%ERRORLEVEL%]==[0] (
    echo ERROR: An error occurred while kicking off mac release.
    echo Continue submitting the rest of the jobs
  )
@echo kick off windows debug
call build.cmd  @windows_debug.rsp
if NOT [%ERRORLEVEL%]==[0] (
    echo ERROR: An error occurred while kicking off windows debug.
    echo Continue submitting the rest of the jobs
  )
@echo kick off window release
call build.cmd  @windows_release.rsp
if NOT [%ERRORLEVEL%]==[0] (
    echo ERROR: An error occurred while kicking off windows release.
    echo Continue submitting the rest of the jobs
  )
@echo kick off Linux debug
call build.cmd   @%ubuntu_debug.rsp
if NOT [%ERRORLEVEL%]==[0] (
    echo ERROR: An error occurred while kicking off linux debug.
    echo Continue submitting the rest of the jobs
  )
@echo kick off Linux release
call build.cmd   @ubuntu_release.rsp
if NOT [%ERRORLEVEL%]==[0] (
    echo ERROR: An error occurred while kicking off Linux release.
  )

popd
endlocal
