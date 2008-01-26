@echo off

echo Copying N2 library files and edit interface to the templates
xcopy /s/Y/R .\Deploy\* ..\Templates\UI\

echo Done!

Pause