"c:\Program Files (x86)\Inno Setup 5\ISCC.exe" n2cms.iss
"C:\Program Files (x86)\Windows Kits\8.0\bin\x86\signtool.exe" sign /i StartCom /t "http://timestamp.comodoca.com/authenticode" ..\n2setup.exe
pause
