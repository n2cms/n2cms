@echo off
ECHO Assuming MSBuild location %windir%\Microsoft.NET\Framework\v3.5\MSBuild.exe 
ECHO BUILDING N2

msbuild.bat /target:Deploy

ECHO DONE!
PAUSE