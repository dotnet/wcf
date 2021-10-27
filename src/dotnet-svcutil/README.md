# dotnet-svcutil -- command-line tool for generating a web service reference compatible with .NET Core and .NET Standard projects.

## How to build

Execute following commandline under repository's root directory:
`eng\common\cibuild.cmd -preparemachine -configuration Release -projects dotnet-svcutil.sln /p:Test=false /p:Sign=false`

Alternatively, run `build.cmd`  which located at same directory of this readme file.

The built package is placed at:
`[RepoRootDir]/artifacts/artifacts/packages/[Release/Debug]/[Shipping/NonShipping]/[dotnet-svcutil.*.nupkg]`

## How to run test

Execute following commandline under repository's root directory:
`eng\common\cibuild.cmd -preparemachine -configuration Release -projects dotnet-svcutil.sln /p:Test=True /p:Sign=false`

Test result summary could be found at:
`[RepoRootDir]/artifacts/artifacts/TestResults/[Release/Debug]/dotnet-svcutil-lib.Tests*.xml`

Test result details could be found at:
`[RepoRootDir]/artifacts/artifacts/TestOutput/TestResults/`, each test variation has a corresponding folder generated.

## How to install and uninstall the tool

Uninstall: 
`dotnet tool uninstall --global dotnet-svcutil`

Install:
`dotnet tool install --global --add-source [RepoRootDir]/artifacts/packages/[Release/Debug]/[Shipping/NonShipping] dotnet-svcutil --version [tool-version]`