#include <Array.au3>
#include <WinAPIShPath.au3>

;Reading command line
Local $CmdLine = _WinAPI_CommandLineToArgv($CmdLineRaw)

Local $TabName= $CmdLine[1]
Local $BrowserTabName= StringReplace($TabName,"*"," ") ; replacing * by spaces.

WinActivate($BrowserTabName,"") ; focusing to window

Send("^j") ; Press CTRL + J
Sleep(1000)
Send("{ENTER}") ; Prees Enter
Sleep(1000)
Send("!{F4}") ; Press Alt + F4
sleep(1000)