echo off
setlocal
echo **********************************
echo Starting WCF Service on Docker
echo **********************************

set _exitCode=0
::todo use vars
set "ServiceImageTag=wcf:service"
set "ServiceContainerName=WCFServiceContainer"
set "ServiceHostName=wcfservicehost"

:: Make sure docker is running 
docker ps>nul 2>&1
if ERRORLEVEL 1 (
    echo. & echo ERROR: Please make sure docker is running.
    goto :Failure
)

::make sure we are using Windows Containers
CALL "C:\Program Files\Docker\Docker\DockerCli.exe" -SwitchWindowsEngine

:: Check if WCF service is already running 
FOR /F "tokens=* USEBACKQ" %%F IN (`docker container inspect -f '{{.State.Status}}' %ServiceContainerName%`) DO (
if %%F == 'running' (
    echo. & echo WCF Service container is already running.
    goto :Done 
))

:: change directory to wcf folder
cd ../../../..

echo. & echo Building Docker image for WCF service

:: Building docker image.
docker build -f ./Docker/Service/Dockerfile -t %ServiceImageTag% .
if ERRORLEVEL 1 (
    echo. & echo ERROR: Building docker image failed.
    goto :Failure
)
echo. & echo Building image success..

:: Starting docker container from the image.
echo. & echo Starting WCF Service Container
:: run docker container and mount current directory for wcf source as container volume - C:\wcf
docker run --name %ServiceContainerName% --rm -it -d -h %ServiceHostName% -v "%cd%":"C:\wcf" %ServiceImageTag%
if ERRORLEVEL 1 (
    echo. & echo ERROR: Starting WCF service container failed.
    goto :Failure
)

echo. & echo Started WCF Service Container.
:: print service container IP address
docker inspect --format="{{.NetworkSettings.Networks.nat.IPAddress}}" %ServiceContainerName%
echo %ServiceHostName%

exit /b

:Failure
echo. & echo Error...
set _exitCode=1

:Done
exit /b %_exitCode%
endlocal
