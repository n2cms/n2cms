@echo off
cd build
cmd /c "build.bat /target:Deploy"
start ..\output & pause & exit
