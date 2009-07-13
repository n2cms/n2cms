@echo off
cd ..\build
cmd /c "build.bat /target:Framework-PrepareDependencies" & pause & exit 
