strName      = WScript.Arguments.Item(0)
strArgs      = ""
If WScript.Arguments.Count > 1 Then
    For i = 1 To WScript.Arguments.Count - 1
        strArgs = strArgs & " " & WScript.Arguments.Item(i)
    Next
End If

Set objShell = CreateObject("Shell.Application")

dim fso,currentDirectory,cmdArgs
Set fso = CreateObject("Scripting.FileSystemObject")
currentDirectory = fso.GetAbsolutePathName(".")
cmdArgs = currentDirectory & " " & strArgs
objShell.ShellExecute WScript.Arguments(0), cmdArgs,"", "runas", 1
