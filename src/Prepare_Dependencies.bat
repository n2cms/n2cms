@echo off
cd ..\build
cmd /c "build.bat /target:Core-PrepareDependencies" & pause & exit 
