echo off
setlocal

pushd  %~dp0..\..\..\..
REM Locate any CoreRun.exe to be able to execute CertificateCleanup.exe
REM Because CertificateCleanup itself is a test project, we are guaranteed
REM it will contain CoreRun.exe even if no other tests have been built.
for /f "delims=" %%A in ('where /F /R bin\tests corerun.exe') do set "CoreRunVar=%%A"
if '%CoreRunVar%' EQU '' (
  Echo Could not cleanup certificiates because was unable to locate CoreRun.exe under %~dp0..\..\..\..\bin
  goto done
)

for /f "delims=" %%A in ('where /F /R bin CertificateCleanup.exe') do set "CertCleanupVar=%%A"
if '%CoreRunVar%' EQU '' (
  Echo Could not cleanup certificiates because was unable to locate CertificateCleanup.exe under %~dp0..\..\..\..\bin
  goto done
)

echo Cleaning up certificates using: %CoreRunVar% %CertCleanupVar%
%CoreRunVar% %CertCleanupVar%

:done
popd
exit /b
