#include <Array.au3>
#include <WinAPIShPath.au3>

;Reading command line
Local $CmdLine = _WinAPI_CommandLineToArgv($CmdLineRaw)

Local $TabName= "Edit Assessment - AH - Internet Explorer - [InPrivate]"
Local $Description = $CmdLine[1]

WinActivate($TabName,"") ; focusing to window

Send("+{TAB}")
Sleep(1000)
Send($Description)
Sleep(1000)

;End