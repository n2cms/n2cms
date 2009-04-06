@echo off
cmd /c "build.bat /target:Deploy /p:Configuration=Release /p:Platform=^"Any CPU^"" & pause & exit
