$logfile = Join-Path $PSScriptRoot 'UpdateSvcutilCorePackage.log'
"Log file for ModifySvcutilCorePackage............."  | Out-File $logfile

if ($args.Length -ne 1)
{
    "Invalid number of input parameters" | Out-File $logfile -Append
    exit;
}

$path = $args[0]
if (![IO.Directory]::Exists($path))
{
    "The path " + $path + " not exist" | Out-File $logfile -Append
    exit
}

$target = Get-Childitem –Path $path | Where-Object Name -like svcutilcore*nupkg | Select-Object -First 1

#Write-Output $target
If (!$target)
{
    $errormessage = "Package file doesn't exist"
    Write-Output $errormessage
    $errormessage | Out-File $logfile -Append
    exit;
}

"The target is " + $target | Out-File $logfile -Append

$zipfileName = Join-Path $path $target
if(![System.IO.File]::Exists($zipfileName)){
    $errormessage = "Package File doesn't exist"
    Write-Output error
    $errormessage | Out-File $logfile
    exit;
}

$message = "Modifying Package: " + $zipfileName

Write-Output $message
$message | Out-File $logfile

Add-Type -assembly  System.IO.Compression.FileSystem

Try
{
    #Open the zip file
    "Open the package" | Out-File $logfile -Append
    $zip =  [System.IO.Compression.ZipFile]::Open($zipfileName,"Update")

    #find svcutilcore.nuspec in the package
    "Search for svcutilcore.nuspec" | Out-File $logfile -Append
    $myfile = $zip.Entries.Where({$_.name -eq 'svcutilcore.nuspec'})
    if(!$myfile)
    {
        $errormessage = "svcutilcore.nuspec doesn't exist in the package"
        Write-Output $errormessage
        $errormessage | Out-File $logfile -Append
        exit;
    }

    "Open svcutilcore.nuspec for read" | Out-File $logfile -Append
    $streamreader = [System.IO.StreamReader]($myfile).Open()
    $filecontent = $streamreader.ReadToEnd()
    "Before Update ............" | Out-File $logfile -Append
    $filecontent | Out-File $logfile -Append
    "`n" | Out-File $logfile -Append
    $streamreader.Close()
    $streamreader.Dispose()

    if(!$filecontent)
    {
        $errormessage = "File content is empty"
        Write-Output $errormessage
        $errormessage | Out-File $logfile -Append
        exit;
    }
    

    "Read content as Xml and Insert a line" | Out-File $logfile -Append
    $doc = $filecontent -as [XML]

    $newXmlElement = $doc.CreateElement('dependency')
    $newXMlElement.SetAttribute('id','Microsoft.NETCore.App')
    $newXMlElement.SetAttribute('version','2.0.0')
    $newXMlElement.SetAttribute('exclude','Build,Analyzers')
    $doc.package.metadata.dependencies.group.InsertAfter($newXMlElement, $doc.configuration.runtime.legacyUnhandledExceptionPolicy)
    $doc = [xml] $doc.OuterXml.Replace(" xmlns=`"`"", "")

    "Open svcutilcore.nuspec for write" | Out-File $logfile -Append
    $streamwriter = [System.IO.StreamWriter]($myfile).Open()
    $streamwriter.BaseStream.SetLength(0)
    $doc.Save($streamwriter)

    $streamwriter.Flush()
    $streamwriter.Close()
    $streamwriter.Dispose()

    $streamreader = [System.IO.StreamReader]($myfile).Open()
    $filecontent = $streamreader.ReadToEnd()
    "After Update ............" | Out-File $logfile -Append
    $filecontent | Out-File $logfile -Append
    "`n" | Out-File $logfile -Append
    $streamreader.Close()
    $streamreader.Dispose()

    "Close the package" | Out-File $logfile -Append
    $zip.Dispose()

    $message = "Done!"
    Write-Output $message
    $message | Out-File $logfile -Append
}
Catch
{
    $ErrorMessage = $_.Exception.Message
    Write-Output  $ErrorMessage
    Write-Output $_.Exception.InvocationInfo.ScriptLineNumber
    $ErrorMessage | Out-File $logfile -Append
    $line = $_.Exception.InvocationInfo.ScriptLineNumber
    $line | Out-File $logfile -Append
}