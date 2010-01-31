@echo off
cd ..\build
cmd /c "build.bat /t:Prepare-Framework /t:Prepare-Templates-Mvc /t:Prepare-Templates-WebForms" & pause & exit 
