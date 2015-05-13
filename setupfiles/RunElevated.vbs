Set objShell = CreateObject("Shell.Application")
dim fso,currentDirectory
Set fso = CreateObject("Scripting.FileSystemObject")
currentDirectory = fso.GetAbsolutePathName(".")
objShell.ShellExecute WScript.Arguments(0), currentDirectory,"", "runas", 1
