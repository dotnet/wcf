param (
    [Parameter(Mandatory=$true)]
    [string] $RepositoryPath
)

$ErrorActionPreference = 'Stop'

$localSystemSid = 'S-1-5-18'
$localServiceSid = 'S-1-5-19'
$networkServiceSid = 'S-1-5-20'
$administratorsGroupSid = 'S-1-5-32-544'

function Invoke-CheckedProcess
{
    param ([string] $FilePath, [string[]] $Arguments)

    & $FilePath @Arguments
    if ($LASTEXITCODE -ne 0)
    {
        throw "'$FilePath' failed with exit code $LASTEXITCODE."
    }
}

function Get-CertificateSnapshot
{
    $snapshots = @()
    try
    {
        foreach ($location in @('LocalMachine', 'CurrentUser'))
        {
            foreach ($name in @('My', 'Root', 'TrustedPeople'))
            {
                $store = New-Object System.Security.Cryptography.X509Certificates.X509Store($name, $location)
                try
                {
                    $store.Open('ReadOnly, IncludeArchived')
                    # Keep the certificate contexts, including their original key-provider associations.
                    # Removing a certificate from X509Store does not delete its persisted private key.
                    $certificates = @($store.Certificates.Find('FindByIssuerName', 'DO_NOT_TRUST_WcfBridgeRootCA', $false))
                    $snapshots += [pscustomobject]@{ Name = $name; Location = $location; Certificates = $certificates }
                }
                finally
                {
                    $store.Close()
                }
            }
        }
        return $snapshots
    }
    catch
    {
        foreach ($snapshot in $snapshots)
        {
            foreach ($certificate in $snapshot.Certificates) { $certificate.Dispose() }
        }
        throw
    }
}

function Restore-CertificateSnapshot
{
    param ([object[]] $Snapshots)

    foreach ($snapshot in $Snapshots)
    {
        $store = New-Object System.Security.Cryptography.X509Certificates.X509Store($snapshot.Name, $snapshot.Location)
        try
        {
            $store.Open('ReadWrite, IncludeArchived')
            foreach ($certificate in $store.Certificates.Find('FindByIssuerName', 'DO_NOT_TRUST_WcfBridgeRootCA', $false))
            {
                try
                {
                    if ($snapshot.Certificates.Thumbprint -notcontains $certificate.Thumbprint)
                    {
                        $store.Remove($certificate)
                    }
                }
                finally { $certificate.Dispose() }
            }
            foreach ($certificate in $snapshot.Certificates)
            {
                # CurrentUser trust stores can expose inherited LocalMachine entries.
                # Restore machine stores first and do not remove or duplicate inherited originals.
                $existing = $store.Certificates.Find('FindByThumbprint', $certificate.Thumbprint, $false)
                try
                {
                    if ($existing.Count -eq 0) { $store.Add($certificate) }
                }
                finally { foreach ($item in $existing) { $item.Dispose() } }
                $restored = $store.Certificates.Find('FindByThumbprint', $certificate.Thumbprint, $false)
                try
                {
                    if ($restored.Count -ne 1 -or $restored[0].HasPrivateKey -ne $certificate.HasPrivateKey)
                    {
                        throw "Certificate '$($certificate.Thumbprint)' was not restored with its original key association."
                    }
                    if ($certificate.HasPrivateKey)
                    {
                        $key = [System.Security.Cryptography.X509Certificates.RSACertificateExtensions]::GetRSAPrivateKey($restored[0])
                        try
                        {
                            if ($null -eq $key) { throw "Cannot reopen private key for '$($certificate.Thumbprint)'." }
                            $null = $key.SignData([byte[]]@(1, 2, 3), [System.Security.Cryptography.HashAlgorithmName]::SHA256, [System.Security.Cryptography.RSASignaturePadding]::Pkcs1)
                        }
                        finally { if ($null -ne $key) { $key.Dispose() } }
                    }
                }
                finally
                {
                    foreach ($item in $restored) { $item.Dispose() }
                }
            }
        }
        finally
        {
            $store.Close()
        }
    }
}

function Get-RunningTestHosts
{
    $hosts = @(Get-CimInstance Win32_Process -Filter "Name='SelfHostedWCFService.exe'")
    foreach ($hostProcess in $hosts)
    {
        $owner = Invoke-CimMethod -InputObject $hostProcess -MethodName GetOwner
        $currentUser = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name
        if ($owner.ReturnValue -ne 0 -or "$($owner.Domain)\$($owner.User)" -ne $currentUser)
        {
            throw "Cannot restart process $($hostProcess.ProcessId) as its original account; refresh has not started."
        }
        if (-not (Test-Path -LiteralPath $hostProcess.ExecutablePath -PathType Leaf))
        {
            throw "Original host binary '$($hostProcess.ExecutablePath)' is unavailable."
        }
        # The Framework test host has no command-line arguments.
        if ($hostProcess.CommandLine.Trim().Trim('"') -ne $hostProcess.ExecutablePath)
        {
            throw "Unexpected command line for process $($hostProcess.ProcessId); refresh has not started."
        }
    }
    return $hosts
}

function Get-AppPoolAccount
{
    param ([object] $Pool)

    switch ($Pool.ProcessModel.IdentityType.ToString())
    {
        'ApplicationPoolIdentity' { return "IIS APPPOOL\$($Pool.Name)" }
        'LocalSystem' { $sid = $localSystemSid }
        'LocalService' { $sid = $localServiceSid }
        'NetworkService' { $sid = $networkServiceSid }
        'SpecificUser'
        {
            if ([string]::IsNullOrWhiteSpace($Pool.ProcessModel.UserName))
            {
                throw "Pool '$($Pool.Name)' has no configured SpecificUser account."
            }
            return $Pool.ProcessModel.UserName
        }
        default { throw "Pool '$($Pool.Name)' has unsupported identity '$($Pool.ProcessModel.IdentityType)'." }
    }

    return (New-Object System.Security.Principal.SecurityIdentifier($sid)).Translate([System.Security.Principal.NTAccount]).Value
}

function Restart-TestHosts
{
    param ([object[]] $Hosts)

    foreach ($hostProcess in $Hosts)
    {
        $process = Start-Process -FilePath $hostProcess.ExecutablePath -WorkingDirectory (Split-Path -Parent $hostProcess.ExecutablePath) -PassThru
        $ready = $false
        for ($attempt = 0; $attempt -lt 30; $attempt++)
        {
            Start-Sleep -Seconds 2
            $process.Refresh()
            if ($process.HasExited) { throw "Restored test host exited with code $($process.ExitCode)." }
            try
            {
                $response = Invoke-WebRequest -Uri 'http://localhost/TestHost.svc/Ping' -UseBasicParsing -TimeoutSec 2
                if ($response.StatusCode -eq 200) { $ready = $true; break }
            }
            catch [System.Net.WebException]
            {
                if ($attempt -eq 29) { throw }
            }
        }
        if (-not $ready) { throw 'Restored test host did not become responsive.' }
        Write-Output "Restored test host is responsive (PID $($process.Id))."
    }
}

function Invoke-CertificateRefresh
{
    param ([string] $Root)

    $scripts = Join-Path $Root 'src\System.Private.ServiceModel\tools\scripts'
    $generator = Join-Path $Root 'artifacts\bin\CertificateGenerator\Release\net10.0\CertificateGenerator.exe'
    if (-not (Test-Path -LiteralPath $generator -PathType Leaf)) { throw "Generator not found: $generator" }

    $crlPath = 'C:\WCFTest\test.crl'
    $configPath = [System.IO.Path]::ChangeExtension($generator, '.dll.config')
    if (Test-Path -LiteralPath $configPath)
    {
        [xml] $config = Get-Content -LiteralPath $configPath -Raw
        $setting = @($config.configuration.appSettings.add | Where-Object key -eq 'CrlFileLocation')
        if ($setting.Count -gt 0) { $crlPath = $setting[0].value }
    }
    if (-not [System.IO.Path]::IsPathRooted($crlPath)) { throw 'CRL path must be absolute for rollback.' }

    Import-Module IISAdministration -ErrorAction Stop
    $pools = @(Get-IISAppPool | Where-Object { $_.Name -like '*wcfservice*' })
    $accounts = @{}
    foreach ($pool in $pools)
    {
        $account = Get-AppPoolAccount -Pool $pool
        # Validate identities before stopping hosts or replacing certificates.
        $null = (New-Object System.Security.Principal.NTAccount($account)).Translate([System.Security.Principal.SecurityIdentifier])
        $accounts[$pool.Name] = $account
    }
    $hosts = @(Get-RunningTestHosts)
    $crlExists = Test-Path -LiteralPath $crlPath -PathType Leaf
    $crl = if ($crlExists) { [System.IO.File]::ReadAllBytes($crlPath) } else { $null }
    $snapshots = @(Get-CertificateSnapshot)
    $stoppedHosts = @()
    $generationStarted = $false
    $backupPath = Join-Path ([System.IO.Path]::GetTempPath()) ("WcfCertificateRefresh-" + [guid]::NewGuid().ToString('N'))
    $retainBackup = $false
    try
    {
        $null = New-Item -ItemType Directory -Path $backupPath
        $acl = Get-Acl -LiteralPath $backupPath
        $acl.SetAccessRuleProtection($true, $false)
        foreach ($identity in @([System.Security.Principal.WindowsIdentity]::GetCurrent().User, (New-Object System.Security.Principal.SecurityIdentifier($localSystemSid)), (New-Object System.Security.Principal.SecurityIdentifier($administratorsGroupSid))))
        {
            $acl.AddAccessRule((New-Object System.Security.AccessControl.FileSystemAccessRule($identity, 'FullControl', 'ContainerInherit, ObjectInherit', 'None', 'Allow')))
        }
        Set-Acl -LiteralPath $backupPath -AclObject $acl
        foreach ($snapshot in $snapshots)
        {
            foreach ($certificate in $snapshot.Certificates)
            {
                # SerializedCert preserves provider properties, not private-key material.
                $file = Join-Path $backupPath "$($snapshot.Location)-$($snapshot.Name)-$($certificate.Thumbprint).cer"
                [System.IO.File]::WriteAllBytes($file, $certificate.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::SerializedCert))
            }
        }
        if ($crlExists) { [System.IO.File]::WriteAllBytes((Join-Path $backupPath 'previous.crl'), $crl) }
        [pscustomobject]@{ CrlPath = $crlPath; CrlExisted = $crlExists; Hosts = $hosts } | Export-Clixml -LiteralPath (Join-Path $backupPath 'state.xml')

        foreach ($hostProcess in $hosts)
        {
            $process = Get-Process -Id $hostProcess.ProcessId -ErrorAction Stop
            try
            {
                Stop-Process -Id $hostProcess.ProcessId -ErrorAction Stop
                $stoppedHosts += $hostProcess
                if (-not $process.WaitForExit(30000)) { throw "Test host $($hostProcess.ProcessId) did not stop." }
            }
            finally { $process.Dispose() }
        }
        $generationStarted = $true
        Invoke-CheckedProcess -FilePath $generator -Arguments @()
        foreach ($pool in $pools)
        {
            $account = $accounts[$pool.Name]
            Invoke-CheckedProcess -FilePath 'powershell.exe' -Arguments @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', (Join-Path $scripts 'CertificatePrivateKeyPermissions.ps1'), '-WcfServiceAccount', $account)
        }
        # Commit only after every grant succeeds. The caller must not change HTTPS bindings before this returns.
        Write-Output 'Certificate regeneration and all private-key grants succeeded.'
    }
    catch
    {
        $failure = $_
        $recoveryErrors = @()
        Write-Warning "Certificate refresh failed: $failure. Restoring the previous state."
        if ($generationStarted)
        {
            try { Restore-CertificateSnapshot -Snapshots $snapshots }
            catch { $recoveryErrors += "Certificate restoration: $_" }
            try
            {
                if ($crlExists) { [System.IO.File]::WriteAllBytes($crlPath, $crl) }
                elseif (Test-Path -LiteralPath $crlPath -PathType Leaf) { Remove-Item -LiteralPath $crlPath }
            }
            catch { $recoveryErrors += "CRL restoration: $_" }
        }
        if ($recoveryErrors.Count -eq 0)
        {
            try
            {
                if ($generationStarted)
                {
                    foreach ($pool in $pools | Where-Object { $_.State -eq 'Started' })
                    {
                        Invoke-CheckedProcess -FilePath "$env:WINDIR\System32\inetsrv\appcmd.exe" -Arguments @('recycle', 'apppool', "/apppool.name:$($pool.Name)")
                    }
                }
            }
            catch { $recoveryErrors += "IIS pool recovery: $_" }
            try { Restart-TestHosts -Hosts $stoppedHosts }
            catch { $recoveryErrors += "Service recovery: $_" }
        }
        if ($recoveryErrors.Count -gt 0)
        {
            $retainBackup = $true
            throw "Refresh failed: $failure. Recovery also failed: $($recoveryErrors -join '; '). Manual recovery is required. Original certificate contexts and CRL are retained at '$backupPath'; do not delete their persisted private keys."
        }
        throw "Refresh failed: $failure. Previous certificates, key associations and CRL were restored; HTTPS bindings were not changed. Previously stopped hosts were recovered."
    }
    finally
    {
        foreach ($snapshot in $snapshots)
        {
            foreach ($certificate in $snapshot.Certificates) { $certificate.Dispose() }
        }
        if (-not $retainBackup -and (Test-Path -LiteralPath $backupPath -PathType Container))
        {
            try { Remove-Item -LiteralPath $backupPath -Recurse -Force }
            catch { Write-Warning "Certificate refresh succeeded, but temporary backup '$backupPath' could not be removed: $_" }
        }
    }
}

if ($MyInvocation.InvocationName -ne '.')
{
    try
    {
        Invoke-CertificateRefresh -Root $RepositoryPath
        exit 0
    }
    catch
    {
        Write-Error $_ -ErrorAction Continue
        exit 1
    }
}
