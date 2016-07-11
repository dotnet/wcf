param (
    [Parameter(Mandatory=$true)]
    [string] $Url
)
$request = New-Object System.Net.WebClient
For ($i=0; $i -le 60; $i++)
{
    try
    {
        $request.DownloadString($Url)
    }
    catch
    {
        Start-Sleep -s 5
        continue;
    }
    exit 0;
}
exit 1;
