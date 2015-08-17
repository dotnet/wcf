'Capture command line arguments to forward to program we start
strName      = WScript.Arguments.Item(0)
cmdArgs      = ""
If WScript.Arguments.Count > 1 Then
    For i = 1 To WScript.Arguments.Count - 1
        cmdArgs = cmdArgs & " " & WScript.Arguments.Item(i)
    Next
End If

Set objShell = CreateObject("Shell.Application")
objShell.ShellExecute WScript.Arguments(0), cmdArgs,"", "runas", 1
