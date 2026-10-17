Set WshShell = CreateObject("WScript.Shell")
strPath = CreateObject("Scripting.FileSystemObject").GetParentFolderName(WScript.ScriptFullName)
WshShell.CurrentDirectory = strPath & "\Ejecutable"
WshShell.Run """" & strPath & "\Ejecutable\InventarioMinero.WPF.exe""", 1, False
