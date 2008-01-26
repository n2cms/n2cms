@echo off

echo Copying N2 library files and edit interface for C#
xcopy /s/Y/R .\Deploy\* ..\Examples\Parts\WebSite\
xcopy /s/Y/R ..\Examples\Parts\Data ..\Examples\Parts\WebSite\

echo Done!

Pause