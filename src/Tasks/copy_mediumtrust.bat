@echo off

echo Copying N2 library files and edit interface for C#
xcopy /s/Y/R .\Deploy\* ..\Examples\MediumTrust\WebProject

echo Done!

Pause