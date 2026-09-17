[CmdletBinding()]
param(
    [string]$BaseRef,
    [switch]$NewFilesOnly,
    [switch]$IncludeExcluded
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Invoke-GitLines
{
    param([string[]]$Arguments)

    [string[]]$output = @(& git @Arguments)
    if ($LASTEXITCODE -ne 0)
    {
        throw "git $($Arguments -join ' ') failed with exit code $LASTEXITCODE."
    }

    return @($output | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
}

function Resolve-BaseCommit
{
    param([string]$RequestedBaseRef)

    if (-not [string]::IsNullOrWhiteSpace($RequestedBaseRef))
    {
        & git rev-parse --verify "$RequestedBaseRef`^{commit}" *> $null
        if ($LASTEXITCODE -ne 0)
        {
            throw "Base ref '$RequestedBaseRef' does not resolve to a commit."
        }

        return (@(Invoke-GitLines @("merge-base", "HEAD", $RequestedBaseRef)))[0].Trim()
    }

    [string[]]$candidates = @("upstream/main", "origin/main", "main")
    foreach ($candidate in $candidates)
    {
        & git rev-parse --verify "$candidate`^{commit}" *> $null
        if ($LASTEXITCODE -eq 0)
        {
            return (@(Invoke-GitLines @("merge-base", "HEAD", $candidate)))[0].Trim()
        }
    }

    return (@(Invoke-GitLines @("rev-parse", "HEAD")))[0].Trim()
}

function Test-IsExcludedPath
{
    param([string]$Path)

    [string]$normalized = $Path.Replace("\", "/")
    [string]$fileName = [IO.Path]::GetFileName($normalized)

    if ($normalized -match "(^|/)(bin|obj|\.dotnet)(/|$)")
    {
        return $true
    }

    if ($normalized -match "(^|/)ref(/|$)")
    {
        return $true
    }

    if ($normalized -match "^eng/common/")
    {
        return $true
    }

    if ($normalized -match "^src/dotnet-svcutil/lib/tests/(Baselines|TestCases)/")
    {
        return $true
    }

    if ($fileName -match "(\.g|\.generated|\.designer)\.cs$")
    {
        return $true
    }

    if ($fileName -match "(^|_)AssemblyInfo\.cs$")
    {
        return $true
    }

    if ($fileName -eq "AsmOffsets.cs" -or $fileName.EndsWith(".notsupported.cs"))
    {
        return $true
    }

    return $false
}

[string]$repositoryRoot =
    (@(Invoke-GitLines @("rev-parse", "--show-toplevel")))[0].Trim()

Push-Location $repositoryRoot
try
{
    [string]$baseCommit = Resolve-BaseCommit -RequestedBaseRef $BaseRef
    [string]$diffFilter = if ($NewFilesOnly) { "A" } else { "ACMR" }

    [System.Collections.Generic.List[string]]$paths = [System.Collections.Generic.List[string]]::new()

    foreach ($path in (Invoke-GitLines @(
        "diff", "--name-only", "--diff-filter=$diffFilter",
        "$baseCommit...HEAD", "--", "*.cs")))
    {
        $paths.Add($path)
    }

    foreach ($path in (Invoke-GitLines @(
        "diff", "--name-only", "--diff-filter=$diffFilter", "--", "*.cs")))
    {
        $paths.Add($path)
    }

    foreach ($path in (Invoke-GitLines @(
        "diff", "--cached", "--name-only", "--diff-filter=$diffFilter",
        "--", "*.cs")))
    {
        $paths.Add($path)
    }

    foreach ($path in (Invoke-GitLines @(
        "ls-files", "--others", "--exclude-standard", "--", "*.cs")))
    {
        $paths.Add($path)
    }

    [string[]]$uniquePaths = @(
        $paths |
            ForEach-Object { $_.Replace("\", "/") } |
            Sort-Object -Unique
    )

    foreach ($path in $uniquePaths)
    {
        if ($IncludeExcluded -or -not (Test-IsExcludedPath -Path $path))
        {
            Write-Output $path
        }
    }
}
finally
{
    Pop-Location
}
