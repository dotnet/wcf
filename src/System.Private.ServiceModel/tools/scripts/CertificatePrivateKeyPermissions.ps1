param (
    [Parameter(Mandatory=$true)]
    [string] $WcfServiceAccount
)

$ErrorActionPreference = 'Stop'

try
{
    $certStores = @('My','TrustedPeople')

    foreach ($certStore in $certStores)
    {	
        #Get the list of WCF personal certificates having private key
        $certs = Get-ChildItem Cert:\LocalMachine\$certStore | where {$_.Issuer -like 'CN=DO_NOT_TRUST_WcfBridgeRootCA*' -and $_.HasPrivateKey -eq "True"}

        #Locate certificate based on provided thumbprint
        foreach ($cert in $certs)
        {
            $privateKey = [System.Security.Cryptography.X509Certificates.RSACertificateExtensions]::GetRSAPrivateKey($cert)
            try
            {
                if ($privateKey -is [System.Security.Cryptography.RSACng] -and
                    $privateKey.Key.Provider.Provider -eq 'Microsoft Software Key Storage Provider')
                {
                    if (-not $privateKey.Key.IsMachineKey -or [string]::IsNullOrEmpty($privateKey.Key.UniqueName))
                    {
                        throw "Certificate '$($cert.Thumbprint)' does not have a persisted machine RSA key."
                    }

                    $keyPath = Join-Path $env:ProgramData "Microsoft\Crypto\Keys\$($privateKey.Key.UniqueName)"
                    $acl = Get-Acl -LiteralPath $keyPath
                    $access = New-Object System.Security.AccessControl.FileSystemAccessRule($WcfServiceAccount, "Read", "Allow")
                    $acl.AddAccessRule($access)
                    Set-Acl -LiteralPath $keyPath -AclObject $acl
                    continue
                }

                # GetRSAPrivateKey can wrap a legacy CSP key in RSACng; reopen its CSP container for ACL access.
                if ($privateKey -is [System.Security.Cryptography.RSACng])
                {
                    $privateKey.Dispose()
                    $privateKey = $null
                    $privateKey = $cert.get_PrivateKey()
                }

                if ($privateKey -isnot [System.Security.Cryptography.RSACryptoServiceProvider])
                {
                    throw "Certificate '$($cert.Thumbprint)' does not have a supported RSA private key."
                }

                $keyInfo = $privateKey.CspKeyContainerInfo
                $csp = New-Object System.Security.Cryptography.CspParameters($keyInfo.ProviderType, $keyInfo.ProviderName, $keyInfo.KeyContainerName)

                $csp.Flags = "UseExistingKey","UseMachineKeyStore"
                $csp.CryptoKeySecurity = $keyInfo.get_CryptoKeySecurity()
                $csp.KeyNumber = $keyInfo.KeyNumber

                $access = New-Object System.Security.AccessControl.CryptoKeyAccessRule($WcfServiceAccount, "GenericRead", "Allow")
                $csp.CryptoKeySecurity.AddAccessRule($access)

                $rsa2 = New-Object System.Security.Cryptography.RSACryptoServiceProvider($csp)
                $rsa2.Dispose()
            }
            finally
            {
                if ($null -ne $privateKey)
                {
                    $privateKey.Dispose()
                }
            }
        }
    }
    exit 0;
}
catch
{
    Write-Error "Failed to grant certificate private-key access to '$WcfServiceAccount': $_`n$($_.InvocationInfo.PositionMessage)" -ErrorAction Continue
    exit 1;
}
