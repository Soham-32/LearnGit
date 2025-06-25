#include <Array.au3>
#include <WinAPIShPath.au3>

;Reading command line
Local $CmdLine = _WinAPI_CommandLineToArgv($CmdLineRaw)

Local $TabName= $CmdLine[1]
Local $FirstName = $CmdLine[2]
Local $LastName = $CmdLine[3]
Local $Email= $CmdLine[4]
Local $BrowserTabName= StringReplace($TabName,"*"," ") & " - AH - Internet Explorer - [InPrivate]"


WinActivate($BrowserTabName,"") ; focusing to window

Send($FirstName)
Send("{TAB}")
Sleep(1000)

Send($LastName)
Send("{TAB}")
Sleep(1000)

Send($Email)
Sleep(1000)

;End
