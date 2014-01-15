@echo off
cd /d %~dp0..\build
cmd /c "build.bat /t:Source-PrepareDependencies" & cd ..\src & pause & exit /b
