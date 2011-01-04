@echo off
cd ..\build
cmd /c "build.bat /t:Framework-PrepareDependencies /t:Templates-Mvc-PrepareDependencies /t:Templates-PrepareDependencies" & pause & exit 
