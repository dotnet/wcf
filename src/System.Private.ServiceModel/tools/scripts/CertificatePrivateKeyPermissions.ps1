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
            # Call getters explicitly so PowerShell does not hide provider or privilege errors.
            $privateKey = $cert.get_PrivateKey()
            try
            {
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
                $privateKey.Dispose()
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
