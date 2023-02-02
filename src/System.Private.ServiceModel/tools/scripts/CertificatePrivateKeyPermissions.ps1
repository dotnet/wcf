param (
    [Parameter(Mandatory=$true)]
    [string] $WcfServiceAccount
)

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
	        #Create new CSP object based on existing certificate provider and key name
	        $csp = New-Object System.Security.Cryptography.CspParameters($cert.PrivateKey.CspKeyContainerInfo.ProviderType, $cert.PrivateKey.CspKeyContainerInfo.ProviderName, $cert.PrivateKey.CspKeyContainerInfo.KeyContainerName)

	        # Set flags and key security based on existing cert
	        $csp.Flags = "UseExistingKey","UseMachineKeyStore"
	        $csp.CryptoKeySecurity = $cert.PrivateKey.CspKeyContainerInfo.CryptoKeySecurity
	        $csp.KeyNumber = $cert.PrivateKey.CspKeyContainerInfo.KeyNumber

	        # Create new access rule for GenericRead
	        $access = New-Object System.Security.AccessControl.CryptoKeyAccessRule($WcfServiceAccount, "GenericRead", "Allow")
	        # Add access rule to CSP object
	        $csp.CryptoKeySecurity.AddAccessRule($access)

	        #Create new CryptoServiceProvider object which updates Key with CSP information created/modified above
	        $rsa2 = New-Object System.Security.Cryptography.RSACryptoServiceProvider($csp)
        }
    }
    exit 0;
}
catch
{
    exit 1;
}
