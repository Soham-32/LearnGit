#include <Array.au3>
#include <WinAPIShPath.au3>

;Reading command line
Local $CmdLine = _WinAPI_CommandLineToArgv($CmdLineRaw)

Local $FilePath= $CmdLine[1]
$FilePath = StringReplace($FilePath,"*"," ")
Local $WindowTitle= "Choose File to Upload"

WinWait($WindowTitle,"",15) ; waiting for window
Sleep(2000)
WinActivate($WindowTitle,"") ; focusing to window
ControlFocus($WindowTitle,"","Edit1") ; focusing on File Name field
ControlSetText($WindowTitle,"","Edit1",$FilePath) ; settings file path
ControlClick($WindowTitle,"","Button1") ; click on Open button

;End
