$ErrorActionPreference = 'Stop'

$root = Join-Path ([System.IO.Path]::GetTempPath()) ("WcfConfigurationTests-" + [guid]::NewGuid().ToString('N'))
$scripts = Split-Path -Parent $PSScriptRoot
$helper = Join-Path $scripts 'Copy-CertificateGeneratorConfiguration.ps1'
$null = New-Item -ItemType Directory -Path $root
$settings = Join-Path $root 'CertificateGenerator.dll.config'
$repository = Join-Path $root 'checkout'
$output = Join-Path $repository 'artifacts\bin\CertificateGenerator\Release\net10.0\CertificateGenerator.dll.config'
$valid = '<configuration><appSettings><add key="testserverbase" value="WcfServiceMaster"/><add key="CertExpirationInDay" value="99"/><add key="CrlFileLocation" value="C:\WCFTest\test.crl"/></appSettings></configuration>'

function Assert-Rejected
{
    param ([string] $Path)

    & $env:ComSpec /c "powershell.exe -NoProfile -ExecutionPolicy Bypass -File `"$helper`" -SourcePath `"$Path`" -DestinationPath `"$output`" > `"$root\rejected.log`" 2>&1"
    if ($LASTEXITCODE -eq 0) { throw "Invalid settings accepted: $Path" }
    if ((Get-Content -LiteralPath $output -Raw).Trim() -ne $valid) { throw 'Invalid settings changed the output configuration.' }
}

try
{
    $null = New-Item -ItemType Directory -Path (Split-Path -Parent $output)
    Set-Content -LiteralPath $settings -Value $valid
    & $helper -SourcePath $settings -DestinationPath $output
    & $helper -SourcePath $settings
    if ((Get-Content -LiteralPath $output -Raw).Trim() -ne $valid) { throw 'Settings did not round-trip.' }

    & git -C $repository init --quiet
    if ($LASTEXITCODE -ne 0) { throw 'Temporary git initialization failed.' }
    & git -C $repository clean -xdfq
    if ($LASTEXITCODE -ne 0 -or (Test-Path -LiteralPath $output) -or -not (Test-Path -LiteralPath $settings))
    {
        throw 'Checkout cleanup did not preserve only the external settings.'
    }
    $null = New-Item -ItemType Directory -Path (Split-Path -Parent $output)
    Set-Content -LiteralPath $output -Value ($valid.Replace('WcfServiceMaster', '').Replace('99', '90'))
    & $helper -SourcePath $settings -DestinationPath $output
    if ((Get-Content -LiteralPath $output -Raw).Trim() -ne $valid) { throw 'Settings were not reapplied after rebuilding.' }
    Write-Host 'Real git clean and simulated rebuild preserved and restored IIS settings.'

    $invalid = Join-Path $root 'invalid.config'
    foreach ($content in @(
        '<configuration>',
        $valid.Replace('value="WcfServiceMaster"', 'value=""'),
        $valid.Replace('value="99"', 'value="0"'),
        $valid.Replace('value="99"', 'value="-1"'),
        $valid.Replace('value="99"', 'value="invalid"'),
        $valid.Replace('C:\WCFTest\test.crl', 'test.crl'),
        $valid.Replace('C:\WCFTest\test.crl', 'C:\WCFTest\other.crl'),
        $valid.Replace('</appSettings>', '<add key="testserverbase" value="Other"/></appSettings>'),
        $valid.Replace('<add key="CertExpirationInDay" value="99"/>', ''),
        ('<!DOCTYPE configuration [<!ENTITY settings "value">]>' + $valid)
    ))
    {
        Set-Content -LiteralPath $invalid -Value $content
        Assert-Rejected -Path $invalid
    }
    Assert-Rejected -Path (Join-Path $root 'missing.config')
    Write-Host 'Malformed, missing, duplicate, default and invalid settings rejected without changing output.'

    & $env:ComSpec /c "powershell.exe -NoProfile -ExecutionPolicy Bypass -File `"$helper`" -SourcePath `"$settings`" -DestinationPath `"$root\missing\output.config`" > `"$root\copy-failure.log`" 2>&1"
    if ($LASTEXITCODE -eq 0) { throw 'Configuration copy failure returned success.' }

    $refresh = Get-Content -LiteralPath (Join-Path $scripts 'RefreshServerCertificates.cmd') -Raw
    $prefix = $refresh.Substring(0, $refresh.IndexOf(':: Preserve certificates, CRL'))
    $prefix = $prefix.Replace('SET _GITREPO=C:\git\wcf', "SET _GITREPO=$repository")
    $prefix = $prefix.Replace('SET _CERTCONFIG=C:\WCFTest\CertificateGenerator.dll.config', "SET _CERTCONFIG=$settings")
    $harness = Join-Path $root 'refresh-prefix.cmd'
    Set-Content -LiteralPath $harness -Encoding ascii -Value ($prefix + "`r`necho ready>`"$root\ready`"`r`n:end`r`nexit /b %_EXITCODE%")
    $stubScripts = Join-Path $repository 'src\System.Private.ServiceModel\tools\scripts'
    $null = New-Item -ItemType Directory -Path $stubScripts
    Copy-Item -LiteralPath $helper -Destination $stubScripts
    Set-Content -LiteralPath (Join-Path $root 'ScheduledRestartWcfSelfHostedService.cmd') -Value '@exit /b 0'
    $build = Join-Path $stubScripts 'BuildCertUtil.cmd'
    foreach ($case in @('success', 'missing-settings', 'invalid-settings', 'build-failure', 'copy-failure'))
    {
        Set-Content -LiteralPath $settings -Value $valid
        Set-Content -LiteralPath $output -Value 'build defaults'
        $marker = Join-Path $root 'ready'
        $buildMarker = Join-Path $root 'built'
        foreach ($path in @($marker, $buildMarker))
        {
            if (Test-Path -LiteralPath $path) { Remove-Item -LiteralPath $path }
        }
        $buildBody = "@echo off`r`necho built>`"$buildMarker`"`r`nexit /b 0"
        switch ($case)
        {
            'missing-settings' { Remove-Item -LiteralPath $settings }
            'invalid-settings' { Set-Content -LiteralPath $settings -Value '<configuration/>' }
            'build-failure' { $buildBody = '@exit /b 7' }
            'copy-failure' { $buildBody = "@echo off`r`ndel `"$settings`"`r`nexit /b 0" }
        }
        Set-Content -LiteralPath $build -Encoding ascii -Value $buildBody
        & $env:ComSpec /c "`"$harness`""
        $expectedSuccess = $case -eq 'success'
        if (($LASTEXITCODE -eq 0) -ne $expectedSuccess -or (Test-Path -LiteralPath $marker) -ne $expectedSuccess)
        {
            throw "Refresh configuration gate failed: $case"
        }
        if ($expectedSuccess -and (Get-Content -LiteralPath $output -Raw).Trim() -ne $valid) { throw 'Refresh used build defaults.' }
        if ($case -in @('missing-settings', 'invalid-settings') -and (Test-Path -LiteralPath $buildMarker)) { throw 'Invalid configuration did not gate the build.' }
        Write-Host "Scheduled refresh configuration case '$case' passed."
    }

    $setup = Get-Content -LiteralPath (Join-Path $scripts 'SetupWcfIISHostedService.cmd') -Raw
    $start = $setup.IndexOf(':: Install certificates if')
    $end = $setup.IndexOf(':: Use master repo', $start)
    $block = $setup.Substring($start, $end - $start)
    Copy-Item -LiteralPath $helper -Destination $root
    $setupHarness = Join-Path $root 'setup-skip.cmd'
    Set-Content -LiteralPath $setupHarness -Encoding ascii -Value @"
@echo off
setlocal
set _wcfTestDir=$root
$block
exit /b 3
:SkipCertInstall
exit /b 0
:Failure
exit /b 1
:Run
call %*
exit /b
"@
    foreach ($existing in @($true, $false))
    {
        if ($existing) { Set-Content -LiteralPath $settings -Value $valid }
        else { Remove-Item -LiteralPath $settings }
        & $env:ComSpec /c "`"`"$setupHarness`" > `"$root\setup.log`" 2>&1`""
        if (($LASTEXITCODE -eq 0) -ne $existing) { throw 'IIS setup skip did not validate saved settings.' }
        if ($existing -and (Get-Content -LiteralPath $settings -Raw).Trim() -ne $valid) { throw 'PR setup replaced shared settings.' }
    }
    Write-Host 'Existing IIS setup preserves shared settings and rejects missing migration.'

    $start = $setup.IndexOf('echo Run CertificateGenerator tool.')
    $end = $setup.IndexOf('echo Configue SSL certificate ports', $start)
    $block = $setup.Substring($start, $end - $start)
    $block = $block.Replace('call :Run %certGen%', "echo generated>`"$root\generated`"")
    $setupHarness = Join-Path $root 'setup-generation.cmd'
    $dataDirectory = Join-Path $root 'new-certificate-data'
    Set-Content -LiteralPath $setupHarness -Encoding ascii -Value @"
@echo off
setlocal
set _wcfTestDir=$dataDirectory
set _certRepo=$repository
set _certService=WcfServiceMaster
set _certExpirationInDays=99
$block
exit /b 0
:Failure
exit /b 1
:Run
call %*
exit /b
"@
    & $env:ComSpec /c "`"$setupHarness`""
    if ($LASTEXITCODE -ne 0) { throw 'IIS setup did not save configuration before generation.' }
    $saved = Join-Path $dataDirectory 'CertificateGenerator.dll.config'
    $acl = Get-Acl -LiteralPath $saved
    if (-not $acl.AreAccessRulesProtected -or @($acl.Access | Where-Object IsInherited).Count -ne 0)
    {
        throw 'Saved settings retained inherited permissions.'
    }
    [xml] $configuration = Get-Content -LiteralPath $saved -Raw
    $actual = @{}
    foreach ($item in $configuration.configuration.appSettings.add) { $actual[$item.key] = $item.value }
    if ($actual['testserverbase'] -ne 'WcfServiceMaster' -or $actual['CertExpirationInDay'] -ne '99' -or $actual['CrlFileLocation'] -ne "$dataDirectory\test.crl")
    {
        throw 'IIS setup did not preserve its selected service, lifetime and CRL location.'
    }
    $originalSddl = (Get-Acl -LiteralPath $saved).Sddl
    $updated = (Get-Content -LiteralPath $saved -Raw).Replace('value="99"', 'value="100"')
    $updateSource = Join-Path $root 'updated.config'
    Set-Content -LiteralPath $updateSource -Value $updated
    & $helper -SourcePath $updateSource -DestinationPath $saved -ProtectDestination
    & $helper -SourcePath $saved -DestinationPath $output
    if ((Get-Content -LiteralPath $output -Raw).Trim() -ne $updated.Trim() -or (Get-Acl -LiteralPath $saved).Sddl -ne $originalSddl)
    {
        throw 'Repeated settings update or refresh changed the protected ACL or lost the updated values.'
    }
    Write-Host 'Repeated protected settings update and refresh read/copy preserve values and ACLs.'
    Remove-Item -LiteralPath (Join-Path $root 'generated')
    Remove-Item -LiteralPath $saved
    $null = New-Item -ItemType Directory -Path $saved
    & $env:ComSpec /c "`"`"$setupHarness`" > `"$root\setup-failure.log`" 2>&1`""
    if ($LASTEXITCODE -eq 0 -or (Test-Path -LiteralPath (Join-Path $root 'generated')))
    {
        throw 'IIS setup continued generation after configuration persistence failed.'
    }
    Write-Host 'IIS setup automatically saves selected settings and gates generation on persistence failure.'
}
finally
{
    Remove-Item -LiteralPath $root -Recurse -Force
}
