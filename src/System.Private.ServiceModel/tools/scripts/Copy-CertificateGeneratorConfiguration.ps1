param (
    [Parameter(Mandatory=$true)]
    [string] $SourcePath,
    [string] $DestinationPath,
    [switch] $ProtectDestination
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path -LiteralPath $SourcePath -PathType Leaf))
{
    throw "Certificate generator settings not found: '$SourcePath'. Run IIS certificate setup or migrate the verified IIS generator configuration before refreshing."
}

$document = New-Object System.Xml.XmlDocument
$document.XmlResolver = $null
$readerSettings = New-Object System.Xml.XmlReaderSettings
$readerSettings.DtdProcessing = [System.Xml.DtdProcessing]::Prohibit
$reader = [System.Xml.XmlReader]::Create((Resolve-Path -LiteralPath $SourcePath).ProviderPath, $readerSettings)
try { $document.Load($reader) }
finally { $reader.Dispose() }

$values = @{}
foreach ($name in @('testserverbase', 'CertExpirationInDay', 'CrlFileLocation'))
{
    $settings = @($document.SelectNodes("/configuration/appSettings/add[@key='$name']"))
    if ($settings.Count -ne 1 -or [string]::IsNullOrWhiteSpace($settings[0].GetAttribute('value')))
    {
        throw "Expected one nonempty '$name' setting in '$SourcePath'."
    }
    $values[$name] = $settings[0].GetAttribute('value')
}

$days = 0
if (-not [int]::TryParse($values['CertExpirationInDay'], [ref] $days) -or $days -le 0)
{
    throw "Invalid certificate validity in '$SourcePath'; expected a positive number of days."
}
if ($values['testserverbase'] -notmatch '^[A-Za-z0-9_-]+$')
{
    throw "Invalid IIS application name in '$SourcePath'."
}
if ($values['CrlFileLocation'] -notmatch '^[A-Za-z]:\\' -or [System.IO.Path]::GetFileName($values['CrlFileLocation']) -ne 'test.crl')
{
    throw "Expected an absolute Windows test.crl path in '$SourcePath'."
}

if ($DestinationPath)
{
    if (Test-Path -LiteralPath $DestinationPath -PathType Container)
    {
        throw "Configuration destination is a directory: '$DestinationPath'."
    }
    Copy-Item -LiteralPath $SourcePath -Destination $DestinationPath -Force -ErrorAction Stop
    if ($ProtectDestination)
    {
        $acl = New-Object System.Security.AccessControl.FileSecurity
        $acl.SetAccessRuleProtection($true, $false)
        $localSystemSid = 'S-1-5-18'
        $administratorsGroupSid = 'S-1-5-32-544'
        foreach ($identity in @([System.Security.Principal.WindowsIdentity]::GetCurrent().User, (New-Object System.Security.Principal.SecurityIdentifier($localSystemSid)), (New-Object System.Security.Principal.SecurityIdentifier($administratorsGroupSid))))
        {
            $acl.AddAccessRule((New-Object System.Security.AccessControl.FileSystemAccessRule($identity, 'FullControl', 'Allow')))
        }
        Set-Acl -LiteralPath $DestinationPath -AclObject $acl
    }
    Write-Output "Applied certificate generator settings: '$SourcePath' -> '$DestinationPath'."
}
else
{
    Write-Output "Validated certificate generator settings: '$SourcePath'."
}
