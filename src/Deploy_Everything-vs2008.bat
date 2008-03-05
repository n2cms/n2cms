@echo off
ECHO DEPLOYING N2 to 'Deploy' Folder

msbuild.bat /target:Deploy

ECHO DONE!
PAUSE