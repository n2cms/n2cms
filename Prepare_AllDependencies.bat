@echo off
cd /d %~dp0build
cmd /c "build.bat /target:PrepareDependencies" & cd .. & pause & exit /b
