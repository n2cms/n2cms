@echo off

@echo Copying N2 library files and edit interface to the templates
xcopy /s/Y/R ..\Output\Core\* ..\Templates\UI\
xcopy /s/Y/R ..\Output\Core\Bin\N2.dll ..\Templates\lib\

echo Done!

Pause