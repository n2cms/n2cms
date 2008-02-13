@echo off

@echo Copying N2 library files and edit interface to the templates
@xcopy /s/Y/R ..\Output\Core\* ..\Templates\UI\

echo Done!

Pause