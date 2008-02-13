@echo off

echo Copying N2 library files and edit interface
xcopy /s/Y/R ..\Output\Core\* ..\Examples\Globalization\WebSite\
xcopy /s/Y/R ..\Examples\Globalization\Data\* ..\Examples\Globalization\WebSite\
xcopy /s/Y/R ..\Output\Core\Bin\N2.dll ..\Examples\Globalization\Lib

echo Done!

Pause