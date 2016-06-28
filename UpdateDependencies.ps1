#
# Copyright (c) .NET Foundation and contributors. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
#

# This script updates all the project.json files with the build version passed in

param(
    [Parameter(Mandatory=$true)][string]$NewVersion
    )

# Updates the dir.props file with the new build number
function UpdateValidDependencyVersionsFile
{
    $DirPropsPath = "$PSScriptRoot\dir.props"
    
    $DirPropsContent = Get-Content $DirPropsPath | % { 
        $_ -replace "<WCFExpectedPrerelease>.*</WCFExpectedPrerelease>","<WCFExpectedPrerelease>$NewVersion</WCFExpectedPrerelease>"
    }
    Set-Content $DirPropsPath $DirPropsContent

    return $true
}

# Updates all the project.json files with out of date version numbers
function RunUpdatePackageDependencyVersions
{
    cmd /c $PSScriptRoot\build.cmd /t:UpdateInvalidPackageVersions | Out-Host

    return $LASTEXITCODE -eq 0
}

if (!(UpdateValidDependencyVersionsFile))
{
    Exit -1
}

if (!(RunUpdatePackageDependencyVersions))
{
    Exit -1
}

Write-Host -ForegroundColor Green "Successfully updated dependencies from the latest build numbers"

exit $LastExitCode
