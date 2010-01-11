@echo off
cd ..\build
cmd /c "build.bat /target:Framework-PrepareDependencies & build.bat /target:Templates-PrepareDependencies & build.bat /target:Templates-Mvc-PrepareDependencies" & pause & exit 
