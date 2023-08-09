[CmdletBinding(PositionalBinding=$false)]
Param(
  [switch][Alias('h')]$help,
  [switch][Alias('b')]$build,
  [switch][Alias('t')]$test,
  [string][Alias('c')]$configuration = "Debug",
  [string]$vs,
  [string]$os,
  [switch]$coverage,
  [string]$testscope,
  [string]$arch,
  [string]$librariesConfiguration,
  [Parameter(ValueFromRemainingArguments=$true)][String[]]$properties
)

function Get-Help() {
  Write-Host "Common settings:"
  Write-Host "  -os                     Build operating system: Windows_NT or Unix"
  Write-Host "  -arch                   Build platform: x86, x64, arm or arm64"
  Write-Host "  -configuration <value>  Build configuration: Debug or Release (short: -c)"
  Write-Host "  -verbosity <value>      MSBuild verbosity: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic] (short: -v)"
  Write-Host "  -binaryLog              Output binary log (short: -bl)"
  Write-Host "  -help                   Print help and exit (short: -h)"
  Write-Host ""

  Write-Host "Actions (defaults to -restore -build):"
  Write-Host "  -restore                Restore dependencies (short: -r)"
  Write-Host "  -build                  Build all source projects (short: -b)"
  Write-Host "  -rebuild                Rebuild all source projects"
  Write-Host "  -test                   Run all unit tests (short: -t)"
  Write-Host "  -pack                   Package build outputs into NuGet packages"
  Write-Host "  -sign                   Sign build outputs"
  Write-Host "  -publish                Publish artifacts (e.g. symbols)"
  Write-Host "  -clean                  Clean the solution"
  Write-Host ""

  Write-Host "Libraries settings:"
  Write-Host "  -vs                     Open the solution with VS for Test Explorer support. Path to solution file"
  Write-Host "  -coverage               Collect code coverage when testing"
  Write-Host "  -testscope              Scope tests, allowed values: innerloop, outerloop, all"
  Write-Host ""

  Write-Host "Command-line arguments not listed above are passed thru to msbuild."
  Write-Host "The above arguments can be shortened as much as to be unambiguous (e.g. -con for configuration, -t for test, etc.)."
}

if ($help -or (($null -ne $properties) -and ($properties.Contains('/help') -or $properties.Contains('/?')))) {
  Get-Help
  exit 0
}

# VS Test Explorer support for libraries
if ($vs) {
  . $PSScriptRoot\common\tools.ps1

  # This tells MSBuild to load the SDK from the directory of the bootstrapped SDK
  $env:DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR=InitializeDotNetCli -install:$false

  # This tells .NET Core not to go looking for .NET Core in other places
  $env:DOTNET_MULTILEVEL_LOOKUP=0;

  # Put our local dotnet.exe on PATH first so Visual Studio knows which one to use
  $env:PATH=($env:DOTNET_ROOT + ";" + $env:PATH);

  # Launch Visual Studio with the locally defined environment variables
  Invoke-Item "$vs"

  exit 0
}

# Check if an action is passed in
$actions = "r","restore","b","build","rebuild","t","test","pack","sign","publish","clean"
$actionPassedIn = @(Compare-Object -ReferenceObject @($PSBoundParameters.Keys) -DifferenceObject $actions -ExcludeDifferent -IncludeEqual).Length -ne 0
if ($null -ne $properties -and $actionPassedIn -ne $true) {
  $actionPassedIn = @(Compare-Object -ReferenceObject $properties -DifferenceObject $actions.ForEach({ "-" + $_ }) -ExcludeDifferent -IncludeEqual).Length -ne 0
}

if (!$actionPassedIn) {
  $arguments = "-restore -build"
}

foreach ($argument in $PSBoundParameters.Keys)
{
  switch($argument)
  {
    "build"                { $arguments += " -build" }
    "test"                 { $arguments += " -test" }
    "configuration"        { $configuration = (Get-Culture).TextInfo.ToTitleCase($($PSBoundParameters[$argument])); $arguments += " -configuration $configuration" }
    "os"                   { $arguments += " /p:TargetOS=$($PSBoundParameters[$argument])" }
    "arch"                 { $arguments += " /p:TargetArchitecture=$($PSBoundParameters[$argument])" }
    "properties"           { $arguments += " " + $properties }
	  "testscope"            
        {
         if ($testscope -eq "outerloop" -or $testscope -eq "all") { $arguments += " /p:IntegrationTest=true" }
         if ($testscope -eq "wcf") { $arguments += " -projects System.ServiceModel.sln /p:IntegrationTest=true" }
         if ($testscope -eq "svcutil") { $arguments += " -projects dotnet-svcutil.sln /p:IntegrationTest=true" }
        }
    default                { $arguments += " /p:$argument=$($PSBoundParameters[$argument])" }
  }
}

Invoke-Expression "& `"$PSScriptRoot/common/build.ps1`" $arguments"
exit $lastExitCode
