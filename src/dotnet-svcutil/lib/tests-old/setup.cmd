
IF NOT "%BuildConfiguration%"=="Release" SET BuildConfiguration=Debug

ECHO.Configuration: %BuildConfiguration%

ECHO.Script folder: %1

SET BINSOURCE=%1..\..\..\..\artifacts\bin\dotnet-svcutil.test\%BuildConfiguration%\netcoreapp2.1\
PUSHD %BINSOURCE%
SET BINSOURCE=%cd%
POPD

SET PACKAGESOURCE=%1..\..\..\..\artifacts\packages\$BuildConfiguration%\Shipping\

SET TESTSOURCE=%1TestCases
SET BASELINESOURCE=%1Baselines
SET TARGET=%BINSOURCE%\netcoreapp2.1\publish

ECHO.BINSOURCE=%BINSOURCE%
ECHO.TESTSOURCE=%TESTSOURCE%
ECHO.TARGET=%TARGET%

RD /S /Q %TARGET%

PUSHD %1\Source
dotnet publish -f netcoreapp2.1 -c %BuildConfiguration%
POPD

ECHO.
ECHO.Copy test cases
XCOPY /I /E /Q "%TESTSOURCE%" "%TARGET%\TestCases\"

ECHO.
ECHO.Copy baselines
XCOPY /I /E /Q "%BASELINESOURCE%" "%TARGET%\Baselines\"

ECHO.
ECHO.Copy nuget package
FOR /F %%f IN ('dir %PACKAGESOURCE%\dotnet-svcutil-lib.*.nupkg /o-n /b') DO (
  ECHO.%%f | findstr /C:"symbols">nul || (
    COPY "%PACKAGESOURCE%\%%f" "%TARGET%\TestCases" /V
    SET FULLNAME=%%f
    GOTO :END
  )
)

:END

ECHO.%FULLNAME% was copied