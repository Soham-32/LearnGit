#include <Array.au3>
#include <WinAPIShPath.au3>

;Reading command line
Local $CmdLine = _WinAPI_CommandLineToArgv($CmdLineRaw)

Local $TabName= "Batch Edit Individual Assessment - AH - Internet Explorer - [InPrivate]"
Local $FirstNameValue = $CmdLine[1]
Local $LastNameValue = $CmdLine[2]
Local $EmailValue = $CmdLine[3]

WinActivate($TabName,"") ; focusing to window

Send($FirstNameValue)
Send("{TAB}")
Sleep(1000)

Send($LastNameValue)
Send("{TAB}")
Sleep(1000)
Send("{TAB}")
Sleep(1000)

Send($EmailValue)
Sleep(1000)

;End