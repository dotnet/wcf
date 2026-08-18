[CmdletBinding()]
param(
    [string]$Workspace,
    [string]$BaseRef,
    [ValidateSet("All", "Whitespace", "Style", "Analyzers")]
    [string]$Category = "All",
    [switch]$VerifyOnly,
    [switch]$Apply,
    [switch]$AllowModifiedFiles
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ($VerifyOnly -and $Apply)
{
    throw "Specify either -VerifyOnly or -Apply, not both."
}

if (-not $VerifyOnly -and -not $Apply)
{
    $VerifyOnly = $true
}

[string]$scriptRoot = $PSScriptRoot
[string[]]$repositoryRootOutput = @(& git rev-parse --show-toplevel)
if ($LASTEXITCODE -ne 0 -or $repositoryRootOutput.Count -eq 0)
{
    throw "The current directory is not inside a git repository."
}
[string]$repositoryRoot = $repositoryRootOutput[0].Trim()

[string]$getChangedFilesScript = Join-Path $scriptRoot "Get-ChangedCSharpFiles.ps1"
if (-not (Test-Path -LiteralPath $getChangedFilesScript))
{
    throw "Changed-file helper was not found at '$getChangedFilesScript'."
}

[System.Collections.Generic.List[string]]$allChangedFiles =
    [System.Collections.Generic.List[string]]::new()

foreach ($path in (& $getChangedFilesScript -BaseRef $BaseRef))
{
    $allChangedFiles.Add($path)
}

if ($allChangedFiles.Count -eq 0)
{
    Write-Host "No changed C# files are eligible for formatting."
    exit 0
}

[System.Collections.Generic.List[string]]$targetFiles =
    [System.Collections.Generic.List[string]]::new()

if ($Apply -and -not $AllowModifiedFiles)
{
    foreach ($path in (& $getChangedFilesScript -BaseRef $BaseRef -NewFilesOnly))
    {
        $targetFiles.Add($path)
    }

    [string[]]$skippedFiles = @(
        $allChangedFiles |
            Where-Object { -not $targetFiles.Contains($_) }
    )

    if ($skippedFiles.Count -gt 0)
    {
        Write-Warning (
            "dotnet format is file-scoped. Skipping modified existing files " +
            "in apply mode:`n - " + ($skippedFiles -join "`n - ")
        )
        Write-Warning (
            "Format their changed hunks manually, then run this script with " +
            "-VerifyOnly. Use -AllowModifiedFiles only when the user explicitly " +
            "accepts whole-file formatting."
        )
    }
}
else
{
    foreach ($path in $allChangedFiles)
    {
        $targetFiles.Add($path)
    }
}

if ($targetFiles.Count -eq 0)
{
    Write-Host "No safe whole-file apply targets were found."
    exit 0
}

[string[]]$localDotnetCandidates = @(
    (Join-Path $repositoryRoot ".dotnet/dotnet.exe"),
    (Join-Path $repositoryRoot ".dotnet/dotnet")
)

[string]$dotnetCommand = $null
foreach ($candidate in $localDotnetCandidates)
{
    if (Test-Path -LiteralPath $candidate)
    {
        $dotnetCommand = $candidate
        break
    }
}

if ([string]::IsNullOrWhiteSpace($dotnetCommand))
{
    [System.Management.Automation.CommandInfo]$dotnet =
        Get-Command dotnet -ErrorAction SilentlyContinue
    if ($null -eq $dotnet)
    {
        throw "dotnet was not found. Restore the repository toolchain first."
    }

    $dotnetCommand = $dotnet.Source
}

function Get-WorkspaceForPath
{
    param(
        [string]$Path,
        [string]$ExplicitWorkspace
    )

    if (-not [string]::IsNullOrWhiteSpace($ExplicitWorkspace))
    {
        return $ExplicitWorkspace
    }

    [string]$normalized = $Path.Replace("\", "/")
    if ($normalized.StartsWith("src/dotnet-svcutil/"))
    {
        return "dotnet-svcutil.sln"
    }

    if ($normalized.StartsWith("src/svcutilcore/"))
    {
        return "dotnet-svcutil.xmlserializer.sln"
    }

    return "System.ServiceModel.sln"
}

[hashtable]$workspaceGroups = @{}
foreach ($path in $targetFiles)
{
    [string]$workspaceName =
        Get-WorkspaceForPath -Path $path -ExplicitWorkspace $Workspace

    if (-not $workspaceGroups.ContainsKey($workspaceName))
    {
        $workspaceGroups[$workspaceName] =
            [System.Collections.Generic.List[string]]::new()
    }

    $workspaceGroups[$workspaceName].Add($path)
}

Write-Host (
    "Formatting mode: " +
    $(if ($VerifyOnly) { "verify" } else { "apply" })
)
Write-Host "Category: $Category"

Push-Location $repositoryRoot
try
{
    foreach ($workspaceName in ($workspaceGroups.Keys | Sort-Object))
    {
        [string]$workspacePath = if ([IO.Path]::IsPathRooted($workspaceName))
        {
            $workspaceName
        }
        else
        {
            Join-Path $repositoryRoot $workspaceName
        }

        if (-not (Test-Path -LiteralPath $workspacePath))
        {
            throw "Workspace '$workspacePath' does not exist."
        }

        [System.Collections.Generic.List[string]]$formatArguments =
            [System.Collections.Generic.List[string]]::new()
        $formatArguments.Add("format")

        if ($Category -ne "All")
        {
            $formatArguments.Add($Category.ToLowerInvariant())
        }

        $formatArguments.Add($workspacePath)
        $formatArguments.Add("--no-restore")

        if ($VerifyOnly)
        {
            $formatArguments.Add("--verify-no-changes")
        }

        $formatArguments.Add("--include")
        foreach ($path in $workspaceGroups[$workspaceName])
        {
            $formatArguments.Add($path)
        }

        Write-Host "Workspace: $workspaceName"
        Write-Host "Files:"
        foreach ($path in $workspaceGroups[$workspaceName])
        {
            Write-Host " - $path"
        }

        & $dotnetCommand @formatArguments
        if ($LASTEXITCODE -ne 0)
        {
            exit $LASTEXITCODE
        }
    }

    exit 0
}
finally
{
    Pop-Location
}
