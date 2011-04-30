@echo off
cd /d %~dp0build
cmd /c "build.bat /target:PrepareDependencies" & pause & exit /b
