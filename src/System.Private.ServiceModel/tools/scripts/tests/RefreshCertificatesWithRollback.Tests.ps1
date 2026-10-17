$ErrorActionPreference = 'Stop'

$root = Join-Path ([System.IO.Path]::GetTempPath()) ("WcfRefreshTests-" + [guid]::NewGuid().ToString('N'))
$generatorDirectory = Join-Path $root 'artifacts\bin\CertificateGenerator\Release\net10.0'
$null = New-Item -ItemType Directory -Path $generatorDirectory
$null = New-Item -ItemType File -Path (Join-Path $generatorDirectory 'CertificateGenerator.exe')
$crlPath = Join-Path $root 'test.crl'
Set-Content -LiteralPath (Join-Path $generatorDirectory 'CertificateGenerator.dll.config') -Value "<configuration><appSettings><add key='CrlFileLocation' value='$crlPath'/></appSettings></configuration>"
. (Join-Path $PSScriptRoot '..\RefreshCertificatesWithRollback.ps1') -RepositoryPath $root

function Import-Module {}
function Get-IISAppPool
{
    return [pscustomobject]@{ Name = 'WcfServiceTest'; State = 'Started'; ProcessModel = [pscustomobject]@{ IdentityType = 'SpecificUser'; UserName = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name } }
}
function Get-RunningTestHosts
{
    if ($script:failure -ne 'acl-without-host') { return [pscustomobject]@{ ProcessId = 123; ExecutablePath = 'original-host.exe' } }
}
function Get-CertificateSnapshot
{
    $script:events.Add('snapshot')
    return [pscustomobject]@{ Certificates = @() }
}
function Get-Process
{
    $process = [pscustomobject]@{}
    $process | Add-Member -MemberType ScriptMethod -Name WaitForExit -Value { param ($Timeout) return $true }
    $process | Add-Member -MemberType ScriptMethod -Name Dispose -Value {}
    return $process
}
function Stop-Process { $script:events.Add('stop') }
function Restore-CertificateSnapshot
{
    $script:events.Add('restore')
    if ($script:failure -eq 'restore') { throw 'Injected restoration failure' }
}
function Restart-TestHosts
{
    param ($Hosts)
    if ($script:failure -eq 'acl-without-host')
    {
        if (@($Hosts).Count -ne 0) { throw 'Recovery started a host that was not previously running.' }
        return
    }
    if (@($Hosts).Count -ne 1 -or $Hosts[0].ExecutablePath -ne 'original-host.exe') { throw 'Original host state was not preserved.' }
    $script:events.Add('restart')
}
function Invoke-CheckedProcess
{
    param ($FilePath, $Arguments)
    if ($FilePath -like '*CertificateGenerator.exe')
    {
        $script:events.Add('generate')
        [System.IO.File]::WriteAllBytes($crlPath, [byte[]]@(9, 9, 9))
        if ($script:failure -eq 'generator') { throw 'Injected generator failure' }
    }
    elseif ($FilePath -eq 'powershell.exe')
    {
        if ($Arguments[-1] -ne [System.Security.Principal.WindowsIdentity]::GetCurrent().Name) { throw 'ACL invocation did not use the configured worker account.' }
        $script:events.Add('grant')
        if ($script:failure -in @('acl', 'restore', 'acl-without-crl', 'acl-without-host')) { throw 'Injected ACL failure' }
    }
    elseif ($FilePath -like '*appcmd.exe') { $script:events.Add('recycle') }
    else { throw "Unexpected external process: $FilePath" }
}

try
{
    $retainedBackups = @()
    $expectedLocalSystemSid = 'S-1-5-18'
    $expectedLocalServiceSid = 'S-1-5-19'
    $expectedNetworkServiceSid = 'S-1-5-20'
    foreach ($identity in @('ApplicationPoolIdentity', 'LocalSystem', 'LocalService', 'NetworkService', 'SpecificUser'))
    {
        $user = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name
        $pool = [pscustomobject]@{ Name = 'WcfServiceTest'; ProcessModel = [pscustomobject]@{ IdentityType = $identity; UserName = $user } }
        $expected = switch ($identity)
        {
            'ApplicationPoolIdentity' { 'IIS APPPOOL\WcfServiceTest' }
            'SpecificUser' { $user }
            default
            {
                $sid = switch ($identity)
                {
                    'LocalSystem' { $expectedLocalSystemSid }
                    'LocalService' { $expectedLocalServiceSid }
                    'NetworkService' { $expectedNetworkServiceSid }
                }
                (New-Object System.Security.Principal.SecurityIdentifier($sid)).Translate([System.Security.Principal.NTAccount]).Value
            }
        }
        if ((Get-AppPoolAccount -Pool $pool) -ne $expected) { throw "Wrong account for $identity." }
        Write-Host "$identity account resolution passed."
    }
    foreach ($identity in @('SpecificUser', 'Unsupported'))
    {
        $pool = [pscustomobject]@{ Name = 'WcfServiceTest'; ProcessModel = [pscustomobject]@{ IdentityType = $identity; UserName = '' } }
        $rejected = $false
        try { $null = Get-AppPoolAccount -Pool $pool }
        catch { $rejected = $true }
        if (-not $rejected) { throw "Invalid $identity configuration was accepted." }
    }
    foreach ($case in @('success', 'generator', 'acl', 'restore', 'acl-without-crl', 'acl-without-host'))
    {
        $script:failure = $case
        $script:events = New-Object 'System.Collections.Generic.List[string]'
        [System.IO.File]::WriteAllBytes($crlPath, [byte[]]@(1, 2, 3))
        if ($case -eq 'acl-without-crl') { Remove-Item -LiteralPath $crlPath }
        $caught = $null
        try { Invoke-CertificateRefresh -Root $root }
        catch { $caught = $_ }
        $expected = switch ($case)
        {
            'success' { 'snapshot,stop,generate,grant' }
            'generator' { 'snapshot,stop,generate,restore,recycle,restart' }
            'acl' { 'snapshot,stop,generate,grant,restore,recycle,restart' }
            'restore' { 'snapshot,stop,generate,grant,restore' }
            'acl-without-crl' { 'snapshot,stop,generate,grant,restore,recycle,restart' }
            'acl-without-host' { 'snapshot,generate,grant,restore,recycle' }
        }
        if (($script:events -join ',') -ne $expected) { throw "$case event sequence: $($script:events -join ','); expected $expected." }
        if (($case -eq 'success') -ne ($null -eq $caught)) { throw "$case returned the wrong success/failure result." }
        $expectedCrl = if ($case -eq 'success') { '9,9,9' } else { '1,2,3' }
        if ($case -eq 'acl-without-crl')
        {
            if (Test-Path -LiteralPath $crlPath) { throw 'Recovery left a CRL that did not previously exist.' }
        }
        elseif (([System.IO.File]::ReadAllBytes($crlPath) -join ',') -ne $expectedCrl) { throw "$case did not preserve the expected CRL." }
        if ($case -eq 'restore')
        {
            if ("$caught" -notlike '*Manual recovery is required*') { throw 'Restoration failure was not surfaced.' }
            $backup = [regex]::Match("$caught", "retained at '([^']+)'").Groups[1].Value
            if ([string]::IsNullOrEmpty($backup) -or -not (Test-Path -LiteralPath (Join-Path $backup 'state.xml'))) { throw 'Recovery backup was not retained.' }
            $retainedBackups += $backup
        }
        Write-Host "$case recovery case passed."
    }

    $batch = Get-Content -LiteralPath (Join-Path $PSScriptRoot '..\RefreshServerCertificates.cmd') -Raw
    $start = $batch.IndexOf('powershell -NoProfile -ExecutionPolicy unrestricted -File')
    $end = $batch.IndexOf(':: Configure HTTPS ports', $start)
    if ($start -lt 0 -or $end -lt 0) { throw 'Could not locate the scheduled refresh invocation.' }
    $block = $batch.Substring($start, $end - $start)
    $block = [regex]::Replace($block, '(?m)^powershell[^\r\n]*', 'cmd /c exit %WCF_REFRESH_TEST_EXIT%')
    $harness = Join-Path $root 'refresh-harness.cmd'
    Set-Content -LiteralPath $harness -Encoding ascii -Value @"
@echo off
setlocal
set _EXITCODE=0
set _LOGFILE="$root\refresh.log"
$block
echo continued>"$root\continued.log"
:end
exit /b %_EXITCODE%
"@
    foreach ($code in @(0, 1))
    {
        $env:WCF_REFRESH_TEST_EXIT = $code.ToString()
        $marker = Join-Path $root 'continued.log'
        if (Test-Path -LiteralPath $marker) { Remove-Item -LiteralPath $marker }
        & $env:ComSpec /c "`"$harness`""
        if ($LASTEXITCODE -ne $code -or (Test-Path -LiteralPath $marker) -ne ($code -eq 0))
        {
            throw 'Scheduled refresh did not preserve the helper exit code or gate subsequent steps.'
        }
        Write-Host "Scheduled caller exit=$code propagation passed."
    }
}
finally
{
    Remove-Item -LiteralPath $root -Recurse -Force
    foreach ($backup in $retainedBackups) { Remove-Item -LiteralPath $backup -Recurse -Force }
}
