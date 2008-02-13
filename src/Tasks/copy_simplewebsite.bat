@echo off

echo Copying N2 library files and edit interface for C#
xcopy /s/Y/R ..\Output\Core\* ..\Examples\SimpleWebSite\CS\
xcopy /s/Y/R ..\Examples\SimpleWebSite\Data ..\Examples\SimpleWebSite\CS\

echo Copying N2 library files and edit interface for Visual Basic
xcopy /s/Y/R ..\Output\Core\* ..\Examples\SimpleWebSite\VB\
xcopy /s/Y/R ..\Examples\SimpleWebSite\Data ..\Examples\SimpleWebSite\VB\

echo Done!

Pause