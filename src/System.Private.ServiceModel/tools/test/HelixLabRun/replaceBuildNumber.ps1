
$buildStr = Get-Date -UFormat "%Y%m%d%H%M"
$buildStr = $buildStr.Substring(3)
$Files = Get-ChildItem *.rsp
foreach ($templateFile in $Files)
{
    (get-content  $templateFile) | foreach-object {$_ -replace "tempBuildNumber", $buildStr} | set-content $templateFile
}
