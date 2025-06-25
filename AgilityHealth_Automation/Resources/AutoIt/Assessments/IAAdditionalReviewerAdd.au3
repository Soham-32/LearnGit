#include <Array.au3>
#include <WinAPIShPath.au3>

;Reading command line
Local $CmdLine = _WinAPI_CommandLineToArgv($CmdLineRaw)

Local $TabName= "IndividualReview - AH - Internet Explorer - [InPrivate]"
Local $FirstName = $CmdLine[1]
Local $LastName = $CmdLine[2]
Local $Email= $CmdLine[3]

WinActivate($TabName,"") ; focusing to window

Send($FirstName)
Send("{TAB}")
Sleep(1000)

Send($LastName)
Send("{TAB}")
Sleep(1000)
Send("{TAB}")
Sleep(1000)

Send($Email)
Sleep(1000)

;End
